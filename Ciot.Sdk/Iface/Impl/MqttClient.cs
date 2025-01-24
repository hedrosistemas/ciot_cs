using Ciot.Protos.V2;
using Ciot.Sdk.Common.Error;
using LanguageExt;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ciot.Sdk.Iface.Impl
{
    public class MqttClient : IIface
    {
        public event EventHandler<Event> OnEvent;
        public IfaceInfo Info { get; private set; }

        private readonly MqttClientStatus status;
        private MqttClientCfg cfg;
        private IMqttClient client;
        private TaskCompletionSource<byte[]> responseTcs;

        public MqttClient(IfaceInfo info)
        {
            status = new MqttClientStatus();
            Info = info;
        }

        public Err Start(MqttClientCfg cfg)
        {
            this.cfg = cfg;

            var uri = new Uri(cfg.Url);
            var factory = new MqttFactory();
            var opts = new MqttClientOptionsBuilder()
                .WithClientId(cfg.ClientId)
                .WithTcpServer(uri.Host, uri.Port)
                .WithCleanSession()
                .WithCredentials(cfg.User, cfg.Password)
                .Build();

            client = factory.CreateMqttClient();
            client.ConnectedAsync += Client_ConnectedAsync;
            client.DisconnectedAsync += Client_DisconnectedAsync;
            client.ApplicationMessageReceivedAsync += Client_ApplicationMessageReceivedAsync;

            client.ConnectAsync(opts).Wait();

            return Err.Ok;
        }

        private Task Client_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            OnEvent?.Invoke(this, new Event
            {
                Type = EventType.Stopped,
            });
            return Task.CompletedTask;
        }

        public Err Stop()
        {
            client.DisconnectAsync();
            client.Dispose();
            return Err.Ok;
        }

        private Task Client_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            var topicFilter = new MqttTopicFilterBuilder()
                .WithTopic(cfg.Topics.Sub)
                .Build();
            client.SubscribeAsync(topicFilter);
            OnEvent?.Invoke(this, new Event
            {
                Type = EventType.Started,
            });
            return Task.CompletedTask;
        }

        protected virtual Task Client_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            if (responseTcs != null && !responseTcs.Task.IsCompleted)
            {
                responseTcs.SetResult(arg.ApplicationMessage.PayloadSegment.ToArray());
            }

            return Task.CompletedTask;
        }

        public Err GetData(MsgData data)
        {
            if (data.TypeCase != MsgData.TypeOneofCase.MqttClient)
            {
                return Err.InvalidType;
            }

            switch (data.MqttClient.TypeCase)
            {
                case MqttClientData.TypeOneofCase.Config:
                    data.MqttClient.Config = cfg;
                    break;
                case MqttClientData.TypeOneofCase.Status:
                    data.MqttClient.Status = status;
                    break;
                default:
                    return Err.InvalidType;
            }

            return Err.Ok;
        }

        public Err ProcessData(MsgData data)
        {
            if (data.TypeCase != MsgData.TypeOneofCase.MqttClient)
            {
                return Err.InvalidType;
            }

            switch (data.MqttClient.TypeCase)
            {
                case MqttClientData.TypeOneofCase.Config:
                    Start(data.MqttClient.Config);
                    break;
                case MqttClientData.TypeOneofCase.Request:
                    return Err.NotImplemented;
                default:
                    return Err.InvalidType;
            }

            return Err.Ok;
        }

        public async Task<Either<ErrorBase, byte[]>> SendData(byte[] data)
        {
            try
            {
                responseTcs = new TaskCompletionSource<byte[]>();
                await client.PublishBinaryAsync(cfg.Topics.Pub, data, MqttQualityOfServiceLevel.AtMostOnce, false);
                Console.WriteLine("Mensagem publicada. Aguardando resposta...");
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                cts.Token.Register(() => responseTcs.TrySetCanceled(), useSynchronizationContext: false);
                var response = await responseTcs.Task;
                return response.ToArray();
            }
            catch (TaskCanceledException)
            {
                return new ErrorInternal("Timeout: Nenhuma resposta recebida.");
            }
            catch (Exception ex)
            {
                return new ErrorException(ex);
            }
        }

        protected void TriggerEvent(Event e)
        {
            OnEvent?.Invoke(this, e);
        }
    }
}
