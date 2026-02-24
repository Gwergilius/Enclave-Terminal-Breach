using Enclave.Echelon.Core.Services;
using Enclave.Phosphor;
using Enclave.Shared.IO;
using Enclave.Shared.Models;
using Enclave.Raven.Phases;
using Enclave.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enclave.Raven.Tests;

/// <summary>
/// Smoke tests for <see cref="Startup.ConfigureServices"/>: verifies that all key services can be resolved.
/// </summary>
[UnitTest, TestOf(nameof(Startup))]
public class StartupTests
{
    private static IConfiguration CreateMinimalConfiguration() =>
        new ConfigurationBuilder().Build();

    [Fact]
    public void ConfigureServices_ResolvesIGameSession()
    {
        var services = new ServiceCollection();
        Startup.ConfigureServices(services, CreateMinimalConfiguration());
        using var scope = services.BuildServiceProvider().CreateScope();

        var session = scope.ServiceProvider.GetRequiredService<IGameSession>();

        session.ShouldNotBeNull();
    }

    [Fact]
    public void ConfigureServices_ResolvesIConsoleIO()
    {
        var services = new ServiceCollection();
        Startup.ConfigureServices(services, CreateMinimalConfiguration());
        var provider = services.BuildServiceProvider();

        var console = provider.GetRequiredService<IConsoleIO>();

        console.ShouldNotBeNull();
    }

    [Fact]
    public void ConfigureServices_ResolvesISolverFactory()
    {
        var services = new ServiceCollection();
        Startup.ConfigureServices(services, CreateMinimalConfiguration());
        var provider = services.BuildServiceProvider();

        var factory = provider.GetRequiredService<ISolverFactory>();

        factory.ShouldNotBeNull();
        factory.GetSolver().ShouldNotBeNull();
    }

    [Fact]
    public void ConfigureServices_ResolvesAllPhases()
    {
        var services = new ServiceCollection();
        Startup.ConfigureServices(services, CreateMinimalConfiguration());
        using var scope = services.BuildServiceProvider().CreateScope();

        scope.ServiceProvider.GetRequiredService<IStartupBadgePhase>().ShouldNotBeNull();
        scope.ServiceProvider.GetRequiredService<IResetScopePhase>().ShouldNotBeNull();
        scope.ServiceProvider.GetRequiredService<IDataInputPhase>().ShouldNotBeNull();
        scope.ServiceProvider.GetRequiredService<IHackingLoopPhase>().ShouldNotBeNull();
        scope.ServiceProvider.GetRequiredService<IPlayAgainPhase>().ShouldNotBeNull();
    }

    [Fact]
    public void ConfigureServices_ResolvesIPhaseRunner()
    {
        var services = new ServiceCollection();
        Startup.ConfigureServices(services, CreateMinimalConfiguration());
        var provider = services.BuildServiceProvider();

        var runner = provider.GetRequiredService<IPhaseRunner>();

        runner.ShouldNotBeNull();
    }

    [Fact]
    public void ConfigureServices_ResolvesIConsoleIO_AsSingleton()
    {
        var services = new ServiceCollection();
        Startup.ConfigureServices(services, CreateMinimalConfiguration());
        var provider = services.BuildServiceProvider();

        var a = provider.GetRequiredService<IConsoleIO>();
        var b = provider.GetRequiredService<IConsoleIO>();

        a.ShouldBeSameAs(b);
    }

    [Fact]
    public void ConfigureServices_ResolvesIPhosphorCanvasAndIPhosphorWriter()
    {
        var services = new ServiceCollection();
        Startup.ConfigureServices(services, CreateMinimalConfiguration());
        var provider = services.BuildServiceProvider();

        var canvas = provider.GetRequiredService<IPhosphorCanvas>();
        var writer = provider.GetRequiredService<IPhosphorWriter>();

        canvas.ShouldNotBeNull();
        writer.ShouldNotBeNull();
        canvas.ShouldBeOfType<AnsiPhosphorCanvas>();
        writer.ShouldBeOfType<PhosphorTypewriter>();
    }

    [Fact]
    public void ConfigureServices_ResolvesIPhosphorKeyboardHandler()
    {
        var services = new ServiceCollection();
        Startup.ConfigureServices(services, CreateMinimalConfiguration());
        var provider = services.BuildServiceProvider();

        var handler = provider.GetRequiredService<IPhosphorReader>();

        handler.ShouldNotBeNull();
    }

    [Fact]
    public void ConfigureServices_ResolvesIGameSession_AsScoped()
    {
        var services = new ServiceCollection();
        Startup.ConfigureServices(services, CreateMinimalConfiguration());
        var provider = services.BuildServiceProvider();

        IGameSession session1;
        IGameSession session2;
        using (var scope1 = provider.CreateScope())
        {
            session1 = scope1.ServiceProvider.GetRequiredService<IGameSession>();
        }
        using (var scope2 = provider.CreateScope())
        {
            session2 = scope2.ServiceProvider.GetRequiredService<IGameSession>();
        }

        session1.ShouldNotBeNull();
        session2.ShouldNotBeNull();
        session1.ShouldNotBeSameAs(session2);
    }
}
