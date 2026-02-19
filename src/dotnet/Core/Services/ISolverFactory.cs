namespace Enclave.Echelon.Core.Services;

/// <summary>
/// Factory that creates or returns the <see cref="IPasswordSolver"/> instance according to configuration.
/// </summary>
public interface ISolverFactory
{
    /// <summary>
    /// Returns the password solver to use (e.g. chosen by intelligence level from config).
    /// </summary>
    IPasswordSolver GetSolver();
}
