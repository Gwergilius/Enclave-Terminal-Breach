using Enclave.Echelon.Core.Models;
using Enclave.Echelon.Core.Services;

namespace Enclave.Echelon.Core.Tests.Services;

/// <summary>
/// Shared unit tests for all <see cref="IPasswordSolver"/> implementations.
/// Verifies contract and Algorithm.md behaviour (information score, narrowing). Derived classes provide the solver and optional strategy-specific expectations.
/// </summary>
[UnitTest]
public abstract class PasswordSolverTestsBase
{
    protected abstract IPasswordSolver Solver { get; }

    #region GetBestGuess

    [Fact]
    public void GetBestGuess_WithEmptyCandidates_ReturnsNull()
    {
        var candidates = Array.Empty<Password>();

        var result = Solver.GetBestGuess(candidates);

        result.ShouldBeNull();
    }

    [Fact]
    public void GetBestGuess_WithSingleCandidate_ReturnsThatPassword()
    {
        var single = new Password("TERMS");
        var candidates = new[] { single };

        var result = Solver.GetBestGuess(candidates);

        result.ShouldBe(single);
        result!.Word.ShouldBe("TERMS");
    }

    /// <summary>
    /// Algorithm.md: TERMS, TEXAS, TIRES, TANKS — GetBestGuess returns one of the optimal guesses (score 3: TERMS, TEXAS, TIRES); TANKS has score 2.
    /// Strategy may return one of the best-scoring set or any candidate; derived classes can override expected set.
    /// </summary>
    [Fact]
    public virtual void GetBestGuess_WithTermsTexasTiresTanks_ReturnsOneOfBestScoringGuesses()
    {
        var candidates = new[] { "TERMS", "TEXAS", "TIRES", "TANKS" }.Select(w => new Password(w)).ToList();
        var acceptableWords = GetAcceptableBestGuessWordsForTermsTexasTiresTanks();

        var result = Solver.GetBestGuess(candidates);

        result.ShouldNotBeNull();
        acceptableWords.ShouldContain(result.Word);
    }

    /// <summary>
    /// Algorithm.md: SALES is the only guess with four distinct outcomes for SALES, SALTY, SAUCE, SAVES. Derived may accept only SALES or any (e.g. Random).
    /// </summary>
    [Fact]
    public virtual void GetBestGuess_WithSalesSaltySauceSaves_ReturnsSales()
    {
        var candidates = new[] { "SALES", "SALTY", "SAUCE", "SAVES" }.Select(w => new Password(w)).ToList();
        var acceptable = GetAcceptableBestGuessWordsForSalesSaltySauceSaves();

        var result = Solver.GetBestGuess(candidates);

        result.ShouldNotBeNull();
        acceptable.ShouldContain(result.Word);
    }

    /// <summary>
    /// Algorithm.md Example 3 (tie-breaker): DANTA, DHOBI, LILTS, OAKUM, ALEFS — LILTS has score 3 and smallest worst-case (2).
    /// Strategy may return LILTS only or one of the best-score set; derived classes override expected set if needed.
    /// </summary>
    [Fact]
    public virtual void GetBestGuess_WithDantaDhobiLiltsOakumAlefs_ReturnsOneOfBest()
    {
        var candidates = new[] { "DANTA", "DHOBI", "LILTS", "OAKUM", "ALEFS" }.Select(w => new Password(w)).ToList();
        var acceptableWords = GetAcceptableBestGuessWordsForDantaDhobiLiltsOakumAlefs();

        var result = Solver.GetBestGuess(candidates);

        result.ShouldNotBeNull();
        acceptableWords.ShouldContain(result.Word);
    }

    #endregion

    #region GetBestGuesses

    [Fact]
    public void GetBestGuesses_WithNullCandidates_ThrowsArgumentNullException()
    {
        IEnumerable<Password>? candidates = null;

        var ex = Should.Throw<ArgumentNullException>(() => Solver.GetBestGuesses(candidates!));

        ex.ParamName.ShouldBe("candidates");
    }

    [Fact]
    public void GetBestGuesses_WithEmptyCandidates_ReturnsEmptyList()
    {
        var candidates = Array.Empty<Password>();

        var result = Solver.GetBestGuesses(candidates);

        result.ShouldBeEmpty();
    }

    [Fact]
    public void GetBestGuesses_WithSingleCandidate_ReturnsThatPassword()
    {
        var single = new Password("TERMS");
        var candidates = new[] { single };

        var result = Solver.GetBestGuesses(candidates);

        result.ShouldHaveSingleItem();
        result[0].Word.ShouldBe("TERMS");
    }

    /// <summary>
    /// Algorithm.md: TERMS, TEXAS, TIRES, TANKS — best guesses have score 3 (TERMS, TEXAS, TIRES). Derived classes define expected count and set.
    /// </summary>
    [Fact]
    public virtual void GetBestGuesses_WithTermsTexasTiresTanks_ReturnsExpectedBestSet()
    {
        var candidates = new[] { "TERMS", "TEXAS", "TIRES", "TANKS" }.Select(w => new Password(w)).ToList();
        var result = Solver.GetBestGuesses(candidates);
        AssertGetBestGuessesForTermsTexasTiresTanks(result, candidates);
    }

    /// <summary>
    /// Algorithm.md: SALES, SALTY, SAUCE, SAVES — only SALES has score 4. Derived may assert single SALES or single any (e.g. Random).
    /// </summary>
    [Fact]
    public virtual void GetBestGuesses_WithSalesSaltySauceSaves_ReturnsOnlySales()
    {
        var candidates = new[] { "SALES", "SALTY", "SAUCE", "SAVES" }.Select(w => new Password(w)).ToList();
        var result = Solver.GetBestGuesses(candidates);
        AssertGetBestGuessesForSalesSaltySauceSaves(result, candidates);
    }

    /// <summary>
    /// Algorithm.md Example 3: tie-breaker — only LILTS has worst-case 2. Derived classes define expected count and set.
    /// </summary>
    [Fact]
    public virtual void GetBestGuesses_WithDantaDhobiLiltsOakumAlefs_ReturnsExpectedBestSet()
    {
        var candidates = new[] { "DANTA", "DHOBI", "LILTS", "OAKUM", "ALEFS" }.Select(w => new Password(w)).ToList();
        var result = Solver.GetBestGuesses(candidates);
        AssertGetBestGuessesForDantaDhobiLiltsOakumAlefs(result, candidates);
    }

    #endregion

    #region CalculateInformationScore (Algorithm.md requirements)

    [Fact]
    public void CalculateInformationScore_SalesAgainstFourCandidates_ReturnsScore4()
    {
        var guess = new Password("SALES");
        var candidates = new[] { "SALES", "SALTY", "SAUCE", "SAVES" }.Select(w => new Password(w)).ToList();

        var scoreInfo = Solver.CalculateInformationScore(guess, candidates);

        scoreInfo.Password.Word.ShouldBe("SALES");
        scoreInfo.Value.ShouldBe(4);
        scoreInfo.WorstCase.ShouldBe(1);
    }

    [Fact]
    public void CalculateInformationScore_TermsAgainstFourCandidates_ReturnsScore3()
    {
        var guess = new Password("TERMS");
        var candidates = new[] { "TERMS", "TEXAS", "TIRES", "TANKS" }.Select(w => new Password(w)).ToList();

        var scoreInfo = Solver.CalculateInformationScore(guess, candidates);

        scoreInfo.Value.ShouldBe(3);
        scoreInfo.WorstCase.ShouldBe(2);
    }

    [Fact]
    public void CalculateInformationScore_TanksAgainstFourCandidates_ReturnsScore2WorstCase3()
    {
        var guess = new Password("TANKS");
        var candidates = new[] { "TERMS", "TEXAS", "TIRES", "TANKS" }.Select(w => new Password(w)).ToList();

        var scoreInfo = Solver.CalculateInformationScore(guess, candidates);

        scoreInfo.Value.ShouldBe(2);
        scoreInfo.WorstCase.ShouldBe(3);
    }

    [Fact]
    public void CalculateInformationScore_TexasAgainstFourCandidates_ReturnsScore3WorstCase2()
    {
        var guess = new Password("TEXAS");
        var candidates = new[] { "TERMS", "TEXAS", "TIRES", "TANKS" }.Select(w => new Password(w)).ToList();

        var scoreInfo = Solver.CalculateInformationScore(guess, candidates);

        scoreInfo.Value.ShouldBe(3);
        scoreInfo.WorstCase.ShouldBe(2);
    }

    [Fact]
    public void CalculateInformationScore_LiltsAgainstFiveCandidates_ReturnsScore3WorstCase2()
    {
        var guess = new Password("LILTS");
        var candidates = new[] { "DANTA", "DHOBI", "LILTS", "OAKUM", "ALEFS" }.Select(w => new Password(w)).ToList();

        var scoreInfo = Solver.CalculateInformationScore(guess, candidates);

        scoreInfo.Value.ShouldBe(3);
        scoreInfo.WorstCase.ShouldBe(2);
    }

    [Fact]
    public void CalculateInformationScore_WithSingleCandidate_SelfComparisonIncluded()
    {
        var guess = new Password("TERMS");
        var candidates = new[] { guess };

        var scoreInfo = Solver.CalculateInformationScore(guess, candidates);

        scoreInfo.Value.ShouldBe(1);
        scoreInfo.WorstCase.ShouldBe(1);
        scoreInfo[5].ShouldBe(1);
    }

    [Fact]
    public void CalculateInformationScore_WithNullPassword_ThrowsArgumentNullException()
    {
        Password? password = null;
        var candidates = new[] { new Password("TERMS") };

        var ex = Should.Throw<ArgumentNullException>(() => Solver.CalculateInformationScore(password!, candidates));

        ex.ParamName.ShouldBe("password");
    }

    [Fact]
    public void CalculateInformationScore_WithNullCandidates_ThrowsArgumentNullException()
    {
        var password = new Password("TERMS");
        IEnumerable<Password>? candidates = null;

        var ex = Should.Throw<ArgumentNullException>(() => Solver.CalculateInformationScore(password, candidates!));

        ex.ParamName.ShouldBe("candidates");
    }

    #endregion

    #region NarrowCandidates

    [Fact]
    public void NarrowCandidates_WithNullCandidates_ThrowsArgumentNullException()
    {
        IEnumerable<Password>? candidates = null;
        var guess = new Password("TERMS");

        var ex = Should.Throw<ArgumentNullException>(() => Solver.NarrowCandidates(candidates!, guess, 2));

        ex.ParamName.ShouldBe("candidates");
    }

    [Fact]
    public void NarrowCandidates_WithNullGuess_ThrowsArgumentNullException()
    {
        var candidates = new[] { new Password("TERMS") };
        Password? guess = null;

        var ex = Should.Throw<ArgumentNullException>(() => Solver.NarrowCandidates(candidates, guess!, 2));

        ex.ParamName.ShouldBe("guess");
    }

    [Fact]
    public void NarrowCandidates_KeepsOnlyCandidatesWithSameMatchCountAsResponse()
    {
        var candidates = new[] { "TERMS", "TEXAS", "TIRES", "TANKS" }.Select(w => new Password(w)).ToList();
        var guess = new Password("TERMS");
        var response = 2;

        var result = Solver.NarrowCandidates(candidates, guess, response);

        result.Count.ShouldBe(1);
        result[0].Word.ShouldBe("TANKS");
    }

    #endregion

    #region Strategy-specific expectations (override in derived classes)

    /// <summary>Acceptable words for GetBestGuess(TERMS, TEXAS, TIRES, TANKS). Default: best score only (exclude TANKS).</summary>
    protected virtual IReadOnlySet<string> GetAcceptableBestGuessWordsForTermsTexasTiresTanks() =>
        new HashSet<string> { "TERMS", "TEXAS", "TIRES" };

    /// <summary>Acceptable words for GetBestGuess(DANTA, DHOBI, LILTS, OAKUM, ALEFS). Default: LILTS only (tie-breaker).</summary>
    protected virtual IReadOnlySet<string> GetAcceptableBestGuessWordsForDantaDhobiLiltsOakumAlefs() =>
        new HashSet<string> { "LILTS" };

    /// <summary>Acceptable words for GetBestGuess(SALES, SALTY, SAUCE, SAVES). Default: SALES only (only best score).</summary>
    protected virtual IReadOnlySet<string> GetAcceptableBestGuessWordsForSalesSaltySauceSaves() =>
        new HashSet<string> { "SALES" };

    /// <summary>Expected (count, sorted words) for GetBestGuesses(TERMS, TEXAS, TIRES, TANKS). Default: 3 items (TERMS, TEXAS, TIRES).</summary>
    protected virtual (int Count, IEnumerable<string> Words) GetExpectedGetBestGuessesForTermsTexasTiresTanks() =>
        (3, new[] { "TERMS", "TEXAS", "TIRES" });

    /// <summary>Expected (count, sorted words) for GetBestGuesses(DANTA, DHOBI, LILTS, OAKUM, ALEFS). Default: 1 item (LILTS).</summary>
    protected virtual (int Count, IEnumerable<string> Words) GetExpectedGetBestGuessesForDantaDhobiLiltsOakumAlefs() =>
        (1, new[] { "LILTS" });

    /// <summary>Assert result of GetBestGuesses(TERMS, TEXAS, TIRES, TANKS). Default: count and exact set.</summary>
    protected virtual void AssertGetBestGuessesForTermsTexasTiresTanks(IReadOnlyList<Password> result, List<Password> candidates)
    {
        var (expectedCount, expectedWords) = GetExpectedGetBestGuessesForTermsTexasTiresTanks();
        result.Count.ShouldBe(expectedCount);
        result.Select(p => p.Word).OrderBy(w => w).ShouldBe(expectedWords.OrderBy(w => w));
    }

    /// <summary>Assert result of GetBestGuesses(DANTA, DHOBI, LILTS, OAKUM, ALEFS). Default: count and exact set.</summary>
    protected virtual void AssertGetBestGuessesForDantaDhobiLiltsOakumAlefs(IReadOnlyList<Password> result, List<Password> candidates)
    {
        var (expectedCount, expectedWords) = GetExpectedGetBestGuessesForDantaDhobiLiltsOakumAlefs();
        result.Count.ShouldBe(expectedCount);
        result.Select(p => p.Word).OrderBy(w => w).ShouldBe(expectedWords.OrderBy(w => w));
    }

    /// <summary>Assert result of GetBestGuesses(SALES, SALTY, SAUCE, SAVES). Default: single SALES.</summary>
    protected virtual void AssertGetBestGuessesForSalesSaltySauceSaves(IReadOnlyList<Password> result, List<Password> candidates)
    {
        result.ShouldHaveSingleItem();
        result[0].Word.ShouldBe("SALES");
    }

    #endregion
}
