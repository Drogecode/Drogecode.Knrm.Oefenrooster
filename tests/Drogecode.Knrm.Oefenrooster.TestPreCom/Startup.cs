using Microsoft.Extensions.DependencyInjection;
using Xunit.DependencyInjection.Logging;

namespace Drogecode.Knrm.Oefenrooster.TestPreCom;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(lb => lb.AddXunitOutput());
    }
}
