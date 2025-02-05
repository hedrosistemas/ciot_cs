using Ciot.Sdk.Common.Error;
using LanguageExt;
using System.Collections.Generic;

namespace Ciot.Sdk.Iface
{
    public interface IIfaceRepository
    {
        Either<ErrorBase, Msg> CreateIface(Msg msg);

        Either<ErrorBase, Msg> GetIfaceById(uint id);

        Either<ErrorBase, List<Msg>> GetIfaces();

        Either<ErrorBase, Msg> UpdateIface(Msg msg);

        Either<ErrorBase, Msg> DeleteIface(uint id);
    }
}
