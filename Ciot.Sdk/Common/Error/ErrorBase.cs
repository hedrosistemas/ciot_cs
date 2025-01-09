using Ciot.Sdk.Protos.V2;
using Grpc.Core;

namespace Ciot.Sdk.Common.Error
{
    public class ErrorBase : RpcException
    {
        public ErrorBase(StatusCode statusCode, Err err, string message = "") : base(new Grpc.Core.Status(statusCode, message), new Metadata
            {
                    { "error-code", $"{(int)err}" }
            })
        { }
    }
}
