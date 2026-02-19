using Enclave.Common.Extensions;
using Enclave.Common.Test.Core;

namespace Enclave.Echelon.Common.Tests.Extensions;

/// <summary>
/// Unit tests for <see cref="RandomExtensions"/>.
/// </summary>
[UnitTest, TestOf(nameof(RandomExtensions))]
public class RandomExtensionsTests
{
    [Fact]
    public void Enforce_WhenNull_ReturnsNewRandom()
    {
        Random? random = null;

        var result = random.Enforce();

        result.ShouldNotBeNull();
    }

    [Fact]
    public void Enforce_WhenNotNull_ReturnsSameInstance()
    {
        var random = new Random(42);

        var result = random.Enforce();

        result.ShouldBeSameAs(random);
    }
}
