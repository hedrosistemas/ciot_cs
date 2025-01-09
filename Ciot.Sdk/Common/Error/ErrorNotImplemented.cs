using Ciot.Sdk.Protos.V2;
using Grpc.Core;

namespace Ciot.Sdk.Common.Error
{
    public class ErrorNotImplemented : ErrorBase
    {
        public ErrorNotImplemented(string message = "Not implemented") : base(StatusCode.Internal, Err.NotImplemented, message)
        {

        }
    }
}
