using System.Text;
using Enclave.Common.Drawing;
using Enclave.Phosphor;
using Enclave.Raven.Screens.DataInput;

namespace Enclave.Raven.Tests.Screens.DataInput;

[UnitTest, TestOf(nameof(DataInputCandidatesComponent))]
public class DataInputCandidatesComponentTests
{
    private static Layer MakeLayer(int rows = 7) => new(new Rectangle(0, 0, 80, rows), zOrder: 0);

    private static string ReadRow(Layer layer, int row)
    {
        var sb = new StringBuilder();
        for (var col = layer.Bounds.Left; col <= layer.Bounds.Right; col++)
            sb.Append(layer.GetCell(col, row).Character);
        return sb.ToString().TrimEnd('\0');
    }

    [Fact]
    public void Render_WithNoData_LeavesLayerEmpty()
    {
        var layer = MakeLayer();
        var sut = new DataInputCandidatesComponent(layer.Bounds);

        sut.Render(new LayerWriter(layer));

        ReadRow(layer, 0).ShouldBeEmpty();
    }

    [Fact]
    public void Render_WithCountAndList_WritesFromRow0()
    {
        var layer = MakeLayer();
        var sut = new DataInputCandidatesComponent(layer.Bounds)
        {
            CandidateCountLine = "3 candidate(s):",
            CandidateListText  = "AAA  BBB  CCC",
        };

        sut.Render(new LayerWriter(layer));

        ReadRow(layer, 0).ShouldBe("3 candidate(s):");
        ReadRow(layer, 1).ShouldBe("AAA  BBB  CCC");
        ReadRow(layer, 2).ShouldBeEmpty();
    }

    [Fact]
    public void Render_WithMultiLineList_FillsConsecutiveRows()
    {
        var layer = MakeLayer();
        var sut = new DataInputCandidatesComponent(layer.Bounds)
        {
            CandidateCountLine = "7 candidate(s):",
            CandidateListText  = "GRASS  SCOPE  SHOPS  SLAVE  SNAKE\nSTOPS  WEARS",
        };

        sut.Render(new LayerWriter(layer));

        ReadRow(layer, 0).ShouldBe("7 candidate(s):");
        ReadRow(layer, 1).ShouldBe("GRASS  SCOPE  SHOPS  SLAVE  SNAKE");
        ReadRow(layer, 2).ShouldBe("STOPS  WEARS");
    }
}
