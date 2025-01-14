using Ciot.Protos.V2;
using Grpc.Core;
using System;

namespace Ciot.Sdk.Common.Error
{
    public class ErrorInternal : ErrorBase
    {
        public ErrorInternal(string message = "Internal error") : base(StatusCode.Internal, Err.Exception, message)
        {

        }
    }
}
