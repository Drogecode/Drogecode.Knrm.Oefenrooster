using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using System.Diagnostics;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class FunctionService : IFunctionService
{
    private readonly ILogger<FunctionService> _logger;
    private readonly Database.DataContext _database;
    public FunctionService(ILogger<FunctionService> logger, Database.DataContext database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task<AddFunctionResponse> AddFunction(DrogeFunction function, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new AddFunctionResponse();
        function.Id = Guid.NewGuid();
        var functions = await _database.UserFunctions.Where(x => x.CustomerId == customerId).OrderBy(x => x.Order).ToListAsync(clt);
        var order = -1;
        foreach (var dbFunction in functions)
        {
            if (dbFunction.Order > order)
                order = dbFunction.Order;
        }
        order += 10;
        _database.UserFunctions.Add(new Database.Models.DbUserFunctions
        {
            Id = function.Id,
            CustomerId = customerId,
            Name = function.Name,
            Order = order,
            TrainingTarget = function.TrainingTarget,
            TrainingOnly = function.TrainingOnly,
            IsDefault = function.Default,
            IsActive = function.Active,
        });
        result.Success = (await _database.SaveChangesAsync(clt) > 0);
        result.NewFunction = function;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<MultipleFunctionsResponse> GetAllFunctions(Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new MultipleFunctionsResponse { Functions = new List<DrogeFunction>() };
        var functions = await _database.UserFunctions.Where(x => x.CustomerId == customerId).OrderBy(x => x.Order).ToListAsync(clt);
        foreach (var function in functions)
        {
            result.Functions.Add(new DrogeFunction
            {
                Id = function.Id,
                RoleId = function.RoleId,
                Name = function.Name,
                Order = function.Order,
                TrainingTarget = function.TrainingTarget,
                TrainingOnly = function.TrainingOnly,
                Default = function.IsDefault,
                Active = function.IsActive,
            });
        }
        result.Success = true;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }
}
