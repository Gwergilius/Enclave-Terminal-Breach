using Enclave.Echelon.Core.Services;

namespace Enclave.Ghost.Services;

/// <summary>
/// Default solver configuration for GHOST: BestBucket (intelligence level 1).
/// </summary>
public sealed class GhostSolverConfig : ISolverConfiguration
{
    /// <inheritdoc />
    public SolverLevel Level => SolverLevel.BestBucket;
}
