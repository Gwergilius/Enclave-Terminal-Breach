using System.Text;
using Enclave.Common.Drawing;
using Enclave.Phosphor;
using Enclave.Raven.Screens.DataInput;

namespace Enclave.Raven.Tests.Screens.DataInput;

[UnitTest, TestOf(nameof(DataInputErrorComponent))]
public class DataInputErrorComponentTests
{
    private const int Width = 80;

    private static Layer MakeLayer(int topRow = 0) => new(new Rectangle(0, topRow, Width, 1), zOrder: 0);

    private static string ReadRow(Layer layer, int row)
    {
        var sb = new StringBuilder();
        for (var col = layer.Bounds.Left; col <= layer.Bounds.Right; col++)
            sb.Append(layer.GetCell(col, row).Character);
        return sb.ToString().TrimEnd('\0');
    }

    [Fact]
    public void Render_WithNullMessage_WritesEmptyRow()
    {
        var layer = MakeLayer();
        var sut = new DataInputErrorComponent(layer.Bounds);

        sut.Render(new LayerWriter(layer));

        ReadRow(layer, 0).ShouldBeEmpty();
    }

    [Fact]
    public void Render_WithMessage_WritesOnRow0()
    {
        var layer = MakeLayer();
        var sut = new DataInputErrorComponent(layer.Bounds) { ErrorMessage = "Already in list (ignored): GRASS" };

        sut.Render(new LayerWriter(layer));

        ReadRow(layer, 0).ShouldBe("Already in list (ignored): GRASS");
    }
}
