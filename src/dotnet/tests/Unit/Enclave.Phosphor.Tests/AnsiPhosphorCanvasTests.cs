namespace Enclave.Phosphor.Tests;

/// <summary>
/// Unit tests for <see cref="AnsiPhosphorCanvas"/> (production implementation).
/// Uses <see cref="TestableConsoleIO"/> to verify output and behavior without a real terminal.
/// </summary>
[UnitTest, TestOf(nameof(AnsiPhosphorCanvas))]
public sealed class AnsiPhosphorCanvasTests
{
    private static AnsiPhosphorCanvas CreateCanvas(PhosphorTheme theme, TestableConsoleIO console, IScreenOptions? screenOptions = null) =>
        new(theme, console, screenOptions ?? new ScreenOptions());

    [Fact]
    public void Constructor_NullTheme_ThrowsArgumentNullException()
    {
        var console = new TestableConsoleIO();
        var ex = Should.Throw<ArgumentNullException>(() => new AnsiPhosphorCanvas(null!, console, new ScreenOptions()));
        ex.ParamName.ShouldBe("theme");
    }

    [Fact]
    public void Constructor_NullConsole_ThrowsArgumentNullException()
    {
        var theme = PhosphorThemeFactory.Create("green");
        var ex = Should.Throw<ArgumentNullException>(() => new AnsiPhosphorCanvas(theme, null!, new ScreenOptions()));
        ex.ParamName.ShouldBe("console");
    }

    [Fact]
    public void Constructor_NullScreenOptions_ThrowsArgumentNullException()
    {
        var theme = PhosphorThemeFactory.Create("green");
        var console = new TestableConsoleIO();
        var ex = Should.Throw<ArgumentNullException>(() => new AnsiPhosphorCanvas(theme, console, null!));
        ex.ParamName.ShouldBe("screenOptions");
    }

    [Fact]
    public void Style_WhenSetToInvalidEnumValue_CoercesToNormal()
    {
        var theme = PhosphorThemeFactory.Create("green");
        var console = new TestableConsoleIO();
        var canvas = CreateCanvas(theme, console);

        canvas.Style = (CharStyle)99;

        canvas.Style.ShouldBe(CharStyle.Normal);
    }

    [Fact]
    public void Initialize_SmallTerminal_WritesErrorAndThrowsInvalidOperationException()
    {
        // Arrange
        var theme = PhosphorThemeFactory.Create("green");
        var console = new TestableConsoleIO { Dimensions = (40, 20) };
        var canvas = CreateCanvas(theme, console);

        // Act & Assert
        var ex = Should.Throw<InvalidOperationException>(() => canvas.Initialize("test"));
        ex.Message.ShouldContain("Terminal too small");
        ex.Message.ShouldContain("40×20");
        ex.Message.ShouldContain("80×24");
        console.OutputEncoding.ShouldBe(System.Text.Encoding.UTF8);
        console.Written.ShouldContain(s => s.Contains("Terminal too small"));
    }

    [Fact]
    public void Initialize_SmallTerminal_UsesScreenOptionsForMinimumAndEncoding()
    {
        // Arrange - custom options: 100×30 minimum, ASCII encoding
        var theme = PhosphorThemeFactory.Create("green");
        var console = new TestableConsoleIO { Dimensions = (50, 20) };
        var options = new ScreenOptions(MinWidth: 100, MinHeight: 30, Encoding: System.Text.Encoding.ASCII);
        var canvas = new AnsiPhosphorCanvas(theme, console, options);

        // Act & Assert
        var ex = Should.Throw<InvalidOperationException>(() => canvas.Initialize("test"));
        ex.Message.ShouldContain("100×30");
        console.OutputEncoding.ShouldBe(System.Text.Encoding.ASCII);
    }

    [Fact]
    public void Initialize_CallsSemanticDisplayAndSetsTitle()
    {
        // Arrange
        var theme = PhosphorThemeFactory.Create("green");
        var console = new TestableConsoleIO();
        var canvas = CreateCanvas(theme, console);

        // Act
        canvas.Initialize("RAVEN v1.0 – ENCLAVE SIGINT");

        // Assert
        console.Title.ShouldBe("RAVEN v1.0 – ENCLAVE SIGINT");
        console.SemanticCalls.ShouldContain("HideCursor");
        console.SemanticCalls.ShouldContain("SetBackgroundColor:#0C190C");
        console.SemanticCalls.ShouldContain("ClearScreen");
    }

    [Fact]
    public void Write_WhenNotInitialized_ThrowsInvalidOperationException()
    {
        // Arrange
        var theme = PhosphorThemeFactory.Create("green");
        var console = new TestableConsoleIO();
        var canvas = CreateCanvas(theme, console);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => canvas.Write("text"));
    }

    [Fact]
    public void Write_WhenInitialized_SetsForegroundColorAndWritesText()
    {
        // Arrange
        var theme = PhosphorThemeFactory.Create("green");
        var console = new TestableConsoleIO();
        var canvas = CreateCanvas(theme, console);
        canvas.Initialize("test");

        // Act
        canvas.Style = CharStyle.Bright;
        canvas.Write("OK");

        // Assert - semantic SetForegroundColor (green Bright = #66FF66) then Write
        console.SemanticCalls.ShouldContain("SetForegroundColor:#66FF66");
        console.Written.ShouldContain("OK");
    }

    [Fact]
    public void WriteLine_WithText_SetsForegroundColorOutputsTextAndNewline()
    {
        // Arrange
        var theme = PhosphorThemeFactory.Create("green");
        var console = new TestableConsoleIO();
        var canvas = CreateCanvas(theme, console);
        canvas.Initialize("test");
        canvas.Style = CharStyle.Bright;

        // Act
        canvas.WriteLine("Hello");

        // Assert
        console.SemanticCalls.ShouldContain("SetForegroundColor:#66FF66");
        console.Written.ShouldContain("Hello");
        console.Written.Last().ShouldBe(string.Empty);
    }

    [Fact]
    public void WriteLine_WithNull_OutputsOnlyNewline()
    {
        // Arrange
        var theme = PhosphorThemeFactory.Create("green");
        var console = new TestableConsoleIO();
        var canvas = CreateCanvas(theme, console);
        canvas.Initialize("test");

        // Act
        canvas.WriteLine();

        // Assert - only newline (empty string from WriteLine())
        console.Written.Last().ShouldBe(string.Empty);
    }

    [Fact]
    public void WriteLine_WhenNotInitialized_WithText_ThrowsInvalidOperationException()
    {
        // Arrange - WriteLine with text calls Write, which validates initialized
        var theme = PhosphorThemeFactory.Create("green");
        var console = new TestableConsoleIO();
        var canvas = CreateCanvas(theme, console);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => canvas.WriteLine("text"));
    }

    [Fact]
    public void Write_NullText_ThrowsArgumentNullException()
    {
        // Arrange
        var theme = PhosphorThemeFactory.Create("green");
        var console = new TestableConsoleIO();
        var canvas = CreateCanvas(theme, console);

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => canvas.Write(null!));
    }

    [Fact]
    public void Initialize_NullTitle_ThrowsArgumentNullException()
    {
        // Arrange
        var theme = PhosphorThemeFactory.Create("green");
        var console = new TestableConsoleIO();
        var canvas = CreateCanvas(theme, console);

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => canvas.Initialize(null!));
    }

    [Fact]
    public void Dispose_ResetsStyleShowsCursorAndFlushes()
    {
        // Arrange
        var theme = PhosphorThemeFactory.Create("green");
        var console = new TestableConsoleIO();
        var canvas = CreateCanvas(theme, console);
        canvas.Initialize("test");

        // Act
        canvas.Dispose();

        // Assert
        console.SemanticCalls.ShouldContain("ResetStyle");
        console.SemanticCalls.ShouldContain("ShowCursor");
    }

    [Fact]
    public void ClearScreen_WhenInitialized_CallsSetBackgroundColorAndClearScreenOnConsole()
    {
        // Arrange
        var theme = PhosphorThemeFactory.Create("green");
        var console = new TestableConsoleIO();
        var canvas = CreateCanvas(theme, console);
        canvas.Initialize("test");
        var callsBefore = console.SemanticCalls.Count;

        // Act
        canvas.ClearScreen();

        // Assert - re-applies theme background then clears
        console.SemanticCalls.ShouldContain("SetBackgroundColor:#0C190C");
        console.SemanticCalls.ShouldContain("ClearScreen");
        console.SemanticCalls.Count.ShouldBe(callsBefore + 2);
        console.SemanticCalls[^2].ShouldBe("SetBackgroundColor:#0C190C");
        console.SemanticCalls[^1].ShouldBe("ClearScreen");
    }

    [Fact]
    public void ClearScreen_WhenNotInitialized_ThrowsInvalidOperationException()
    {
        var theme = PhosphorThemeFactory.Create("green");
        var console = new TestableConsoleIO();
        var canvas = CreateCanvas(theme, console);

        Should.Throw<InvalidOperationException>(() => canvas.ClearScreen());
    }

}
