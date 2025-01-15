using Ciot.Protos.V2;
using Ciot.Sdk.Common.Error;
using Ciot.Sdk.Config;
using Ciot.Sdk.Iface.Impl;
using Google.Protobuf;
using LanguageExt;
using System.Collections.Generic;
using System.IO;

namespace Ciot.Sdk.Iface
{
    public class IfaceManager : IfaceEventManager, IIfaceManager
    {
        private readonly IConfigRepository configRepository;

        private IfaceInfo selectedIface;

        private const string selectedIfaceFile = "selectedIface.json";

        private Dictionary<uint, IIface> ifaces;

        public IfaceManager(IConfigRepository configRepository) 
        {
            this.configRepository = configRepository;
            if (File.Exists(selectedIfaceFile))
            {
                var content = File.ReadAllText(selectedIfaceFile);
                selectedIface = Newtonsoft.Json.JsonConvert.DeserializeObject<IfaceInfo>(content);
            }
            ifaces = new Dictionary<uint, IIface>();
        }

        public Either<ErrorBase, IIface> CreateIface(IfaceInfo iface)
        {
            if (ifaces.ContainsKey(iface.Id) && ifaces[iface.Id].Info.Type == iface.Type)
            {
                return Either<ErrorBase, IIface>.Right(ifaces[iface.Id]);
            }

            var result = configRepository.GetConfigById(iface.Id);
            return result.Match(
                r =>
                {
                    IIface newIface = null;

                    switch (iface.Type)
                    {
                        case IfaceType.Undefined:
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
                    ifaces.Add(newIface.Info.Id, newIface);
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
                    File.WriteAllText(selectedIfaceFile, Newtonsoft.Json.JsonConvert.SerializeObject(selectedIface));
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

        public Either<ErrorBase, Unit> SubscribeToEvents(SubscribeToEventsRequest request)
        {
            var result = CreateIface(request.Iface);
            return result.Match(
                r => Subscribe(r),
                l => l);
        }

        public Either<ErrorBase, Unit> UnsubscribeToEvents(IfaceInfo iface)
        {
            var result = CreateIface(iface);
            return result.Match(
                r => Unsubscribe(r),
                l => l);
        }
    }
}
