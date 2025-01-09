using Ciot.Sdk.Common.Error;
using Ciot.Sdk.Config;
using Ciot.Sdk.Iface.Impl;
using Google.Protobuf;
using LanguageExt;

namespace Ciot.Sdk.Iface
{
    public class IfaceManager : IIfaceManager
    {
        private readonly IConfigRepository configRepository;

        private IfaceInfo selectedIface;

        public IfaceManager(IConfigRepository configRepository) 
        { 
            this.configRepository = configRepository;
        }

        public Either<ErrorBase, IIface> CreateIface(IfaceInfo iface)
        {
            var result = configRepository.GetConfigById(iface.Id);
            return result.Match(
                r =>
                {
                    IIface newIface = null;

                    switch (iface.Type)
                    {
                        case IfaceType.Unknown:
                            return new ErrorNotImplemented("Interface not implemented");
                        case IfaceType.Custom:
                            return new ErrorNotImplemented("Interface not implemented");
                        case IfaceType.Ciot:
                            return new ErrorNotImplemented("Interface not implemented");
                        case IfaceType.Storage:
                            return new ErrorNotImplemented("Interface not implemented");
                        case IfaceType.Sys:
                            return new ErrorNotImplemented("Interface not implemented");
                        case IfaceType.Uart:
                            newIface = new Uart(iface);
                            break;
                        case IfaceType.Usb:
                            return new ErrorNotImplemented("Interface not implemented");
                        case IfaceType.Tcp:
                            return new ErrorNotImplemented("Interface not implemented");
                        case IfaceType.Eth:
                            return new ErrorNotImplemented("Interface not implemented");
                        case IfaceType.Wifi:
                            return new ErrorNotImplemented("Interface not implemented");
                        case IfaceType.Ble:
                            return new ErrorNotImplemented("Interface not implemented");
                        case IfaceType.BleScn:
                            return new ErrorNotImplemented("Interface not implemented");
                        case IfaceType.BleAdv:
                            return new ErrorNotImplemented("Interface not implemented");
                        case IfaceType.Gpio:
                            return new ErrorNotImplemented("Interface not implemented");
                        case IfaceType.Ntp:
                            return new ErrorNotImplemented("Interface not implemented");
                        case IfaceType.Ota:
                            return new ErrorNotImplemented("Interface not implemented");
                        case IfaceType.Dfu:
                            return new ErrorNotImplemented("Interface not implemented");
                        case IfaceType.HttpClient:
                            newIface = new HttpClient(iface);
                            break;
                        case IfaceType.HttpServer:
                            return new ErrorNotImplemented("Interface not implemented");
                        case IfaceType.MqttClient:
                            newIface = new MqttClient(iface);
                            break;
                        case IfaceType.Socket:
                            return new ErrorNotImplemented("Interface not implemented");
                        case IfaceType.Bridge:
                            return new ErrorNotImplemented("Interface not implemented");
                        case IfaceType.IotaClient:
                            return new ErrorNotImplemented("Interface not implemented");
                        case IfaceType.IotaServer:
                            return new ErrorNotImplemented("Interface not implemented");
                        default:
                            return new ErrorNotImplemented("Interface not implemented");
                    }

                    if(newIface == null)
                    {
                        return new ErrorInternal("Error creating interface");
                    }

                    newIface.ProcessData(r.Data);
                    return Either<ErrorBase, IIface>.Right(newIface);
                },
                l =>
                {
                    return l;
                });
        }

        public Either<ErrorBase, IfaceInfo> GetSelectedIface()
        {
            if(selectedIface == null)
            {
                return new ErrorNotFound();
            }
            else
            {
                return selectedIface;
            }
        }

        public Either<ErrorBase, IfaceInfo> SelectIface(uint id)
        {
            var result = configRepository.GetConfigById(id);
            return result.Match(
                r =>
                {
                    selectedIface = r.Iface;
                    return Either<ErrorBase, IfaceInfo>.Right(selectedIface);
                },
                l => l);
        }

        public Either<ErrorBase, Msg> SendMessage(Msg msg)
        {
            if(selectedIface == null)
            {
                return new ErrorInvalidOperation("Invalid selected interface");
            }
            var createIface = CreateIface(selectedIface);
            return createIface.Match(
                r =>
                {
                    var data = msg.ToByteArray();
                    var task = r.SendData(data);
                    task.Wait();
                    var result = task.GetAwaiter().GetResult();
                    r.Stop();
                    return result.Match(
                        right =>
                        {
                            var response = Msg.Parser.ParseFrom(right);
                            return Either<ErrorBase, Msg>.Right(response);
                        },
                        l =>
                        {
                            return l;
                        });
                },
                l =>
                {
                    return l;
                });
        }
    }
}
