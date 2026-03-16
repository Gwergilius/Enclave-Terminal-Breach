namespace Enclave.Phosphor.Tests;

/// <summary>
/// Unit tests for <see cref="LayerWriter"/> — streaming writes, newline handling, clipping,
/// coordinate translation, and direct <see cref="LayerWriter.SetCell"/> bounds guarding.
/// </summary>
[UnitTest, TestOf(nameof(LayerWriter))]
public sealed class LayerWriterTests
{
    // --- Helpers ------------------------------------------------------------

    private static (Layer Layer, LayerWriter Writer) Create(int width, int height)
    {
        var layer = new Layer(new Rectangle(0, 0, width, height), zOrder: 0);
        return (layer, new LayerWriter(layer));
    }

    // --- Write: basic text --------------------------------------------------

    [Fact]
    public void Write_BasicText_FillsLayerLeftToRight()
    {
        var (layer, sut) = Create(10, 5);

        sut.Write("ABC");

        layer.GetCell(0, 0).Character.ShouldBe('A');
        layer.GetCell(1, 0).Character.ShouldBe('B');
        layer.GetCell(2, 0).Character.ShouldBe('C');
    }

    [Fact]
    public void Write_AppliesCurrentStyle()
    {
        var (layer, sut) = Create(5, 1);
        sut.Style = CharStyle.Bright;

        sut.Write("AB");

        layer.GetCell(0, 0).Style.ShouldBe(CharStyle.Bright);
        layer.GetCell(1, 0).Style.ShouldBe(CharStyle.Bright);
    }

    [Fact]
    public void Write_StyleChangeBetweenCalls_AffectsSubsequentChars()
    {
        var (layer, sut) = Create(5, 1);

        sut.Write("A");
        sut.Style = CharStyle.Bright;
        sut.Write("B");

        layer.GetCell(0, 0).Style.ShouldBe(CharStyle.Normal);
        layer.GetCell(1, 0).Style.ShouldBe(CharStyle.Bright);
    }

    // --- Write: newline handling --------------------------------------------

    [Fact]
    public void Write_Newline_MovesToNextRowAtCol0()
    {
        var (layer, sut) = Create(10, 5);

        sut.Write("AB\nCD");

        layer.GetCell(0, 0).Character.ShouldBe('A');
        layer.GetCell(1, 0).Character.ShouldBe('B');
        layer.GetCell(0, 1).Character.ShouldBe('C');
        layer.GetCell(1, 1).Character.ShouldBe('D');
    }

    [Fact]
    public void Write_MultipleNewlines_EachStartsAtCol0()
    {
        var (layer, sut) = Create(10, 5);

        sut.Write("A\nB\nC");

        layer.GetCell(0, 0).Character.ShouldBe('A');
        layer.GetCell(0, 1).Character.ShouldBe('B');
        layer.GetCell(0, 2).Character.ShouldBe('C');
    }

    // --- Write: clipping ----------------------------------------------------

    [Fact]
    public void Write_PastRightEdge_ExtraCharsClipped()
    {
        var (layer, sut) = Create(width: 3, height: 1);

        sut.Write("ABCDE"); // 5 chars into a 3-column layer

        layer.GetCell(0, 0).Character.ShouldBe('A');
        layer.GetCell(1, 0).Character.ShouldBe('B');
        layer.GetCell(2, 0).Character.ShouldBe('C');
        // D and E are silently dropped; no exception
    }

    [Fact]
    public void Write_PastRightEdgeThenNewline_NextRowStartsAtCol0()
    {
        // Column counter keeps advancing past the right edge so that the subsequent
        // '\n' resets it to 0 correctly — not to some mid-line position.
        var (layer, sut) = Create(width: 3, height: 2);

        sut.Write("ABCDE\nX");

        layer.GetCell(0, 1).Character.ShouldBe('X');
    }

    [Fact]
    public void Write_PastBottomEdge_CharsDiscarded()
    {
        var (layer, sut) = Create(width: 5, height: 2);

        sut.Write("A\nB\nC"); // 'C' would land on row 2 which is outside height=2

        layer.GetCell(0, 0).Character.ShouldBe('A');
        layer.GetCell(0, 1).Character.ShouldBe('B');
        // No exception — excess content is silently discarded (no scroll in 2.0)
    }

    // --- MoveTo -------------------------------------------------------------

    [Fact]
    public void MoveTo_ThenWrite_StartsAtGivenPosition()
    {
        var (layer, sut) = Create(10, 5);

        sut.MoveTo(3, 2);
        sut.Write("X");

        layer.GetCell(3, 2).Character.ShouldBe('X');
        layer.GetCell(0, 0).ShouldBe(VirtualCell.Empty); // origin untouched
    }

    // --- SetCell ------------------------------------------------------------

    [Fact]
    public void SetCell_InBounds_TranslatesToAbsoluteCoordinates()
    {
        // Layer positioned at offset (5, 3); LayerWriter uses relative coords.
        var layer = new Layer(new Rectangle(5, 3, 10, 5), zOrder: 0);
        var sut   = new LayerWriter(layer);

        sut.SetCell(0, 0, new VirtualCell('Z', CharStyle.Bright));

        layer.GetCell(5, 3).Character.ShouldBe('Z');
        layer.GetCell(5, 3).Style.ShouldBe(CharStyle.Bright);
    }

    [Fact]
    public void SetCell_OutOfBounds_IsIgnoredWithoutException()
    {
        var (_, sut) = Create(5, 5);

        Should.NotThrow(() => sut.SetCell(-1,  0, new VirtualCell('X')));
        Should.NotThrow(() => sut.SetCell( 0, -1, new VirtualCell('X')));
        Should.NotThrow(() => sut.SetCell( 5,  0, new VirtualCell('X')));
        Should.NotThrow(() => sut.SetCell( 0,  5, new VirtualCell('X')));
    }
}
