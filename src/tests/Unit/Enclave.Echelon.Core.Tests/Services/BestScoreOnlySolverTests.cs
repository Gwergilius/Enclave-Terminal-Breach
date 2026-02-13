using Enclave.Echelon.Core.Services;

namespace Enclave.Echelon.Core.Tests.Services;

/// <summary>
/// Unit tests for <see cref="BestScoreOnlySolver"/> (best score only, random tie-break). Same contract as base; best-score set matches TieBreaker for these examples.
/// For DANTA/DHOBI/LILTS/OAKUM/ALEFS all five share the same best score, so all five are returned.
/// </summary>
[UnitTest, TestOf(nameof(BestScoreOnlySolver))]
public class BestScoreOnlySolverTests : PasswordSolverTestsBase
{
    private const int Seed = 42;

    protected override IPasswordSolver Solver { get; } = new BestScoreOnlySolver(Seed);

    protected override IReadOnlySet<string> GetAcceptableBestGuessWordsForDantaDhobiLiltsOakumAlefs() =>
        new HashSet<string> { "DANTA", "DHOBI", "LILTS", "OAKUM", "ALEFS" };

    protected override (int Count, IEnumerable<string> Words) GetExpectedGetBestGuessesForDantaDhobiLiltsOakumAlefs() =>
        (5, new[] { "DANTA", "DHOBI", "LILTS", "OAKUM", "ALEFS" });
}
