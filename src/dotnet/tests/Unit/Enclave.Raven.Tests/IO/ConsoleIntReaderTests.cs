using Enclave.Raven.IO;

namespace Enclave.Raven.Tests.IO;

/// <summary>
/// Unit tests for <see cref="ConsoleIntReader"/> (via <see cref="TestConsoleIO"/>).
/// Tests edge cases: line == null, invalid input, out of range.
/// </summary>
[UnitTest, TestOf(nameof(ConsoleIntReader))]
public class ConsoleIntReaderTests
{
    [Fact]
    public void Read_WhenReadLineReturnsNull_ReturnsDefaultValue()
    {
        var console = new TestConsoleIO();
        console.AddReadLineResponse(null);

        var result = console.ReadInt(0, 5, 3, "Enter: ");

        result.ShouldBe(3);
    }

    [Fact]
    public void Read_WhenReadLineReturnsNull_DoesNotWriteError()
    {
        var console = new TestConsoleIO();
        console.AddReadLineResponse(null);

        console.ReadInt(0, 5, 3);

        console.WrittenLines.Count.ShouldBe(0);
    }

    [Fact]
    public void Read_WhenInputInvalid_WritesErrorAndRetries()
    {
        var console = new TestConsoleIO();
        console.AddReadLineResponse("abc");
        console.AddReadLineResponse("2");

        var result = console.ReadInt(0, 5, 0);

        result.ShouldBe(2);
        console.WrittenLines.ShouldContain(s => s.Contains('0') && s.Contains('5'));
    }

    [Fact]
    public void Read_WhenInputOutOfRange_WritesErrorAndRetries()
    {
        var console = new TestConsoleIO();
        console.AddReadLineResponse("10");
        console.AddReadLineResponse("3");

        var result = console.ReadInt(0, 5, 0);

        result.ShouldBe(3);
        console.WrittenLines.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Read_WhenInputNegative_WritesErrorAndRetries()
    {
        var console = new TestConsoleIO();
        console.AddReadLineResponse("-1");
        console.AddReadLineResponse("0");

        var result = console.ReadInt(0, 5, 0);

        result.ShouldBe(0);
    }

    [Fact]
    public void Read_WhenInputValidFirstTry_ReturnsValue()
    {
        var console = new TestConsoleIO();
        console.AddReadLineResponse("3");

        var result = console.ReadInt(0, 5, 0);

        result.ShouldBe(3);
        console.WrittenLines.Count.ShouldBe(0);
    }

    [Fact]
    public void Read_WhenInputValidAtBoundaries_ReturnsValue()
    {
        var consoleMin = new TestConsoleIO();
        consoleMin.AddReadLineResponse("0");
        consoleMin.ReadInt(0, 5, 0).ShouldBe(0);

        var consoleMax = new TestConsoleIO();
        consoleMax.AddReadLineResponse("5");
        consoleMax.ReadInt(0, 5, 0).ShouldBe(5);
    }

    [Fact]
    public void Read_UsesCustomPromptAndErrorMessage()
    {
        var console = new TestConsoleIO();
        console.AddReadLineResponse("x");
        console.AddReadLineResponse("1");

        console.ReadInt(0, 5, 0, "Match count? ", "Invalid.");

        console.Written.ShouldContain("Match count? ");
        console.WrittenLines.ShouldContain("Invalid.");
    }
}
