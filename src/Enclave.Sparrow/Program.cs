using System.Diagnostics.CodeAnalysis;
using Enclave.Sparrow.Services;

namespace Enclave.Sparrow;

[ExcludeFromCodeCoverage(Justification = "Straightforward composition root; test by review.")]
internal static class Program
{
    private static int Main(string[] args)
    {
        var services = new ServiceCollection();
        Startup.ConfigureServices(services);
        var provider = services.BuildServiceProvider();

        var runner = provider.GetRequiredService<IPhaseRunner>();
        runner.Run();

        return 0;
    }
}
