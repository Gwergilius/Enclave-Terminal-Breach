using Enclave.Phosphor;

namespace Enclave.Raven.Screens.Help;

/// <summary>
/// Renders the full RAVEN help text with phosphor-themed styling:
/// option names/headers use <see cref="CharStyle.Bright"/>, descriptions use <see cref="CharStyle.Normal"/>,
/// and sub-values / developer notes use <see cref="CharStyle.Dark"/>.
/// </summary>
public sealed class HelpScreenComponent(Rectangle bounds) : IComponent
{
    /// <inheritdoc />
    public Rectangle Bounds { get; } = bounds;

    /// <summary>The row (0-based) at which to start rendering. Defaults to 0.</summary>
    public int Row { get; set; } = 0;

    /// <inheritdoc />
    public void Render(LayerWriter writer)
    {
        writer.MoveTo(0, Row);

        // Usage line
        writer.Style = CharStyle.Bright;  writer.Write("raven [options]\n");
        writer.Style = CharStyle.Normal;  writer.Write("\n");

        // Options section
        writer.Style = CharStyle.Bright;  writer.Write("Options:\n");

        // -i / --intelligence
        writer.Style = CharStyle.Bright;  writer.Write("  -i, --intelligence <level>    ");
        writer.Style = CharStyle.Normal;  writer.Write("Solver intelligence level (default: tie)\n");
        writer.Style = CharStyle.Dark;    writer.Write("                                  0 / house   = Random (HOUSE gambit)\n");
        writer.Style = CharStyle.Dark;    writer.Write("                                  1 / bucket  = Smart (Best-bucket)\n");
        writer.Style = CharStyle.Dark;    writer.Write("                                  2 / tie     = Genius (Tie-breaker)\n");
        writer.Style = CharStyle.Normal;  writer.Write("\n");

        // -t / --theme
        writer.Style = CharStyle.Bright;  writer.Write("  -t, --theme <name>            ");
        writer.Style = CharStyle.Normal;  writer.Write("Color theme (default: green)\n");
        writer.Style = CharStyle.Dark;    writer.Write("                                  green  = Classic green phosphor\n");
        writer.Style = CharStyle.Dark;    writer.Write("                                  amber  = Amber phosphor\n");
        writer.Style = CharStyle.Dark;    writer.Write("                                  white  = White / monochrome\n");
        writer.Style = CharStyle.Dark;    writer.Write("                                  blue   = Blue terminal\n");
        writer.Style = CharStyle.Normal;  writer.Write("\n");

        // -w / --words
        writer.Style = CharStyle.Bright;  writer.Write("  -w, --words <file>            ");
        writer.Style = CharStyle.Normal;  writer.Write("Word list file path (optional)\n");
        writer.Style = CharStyle.Dark;    writer.Write("                                  Load candidates from file instead of manual entry\n");
        writer.Style = CharStyle.Normal;  writer.Write("\n");

        // -h / --help
        writer.Style = CharStyle.Bright;  writer.Write("  -h, --help                    ");
        writer.Style = CharStyle.Normal;  writer.Write("Show this help message and exit\n");

        // -v / --version
        writer.Style = CharStyle.Bright;  writer.Write("  -v, --version                 ");
        writer.Style = CharStyle.Normal;  writer.Write("Show version information and exit\n");
        writer.Style = CharStyle.Normal;  writer.Write("\n");

        // Examples section
        writer.Style = CharStyle.Bright;  writer.Write("Examples:\n");
        writer.Style = CharStyle.Normal;  writer.Write("  raven                         Use defaults (intelligence: tie, theme: green)\n");
        writer.Style = CharStyle.Normal;  writer.Write("  raven -i 0                    Use random solver (HOUSE gambit)\n");
        writer.Style = CharStyle.Normal;  writer.Write("  raven -i house                Same as above (alias)\n");
        writer.Style = CharStyle.Normal;  writer.Write("  raven -t amber                Use amber phosphor theme\n");
        writer.Style = CharStyle.Normal;  writer.Write("  raven -t blue -i 2            Blue theme with optimized solver\n");
        writer.Style = CharStyle.Normal;  writer.Write("  raven -w words.txt            Load candidates from file\n");
        writer.Style = CharStyle.Normal;  writer.Write("  raven -i 2 -w words.txt       Combine intelligence and word list\n");
        writer.Style = CharStyle.Normal;  writer.Write("\n");

        // Developer note (dotnet run usage)
        writer.Style = CharStyle.Dark;    writer.Write("When using dotnet run, pass application options after -- so they reach the app:\n");
        writer.Style = CharStyle.Dark;    writer.Write("  dotnet run --project Enclave.Raven.csproj -- --help\n");
        writer.Style = CharStyle.Dark;    writer.Write("  dotnet run --project Enclave.Raven.csproj -- -t amber -i 2\n");
    }
}
