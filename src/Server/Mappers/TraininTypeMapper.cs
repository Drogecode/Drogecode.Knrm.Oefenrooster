using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class TraininTypeMapper
{
    public static DbRoosterTrainingType ToDb(this PlannerTrainingType trainingType)
    {
        return new DbRoosterTrainingType
        {
            Id = trainingType.Id,
            Name = trainingType.Name,
            ColorLight = trainingType.ColorLight,
            ColorDark = trainingType.ColorDark,
            TextColorLight = trainingType.TextColorLight,
            TextColorDark = trainingType.TextColorDark,
            Order = trainingType.Order,
            CountToTrainingTarget = trainingType.CountToTrainingTarget,
            IsDefault = trainingType.IsDefault
        };
    }
    public static PlannerTrainingType ToDrogecode(this DbRoosterTrainingType trainingType)
    {
        return new PlannerTrainingType
        {
            Id = trainingType.Id,
            Name = trainingType.Name,
            ColorLight = trainingType.ColorLight ?? Colors.Grey.Lighten1,
            ColorDark = trainingType.ColorDark ?? string.Empty,
            TextColorLight = trainingType.TextColorLight,
            TextColorDark = trainingType.TextColorDark,
            Order = trainingType.Order,
            CountToTrainingTarget = trainingType.CountToTrainingTarget,
            IsDefault = trainingType.IsDefault
        };
    }
}
