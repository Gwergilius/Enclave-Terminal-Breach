using Enclave.Echelon.Core.Services;

namespace Enclave.Echelon.Core.Tests.Services;

/// <summary>
/// Unit tests for <see cref="PasswordSolver"/> (tie-breaker strategy). Uses shared base; expectations match Algorithm.md tie-breaker behaviour.
/// </summary>
[UnitTest, TestOf(nameof(PasswordSolver))]
public class PasswordSolverTests : PasswordSolverTestsBase
{
    protected override IPasswordSolver Solver { get; } = new PasswordSolver();
}
