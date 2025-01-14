using Ciot.Protos.V2;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using Ciot.Sdk.Common.Error;
using LanguageExt;

namespace Ciot.Sdk.Iface.Impl
{
    public class HttpClient : IIface
    {
        public event EventHandler<Event> OnEvent;
        public IfaceInfo Info { get; private set; }

        private HttpClientCfg cfg;

        private HttpClientStatus status;

        public HttpClient(IfaceInfo info)
        {
            status = new HttpClientStatus();
            Info = info;
        }

        public Err Start(HttpClientCfg cfg)
        {
            this.cfg = cfg;
            OnEvent?.Invoke(this, new Event
            {
                Type = EventType.Started,
            });
            return Err.Ok;
        }

        public Err Stop()
        {
            return Err.Ok;
        }

        public Err GetData(MsgData data)
        {
            if (data.TypeCase != MsgData.TypeOneofCase.HttpClient)
            {
                return Err.InvalidType;
            }

            switch (data.HttpClient.TypeCase)
            {
                case HttpClientData.TypeOneofCase.Config:
                    data.HttpClient.Config = cfg;
                    break;
                case HttpClientData.TypeOneofCase.Status:
                    data.HttpClient.Status = status;
                    break;
                default:
                    return Err.InvalidType;
            }

            return Err.Ok;
        }

        public Err ProcessData(MsgData data)
        {
            if (data.TypeCase != MsgData.TypeOneofCase.HttpClient)
            {
                return Err.InvalidType;
            }

            switch (data.HttpClient.TypeCase)
            {
                case HttpClientData.TypeOneofCase.Config:
                    Start(data.HttpClient.Config);
                    break;
                case HttpClientData.TypeOneofCase.Request:
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
                var response = await PostBytesAsync(cfg.Url, data);
                var content = await response.Content.ReadAsByteArrayAsync();
                return content;
            }
            catch (Exception ex)
            {
                return new ErrorException(ex);
            }
        }

        public static async Task<HttpResponseMessage> PostBytesAsync(string url, byte[] data)
        {
            using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
            {
                HttpContent content = new ByteArrayContent(data);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                HttpResponseMessage response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                return response;
            }
        }
    }
}
