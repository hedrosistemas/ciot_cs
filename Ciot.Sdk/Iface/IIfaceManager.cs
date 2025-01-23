using Ciot.Protos.V2;
using Ciot.Sdk.Common.Error;
using LanguageExt;

namespace Ciot.Sdk.Iface
{
    public interface IIfaceManager : IIfaceEventManager
    {

        Either<ErrorBase, IfaceInfo> SelectIface(uint id);

        Either<ErrorBase, IfaceInfo> GetSelectedIface();

        Either<ErrorBase, IIface> CreateIface(IfaceInfo ifaceInfo);

        Either<ErrorBase, Msg> SendMessage(Msg msg);

        Either<ErrorBase, Unit> SubscribeToEvents(SubscribeToEventsRequest request);

        Either<ErrorBase, Unit> UnsubscribeToEvents(IfaceInfo iface);

        Either<ErrorBase, Unit> SetIfaceConfig(Msg msg);
    }
}
