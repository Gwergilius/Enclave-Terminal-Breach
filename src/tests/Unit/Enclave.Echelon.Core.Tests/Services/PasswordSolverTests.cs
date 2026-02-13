using Enclave.Echelon.Core.Models;
using Enclave.Echelon.Core.Services;

namespace Enclave.Echelon.Core.Tests.Services;

/// <summary>
/// Unit tests for <see cref="PasswordSolver"/>.
/// Verifies behavior against the requirements in docs/Architecture/Algorithm.md (information score, tie-breaker, best guess).
/// </summary>
[UnitTest, TestOf(nameof(PasswordSolver))]
public class PasswordSolverTests
{
    private readonly IPasswordSolver _solver = new PasswordSolver();

    #region GetBestGuess

    [Fact]
    public void GetBestGuess_WithEmptyCandidates_ReturnsNull()
    {
        var candidates = Array.Empty<Password>();

        var result = _solver.GetBestGuess(candidates);

        result.ShouldBeNull();
    }

    [Fact]
    public void GetBestGuess_WithSingleCandidate_ReturnsThatPassword()
    {
        var single = new Password("TERMS");
        var candidates = new[] { single };

        var result = _solver.GetBestGuess(candidates);

        result.ShouldBe(single);
        result!.Word.ShouldBe("TERMS");
    }

    /// <summary>
    /// Algorithm.md Example: TERMS, TEXAS, TIRES, TANKS — GetBestGuess returns one of the optimal guesses (score 3: TERMS, TEXAS, TIRES).
    /// </summary>
    [Fact]
    public void GetBestGuess_WithTermsTexasTiresTanks_ReturnsOneOfBestScoringGuesses()
    {
        var candidates = new[] { "TERMS", "TEXAS", "TIRES", "TANKS" }.Select(w => new Password(w)).ToList();
        var bestWords = new HashSet<string> { "TERMS", "TEXAS", "TIRES" };

        var result = _solver.GetBestGuess(candidates);

        result.ShouldNotBeNull();
        bestWords.ShouldContain(result.Word);
        result!.Word.ShouldNotBe("TANKS"); // TANKS has score 2
    }

    /// <summary>
    /// Algorithm.md Example: SALES is the only guess with four distinct outcomes for SALES, SALTY, SAUCE, SAVES.
    /// </summary>
    [Fact]
    public void GetBestGuess_WithSalesSaltySauceSaves_ReturnsSales()
    {
        var candidates = new[] { "SALES", "SALTY", "SAUCE", "SAVES" }.Select(w => new Password(w)).ToList();

        var result = _solver.GetBestGuess(candidates);

        result.ShouldNotBeNull();
        result.Word.ShouldBe("SALES");
    }

    /// <summary>
    /// Algorithm.md Example 3 (tie-breaker): DANTA, DHOBI, LILTS, OAKUM, ALEFS — LILTS has same score 3 but smallest worst-case (2).
    /// </summary>
    [Fact]
    public void GetBestGuess_WithDantaDhobiLiltsOakumAlefs_ReturnsLilts()
    {
        var candidates = new[] { "DANTA", "DHOBI", "LILTS", "OAKUM", "ALEFS" }.Select(w => new Password(w)).ToList();

        var result = _solver.GetBestGuess(candidates);

        result.ShouldNotBeNull();
        result.Word.ShouldBe("LILTS");
    }

    #endregion

    #region GetBestGuesses

    [Fact]
    public void GetBestGuesses_WithNullCandidates_ThrowsArgumentNullException()
    {
        IEnumerable<Password>? candidates = null;

        var ex = Should.Throw<ArgumentNullException>(() => _solver.GetBestGuesses(candidates!));

        ex.ParamName.ShouldBe("candidates");
    }

    [Fact]
    public void GetBestGuesses_WithEmptyCandidates_ReturnsEmptyList()
    {
        var candidates = Array.Empty<Password>();

        var result = _solver.GetBestGuesses(candidates);

        result.ShouldBeEmpty();
    }

    [Fact]
    public void GetBestGuesses_WithSingleCandidate_ReturnsThatPassword()
    {
        var single = new Password("TERMS");
        var candidates = new[] { single };

        var result = _solver.GetBestGuesses(candidates);

        result.ShouldHaveSingleItem();
        result[0].Word.ShouldBe("TERMS");
    }

    /// <summary>
    /// Algorithm.md: TERMS, TEXAS, TIRES, TANKS — best guesses have score 3.
    /// With self-comparison: TERMS, TEXAS, TIRES have 3 distinct outcomes; TANKS has only {2, 5} → score 2, so excluded.
    /// </summary>
    [Fact]
    public void GetBestGuesses_WithTermsTexasTiresTanks_ReturnsTermsTexasTires()
    {
        var candidates = new[] { "TERMS", "TEXAS", "TIRES", "TANKS" }.Select(w => new Password(w)).ToList();

        var result = _solver.GetBestGuesses(candidates);

        result.Count.ShouldBe(3);
        result.Select(p => p.Word).OrderBy(w => w).ShouldBe(new[] { "TERMS", "TEXAS", "TIRES" });
    }

    /// <summary>
    /// Algorithm.md: SALES, SALTY, SAUCE, SAVES — only SALES has score 4.
    /// </summary>
    [Fact]
    public void GetBestGuesses_WithSalesSaltySauceSaves_ReturnsOnlySales()
    {
        var candidates = new[] { "SALES", "SALTY", "SAUCE", "SAVES" }.Select(w => new Password(w)).ToList();

        var result = _solver.GetBestGuesses(candidates);

        result.ShouldHaveSingleItem();
        result[0].Word.ShouldBe("SALES");
    }

    /// <summary>
    /// Algorithm.md Example 3: tie-breaker by worst-case bucket size — only LILTS has worst-case 2.
    /// </summary>
    [Fact]
    public void GetBestGuesses_WithDantaDhobiLiltsOakumAlefs_ReturnsOnlyLilts()
    {
        var candidates = new[] { "DANTA", "DHOBI", "LILTS", "OAKUM", "ALEFS" }.Select(w => new Password(w)).ToList();

        var result = _solver.GetBestGuesses(candidates);

        result.ShouldHaveSingleItem();
        result[0].Word.ShouldBe("LILTS");
    }

    #endregion

    #region CalculateInformationScore (Algorithm.md requirements)

    /// <summary>
    /// Algorithm.md: For guess SALES vs SALES, SALTY, SAUCE, SAVES — distinct values {2, 3, 4, 5} → score 4.
    /// </summary>
    [Fact]
    public void CalculateInformationScore_SalesAgainstFourCandidates_ReturnsScore4()
    {
        var guess = new Password("SALES");
        var candidates = new[] { "SALES", "SALTY", "SAUCE", "SAVES" }.Select(w => new Password(w)).ToList();

        var scoreInfo = _solver.CalculateInformationScore(guess, candidates);

        scoreInfo.Password.Word.ShouldBe("SALES");
        scoreInfo.Value.ShouldBe(4); // distinct match counts: 2, 3, 4, 5
        scoreInfo.WorstCase.ShouldBe(1); // each bucket size 1
    }

    /// <summary>
    /// Algorithm.md: TERMS vs TERMS, TEXAS, TIRES, TANKS — distinct {2, 3, 5} → score 3.
    /// </summary>
    [Fact]
    public void CalculateInformationScore_TermsAgainstFourCandidates_ReturnsScore3()
    {
        var guess = new Password("TERMS");
        var candidates = new[] { "TERMS", "TEXAS", "TIRES", "TANKS" }.Select(w => new Password(w)).ToList();

        var scoreInfo = _solver.CalculateInformationScore(guess, candidates);

        scoreInfo.Value.ShouldBe(3);
        // Buckets: 2→1 (TANKS), 3→2 (TEXAS, TIRES), 5→1 (TERMS)
        scoreInfo.WorstCase.ShouldBe(2);
    }

    /// <summary>
    /// TANKS vs TERMS, TEXAS, TIRES, TANKS — match counts 2, 2, 2, 5 → distinct {2, 5} = 2 (score 2); worst-case 3 (three with 2).
    /// Explains why TANKS is not in GetBestGuesses for this set.
    /// </summary>
    [Fact]
    public void CalculateInformationScore_TanksAgainstFourCandidates_ReturnsScore2WorstCase3()
    {
        var guess = new Password("TANKS");
        var candidates = new[] { "TERMS", "TEXAS", "TIRES", "TANKS" }.Select(w => new Password(w)).ToList();

        var scoreInfo = _solver.CalculateInformationScore(guess, candidates);

        scoreInfo.Value.ShouldBe(2);
        scoreInfo.WorstCase.ShouldBe(3);
    }

    /// <summary>
    /// TEXAS vs TERMS, TEXAS, TIRES, TANKS — match counts 3, 5, 2, 2 (self included) → distinct {2, 3, 5} = 3; worst-case 2.
    /// </summary>
    [Fact]
    public void CalculateInformationScore_TexasAgainstFourCandidates_ReturnsScore3WorstCase2()
    {
        var guess = new Password("TEXAS");
        var candidates = new[] { "TERMS", "TEXAS", "TIRES", "TANKS" }.Select(w => new Password(w)).ToList();

        var scoreInfo = _solver.CalculateInformationScore(guess, candidates);

        scoreInfo.Value.ShouldBe(3);
        scoreInfo.WorstCase.ShouldBe(2);
    }

    /// <summary>
    /// Algorithm.md: LILTS vs DANTA, DHOBI, LILTS, OAKUM, ALEFS — score 3, bucket sizes 2,2,1 → worst-case 2.
    /// </summary>
    [Fact]
    public void CalculateInformationScore_LiltsAgainstFiveCandidates_ReturnsScore3WorstCase2()
    {
        var guess = new Password("LILTS");
        var candidates = new[] { "DANTA", "DHOBI", "LILTS", "OAKUM", "ALEFS" }.Select(w => new Password(w)).ToList();

        var scoreInfo = _solver.CalculateInformationScore(guess, candidates);

        scoreInfo.Value.ShouldBe(3);
        scoreInfo.WorstCase.ShouldBe(2);
    }

    /// <summary>
    /// Self-comparison is included: match_count(guess, guess) = word length (Algorithm.md).
    /// </summary>
    [Fact]
    public void CalculateInformationScore_WithSingleCandidate_SelfComparisonIncluded()
    {
        var guess = new Password("TERMS");
        var candidates = new[] { guess };

        var scoreInfo = _solver.CalculateInformationScore(guess, candidates);

        scoreInfo.Value.ShouldBe(1); // one distinct value: 5
        scoreInfo.WorstCase.ShouldBe(1);
        scoreInfo[5].ShouldBe(1);
    }

    [Fact]
    public void CalculateInformationScore_WithNullPassword_ThrowsArgumentNullException()
    {
        Password? password = null;
        var candidates = new[] { new Password("TERMS") };

        var ex = Should.Throw<ArgumentNullException>(() => _solver.CalculateInformationScore(password!, candidates));

        ex.ParamName.ShouldBe("password");
    }

    [Fact]
    public void CalculateInformationScore_WithNullCandidates_ThrowsArgumentNullException()
    {
        var password = new Password("TERMS");
        IEnumerable<Password>? candidates = null;

        var ex = Should.Throw<ArgumentNullException>(() => _solver.CalculateInformationScore(password, candidates!));

        ex.ParamName.ShouldBe("candidates");
    }

    #endregion

    #region NarrowCandidates

    [Fact]
    public void NarrowCandidates_WithNullCandidates_ThrowsArgumentNullException()
    {
        IEnumerable<Password>? candidates = null;
        var guess = new Password("TERMS");

        var ex = Should.Throw<ArgumentNullException>(() => _solver.NarrowCandidates(candidates!, guess, 2));

        ex.ParamName.ShouldBe("candidates");
    }

    [Fact]
    public void NarrowCandidates_WithNullGuess_ThrowsArgumentNullException()
    {
        var candidates = new[] { new Password("TERMS") };
        Password? guess = null;

        var ex = Should.Throw<ArgumentNullException>(() => _solver.NarrowCandidates(candidates, guess!, 2));

        ex.ParamName.ShouldBe("guess");
    }

    [Fact]
    public void NarrowCandidates_KeepsOnlyCandidatesWithSameMatchCountAsResponse()
    {
        var candidates = new[] { "TERMS", "TEXAS", "TIRES", "TANKS" }.Select(w => new Password(w)).ToList();
        var guess = new Password("TERMS");
        var response = 2; // TERMS vs TANKS gives 2; TERMS vs TEXAS/TIRES gives 3 and 3; TERMS vs TERMS gives 5

        var result = _solver.NarrowCandidates(candidates, guess, response);

        result.Count.ShouldBe(1);
        result[0].Word.ShouldBe("TANKS");
    }

    #endregion
}
