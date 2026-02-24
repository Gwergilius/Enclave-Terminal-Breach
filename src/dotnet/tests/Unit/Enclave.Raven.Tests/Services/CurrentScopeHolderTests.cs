using Microsoft.Extensions.DependencyInjection;
using Enclave.Raven.Services;
using Xunit.Categories;

namespace Enclave.Raven.Tests.Services;

/// <summary>
/// Unit tests for <see cref="CurrentScopeHolder"/>.
/// </summary>
[UnitTest, TestOf(nameof(CurrentScopeHolder))]
public class CurrentScopeHolderTests
{
    [Fact]
    public void Constructor_WithNullScopeFactory_ThrowsArgumentNullException()
    {
        var ex = Should.Throw<ArgumentNullException>(() => new CurrentScopeHolder(null!));

        ex.ParamName.ShouldBe("scopeFactory");
    }

    [Fact]
    public void CurrentScope_OnFirstAccess_CallsCreateScopeOnceAndReturnsThatScope()
    {
        var scope = new Mock<IServiceScope>();
        scope.Setup(s => s.ServiceProvider).Returns(Mock.Of<IServiceProvider>());

        var factory = new Mock<IServiceScopeFactory>();
        factory.Setup(f => f.CreateScope()).Returns(scope.Object);

        var holder = new CurrentScopeHolder(factory.Object);

        var current = holder.CurrentScope;

        current.ShouldBe(scope.Object);
        factory.Verify(f => f.CreateScope(), Times.Once);
    }

    [Fact]
    public void CurrentScope_OnSubsequentAccess_ReturnsSameScopeWithoutCallingCreateScopeAgain()
    {
        var scope = new Mock<IServiceScope>();
        scope.Setup(s => s.ServiceProvider).Returns(Mock.Of<IServiceProvider>());

        var factory = new Mock<IServiceScopeFactory>();
        factory.Setup(f => f.CreateScope()).Returns(scope.Object);

        var holder = new CurrentScopeHolder(factory.Object);

        var first = holder.CurrentScope;
        var second = holder.CurrentScope;

        first.ShouldBe(second);
        factory.Verify(f => f.CreateScope(), Times.Once);
    }

    [Fact]
    public void ResetScope_DisposesCurrentScopeAndNextCurrentScopeReturnsNewScope()
    {
        var scope1 = new Mock<IServiceScope>();
        scope1.Setup(s => s.ServiceProvider).Returns(Mock.Of<IServiceProvider>());

        var scope2 = new Mock<IServiceScope>();
        scope2.Setup(s => s.ServiceProvider).Returns(Mock.Of<IServiceProvider>());

        var callCount = 0;
        var factory = new Mock<IServiceScopeFactory>();
        factory.Setup(f => f.CreateScope())
            .Returns(() => (++callCount == 1 ? scope1 : scope2).Object);

        var holder = new CurrentScopeHolder(factory.Object);

        var first = holder.CurrentScope;
        first.ShouldBe(scope1.Object);

        holder.ResetScope();

        scope1.Verify(s => s.Dispose(), Times.Once);

        var afterReset = holder.CurrentScope;
        afterReset.ShouldBe(scope2.Object);
        factory.Verify(f => f.CreateScope(), Times.Exactly(2));
    }

    [Fact]
    public void ResetScope_WhenNoScopeCreatedYet_DoesNotThrow_AndNextCurrentScopeCreatesOne()
    {
        var scope = new Mock<IServiceScope>();
        scope.Setup(s => s.ServiceProvider).Returns(Mock.Of<IServiceProvider>());

        var factory = new Mock<IServiceScopeFactory>();
        factory.Setup(f => f.CreateScope()).Returns(scope.Object);

        var holder = new CurrentScopeHolder(factory.Object);

        holder.ResetScope();

        var current = holder.CurrentScope;
        current.ShouldBe(scope.Object);
        factory.Verify(f => f.CreateScope(), Times.Once);
    }
}
