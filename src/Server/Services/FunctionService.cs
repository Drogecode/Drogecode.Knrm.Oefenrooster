using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using System.Diagnostics;
using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class FunctionService : IFunctionService
{
    private const string FUNC_BY_ID = "func_{0}";

    private readonly ILogger<FunctionService> _logger;
    private readonly Database.DataContext _database;
    private readonly IMemoryCache _memoryCache;

    public FunctionService(ILogger<FunctionService> logger, Database.DataContext database, IMemoryCache memoryCache)
    {
        _logger = logger;
        _database = database;
        _memoryCache = memoryCache;
    }

    public async Task<DrogeFunction?> GetById(Guid customerId, Guid functionId, CancellationToken clt)
    {
        var memoryKey = string.Format(FUNC_BY_ID, functionId);
        var function = _memoryCache.Get<DrogeFunction>(memoryKey);
        if (function is not null)
            return function;
        function = await _database.UserFunctions.Where(x => x.CustomerId == customerId && x.Id == functionId).Select(x => x.ToDrogeFunction()).FirstOrDefaultAsync(clt);
        var cacheOptions = new MemoryCacheEntryOptions();
        cacheOptions.SetSlidingExpiration(TimeSpan.FromSeconds(10));
        cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(1));
        _memoryCache.Set(memoryKey, function, cacheOptions);
        return function;
    }

    public async Task<AddFunctionResponse> AddFunction(DrogeFunction function, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new AddFunctionResponse();
        function.Id = Guid.NewGuid();
        var functions = await _database.UserFunctions.Where(x => x.CustomerId == customerId).OrderBy(x => x.Order).Select(x => x.Order).ToListAsync(clt);
        var order = functions.Prepend(-1).Max() + 10;
        _database.UserFunctions.Add(new DbUserFunctions
        {
            Id = function.Id,
            CustomerId = customerId,
            Name = function.Name,
            Order = order,
            TrainingTarget = function.TrainingTarget,
            TrainingOnly = function.TrainingOnly,
            IsDefault = function.Default,
            IsActive = function.Active,
            IsSpecial = function.Special
        });
        result.Success = (await _database.SaveChangesAsync(clt) > 0);
        result.NewId = function.Id;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<PatchResponse> PatchFunction(Guid customerId, DrogeFunction function, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new PatchResponse();

        var oldFunction = await _database.UserFunctions.FirstOrDefaultAsync(x => x.Id == function.Id && x.CustomerId == customerId, clt);
        if (oldFunction is not null)
        {
            oldFunction.Name = function.Name;
            oldFunction.Order = function.Order;
            oldFunction.RoleId = function.RoleId;
            oldFunction.TrainingTarget = function.TrainingTarget;
            oldFunction.TrainingOnly = function.TrainingOnly;
            oldFunction.IsActive = function.Active;
            oldFunction.IsDefault = function.Default;
            oldFunction.IsSpecial = function.Special;
            _database.UserFunctions.Update(oldFunction);
            result.Success = await _database.SaveChangesAsync(clt) > 0;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<MultipleFunctionsResponse> GetAllFunctions(Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new MultipleFunctionsResponse { Functions = new List<DrogeFunction>() };
        var functions = await _database.UserFunctions
            .Where(x => x.CustomerId == customerId)
            .OrderBy(x => x.Order)
            .AsNoTracking()
            .ToListAsync(clt);
        foreach (var function in functions)
        {
            result.Functions.Add(function.ToDrogeFunction());
        }

        result.Success = true;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }
}