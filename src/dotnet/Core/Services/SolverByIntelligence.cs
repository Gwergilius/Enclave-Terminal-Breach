namespace Enclave.Echelon.Core.Services;

/// <summary>
/// Maps intelligence level (0, 1, 2) to <see cref="IPasswordSolver"/> implementation.
/// </summary>
public static class SolverByIntelligence
{
    /// <summary>
    /// Default intelligence level used when the requested level is out of range (0–2).
    /// </summary>
    public const int DefaultLevel = 1;

    private static readonly Dictionary<int, Func<int, IPasswordSolver>> _solvers = new()
    {
        [0] = seed => new HouseGambitPasswordSolver(seed),
        [1] = seed => new BestBucketPasswordSolver(seed),
        [2] = _ => new TieBreakerPasswordSolver(),
    };

    /// <summary>
    /// Returns the solver for the given intelligence level (0 = HouseGambit, 1 = BestBucket, 2 = TieBreaker).
    /// Levels outside 0–2 fall back to <see cref="DefaultLevel"/>.
    /// </summary>
    public static IPasswordSolver GetSolver(int intelligence, int seed)
    {
        return _solvers.TryGetValue(intelligence, out var factory)
            ? factory(seed)
            : _solvers[DefaultLevel](seed);
    }
}
