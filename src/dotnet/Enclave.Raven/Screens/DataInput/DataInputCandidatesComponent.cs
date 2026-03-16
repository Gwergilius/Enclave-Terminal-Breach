using Enclave.Phosphor;

namespace Enclave.Raven.Screens.DataInput;

/// <summary>
/// Renders the candidates region (top of the data-input screen): "n candidate(s):" and the word list.
/// Uses its own layer so it never overlaps the input region.
/// </summary>
public sealed class DataInputCandidatesComponent(Rectangle bounds) : IComponent
{
    /// <inheritdoc />
    public Rectangle Bounds { get; } = bounds;

    public string? CandidateCountLine { get; set; }
    public string? CandidateListText { get; set; }

    /// <inheritdoc />
    public void Render(LayerWriter writer)
    {
        var row = 0;
        var lastRow = Bounds.Height - 1;

        if (CandidateCountLine is not null && row <= lastRow)
        {
            writer.MoveTo(0, row);
            writer.Write(CandidateCountLine);
            writer.Write("\n");
            row++;
        }
        if (CandidateListText is not null)
        {
            foreach (var line in CandidateListText.Split('\n'))
            {
                if (row > lastRow) break;
                writer.MoveTo(0, row);
                writer.Write(line);
                writer.Write("\n");
                row++;
            }
        }
    }
}
