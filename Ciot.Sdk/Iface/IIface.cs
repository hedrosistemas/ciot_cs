using Ciot.Sdk.Common.Error;
using Ciot.Sdk.Protos.V2;
using LanguageExt;
using System;
using System.Threading.Tasks;

namespace Ciot.Sdk.Iface
{
    public interface IIface
    {
        event EventHandler<Event> OnEvent;
        IfaceInfo Info { get; }
        Err Stop();
        Err ProcessData(MsgData data);
        Task<Either<ErrorBase, byte[]>> SendData(byte[] data);
        Err GetData(MsgData data);
    }
}
