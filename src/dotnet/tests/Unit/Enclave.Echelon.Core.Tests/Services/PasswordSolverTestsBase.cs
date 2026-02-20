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

    private static readonly Dictionary<string, HashSet<Password>> _testData = new ()
    {
        ["TERMS"] = [.. "TERMS TEXAS TIRES TANKS".Split().Select(w => new Password(w))],
        ["SALES"] = [.. "SALES SALTY SAUCE SAVES".Split().Select(w => new Password(w))],
        ["DANTA"] = [.. "DANTA DHOBI LILTS OAKUM ALEFS".Split().Select(w => new Password(w))]
    };

    private static readonly Dictionary<string, HashSet<Password>> _acceptableGuesses = new ()
    {
        ["TERMS"] = [.. "TERMS TEXAS TIRES".Split().Select(w => new Password(w))],
        ["SALES"] = [.. "SALES".Split().Select(w => new Password(w))],
        ["DANTA"] = [.. "LILTS".Split().Select(w => new Password(w))]
    };

    private static readonly Dictionary<string, HashSet<Password>> _expectedBestGuesses = new ()
    {
        ["TERMS"] = [.. "TERMS TEXAS TIRES".Split().Select(w => new Password(w))],
        ["SALES"] = [.. "SALES".Split().Select(w => new Password(w))],
        ["DANTA"] = [.. "LILTS".Split().Select(w => new Password(w))]
    };

    private static readonly Dictionary<string, (int Value, int WorstCase)> _expectedScores = new()
    {
        ["TERMS"] = (3, 2),
        ["TANKS"] = (2, 3),
        ["TEXAS"] = (3, 2),
        ["SALES"] = (4, 1),
        ["LILTS"] = (3, 2)
    };

    protected virtual Dictionary<string, HashSet<Password>> TestData => _testData;
    protected virtual Dictionary<string, HashSet<Password>> AcceptableGuesses => _acceptableGuesses;
    protected virtual Dictionary<string, HashSet<Password>> ExpectedBestGuesses => _expectedBestGuesses;
    protected virtual Dictionary<string, (int Value, int WorstCase)> ExpectedScores => _expectedScores;

    #region GetBestGuess

    [Fact]
    public void GetBestGuess_WithEmptyCandidates_ReturnsNull()
    {
        Password[] candidates = [];

        var result = Solver.GetBestGuess(candidates);

        result.ShouldBeNull();
    }

    [Theory, InlineData("TERMS")]
    public void GetBestGuess_WithSingleCandidate_ReturnsThatPassword(string word)
    {
        var single = new Password(word);
        Password[] candidates = [single];

        var result = Solver.GetBestGuess(candidates);

        result.ShouldBe(single);
        result!.Word.ShouldBe(word);
    }

    /// <summary>
    /// Algorithm.md examples
    /// Strategy may return one of the best-scoring set or any candidate; derived classes can override expected set.
    /// </summary>
    /// <example>
    /// TERMS, TEXAS, TIRES, TANKS — GetBestGuess returns one of the optimal guesses (score 3: TERMS, TEXAS, TIRES); TANKS has score 2.
    /// </example>
    /// <example>
    /// SALES is the only guess with four distinct outcomes for SALES, SALTY, SAUCE, SAVES. Derived may accept only SALES or any (e.g. Random).
    /// </example>
    /// <example>
    /// Example 3 (tie-breaker): DANTA, DHOBI, LILTS, OAKUM, ALEFS — LILTS has score 3 and smallest worst-case (2).
    /// Strategy may return LILTS only or one of the best-score set; derived classes override expected set if needed.
    /// </example>
    [Theory]
    [InlineData("TERMS")]
    [InlineData("SALES")]
    [InlineData("DANTA")]
    public virtual void GetBestGuess_ReturnsOneOfBestScoringGuesses(string example)
    {
        var candidates = TestData[example];
        var acceptableWords = AcceptableGuesses[example];

        var result = Solver.GetBestGuess(candidates);

        result.ShouldNotBeNull();
        acceptableWords.ShouldContain(result);
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
        Password[] candidates = [];

        var result = Solver.GetBestGuesses(candidates);

        result.ShouldBeEmpty();
    }

    [Theory, InlineData("TERMS")]
    public void GetBestGuesses_WithSingleCandidate_ReturnsThatPassword(string word)
    {
        var single = new Password(word);
        var candidates = new[] { single };

        var result = Solver.GetBestGuesses(candidates);

        result.ShouldHaveSingleItem();
        result[0].Word.ShouldBe(word);
    }

    /// <summary>
    /// Algorithm.md:
    /// <list type="bullet">
    /// <item>TERMS, TEXAS, TIRES have score 3; TANKS has score 2. Derived may return all three or any subset (e.g. Random).</item>
    /// <item>SALES, SALTY, SAUCE, SAVES — only SALES has score 4. Derived may assert single SALES or single any (e.g. Random).</item>
    /// <item>Example 3: tie-breaker — only LILTS has worst-case 2. Derived classes define expected count and set</item>
    /// </list>
    /// Derived classes define expected count and set.
    /// </summary>
    [Theory]
    [InlineData("TERMS")]
    [InlineData("SALES")]
    [InlineData("DANTA")]
    public virtual void GetBestGuesses_ReturnsExpectedBestSet(string key)
    {
        var candidates = TestData[key];        
        var result = Solver.GetBestGuesses(candidates);
        AssertGetBestGuessesFor(result, candidates, key);
    }
    #endregion

    #region CalculateInformationScore (Algorithm.md requirements)

    [Theory]
    [InlineData("SALES")]
    [InlineData("TERMS")]
    [InlineData("TANKS")]
    [InlineData("TEXAS")]
    [InlineData("LILTS")]
    public void CalculateInformationScore(string word)
    {
        var guess = new Password(word);
        var candidates = TestData.Values.First(list => list.Contains(guess));
        (int expectedValue, int expectedWorstCase) = ExpectedScores[word];

        var scoreInfo = Solver.CalculateInformationScore(guess, candidates);

        scoreInfo.Password.Word.ShouldBe(word);
        scoreInfo.Value.ShouldBe(expectedValue);
        scoreInfo.WorstCase.ShouldBe(expectedWorstCase);
    }

    [Fact]
    public void CalculateInformationScore_WithSingleCandidate_SelfComparisonIncluded()
    {
        var guess = new Password("TERMS");
        var candidates = new[] { guess };

        var scoreInfo = Solver.CalculateInformationScore(guess, candidates);

        scoreInfo.Value.ShouldBe(1);
        scoreInfo.WorstCase.ShouldBe(1);
        scoreInfo[5].ShouldBe(1);   // Bucket size (value) belonging to a specific matchcount
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
        var candidates = TestData["TERMS"];
        var guess = new Password("TERMS");
        var response = 2;

        var result = Solver.NarrowCandidates(candidates, guess, response);

        result.Count.ShouldBe(1);
        result[0].Word.ShouldBe("TANKS");
    }

    #endregion

    #region Strategy-specific expectations (override in derived classes)
    /** /
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
    /**/

    /// <summary>Assert result of GetBestGuesses(TERMS, TEXAS, TIRES, TANKS). Default: count and exact set.</summary>
    protected virtual void AssertGetBestGuessesFor(IReadOnlyList<Password> result, IEnumerable<Password> candidates, string key)
    {
        var expectedWords = ExpectedBestGuesses[key];
        var expectedCount = expectedWords.Count; // ensure count matches words for easier override
        var message = $"Expected [{string.Join(", ", expectedWords.Words())}], but got [{string.Join(", ", result.Words())}]";
        result.Count.ShouldBe(expectedCount, message);
        result.Words().ShouldBeEquivalentTo(expectedWords.Words(), message);
    }

    #endregion
}
