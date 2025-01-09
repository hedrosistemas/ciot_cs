using Ciot.Sdk.Iface;
using Ciot.Sdk.Protos.V2;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Ciot.Grpc.Services
{
    public class IfaceService(IIfaceRepository ifaceRespository, IIfaceManager ifaceManager) : Sdk.Protos.V2.IfaceService.IfaceServiceBase
    {
        public override Task<CreateIfaceResponse> CreateIface(CreateIfaceRequest request, ServerCallContext context)
        {
            var response = new CreateIfaceResponse();
            var result = ifaceRespository.CreateIface(request.Type);
            return result.Match(
                r =>
                {
                    response.Iface = r;
                    return Task.FromResult(response);
                },
                l => throw l);
        }

        public override Task<GetIfacesResponse> GetIfaces(Empty request, ServerCallContext context)
        {
            var response = new GetIfacesResponse();
            var result = ifaceRespository.GetIfaces();
            return result.Match(
                r =>
                {
                    response.Ifaces.AddRange(r);
                    return Task.FromResult(response);
                },
                l => throw l);
        }

        public override Task<GetIfaceResponse> GetIface(GetIfaceRequest request, ServerCallContext context)
        {
            var response = new GetIfaceResponse();
            var result = ifaceRespository.GetIfaceById(request.Id);
            return result.Match(
                r =>
                {
                    response.Iface = r;
                    return Task.FromResult(response);
                },
                l => throw l);
        }

        public override Task<UpdateIfaceResponse> UpdateIface(UpdateIfaceRequest request, ServerCallContext context)
        {
            var response = new UpdateIfaceResponse();
            var result = ifaceRespository.UpdateIface(request.Id, request.Type);
            return result.Match(
                r =>
                {
                    response.Iface = r;
                    return Task.FromResult(response);
                },
                l => throw l);
        }

        public override Task<DeleteIfaceResponse> DeleteIface(DeleteIfaceRequest request, ServerCallContext context)
        {
            var response = new DeleteIfaceResponse();
            var result = ifaceRespository.DeleteIface(request.Id);
            return result.Match(
                r =>
                {
                    response.Iface = r;
                    return Task.FromResult(response);
                },
                l => throw l);
        }

        public override Task<SendMessageResponse> SendMessage(SendMessageRequest request, ServerCallContext context)
        {
            var response = new SendMessageResponse();
            var result = ifaceManager.SendMessage(request.Message);
            return result.Match(
                r =>
                {
                    response.Message = r;
                    return Task.FromResult(response);
                },
                l => throw l);
        }

        public override Task<SelectIfaceResponse> SelectIface(SelectIfaceRequest request, ServerCallContext context)
        {
            var result = ifaceManager.SelectIface(request.Id);
            return result.Match(
                r =>
                {
                    return Task.FromResult(new SelectIfaceResponse
                    {
                        Iface = r,
                    });
                },
                l => throw l);
        }

        public override Task<GetSelectedIfaceResponse> GetSelectedIface(Empty request, ServerCallContext context)
        {
            var result = ifaceManager.GetSelectedIface();
            return result.Match(
                r =>
                {
                    return Task.FromResult(new GetSelectedIfaceResponse
                    {
                        Iface = r,
                    });
                },
                l => throw l);
        }
    }
}
