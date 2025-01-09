using Ciot.Sdk.Protos.V2;
using Grpc.Core;

namespace Ciot.Sdk.Common.Error
{
    public class ErrorNotFound : ErrorBase
    {
        public ErrorNotFound(string message = "Not Found") : base(StatusCode.NotFound, Err.NotFound, message)
        {

        }
    }
}
