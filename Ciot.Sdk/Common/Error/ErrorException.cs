using Ciot.Protos.V2;
using Grpc.Core;
using System;

namespace Ciot.Sdk.Common.Error
{
    public class ErrorException : ErrorBase
    {
        public ErrorException(Exception ex) : base(StatusCode.Internal, Err.Exception, ex.Message)
        {

        }
    }
}
