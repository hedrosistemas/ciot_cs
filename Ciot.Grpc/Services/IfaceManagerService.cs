using Ciot.Grpc.Common.Stream;
using Ciot.Protos.V2;
using Ciot.Sdk.Common.Error;
using Ciot.Sdk.Iface;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System.Collections.Concurrent;

namespace Ciot.Grpc.Services
{
    public class IfaceManagerService(IIfaceManager ifaceManager, IIfaceRepository ifaceRepository, ConcurrentDictionary<string, Subscriber<Event>> subscribers) : Protos.V2.IfaceManagerService.IfaceManagerServiceBase
    {
        public override Task<CreateIfaceResponse> CreateIface(CreateIfaceRequest request, ServerCallContext context)
        {
            var response = new CreateIfaceResponse();
            var result = ifaceRepository.CreateIface(request.Message);
            return result.Match(
                r =>
                {
                    response.Message = r;
                    ifaceManager.SetIfaceConfig(request.Message);
                    return Task.FromResult(response);
                },
                l => throw l);
        }

        public override Task<GetIfacesResponse> GetIfaces(Empty request, ServerCallContext context)
        {
            var response = new GetIfacesResponse();
            var result = ifaceRepository.GetIfaces();
            return result.Match(
                r =>
                {
                    response.List.AddRange(r);
                    return Task.FromResult(response);
                },
                l => throw l);
        }

        public override Task<GetIfaceResponse> GetIface(GetIfaceRequest request, ServerCallContext context)
        {
            var response = new GetIfaceResponse();
            var result = ifaceRepository.GetIfaceById(request.Id);
            return result.Match(
                r =>
                {
                    response.Message = r;
                    return Task.FromResult(response);
                },
                l => throw l);
        }

        public override Task<UpdateIfaceResponse> UpdateIface(UpdateIfaceRequest request, ServerCallContext context)
        {
            var response = new UpdateIfaceResponse();
            var result = ifaceRepository.UpdateIface(request.Message);
            return result.Match(
                r =>
                {
                    response.Message = r;
                    ifaceManager.SetIfaceConfig(r);
                    return Task.FromResult(response);
                },
                l => throw l);
        }

        public override Task<DeleteIfaceResponse> DeleteIface(DeleteIfaceRequest request, ServerCallContext context)
        {
            var response = new DeleteIfaceResponse();
            var result = ifaceRepository.DeleteIface(request.Id);
            return result.Match(
                r =>
                {
                    response.Message = r;
                    ifaceRepository.DeleteIface(r.Iface.Id);
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

        public override async Task SubscribeToEvents(SubscribeToEventsRequest request, IServerStreamWriter<Event> responseStream, ServerCallContext context)
        {
            var subscriber = new Subscriber<Event>(request.Iface);

            if (subscribers.Count == 0)
            {
                ifaceManager.OnEvent += IfaceManager_OnEvent;
            }

            if (subscribers.TryAdd(request.Id, subscriber))
            {
                subscribers[request.Id] = subscriber;
            }
            else
            {
                throw new ErrorInternal("Error adding subscriber to dictionary");
            }

            var result = ifaceManager.SubscribeToEvents(request);
            await result.Match(
                async r =>
                {
                    try
                    {
                        while (!context.CancellationToken.IsCancellationRequested)
                        {
                            await subscriber.DataAvailable.WaitAsync(context.CancellationToken);
                            while (subscriber.Queue.TryDequeue(out var e))
                            {
                                await responseStream.WriteAsync(e);
                            }
                        }
                    }
                    finally
                    {
                        ifaceManager.UnsubscribeToEvents(request.Iface);
                        subscribers.TryRemove(request.Id, out _);
                        if (subscribers.Count == 0)
                        {
                            ifaceManager.OnEvent -= IfaceManager_OnEvent;
                        }
                    }
                },
                l =>
                {
                    ifaceManager.UnsubscribeToEvents(request.Iface);
                    subscribers.TryRemove(request.Id, out _);
                    if (subscribers.Count == 0)
                    {
                        ifaceManager.OnEvent -= IfaceManager_OnEvent;
                    }
                    throw l;
                });
        }

        private void IfaceManager_OnEvent(object? sender, Event e)
        {
            if (sender is not IIface iface)
            {
                return;
            }

            foreach (var subscriber in subscribers.Values)
            {
                if (subscriber.Iface.Id == iface.Info.Id && subscriber.Iface.Type == iface.Info.Type)
                {
                    subscriber.Queue.Enqueue(e);
                    subscriber.DataAvailable.Set();
                }
            }
        }
    }
}
