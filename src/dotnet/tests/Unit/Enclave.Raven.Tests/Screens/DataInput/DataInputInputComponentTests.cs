using System.Text;
using Enclave.Common.Drawing;
using Enclave.Phosphor;
using Enclave.Raven.Screens.DataInput;

namespace Enclave.Raven.Tests.Screens.DataInput;

[UnitTest, TestOf(nameof(DataInputInputComponent))]
public class DataInputInputComponentTests
{
    private const int Width = 80;

    /// <summary>Input component uses 2 rows (prompt + input line); bounds can be at any Top.</summary>
    private static Layer MakeLayer(int topRow = 0) => new(new Rectangle(0, topRow, Width, 2), zOrder: 0);

    private static string ReadRow(Layer layer, int row)
    {
        var sb = new StringBuilder();
        for (var col = layer.Bounds.Left; col <= layer.Bounds.Right; col++)
            sb.Append(layer.GetCell(col, row).Character);
        return sb.ToString().TrimEnd('\0');
    }

    [Fact]
    public void Render_WithPrompt_WritesPromptRow0_InputLineRow1Cleared()
    {
        var layer = MakeLayer();
        var sut = new DataInputInputComponent(layer.Bounds) { Prompt = "Enter:" };

        sut.Render(new LayerWriter(layer));

        ReadRow(layer, 0).ShouldBe("Enter:");
        ReadRow(layer, 1).Trim().ShouldBeEmpty();
        sut.NextRow.ShouldBe(1);
    }

    [Fact]
    public void Render_WithCurrentLineContent_FillsRow1ThenSpaces()
    {
        var layer = MakeLayer();
        var sut = new DataInputInputComponent(layer.Bounds) { Prompt = ">", CurrentLineContent = "hello" };

        sut.Render(new LayerWriter(layer));

        ReadRow(layer, 0).ShouldBe(">");
        ReadRow(layer, 1).TrimEnd().ShouldBe("hello");
        sut.NextRow.ShouldBe(1);
    }

    [Fact]
    public void NextRow_WhenBoundsAtBottom_ReturnsAbsoluteScreenRow()
    {
        var layer = MakeLayer(topRow: 21);
        var sut = new DataInputInputComponent(layer.Bounds);

        sut.Render(new LayerWriter(layer));

        sut.NextRow.ShouldBe(22);
    }
}
