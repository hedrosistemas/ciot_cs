using Ciot.Protos.V2;
using Ciot.Sdk.Common.Error;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ciot.Sdk.Iface
{
    public class IfaceEventManagerSubscription
    {
        public IIface Iface { get; set; }

        public int Count { get; set; }

        public IfaceEventManagerSubscription(IIface iface) 
        { 
            Iface = iface;
            Count = 1;
        }
    }

    public class IfaceEventManager : IIfaceEventManager
    {
        public event EventHandler<Event> OnEvent;

        public Dictionary<uint, IfaceEventManagerSubscription> subscriptions;

        public IfaceEventManager()
        {
            subscriptions = new Dictionary<uint, IfaceEventManagerSubscription>();
        }

        public Either<ErrorBase, Unit> Subscribe(IIface iface)
        {
            if(subscriptions.ContainsKey(iface.Info.Id))
            {
                var subscription = subscriptions[iface.Info.Id];
                if(subscription.Iface.Info.Id == iface.Info.Id &&
                   subscription.Iface.Info.Type == iface.Info.Type)
                {
                    subscription.Count++;
                    return Unit.Default;
                }
                else
                {
                    return new ErrorNotFound("Interface not found");
                }
            }
            else
            {
                subscriptions.Add(iface.Info.Id, new IfaceEventManagerSubscription(iface));
                iface.OnEvent += Iface_OnEvent;
                return Unit.Default;
            }
        }

        public Either<ErrorBase, Unit> Unsubscribe(IIface iface)
        {
            if (subscriptions.ContainsKey(iface.Info.Id))
            {
                var subscription = subscriptions[iface.Info.Id];
                if (subscription.Iface.Info.Id == iface.Info.Id &&
                   subscription.Iface.Info.Type == iface.Info.Type)
                {
                    subscription.Count--;
                    if(subscription.Count == 0)
                    {
                        subscription.Iface.OnEvent -= Iface_OnEvent;
                        subscriptions.Remove(iface.Info.Id);
                    }
                    return Unit.Default;
                }
                else
                {
                    return new ErrorNotFound("Interface not found");
                }
            }
            else
            {
                return new ErrorNotFound("Subscription not found");
            }
        }

        private void Iface_OnEvent(object sender, Event e)
        {
            OnEvent?.Invoke(sender, e);
        }

        public Either<ErrorBase, List<IfaceEventManagerSubscription>> GetSubscriptions()
        {
            return subscriptions.Values.ToList();
        }
    }
}
