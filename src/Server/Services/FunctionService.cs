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
    public async Task<List<DrogeFunction>> GetAllFunctions(Guid customerId)
    {
        var result = new List<DrogeFunction>();
        var functions = _database.UserFunctions.Where(x => x.CustomerId == customerId);
        foreach (var function in functions)
        {
            result.Add(new DrogeFunction
            {
                Id = function.Id,
                Name = function.Name,
                Order = function.Order,
                TrainingOnly = function.TrainingOnly,
                Default = function.Default
            });
        }
        return result;
    }
}
