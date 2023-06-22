using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using System.Runtime.CompilerServices;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class HolidayMapper
{
    public static DbUserHolidays ToDbHoliday(this Holiday holiday)
    {
        return new DbUserHolidays
        {
            Id = holiday.Id,
            Description = holiday.Description,
            UserId = holiday.UserId,
            Available = holiday.Available,
            ValidFrom = holiday.ValidFrom,
            ValidUntil = holiday.ValidUntil,
        };
    }
    
    public static Holiday ToHoliday(this DbUserHolidays holiday)
    {
        return new Holiday
        {
            Id = holiday.Id,
            Description = holiday.Description,
            UserId = holiday.UserId,
            Available = holiday.Available,
            ValidFrom = holiday.ValidFrom,
            ValidUntil = holiday.ValidUntil,
        };
    }
}
