using Drogecode.Knrm.Oefenrooster.PreCom.Models;

namespace Drogecode.Knrm.Oefenrooster.PreCom.Interfaces;

public interface IPreComClient
{
    Task<LoginResponse> Login(string username, string password);
    Task<PreComUser> GetUserInfo(CancellationToken cancellationToken = default);
    Task<SchedulerAppointment[]> GetUserSchedulerAppointments(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<Group[]> GetAllUserGroups(CancellationToken cancellationToken = default);
    Task<Dictionary<DateTime, int>> GetOccupancyLevels(long groupID, DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<Group> GetAllFunctions(long groupID, DateTime date, CancellationToken cancellationToken = default);
    Task<MsgOut[]> GetMessages(string controlID = default, CancellationToken cancellationToken = default);
    Task<MsgInLog[]> GetAlarmMessages(int msgInID = default, int previousOrNext = default, CancellationToken cancellationToken = default);
    Task SetAvailabilityForAlarmMessage(int msgInID, bool available, CancellationToken cancellationToken = default);
    Task SendMessage(MsgSend msgSend, CancellationToken cancellationToken = default);
}