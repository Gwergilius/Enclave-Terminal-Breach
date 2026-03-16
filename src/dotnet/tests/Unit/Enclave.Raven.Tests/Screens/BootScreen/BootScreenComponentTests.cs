using System.Text;
using Enclave.Common.Drawing;
using Enclave.Phosphor;
using Enclave.Raven.Screens.BootScreen;

namespace Enclave.Raven.Tests.Screens.BootScreen;

[UnitTest, TestOf(nameof(BootScreenComponent))]
public class BootScreenComponentTests
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
        var sut = new BootScreenComponent(layer.Bounds);

        sut.Render(new LayerWriter(layer));

        ReadRow(layer, 0).ShouldBeEmpty();
    }

    [Fact]
    public void Render_WithProductLineOnly_WritesItOnRow0()
    {
        var layer = MakeLayer();
        var sut = new BootScreenComponent(layer.Bounds) { ProductLine = "ENCLAVE 1.0" };

        sut.Render(new LayerWriter(layer));

        ReadRow(layer, 0).ShouldBe("ENCLAVE 1.0");
        ReadRow(layer, 1).ShouldBeEmpty();
    }

    [Fact]
    public void Render_WithAllLines_WritesEachOnConsecutiveRows()
    {
        var layer = MakeLayer();
        var sut = new BootScreenComponent(layer.Bounds)
        {
            ProductLine     = "Product",
            LoadTimeLine    = "Load",
            IntelligenceLine = "Intel",
            DictionaryLine  = "Dict",
        };

        sut.Render(new LayerWriter(layer));

        ReadRow(layer, 0).ShouldBe("Product");
        ReadRow(layer, 1).ShouldBe("Load");
        ReadRow(layer, 2).ShouldBe("Intel");
        ReadRow(layer, 3).ShouldBe("Dict");
    }

    [Fact]
    public void Render_WithNullLineInMiddle_SkipsThatRow()
    {
        var layer = MakeLayer();
        var sut = new BootScreenComponent(layer.Bounds)
        {
            ProductLine  = "Product",
            LoadTimeLine = null,           // skipped
            IntelligenceLine = "Intel",
        };

        sut.Render(new LayerWriter(layer));

        ReadRow(layer, 0).ShouldBe("Product");
        ReadRow(layer, 1).ShouldBe("Intel");  // moves up — no gap for null
        ReadRow(layer, 2).ShouldBeEmpty();
    }
}
