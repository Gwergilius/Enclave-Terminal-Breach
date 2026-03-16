using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using Enclave.Echelon.Core.Services;
using Enclave.Phosphor;
using Enclave.Raven.Screens.BootScreen;
using Enclave.Raven.Screens.DataInput;
using Enclave.Raven.Screens.HackingLoop;
using Enclave.Raven.Screens.KeyPress;
using Enclave.Raven.Services;
using Enclave.Shared.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enclave.Raven.Tests;

/// <summary>
/// Smoke tests for <see cref="Startup.ConfigureServices"/>: verifies that all key services can be resolved.
/// </summary>
[UnitTest, TestOf(nameof(Startup))]
[SupportedOSPlatform("windows")]
[ExcludeFromCodeCoverage(Justification = "Thin wrapper around Console.Write/ReadLine; testing would only verify BCL behavior.")]

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
    public void ConfigureServices_ResolvesAllViewModels()
    {
        var services = new ServiceCollection();
        Startup.ConfigureServices(services, CreateMinimalConfiguration());
        using var scope = services.BuildServiceProvider().CreateScope();

        scope.ServiceProvider.GetRequiredService<BootScreenViewModel>().ShouldNotBeNull();
        scope.ServiceProvider.GetRequiredService<DataInputViewModel>().ShouldNotBeNull();
        scope.ServiceProvider.GetRequiredService<HackingLoopViewModel>().ShouldNotBeNull();
        scope.ServiceProvider.GetRequiredService<KeyPressViewModel>().ShouldNotBeNull();
    }

    [Fact]
    public void ConfigureServices_ResolvesIViewModelRegistry()
    {
        var services = new ServiceCollection();
        Startup.ConfigureServices(services, CreateMinimalConfiguration());
        using var scope = services.BuildServiceProvider().CreateScope();

        var registry = scope.ServiceProvider.GetRequiredService<IViewModelRegistry>();

        registry.ShouldNotBeNull();
        registry.GetViewModel("BootScreen").IsSuccess.ShouldBeTrue();
        registry.GetViewModel("DataInput").IsSuccess.ShouldBeTrue();
        registry.GetViewModel("HackingLoop").IsSuccess.ShouldBeTrue();
        registry.GetViewModel("KeyPress").IsSuccess.ShouldBeTrue();
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
    public void ConfigureServices_ResolvesIPhosphorWriter()
    {
        var services = new ServiceCollection();
        Startup.ConfigureServices(services, CreateMinimalConfiguration());
        var provider = services.BuildServiceProvider();

        var writer = provider.GetRequiredService<IPhosphorWriter>();

        writer.ShouldNotBeNull();
        writer.ShouldBeOfType<AnsiPhosphorCanvas>();
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

    [Fact]
    public void ConfigureServices_ResolvesIVirtualScreen()
    {
        var services = new ServiceCollection();
        Startup.ConfigureServices(services, CreateMinimalConfiguration());
        var provider = services.BuildServiceProvider();

        var screen = provider.GetRequiredService<IVirtualScreen>();

        screen.ShouldNotBeNull();
    }
}
