using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;
using Microsoft.AspNetCore.Mvc;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IPreComService
{
    Task<MultiplePreComAlertsResponse> GetAllAlerts(Guid userId, Guid customerId, int take, int skip, CancellationToken clt);
    string AnalyzeAlert(Guid userId, Guid customerId, object body, out DateTime timestamp, out int? priority);
    void WriteAlertToDb(Guid userId, Guid customerId, DateTime? sendTime, string alert, int? priority, string raw, string? ip);
    Task<bool> PatchAlertToDb(DbPreComAlert alert);
    Task<PutPreComForwardResponse> PutForward(PreComForward forward, Guid customerId, Guid userId, CancellationToken clt);
    Task<PatchPreComForwardResponse> PatchForward(PreComForward forward, Guid customerId, Guid userId, CancellationToken clt);
    Task<MultiplePreComForwardsResponse> GetAllForwards(int take, int skip, Guid userId, Guid customerId, CancellationToken clt);
    Task<PreComForward?> GetForward(Guid forwardId, Guid customerId, CancellationToken clt);
}
