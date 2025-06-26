using Ciot.Protos.V2;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ciot.Sdk.Common.Error
{
    public class ErrorNotSupported : ErrorBase
    {
        public ErrorNotSupported(string message = "Not supported") : base(StatusCode.Internal, Err.NotSupported, message)
        {

        }
    }
}
