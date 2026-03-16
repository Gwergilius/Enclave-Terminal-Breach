using System.Text;
using Enclave.Common.Drawing;
using Enclave.Phosphor;
using Enclave.Raven.Screens.HackingLoop;

namespace Enclave.Raven.Tests.Screens.HackingLoop;

[UnitTest, TestOf(nameof(HackingLoopComponent))]
public class HackingLoopComponentTests
{
    private static Layer MakeLayer() => new(new Rectangle(0, 0, 80, 10), zOrder: 0);

    private static string ReadRow(Layer layer, int row)
    {
        var sb = new StringBuilder();
        for (var col = layer.Bounds.Left; col <= layer.Bounds.Right; col++)
            sb.Append(layer.GetCell(col, row).Character);
        return sb.ToString().TrimEnd('\0');
    }

    [Fact]
    public void Render_WithNoLines_LeavesLayerEmpty()
    {
        var layer = MakeLayer();
        var sut = new HackingLoopComponent(layer.Bounds);

        sut.Render(new LayerWriter(layer));

        ReadRow(layer, 0).ShouldBeEmpty();
    }

    [Fact]
    public void Render_WithLines_WritesEachOnConsecutiveRows()
    {
        var layer = MakeLayer();
        var sut = new HackingLoopComponent(layer.Bounds);
        sut.Lines.AddRange(["Line1", "Line2", "Line3"]);

        sut.Render(new LayerWriter(layer));

        ReadRow(layer, 0).ShouldBe("Line1");
        ReadRow(layer, 1).ShouldBe("Line2");
        ReadRow(layer, 2).ShouldBe("Line3");
    }

    [Fact]
    public void Render_WithEmptyStringLine_WritesBlankRow()
    {
        var layer = MakeLayer();
        var sut = new HackingLoopComponent(layer.Bounds);
        sut.Lines.AddRange(["Before", string.Empty, "After"]);

        sut.Render(new LayerWriter(layer));

        ReadRow(layer, 0).ShouldBe("Before");
        ReadRow(layer, 1).ShouldBeEmpty();
        ReadRow(layer, 2).ShouldBe("After");
    }
}
