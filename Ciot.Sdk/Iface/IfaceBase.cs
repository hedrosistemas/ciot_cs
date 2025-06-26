using Ciot.Protos.V2;
using Ciot.Sdk.Common.Error;
using LanguageExt;
using System;
using System.Threading.Tasks;

namespace Ciot.Sdk.Iface
{
    public class IfaceBase : IIface
    {
        public IfaceInfo Info => throw new NotImplementedException();

        public event EventHandler<Event> OnEvent;

        public Err GetData(MsgData data)
        {
            return Err.NotSupported;
        }

        public Err ProcessData(MsgData data)
        {
            return Err.NotSupported;
        }

        public Task<Either<ErrorBase, byte[]>> SendData(byte[] data)
        {
            return Task.FromResult<Either<ErrorBase, byte[]>>(new ErrorNotSupported());
        }

        public Err Stop()
        {
            throw new NotImplementedException();
        }
    }
}
