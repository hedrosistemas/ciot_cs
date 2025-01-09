using Ciot.Sdk.Protos.V2;
using Grpc.Core;

namespace Ciot.Sdk.Common.Error
{
    public class ErrorInvalidType : ErrorBase
    {
        public ErrorInvalidType(string message = "Invalid Type") : base(StatusCode.Internal, Err.InvalidType, message)
        {

        }
    }
}
