using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;

public class PutDefaultScheduleResponse : BaseResponse
{
    public DefaultSchedule? DefaultSchedule { get; set; }
    public PutDefaultScheduleError Error { get; set; }

}

public enum PutDefaultScheduleError
{
    None = 0,
    IdAlreadyExists = 1,
}
