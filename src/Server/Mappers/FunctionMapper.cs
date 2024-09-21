using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class FunctionMapper
{
    public static DrogeFunction ToDrogeFunction(this DbUserFunctions function)
    {
        return new DrogeFunction
        {
            Id = function.Id,
            RoleId = function.RoleId,
            Name = function.Name,
            Order = function.Order,
            TrainingTarget = function.TrainingTarget,
            TrainingOnly = function.TrainingOnly,
            Default = function.IsDefault,
            Active = function.IsActive,
        };
    }
}