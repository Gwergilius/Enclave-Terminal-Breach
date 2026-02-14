using Enclave.Echelon.Core.Services;

namespace Enclave.Echelon.Core.Tests.Services;

/// <summary>
/// Unit tests for <see cref="TieBreakerPasswordSolver"/> (tie-breaker strategy). Uses shared base; expectations match Algorithm.md tie-breaker behaviour.
/// </summary>
[UnitTest, TestOf(nameof(TieBreakerPasswordSolver))]
public class TieBreakerPasswordSolverTests : PasswordSolverTestsBase
{
    protected override IPasswordSolver Solver { get; } = new TieBreakerPasswordSolver();
}
