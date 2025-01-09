using Ciot.Sdk.Common.Error;
using Ciot.Sdk.Protos.V2;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ciot.Sdk.Iface
{
    public interface IIfaceManager
    {
        Either<ErrorBase, IfaceInfo> SelectIface(uint id);

        Either<ErrorBase, IfaceInfo> GetSelectedIface();

        Either<ErrorBase, IIface> CreateIface(IfaceInfo ifaceInfo);

        Either<ErrorBase, Msg> SendMessage(Msg msg);
    }
}
