using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly ILogger<ConfigurationService> _logger;
    private readonly Database.DataContext _database;
    public ConfigurationService(ILogger<ConfigurationService> logger, Database.DataContext database)
    {
        _logger = logger;
        _database = database;
    }

    public void UpgradeDatabase()
    {
        _database.Database.EnsureCreated();
        _database.Database.Migrate();
    }
}
