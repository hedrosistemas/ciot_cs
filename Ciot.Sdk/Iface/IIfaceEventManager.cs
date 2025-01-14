using Ciot.Protos.V2;
using Ciot.Sdk.Common.Error;
using LanguageExt;
using System;
using System.Collections.Generic;

namespace Ciot.Sdk.Iface
{
    public interface IIfaceEventManager
    {
        event EventHandler<Event> OnEvent;
        Either<ErrorBase, Unit> Subscribe(IIface iface);
        Either<ErrorBase, Unit> Unsubscribe(IIface iface);
        Either<ErrorBase, List<IfaceEventManagerSubscription>> GetSubscriptions();
    }
}
