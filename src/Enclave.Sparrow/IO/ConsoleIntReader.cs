using System.Globalization;

namespace Enclave.Sparrow.IO;

/// <summary>
/// Helper for reading a validated integer from console I/O. Extracted for testability.
/// </summary>
internal static class ConsoleIntReader
{
    /// <summary>
    /// Prompts until a valid integer in [min, max] is entered.
    /// Returns <paramref name="defaultValue"/> when ReadLine returns null.
    /// </summary>
    public static int Read(IConsoleIO console, int min, int max, int defaultValue, string prompt = "Enter value: ", string? errorMessage = null)
    {
        var message = errorMessage ?? string.Format(CultureInfo.InvariantCulture, "Enter a number between {0} and {1}.", min, max);

        int value = min - 1; // invalid sentinel
        while (value < min)
        {
            console.Write(prompt);
            var line = console.ReadLine();

            if (line == null)
            {
                return defaultValue;
            }

            if (!int.TryParse(line.Trim(), CultureInfo.InvariantCulture, out value) || value < min || value > max)
            {
                console.WriteLine(message);
                value = min - 1; // stay in loop
            }
        }

        return value;
    }
}
