using Enclave.Common.Test.Core;
using Enclave.Shared.Phases;
using Enclave.Shared.Services;
using Moq;
using Microsoft.Extensions.DependencyInjection;

namespace Enclave.Shared.Tests.Services;

/// <summary>
/// Unit tests for <see cref="PhaseRunner"/>.
/// </summary>
[UnitTest, TestOf(nameof(PhaseRunner))]
public class PhaseRunnerTests
{
    [Fact]
    public void Run_ExecutesPhasesInOrder()
    {
        var executed = new List<string>();
        var phase = Mock.Of<IPhase>();
        phase.AsMock().Setup(p => p.Run()).Callback(() => executed.Add("phase"));

        var services = new ServiceCollection();
        services.AddSingleton(phase);
        services.AddScoped<IServiceScopeFactory>(sp => new TestScopeFactory(sp));
        var provider = services.BuildServiceProvider();

        var runner = new PhaseRunner(
            provider.GetRequiredService<IServiceScopeFactory>(),
            [typeof(IPhase), typeof(IPhase)]);

        runner.Run();

        executed.Count.ShouldBe(2);
    }

    [Fact]
    public void Run_WhenPhaseDoesNotImplementIPhase_ThrowsInvalidOperationException()
    {
        var services = new ServiceCollection();
        services.AddSingleton<object>(new object());
        services.AddScoped<IServiceScopeFactory>(sp => new TestScopeFactory(sp));
        var provider = services.BuildServiceProvider();

        var runner = new PhaseRunner(
            provider.GetRequiredService<IServiceScopeFactory>(),
            [typeof(object)]);

        var ex = Should.Throw<InvalidOperationException>(() => runner.Run());
        ex.Message.ShouldContain("does not implement");
        ex.Message.ShouldContain("IPhase");
    }

    [Fact]
    public void Constructor_WithNullScopeFactory_ThrowsArgumentNullException()
    {
        var services = new ServiceCollection().BuildServiceProvider();
        var factory = services.GetRequiredService<IServiceScopeFactory>();

        Should.Throw<ArgumentNullException>(() => new PhaseRunner(null!, [typeof(IPhase)]))
            .ParamName.ShouldBe("scopeFactory");
    }

    [Fact]
    public void Constructor_WithNullPhaseTypes_ThrowsArgumentNullException()
    {
        var services = new ServiceCollection().BuildServiceProvider();
        var factory = services.GetRequiredService<IServiceScopeFactory>();

        Should.Throw<ArgumentNullException>(() => new PhaseRunner(factory, null!))
            .ParamName.ShouldBe("phaseTypes");
    }

    private sealed class TestScopeFactory(IServiceProvider provider) : IServiceScopeFactory
    {
        public IServiceScope CreateScope() => new TestScope(provider);
    }

    private sealed class TestScope(IServiceProvider provider) : IServiceScope
    {
        public IServiceProvider ServiceProvider => provider;
        public void Dispose() { }
    }
}
