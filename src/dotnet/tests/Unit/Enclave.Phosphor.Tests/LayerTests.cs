namespace Enclave.Phosphor.Tests;

/// <summary>
/// Unit tests for <see cref="Layer"/> — buffer clearing and layer movement.
/// </summary>
[UnitTest, TestOf(nameof(Layer))]
public sealed class LayerTests
{
    // --- SetCell: Guard rejects control characters ---------------------------

    [Fact]
    public void SetCell_ControlCharacter_Throws()
    {
        var layer = new Layer(new Rectangle(0, 0, 5, 5), zOrder: 0);

        Should.Throw<ArgumentException>(() => layer.SetCell(0, 0, new VirtualCell('\f')));

        layer.GetCell(0, 0).ShouldBe(VirtualCell.Empty); // buffer not modified
    }

    [Fact]
    public void SetCell_EmptyCell_DoesNotThrow()
    {
        // '\0' is C0 control, but VirtualCell.Empty is explicitly allowed by Guard.
        var layer = new Layer(new Rectangle(0, 0, 5, 5), zOrder: 0);

        Should.NotThrow(() => layer.SetCell(0, 0, VirtualCell.Empty));
    }


    // --- Clear --------------------------------------------------------------

    [Fact]
    public void Clear_AllCellsBecomeTransparent()
    {
        var layer = new Layer(new Rectangle(0, 0, 5, 3), zOrder: 0);
        layer.SetCell(0, 0, new VirtualCell('A'));
        layer.SetCell(4, 2, new VirtualCell('Z', CharStyle.Bright));

        layer.Clear();

        for (var row = 0; row < 3; row++)
            for (var col = 0; col < 5; col++)
                layer.GetCell(col, row).ShouldBe(VirtualCell.Empty);
    }

    [Fact]
    public void Clear_CanBeCalledRepeatedly()
    {
        var layer = new Layer(new Rectangle(0, 0, 5, 3), zOrder: 0);

        Should.NotThrow(() =>
        {
            layer.Clear();
            layer.Clear();
            layer.Clear();
        });
    }

    // --- MoveTo -------------------------------------------------------------

    [Fact]
    public void MoveTo_UpdatesBoundsLocation()
    {
        var layer = new Layer(new Rectangle(0, 0, 5, 3), zOrder: 0);

        layer.MoveTo(new Point(10, 5));

        layer.Bounds.Left.ShouldBe(10);
        layer.Bounds.Top.ShouldBe(5);
    }

    [Fact]
    public void MoveTo_BoundsDimensionUnchanged()
    {
        var layer = new Layer(new Rectangle(0, 0, 5, 3), zOrder: 0);

        layer.MoveTo(new Point(10, 5));

        layer.Bounds.Width.ShouldBe(5);
        layer.Bounds.Height.ShouldBe(3);
    }

    [Fact]
    public void MoveTo_PreservesBufferContents_AccessibleAtNewAbsoluteCoords()
    {
        // Layer at (0, 0); write a cell at absolute (2, 1).
        var layer = new Layer(new Rectangle(0, 0, 5, 3), zOrder: 0);
        layer.SetCell(2, 1, new VirtualCell('X', CharStyle.Bright));

        // After moving to (10, 5) the same relative cell (relCol=2, relRow=1)
        // is now at absolute position (12, 6).
        layer.MoveTo(new Point(10, 5));

        var cell = layer.GetCell(12, 6);
        cell.Character.ShouldBe('X');
        cell.Style.ShouldBe(CharStyle.Bright);
    }
}
