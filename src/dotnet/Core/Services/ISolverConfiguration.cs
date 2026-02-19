namespace Enclave.Echelon.Core.Services;

/// <summary>
/// Configuration that supplies the solver level (strategy) to use.
/// </summary>
public interface ISolverConfiguration
{
    /// <summary>
    /// The requested solver level. Invalid or unknown values are treated as <see cref="SolverLevel.Default"/> by the factory.
    /// </summary>
    SolverLevel Level { get; }
}
