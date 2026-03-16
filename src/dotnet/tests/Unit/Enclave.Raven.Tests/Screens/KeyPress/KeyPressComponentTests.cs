using System.Text;
using Enclave.Common.Drawing;
using Enclave.Common.Test.Core;
using Enclave.Phosphor;
using Enclave.Raven.Screens.KeyPress;

namespace Enclave.Raven.Tests.Screens.KeyPress;

[UnitTest, TestOf(nameof(KeyPressComponent))]
public class KeyPressComponentTests
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
    public void Render_WithEmptyPrompt_LeavesLayerEmpty()
    {
        var layer = MakeLayer();
        var sut = new KeyPressComponent(layer.Bounds);

        sut.Render(new LayerWriter(layer));

        ReadRow(layer, 0).ShouldBeEmpty();
    }

    [Fact]
    public void Render_WithPromptAtRow0_WritesOnFirstRow()
    {
        var layer = MakeLayer();
        var sut = new KeyPressComponent(layer.Bounds) { Prompt = "Press any key...", Row = 0 };

        sut.Render(new LayerWriter(layer));

        ReadRow(layer, 0).ShouldBe("Press any key...");
        ReadRow(layer, 1).ShouldBeEmpty();
    }

    [Fact]
    public void Render_WithPromptAtRow5_WritesOnlyOnRow5()
    {
        var layer = MakeLayer();
        var sut = new KeyPressComponent(layer.Bounds) { Prompt = "Press any key...", Row = 5 };

        sut.Render(new LayerWriter(layer));

        ReadRow(layer, 0).ShouldBeEmpty();
        ReadRow(layer, 5).ShouldBe("Press any key...");
        ReadRow(layer, 6).ShouldBeEmpty();
    }
}
