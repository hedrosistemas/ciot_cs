using Ciot.Sdk.Config;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Ciot.Grpc.Services
{
    public class ConfigService(IConfigRepository configRepository) : Ciot.ConfigService.ConfigServiceBase
    {
        public override Task<SetConfigResponse> SetConfig(SetConfigRequest request, ServerCallContext context)
        {
            var response = new SetConfigResponse();
            var result = configRepository.SetConfig(request.Config);
            return result.Match(
                r =>
                {
                    response.Config = r;
                    return Task.FromResult(response);
                },
                l => throw l);
        }

        public override Task<GetConfigsResponse> GetConfigs(Empty request, ServerCallContext context)
        {
            var response = new GetConfigsResponse();
            var result = configRepository.GetConfigs();
            return result.Match(
                r =>
                {
                    response.Configs.AddRange(r);
                    return Task.FromResult(response);
                },
                l => throw l);
        }

        public override Task<GetConfigResponse> GetConfig(GetConfigRequest request, ServerCallContext context)
        {
            var response = new GetConfigResponse();
            var result = configRepository.GetConfigById(request.Id);
            return result.Match(
                r =>
                {
                    response.Config = r;
                    return Task.FromResult(response);
                },
                l => throw l);
        }

        public override Task<UpdateConfigResponse> UpdateConfig(UpdateConfigRequest request, ServerCallContext context)
        {
            var response = new UpdateConfigResponse();
            var result = configRepository.UpdateConfig(request.Config);
            return result.Match(
                r =>
                {
                    response.Config = r;
                    return Task.FromResult(response);
                },
                l => throw l);
        }

        public override Task<DeleteConfigResponse> DeleteConfig(DeleteConfigRequest request, ServerCallContext context)
        {
            var response = new DeleteConfigResponse();
            var result = configRepository.DeleteConfig(request.Id);
            return result.Match(
                r =>
                {
                    response.Config = r;
                    return Task.FromResult(response);
                },
                l => throw l);
        }
    }
}
