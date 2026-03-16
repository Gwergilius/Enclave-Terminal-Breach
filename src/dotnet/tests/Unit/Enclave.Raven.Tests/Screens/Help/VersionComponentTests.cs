using System.Text;
using Enclave.Common.Drawing;
using Enclave.Common.Test.Core;
using Enclave.Phosphor;
using Enclave.Raven.Screens.Help;

namespace Enclave.Raven.Tests.Screens.Help;

[UnitTest, TestOf(nameof(VersionComponent))]
public class VersionComponentTests
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
    public void Render_WithEmptyVersionLine_LeavesLayerEmpty()
    {
        var layer = MakeLayer();
        var sut = new VersionComponent(layer.Bounds);

        sut.Render(new LayerWriter(layer));

        ReadRow(layer, 0).ShouldBeEmpty();
    }

    [Fact]
    public void Render_WithVersionLine_WritesOnRow0()
    {
        var layer = MakeLayer();
        var sut = new VersionComponent(layer.Bounds) { VersionLine = "RAVEN 1.2.3" };

        sut.Render(new LayerWriter(layer));

        ReadRow(layer, 0).ShouldBe("RAVEN 1.2.3");
    }

    [Fact]
    public void Render_UsesCharStyleBright()
    {
        var layer = MakeLayer();
        var sut = new VersionComponent(layer.Bounds) { VersionLine = "RAVEN 1.2.3" };

        sut.Render(new LayerWriter(layer));

        layer.GetCell(0, 0).Style.ShouldBe(CharStyle.Bright);
    }

    [Fact]
    public void Render_WithRowOffset_WritesOnOffsetRow()
    {
        var layer = MakeLayer();
        var sut = new VersionComponent(layer.Bounds) { VersionLine = "RAVEN 1.2.3", Row = 2 };

        sut.Render(new LayerWriter(layer));

        ReadRow(layer, 0).ShouldBeEmpty();
        ReadRow(layer, 2).ShouldBe("RAVEN 1.2.3");
    }
}
