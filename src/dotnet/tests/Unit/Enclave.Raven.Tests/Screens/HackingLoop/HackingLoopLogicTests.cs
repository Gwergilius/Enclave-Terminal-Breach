using Enclave.Common.Test.Core;
using Enclave.Raven.Screens.HackingLoop;
using Enclave.Shared.Models;

namespace Enclave.Raven.Tests.Screens.HackingLoop;

[UnitTest, TestOf(nameof(HackingLoopLogic))]
public class HackingLoopLogicTests
{
    private static IReadOnlyList<int> AllCounts(int wordLength) =>
        Enumerable.Range(0, wordLength + 1).ToList();

    // --- GetPossibleMatchCounts ---

    [Fact]
    public void GetPossibleMatchCounts_ReturnsDistinctMatchCountsFromSession()
    {
        var guess = new Password("HELLO");
        var session = new GameSession();
        session.WordLength = 5;
        session.Add("HELLO");
        session.Add("CELLO");
        session.Add("WORLD");

        var result = HackingLoopLogic.GetPossibleMatchCounts(guess, session);

        result.ShouldBe([1, 4, 5]); // HELLO-WORLD=1, HELLO-CELLO=4, HELLO-HELLO=5
    }

    // --- FormatMatchCountPrompt ---

    [Fact]
    public void FormatMatchCountPrompt_IncludesValuesAndEnterForWordLength()
    {
        var result = HackingLoopLogic.FormatMatchCountPrompt(5, [0, 1, 4, 5]);

        result.ShouldBe("Match count (0, 1, 4, 5 or Enter for 5)? ");
    }

    // --- TryParseMatchCount: blank input ---

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TryParseMatchCount_WithBlankInput_ReturnsTrueAndWordLength(string? input)
    {
        var allowed = AllCounts(5);
        var result = HackingLoopLogic.TryParseMatchCount(input, wordLength: 5, allowed, out var value);

        result.ShouldBeTrue();
        value.ShouldBe(5);
    }

    // --- TryParseMatchCount: valid numeric input (in allowed) ---

    [Theory]
    [InlineData("0",   5, 0)]
    [InlineData("3",   5, 3)]
    [InlineData("5",   5, 5)]
    [InlineData(" 2 ", 5, 2)]
    public void TryParseMatchCount_WithValidNumberInAllowed_ReturnsTrueAndParsedValue(
        string input, int wordLength, int expected)
    {
        var allowed = AllCounts(wordLength);
        var result = HackingLoopLogic.TryParseMatchCount(input, wordLength, allowed, out var value);

        result.ShouldBeTrue();
        value.ShouldBe(expected);
    }

    [Fact]
    public void TryParseMatchCount_WithNumberNotInAllowed_ReturnsFalse()
    {
        var allowed = new[] { 0, 1, 4, 5 }; // 2 and 3 not possible for this guess
        var result = HackingLoopLogic.TryParseMatchCount("2", wordLength: 5, allowed, out _);

        result.ShouldBeFalse();
    }

    // --- TryParseMatchCount: invalid input ---

    [Theory]
    [InlineData("6",   5)]
    [InlineData("-1",  5)]
    [InlineData("abc", 5)]
    [InlineData("3.5", 5)]
    public void TryParseMatchCount_WithInvalidInput_ReturnsFalse(string input, int wordLength)
    {
        var allowed = AllCounts(wordLength);
        var result = HackingLoopLogic.TryParseMatchCount(input, wordLength, allowed, out _);

        result.ShouldBeFalse();
    }

    // --- CollectMatchCount ---

    [Fact]
    public void CollectMatchCount_WithImmediatelyValidInput_ReturnsValueWithoutReprompt()
    {
        var reprompts = 0;
        var allowed = AllCounts(5);

        var result = HackingLoopLogic.CollectMatchCount(5, allowed, readLine: () => "3", onInvalidInput: () => reprompts++);

        result.ShouldBe(3);
        reprompts.ShouldBe(0);
    }

    [Fact]
    public void CollectMatchCount_WithBlankInput_ReturnsWordLength()
    {
        var allowed = AllCounts(5);
        var result = HackingLoopLogic.CollectMatchCount(5, allowed, readLine: () => null, onInvalidInput: () => { });

        result.ShouldBe(5);
    }

    [Fact]
    public void CollectMatchCount_WithInvalidThenValidInput_RepromptsOnce()
    {
        var inputs = new Queue<string?>(["abc", "2"]);
        var reprompts = 0;
        var allowed = AllCounts(5);

        var result = HackingLoopLogic.CollectMatchCount(5, allowed, readLine: () => inputs.Dequeue(), onInvalidInput: () => reprompts++);

        result.ShouldBe(2);
        reprompts.ShouldBe(1);
    }

    [Fact]
    public void CollectMatchCount_CallsOnInvalidInputWhenInputNotInAllowed()
    {
        var inputs = new Queue<string?>(["2", "3"]); // 2 not in allowed
        var reprompts = 0;
        var allowed = new[] { 0, 1, 3, 4, 5 };

        var result = HackingLoopLogic.CollectMatchCount(5, allowed, readLine: () => inputs.Dequeue(), onInvalidInput: () => reprompts++);

        result.ShouldBe(3);
        reprompts.ShouldBe(1);
    }

    // --- ApplyNarrowedCandidates ---

    [Fact]
    public void ApplyNarrowedCandidates_ClearsSessionAndAddsAllNarrowed()
    {
        var session = Mock.Of<IGameSession>();
        var narrowed = new[] { new Password("ALPHA"), new Password("BETA") };

        HackingLoopLogic.ApplyNarrowedCandidates(session, narrowed);

        session.AsMock().Verify(s => s.Clear(), Times.Once);
        session.AsMock().Verify(s => s.Add(narrowed[0]), Times.Once);
        session.AsMock().Verify(s => s.Add(narrowed[1]), Times.Once);
    }

    [Fact]
    public void ApplyNarrowedCandidates_WithEmptyList_OnlyClearsSession()
    {
        var session = Mock.Of<IGameSession>();

        HackingLoopLogic.ApplyNarrowedCandidates(session, []);

        // Clear must be called; no Add calls should happen (implied by empty input)
        session.AsMock().Verify(s => s.Clear(), Times.Once);
    }
}
