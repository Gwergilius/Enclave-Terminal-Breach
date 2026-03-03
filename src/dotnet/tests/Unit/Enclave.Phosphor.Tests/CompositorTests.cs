namespace Enclave.Phosphor.Tests;

/// <summary>
/// Unit tests for <see cref="Compositor"/> (recompose correctness, diff minimality, emit grouping).
/// </summary>
[UnitTest, TestOf(nameof(Compositor))]
public sealed class CompositorTests
{
    // --- Test doubles -------------------------------------------------------

    private sealed class RecordingCursor : IPhosphorCursor
    {
        public readonly List<(int Col, int Row)> Moves = [];
        public void MoveTo(int col, int row) => Moves.Add((col, row));
    }

    // --- Helpers ------------------------------------------------------------

    private static (FakeVirtualScreen Screen, TestPhosphorWriter Writer, RecordingCursor Cursor, Compositor Compositor)
        CreateSut(int width = 10, int height = 5)
    {
        var screen = new FakeVirtualScreen(new Size(width, height));
        var writer = new TestPhosphorWriter();
        var cursor = new RecordingCursor();
        var compositor = new Compositor(screen, writer, cursor);
        return (screen, writer, cursor, compositor);
    }

    // --- Recompose: painter's algorithm -------------------------------------

    [Fact]
    public void Flush_SingleLayer_WritesLayerContent()
    {
        var (screen, writer, cursor, sut) = CreateSut();
        var layer = screen.AddLayer(new Rectangle(0, 0, 3, 1), zOrder: 10);
        layer.SetCell(0, 0, new VirtualCell('A', CharStyle.Bright));
        layer.SetCell(1, 0, new VirtualCell('B', CharStyle.Bright));
        layer.SetCell(2, 0, new VirtualCell('C', CharStyle.Bright));

        sut.Flush(new Rectangle(0, 0, 3, 1));

        // All three cells should have been emitted as one run
        writer.Recorded.ShouldContain(r => r.Text == "ABC" && r.Style == CharStyle.Bright);
    }

    [Fact]
    public void Flush_UpperLayerOverwritesLower()
    {
        var (screen, writer, cursor, sut) = CreateSut();
        var bg = screen.AddLayer(new Rectangle(0, 0, 3, 1), zOrder: 0);
        bg.SetCell(0, 0, new VirtualCell('X', CharStyle.Dark));
        bg.SetCell(1, 0, new VirtualCell('X', CharStyle.Dark));
        bg.SetCell(2, 0, new VirtualCell('X', CharStyle.Dark));

        var fg = screen.AddLayer(new Rectangle(1, 0, 1, 1), zOrder: 10);
        fg.SetCell(1, 0, new VirtualCell('Y', CharStyle.Bright));

        sut.Flush(new Rectangle(0, 0, 3, 1));

        // col 1 should be 'Y', not 'X'
        // The composite should be: X Y X  (two runs: X@Dark, Y@Bright, X@Dark)
        writer.Recorded.ShouldContain(r => r.Text.Contains('Y') && r.Style == CharStyle.Bright);
        // Background cells should still appear
        writer.Recorded.ShouldContain(r => r.Text.Contains('X') && r.Style == CharStyle.Dark);
    }

    [Fact]
    public void Flush_TransparentCellIsSkipped_LayerBelowShows()
    {
        var (screen, writer, cursor, sut) = CreateSut();
        var bg = screen.AddLayer(new Rectangle(0, 0, 3, 1), zOrder: 0);
        bg.SetCell(0, 0, new VirtualCell('B'));
        bg.SetCell(1, 0, new VirtualCell('B'));
        bg.SetCell(2, 0, new VirtualCell('B'));

        // Overlay with one transparent cell in the middle
        var overlay = screen.AddLayer(new Rectangle(0, 0, 3, 1), zOrder: 10);
        overlay.SetCell(0, 0, new VirtualCell('O', CharStyle.Bright));
        // col 1 is left as VirtualCell.Empty (\0) — transparent
        overlay.SetCell(2, 0, new VirtualCell('O', CharStyle.Bright));

        sut.Flush(new Rectangle(0, 0, 3, 1));

        // Middle cell should be 'B' from background layer
        writer.Recorded.ShouldContain(r => r.Text.Contains('B') && r.Style == CharStyle.Normal);
        writer.Recorded.ShouldContain(r => r.Text.Contains('O') && r.Style == CharStyle.Bright);
    }

    // --- Diff minimality ---------------------------------------------------

    [Fact]
    public void Flush_SecondFlushSameContent_EmitsNothing()
    {
        var (screen, writer, cursor, sut) = CreateSut();
        var layer = screen.AddLayer(new Rectangle(0, 0, 2, 1), zOrder: 10);
        layer.SetCell(0, 0, new VirtualCell('A'));
        layer.SetCell(1, 0, new VirtualCell('B'));

        sut.Flush(new Rectangle(0, 0, 2, 1));
        writer.Clear();

        // Flush the same region again — nothing changed, nothing should be written
        sut.Flush(new Rectangle(0, 0, 2, 1));

        writer.Recorded.ShouldBeEmpty();
    }

    [Fact]
    public void Flush_OnlyChangedCellsEmitted()
    {
        var (screen, writer, cursor, sut) = CreateSut();
        var layer = screen.AddLayer(new Rectangle(0, 0, 3, 1), zOrder: 10);
        layer.SetCell(0, 0, new VirtualCell('A'));
        layer.SetCell(1, 0, new VirtualCell('B'));
        layer.SetCell(2, 0, new VirtualCell('C'));

        sut.Flush(new Rectangle(0, 0, 3, 1));
        writer.Clear();
        cursor.Moves.Clear();

        // Change only the middle cell
        layer.SetCell(1, 0, new VirtualCell('Z', CharStyle.Bright));
        sut.Flush(new Rectangle(0, 0, 3, 1));

        // Only 'Z' should have been emitted (not A or C)
        writer.Recorded.ShouldContain(r => r.Text == "Z" && r.Style == CharStyle.Bright);
        var allWritten = string.Concat(writer.Recorded.Select(r => r.Text));
        allWritten.ShouldNotContain("A");
        allWritten.ShouldNotContain("C");
    }

    // --- Invisible layers --------------------------------------------------

    [Fact]
    public void Flush_InvisibleLayer_IsSkipped()
    {
        var (screen, writer, cursor, sut) = CreateSut();
        var layer = screen.AddLayer(new Rectangle(0, 0, 3, 1), zOrder: 10);
        layer.SetCell(0, 0, new VirtualCell('X', CharStyle.Bright));
        layer.SetCell(1, 0, new VirtualCell('X', CharStyle.Bright));
        layer.SetCell(2, 0, new VirtualCell('X', CharStyle.Bright));
        layer.IsVisible = false;

        sut.Flush(new Rectangle(0, 0, 3, 1));

        // No 'X' should appear (invisible layer is skipped; region fills with Space)
        var allWritten = string.Concat(writer.Recorded.Select(r => r.Text));
        allWritten.ShouldNotContain("X");
    }

    // --- Cursor positioning ------------------------------------------------

    [Fact]
    public void Flush_CursorMovesToCorrectPosition()
    {
        var (screen, writer, cursor, sut) = CreateSut();
        var layer = screen.AddLayer(new Rectangle(3, 2, 1, 1), zOrder: 10);
        layer.SetCell(3, 2, new VirtualCell('Z'));

        sut.Flush(new Rectangle(3, 2, 1, 1));

        cursor.Moves.ShouldContain((3, 2));
    }
}
