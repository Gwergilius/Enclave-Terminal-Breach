using System.Text;
using Enclave.Common.Drawing;
using Enclave.Common.Test.Core;
using Enclave.Phosphor;
using Enclave.Raven.Screens.Help;

namespace Enclave.Raven.Tests.Screens.Help;

[UnitTest, TestOf(nameof(HelpScreenComponent))]
public class HelpScreenComponentTests
{
    // Layer wide enough for the longest help line (~80 chars), tall enough for all rows (~35).
    private static Layer MakeLayer() => new(new Rectangle(0, 0, 100, 40), zOrder: 0);

    private static string ReadRow(Layer layer, int row)
    {
        var sb = new StringBuilder();
        for (var col = layer.Bounds.Left; col <= layer.Bounds.Right; col++)
            sb.Append(layer.GetCell(col, row).Character);
        return sb.ToString().TrimEnd('\0');
    }

    // --- usage line ---

    [Fact]
    public void Render_Row0_ContainsUsageLine()
    {
        var layer = MakeLayer();
        var sut = new HelpScreenComponent(layer.Bounds);

        sut.Render(new LayerWriter(layer));

        ReadRow(layer, 0).ShouldStartWith("raven [options]");
    }

    [Fact]
    public void Render_Row0_UsesCharStyleBright()
    {
        var layer = MakeLayer();
        var sut = new HelpScreenComponent(layer.Bounds);

        sut.Render(new LayerWriter(layer));

        layer.GetCell(0, 0).Style.ShouldBe(CharStyle.Bright);
    }

    // --- sub-value rows use Dark style ---

    [Fact]
    public void Render_IntelligenceSubValueRow_UsesCharStyleDark()
    {
        var layer = MakeLayer();
        var sut = new HelpScreenComponent(layer.Bounds);

        sut.Render(new LayerWriter(layer));

        // Row layout from Row=0:
        //  0: "raven [options]"   (Bright)
        //  1: ""                  (Normal/empty)
        //  2: "Options:"          (Bright)
        //  3: "  -i, ... "        (Bright) + description (Normal)
        //  4: "  0 / house..."    (Dark) ← first sub-value
        ReadRow(layer, 4).ShouldNotBeEmpty();
        layer.GetCell(0, 4).Style.ShouldBe(CharStyle.Dark);
    }

    // --- row offset ---

    [Fact]
    public void Render_WithRowOffset_StartsAtOffsetRow()
    {
        var layer = MakeLayer();
        var sut = new HelpScreenComponent(layer.Bounds) { Row = 2 };

        sut.Render(new LayerWriter(layer));

        ReadRow(layer, 0).ShouldBeEmpty();
        ReadRow(layer, 2).ShouldStartWith("raven [options]");
    }

    // --- examples section ---

    [Fact]
    public void Render_ContainsExamplesSection()
    {
        var layer = MakeLayer();
        var sut = new HelpScreenComponent(layer.Bounds);

        sut.Render(new LayerWriter(layer));

        // Scan all rows for "Examples:"
        var found = false;
        for (var row = layer.Bounds.Top; row <= layer.Bounds.Bottom; row++)
        {
            if (ReadRow(layer, row).StartsWith("Examples:")) { found = true; break; }
        }
        found.ShouldBeTrue("Expected to find an 'Examples:' row in the rendered help.");
    }
}
