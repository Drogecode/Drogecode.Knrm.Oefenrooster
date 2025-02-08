using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using MudBlazor;
using MudBlazor.Utilities;

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
            IsDefault = trainingType.IsDefault,
            IsActive = trainingType.IsActive,
        };
    }
    public static PlannerTrainingType ToDrogecode(this DbRoosterTrainingType trainingType)
    {
        return new PlannerTrainingType
        {
            Id = trainingType.Id,
            Name = trainingType.Name,
            ColorLight = trainingType.ColorLight ?? Colors.Gray.Lighten1,
            ColorDark = trainingType.ColorDark ?? string.Empty,
            TextColorLight = trainingType.TextColorLight,
            TextColorDark = trainingType.TextColorDark,
            Order = trainingType.Order,
            CountToTrainingTarget = trainingType.CountToTrainingTarget,
            IsDefault = trainingType.IsDefault,
            IsActive = trainingType.IsActive,
        };
    }

    public static void SecureColors(this PlannerTrainingType trainingType)
    {

        MudColor? colorLight = null, colorDark = null, textColorLight = null, textColorDark = null;
        if (!string.IsNullOrEmpty(trainingType.ColorLight))
        {
            colorLight = trainingType.ColorLight;
            trainingType.ColorLight = colorLight.ToString();
        }
        if (!string.IsNullOrEmpty(trainingType.ColorDark))
        {
            colorDark = trainingType.ColorDark;
            trainingType.ColorDark = colorDark.ToString();
        }
        if (!string.IsNullOrEmpty(trainingType.TextColorLight))
        {
            textColorLight = trainingType.TextColorLight;
            trainingType.TextColorLight = textColorLight.ToString();
        }
        if (!string.IsNullOrEmpty(trainingType.TextColorDark))
        {
            textColorDark = trainingType.TextColorDark;
            trainingType.TextColorDark = textColorDark.ToString();
        }
    }
}
