using Ciot.Protos.V2;
using Ciot.Sdk.Common.Error;
using Ciot.Sdk.Iface;
using Google.Protobuf;
using LanguageExt;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ciot.Sdk.Config
{
    public class ConfigRepository : IConfigRepository
    {
        private const string filename = "configs.dat";

        public IIfaceRepository ifaceRepository;

        private readonly ConfigList configs;

        public ConfigRepository(IIfaceRepository ifaceRepository)
        {
            this.ifaceRepository = ifaceRepository;

            if (File.Exists(filename))
            {
                var data = File.ReadAllBytes(filename);
                configs = ConfigList.Parser.ParseFrom(data);
            }
            else
            {
                configs = new ConfigList();
            }
        }

        public Either<ErrorBase, Msg> SetConfig(Msg config)
        {
            var existentConfig = FindConfig(config.Id);
            if (existentConfig != null)
            {
                return UpdateConfig(config);
            }
            else
            {
                var result = ifaceRepository.GetIfaceById(config.Id);
                return result.Match(
                    r =>
                    {
                        if(MsgTypeIsOk(r.Type, config.Data))
                        {
                            config.Iface = r;
                            configs.Items.Add(config);
                            Save();
                            return Either<ErrorBase, Msg>.Right(config);
                        }
                        else
                        {
                            return new ErrorInvalidType("Invalid message type");
                        }
                    },
                    l =>
                    {
                        if(l is ErrorNotFound)
                        {
                            return new ErrorNotFound($"Interface '{config.Id}' not found");
                        }
                        return l;
                    });
            }
        }

        public Either<ErrorBase, Msg> UpdateConfig(Msg config)
        {
            var existentConfig = FindConfig(config.Id);
            if (existentConfig != null)
            {
                if(existentConfig.Data.TypeCase == config.Data.TypeCase)
                {
                    existentConfig.Data = config.Data;
                    Save();
                    return existentConfig;
                }
                else
                {
                    return new ErrorNotFound("Invalid message type");
                }
            }
            else
            {
                return new ErrorNotFound("Configuration not found");
            }
        }

        public Either<ErrorBase, Msg> GetConfigById(uint id)
        {
            var config = FindConfig(id);
            if (config != null)
            {
               return config;
            }
            else
            {
                return new ErrorNotFound("Configuration not found");
            }
        }

        public Either<ErrorBase, List<Msg>> GetConfigs()
        {
            return configs.Items.ToList();
        }

        public Either<ErrorBase, Msg> DeleteConfig(uint id)
        {
            var config = FindConfig(id);

            if (config != null)
            {
                configs.Items.Remove(config);
                return config;
            }
            else
            {
                return new ErrorNotFound("Configuration not found");
            }
        }

        public bool MsgTypeIsOk(IfaceType type, MsgData msgData)
        {
            switch (msgData.TypeCase)
            {
                case MsgData.TypeOneofCase.Ciot:
                    return false;
                case MsgData.TypeOneofCase.Sys:
                    return false;
                case MsgData.TypeOneofCase.HttpClient:
                    return type == IfaceType.HttpClient && msgData.HttpClient.TypeCase == HttpClientData.TypeOneofCase.Config;
                case MsgData.TypeOneofCase.HttpServer:
                    return type == IfaceType.HttpServer && msgData.HttpServer.TypeCase == HttpServerData.TypeOneofCase.Config;
                case MsgData.TypeOneofCase.MqttClient:
                    return type == IfaceType.MqttClient && msgData.MqttClient.TypeCase == MqttClientData.TypeOneofCase.Config;
                case MsgData.TypeOneofCase.Uart:
                    return type == IfaceType.Uart && msgData.Uart.TypeCase == UartData.TypeOneofCase.Config;
                default:
                    return false;
            }
        }

        private void Save()
        {
            var data = configs.ToByteArray();
            File.WriteAllBytes(filename, data);
        }

        private Msg FindConfig(uint id)
        {
            return configs.Items.ToList().Find((c) => c.Id == id);
        }
    }
}
