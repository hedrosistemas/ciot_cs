using Ciot.Protos.V2;
using Ciot.Sdk.Common.Error;
using LanguageExt;
using System.Collections.Generic;

namespace Ciot.Sdk.Iface
{
    public interface IIfaceRepository
    {
        Either<ErrorBase, IfaceInfo> CreateIface(IfaceType ifaceType);
        Either<ErrorBase, List<IfaceInfo>> GetIfaces();
        Either<ErrorBase, IfaceInfo> GetIfaceById(uint id);
        Either<ErrorBase, IfaceInfo> UpdateIface(uint id, IfaceType ifaceType);
        Either<ErrorBase, IfaceInfo> DeleteIface(uint id);
    }
}
