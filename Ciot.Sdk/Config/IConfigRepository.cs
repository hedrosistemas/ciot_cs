using Ciot.Sdk.Common.Error;
using LanguageExt;
using System.Collections.Generic;

namespace Ciot.Sdk.Config
{
    public interface IConfigRepository
    {
        Either<ErrorBase, Msg> SetConfig(Msg config);

        Either<ErrorBase, Msg> GetConfigById(uint id);

        Either<ErrorBase, List<Msg>> GetConfigs();

        Either<ErrorBase, Msg> UpdateConfig(Msg config);

        Either<ErrorBase, Msg> DeleteConfig(uint id);
    }
}
