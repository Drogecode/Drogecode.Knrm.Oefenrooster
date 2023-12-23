using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit.DependencyInjection.Logging;

namespace Drogecode.Knrm.Oefenrooster.TestPreCom;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(lb => lb.AddXunitOutput());
    }
}
