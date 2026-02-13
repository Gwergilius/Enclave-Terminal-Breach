using Enclave.Echelon.Core.Models;
using Enclave.Echelon.Core.Services;

namespace Enclave.Echelon.Core.Tests.Services;

/// <summary>
/// Unit tests for <see cref="RandomGuessSolver"/> (blind random choice). Same contract as base; accepts any candidate for multi-candidate sets.
/// </summary>
[UnitTest, TestOf(nameof(RandomGuessSolver))]
public class RandomGuessSolverTests : PasswordSolverTestsBase
{
    private const int Seed = 42;

    protected override IPasswordSolver Solver { get; } = new RandomGuessSolver(Seed);

    protected override IReadOnlySet<string> GetAcceptableBestGuessWordsForTermsTexasTiresTanks() =>
        new HashSet<string> { "TERMS", "TEXAS", "TIRES", "TANKS" };

    protected override IReadOnlySet<string> GetAcceptableBestGuessWordsForDantaDhobiLiltsOakumAlefs() =>
        new HashSet<string> { "DANTA", "DHOBI", "LILTS", "OAKUM", "ALEFS" };

    protected override IReadOnlySet<string> GetAcceptableBestGuessWordsForSalesSaltySauceSaves() =>
        new HashSet<string> { "SALES", "SALTY", "SAUCE", "SAVES" };

    protected override (int Count, IEnumerable<string> Words) GetExpectedGetBestGuessesForTermsTexasTiresTanks() =>
        (1, Array.Empty<string>()); // single random element; assertion overridden below

    protected override (int Count, IEnumerable<string> Words) GetExpectedGetBestGuessesForDantaDhobiLiltsOakumAlefs() =>
        (1, Array.Empty<string>());

    protected override void AssertGetBestGuessesForTermsTexasTiresTanks(IReadOnlyList<Password> result, List<Password> candidates)
    {
        result.Count.ShouldBe(1);
        var allowed = new HashSet<string> { "TERMS", "TEXAS", "TIRES", "TANKS" };
        allowed.ShouldContain(result[0].Word);
    }

    protected override void AssertGetBestGuessesForDantaDhobiLiltsOakumAlefs(IReadOnlyList<Password> result, List<Password> candidates)
    {
        result.Count.ShouldBe(1);
        var allowed = new HashSet<string> { "DANTA", "DHOBI", "LILTS", "OAKUM", "ALEFS" };
        allowed.ShouldContain(result[0].Word);
    }

    protected override void AssertGetBestGuessesForSalesSaltySauceSaves(IReadOnlyList<Password> result, List<Password> candidates)
    {
        result.ShouldHaveSingleItem();
        var allowed = new HashSet<string> { "SALES", "SALTY", "SAUCE", "SAVES" };
        allowed.ShouldContain(result[0].Word);
    }
}
