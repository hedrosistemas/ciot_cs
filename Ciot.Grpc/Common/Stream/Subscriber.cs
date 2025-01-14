using Ciot.Protos.V2;
using System.Collections.Concurrent;

namespace Ciot.Grpc.Common.Stream
{
    public class Subscriber<DataType>
    {
        public IfaceInfo Iface { get; private set; }
        public ConcurrentQueue<DataType> Queue { get; } = new();
        public AsyncAutoResetEvent DataAvailable { get; } = new();

        public Subscriber(IfaceInfo iface)
        {
            Iface = iface;
        }
    }
}
