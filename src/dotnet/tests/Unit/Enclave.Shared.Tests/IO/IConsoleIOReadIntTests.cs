using Enclave.Shared.IO;

namespace Enclave.Shared.Tests.IO;

/// <summary>
/// Unit tests for <see cref="ConsoleReaderExtensions.ReadInt"/> when called on <see cref="IConsoleIO"/>.
/// Covers the backward-compatible IConsoleIO.ReadInt(...) API used by Sparrow and other console apps.
/// </summary>
[UnitTest, TestOf(nameof(IConsoleIO))]
public sealed class IConsoleIOReadIntTests
{
    [Fact]
    public void IConsoleIO_ReadInt_WhenReadLineReturnsNull_ReturnsDefaultValue()
    {
        IConsoleIO console = new TestConsoleIO();
        ((TestConsoleIO)console).AddReadLineResponse(null);

        var result = console.ReadInt(0, 5, 3, "Enter: ");

        result.ShouldBe(3);
    }

    [Fact]
    public void IConsoleIO_ReadInt_WhenInputValid_ReturnsParsedValue()
    {
        IConsoleIO console = new TestConsoleIO();
        ((TestConsoleIO)console).AddReadLineResponse("4");

        var result = console.ReadInt(0, 10, 0, "Value: ");

        result.ShouldBe(4);
    }

    [Fact]
    public void IConsoleIO_ReadInt_WhenInputInvalidThenValid_RetriesAndReturnsValue()
    {
        IConsoleIO console = new TestConsoleIO();
        ((TestConsoleIO)console).AddReadLineResponse("bad");
        ((TestConsoleIO)console).AddReadLineResponse("2");

        var result = console.ReadInt(0, 5, 0, "Match count? ", "Invalid.");

        result.ShouldBe(2);
        ((TestConsoleIO)console).WrittenLines.ShouldContain("Invalid.");
        ((TestConsoleIO)console).Written.ShouldContain("Match count? ");
    }

    [Fact]
    public void IConsoleIO_ReadInt_WhenInputOutOfRange_WritesErrorAndRetries()
    {
        IConsoleIO console = new TestConsoleIO();
        ((TestConsoleIO)console).AddReadLineResponse("99");
        ((TestConsoleIO)console).AddReadLineResponse("1");

        var result = console.ReadInt(0, 5, 0);

        result.ShouldBe(1);
        ((TestConsoleIO)console).WrittenLines.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void IConsoleIO_ReadInt_WhenInputAtMinBoundary_ReturnsMin()
    {
        IConsoleIO console = new TestConsoleIO();
        ((TestConsoleIO)console).AddReadLineResponse("0");

        var result = console.ReadInt(0, 5, 0);

        result.ShouldBe(0);
    }

    [Fact]
    public void IConsoleIO_ReadInt_WhenInputAtMaxBoundary_ReturnsMax()
    {
        IConsoleIO console = new TestConsoleIO();
        ((TestConsoleIO)console).AddReadLineResponse("5");

        var result = console.ReadInt(0, 5, 0);

        result.ShouldBe(5);
    }

    [Fact]
    public void IConsoleIO_ReadInt_WithDefaultPromptAndNoErrorMessage_UsesDefaultErrorText()
    {
        IConsoleIO console = new TestConsoleIO();
        ((TestConsoleIO)console).AddReadLineResponse("x");
        ((TestConsoleIO)console).AddReadLineResponse("3");

        console.ReadInt(1, 10, 1);

        ((TestConsoleIO)console).WrittenLines.ShouldContain(s => s.Contains("1") && s.Contains("10"));
    }
}
