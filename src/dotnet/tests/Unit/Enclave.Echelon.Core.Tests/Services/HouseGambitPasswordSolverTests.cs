using Enclave.Echelon.Core.Models;
using Enclave.Echelon.Core.Services;

namespace Enclave.Echelon.Core.Tests.Services;

/// <summary>
/// Unit tests for <see cref="HouseGambitPasswordSolver"/> (HOUSE gambit, blind random choice). Same contract as base; accepts any candidate for multi-candidate sets.
/// </summary>
[UnitTest, TestOf(nameof(HouseGambitPasswordSolver))]
public class HouseGambitPasswordSolverTests : PasswordSolverTestsBase
{
    private const int Seed = 42;

    protected override IPasswordSolver Solver { get; } = new HouseGambitPasswordSolver(new GameRandom(Seed));

    // All candidates are acceptable for all keys, since the solver is blind to scores and just picks randomly from the full set of candidates.
    protected override Dictionary<string, HashSet<Password>> AcceptableGuesses => TestData;

    // All candidates are acceptable for all keys, since the solver is blind to scores and just picks randomly from the full set of candidates.
    protected override Dictionary<string, HashSet<Password>> ExpectedBestGuesses => TestData;

    protected override void AssertGetBestGuessesFor(IReadOnlyList<Password> result, IEnumerable<Password> candidates, string key)
    {
        result.ShouldHaveSingleItem();
        candidates.ShouldContain(result[0]);
    }
}
