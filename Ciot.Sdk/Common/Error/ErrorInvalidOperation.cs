using Ciot.Protos.V2;
using Grpc.Core;

namespace Ciot.Sdk.Common.Error
{
    public class ErrorInvalidOperation : ErrorBase
    {
        public ErrorInvalidOperation(string message = "Invalid Operation") : base(StatusCode.Internal, Err.ImpossibleOp, message)
        {

        }
    }
}
