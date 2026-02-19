namespace Enclave.Common.Extensions;

/// <summary>
/// Extension methods for <see cref="Random"/>.
/// </summary>
public static class RandomExtensions
{
    /// <summary>
    /// Returns the instance if not null; otherwise a new <see cref="Random"/> (stochastic).
    /// Use when an optional <see cref="Random"/> must be non-null for the rest of the logic.
    /// </summary>
    /// <param name="random">The optional random number generator.</param>
    /// <returns>The same instance, or a new <see cref="Random"/> if <paramref name="random"/> is null.</returns>
    public static Random Enforce(this Random? random) => random ?? new Random();
}
