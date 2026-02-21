using Enclave.Shared.IO;
using Moq;

namespace Enclave.Phosphor.Tests;

/// <summary>
/// Unit tests for <see cref="PhosphorReaderExtensions.ReadInt"/> (IConsoleIO overload and IConsoleReader+IConsoleWriter).
/// </summary>
[UnitTest, TestOf(nameof(PhosphorReaderExtensions))]
public sealed class PhosphorReaderExtensionsTests
{
    [Fact]
    public void ReadInt_WhenCalledOnIConsoleIO_ForwardsAndReturnsParsedValue()
    {
        var console = new Mock<IConsoleIO>();
        console.Setup(c => c.ReadLine()).Returns("5");
        console.Setup(c => c.Write(It.IsAny<string>()));
        console.Setup(c => c.WriteLine(It.IsAny<string?>()));

        var result = console.Object.ReadInt(0, 5, 0, "Match count? ");

        result.ShouldBe(5);
        console.Verify(c => c.ReadLine(), Times.Once);
        console.Verify(c => c.Write("Match count? "), Times.Once);
    }

    [Fact]
    public void ReadInt_WhenCalledOnIConsoleIO_WithEmptyInput_ReturnsDefaultValue()
    {
        var console = new Mock<IConsoleIO>();
        console.Setup(c => c.ReadLine()).Returns("");
        console.Setup(c => c.Write(It.IsAny<string>()));

        var result = console.Object.ReadInt(0, 10, 7, "Prompt: ");

        result.ShouldBe(7);
    }

    [Fact]
    public void ReadInt_WhenInputEmpty_ReturnsDefaultValue()
    {
        var reader = new Mock<IConsoleReader>();
        var writer = new Mock<IConsoleWriter>();
        reader.Setup(r => r.ReadLine()).Returns("");
        writer.Setup(w => w.Write(It.IsAny<string>()));

        var result = reader.Object.ReadInt(writer.Object, 0, 10, 3, "Prompt: ");

        result.ShouldBe(3);
        reader.Verify(r => r.ReadLine(), Times.Once);
    }

    [Fact]
    public void ReadInt_WhenInputWhitespaceOnly_ReturnsDefaultValue()
    {
        var reader = new Mock<IConsoleReader>();
        var writer = new Mock<IConsoleWriter>();
        reader.Setup(r => r.ReadLine()).Returns("   ");
        writer.Setup(w => w.Write(It.IsAny<string>()));

        var result = reader.Object.ReadInt(writer.Object, 1, 5, 2, "Prompt: ");

        result.ShouldBe(2);
    }

    [Fact]
    public void ReadInt_WhenInputNull_ReturnsDefaultValue()
    {
        var reader = new Mock<IConsoleReader>();
        var writer = new Mock<IConsoleWriter>();
        reader.Setup(r => r.ReadLine()).Returns((string?)null);
        writer.Setup(w => w.Write(It.IsAny<string>()));

        var result = reader.Object.ReadInt(writer.Object, 0, 5, 4, "Prompt: ");

        result.ShouldBe(4);
    }

    [Fact]
    public void ReadInt_WhenInputOutOfRange_RetriesUntilValid()
    {
        var reader = new Mock<IConsoleReader>();
        var writer = new Mock<IConsoleWriter>();
        var readLineCalls = 0;
        reader.Setup(r => r.ReadLine()).Returns(() => readLineCalls++ == 0 ? "99" : "2");
        writer.Setup(w => w.Write(It.IsAny<string>()));
        var writtenLines = new List<string>();
        writer.Setup(w => w.WriteLine(It.IsAny<string?>())).Callback<string?>(s => writtenLines.Add(s ?? ""));

        var result = reader.Object.ReadInt(writer.Object, 0, 5, 0, "Value: ");

        result.ShouldBe(2);
        writtenLines.Count.ShouldBe(1);
        writtenLines[0].ShouldContain("0");
        writtenLines[0].ShouldContain("5");
    }

    [Fact]
    public void ReadInt_WhenInputNonNumeric_RetriesUntilValid()
    {
        var reader = new Mock<IConsoleReader>();
        var writer = new Mock<IConsoleWriter>();
        var readLineCalls = 0;
        reader.Setup(r => r.ReadLine()).Returns(() => readLineCalls++ == 0 ? "abc" : "3");
        writer.Setup(w => w.Write(It.IsAny<string>()));
        var writtenLines = new List<string>();
        writer.Setup(w => w.WriteLine(It.IsAny<string?>())).Callback<string?>(s => writtenLines.Add(s ?? ""));

        var result = reader.Object.ReadInt(writer.Object, 0, 10, 0, "Value: ", "Invalid number.");

        result.ShouldBe(3);
        writtenLines.ShouldContain("Invalid number.");
    }
}
