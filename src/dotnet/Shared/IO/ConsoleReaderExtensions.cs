using System.Globalization;

namespace Enclave.Shared.IO;

/// <summary>
/// Extension methods for <see cref="IConsoleIO"/> (and any type that provides read + write for prompts).
/// Enables validated integer input for console, Blazor and MAUI implementations.
/// </summary>
public static class ConsoleReaderExtensions
{
    /// <summary>
    /// Prompts until a valid integer in [min, max] is entered.
    /// Returns <paramref name="defaultValue"/> when ReadLine returns null (e.g. EOF).
    /// </summary>
    /// <param name="console">Console that provides ReadLine, Write and WriteLine for prompt and error messages.</param>
    /// <param name="min">Minimum allowed value (inclusive).</param>
    /// <param name="max">Maximum allowed value (inclusive).</param>
    /// <param name="defaultValue">Value returned when input is null.</param>
    /// <param name="prompt">Optional prompt text before reading.</param>
    /// <param name="errorMessage">Optional error message when input is invalid.</param>
    /// <returns>An integer in [min, max].</returns>
    public static int ReadInt(
        this IConsoleIO console,
        int min,
        int max,
        int defaultValue,
        string prompt = "Enter value: ",
        string? errorMessage = null)
    {
        ArgumentNullException.ThrowIfNull(console);
        var message = errorMessage ?? string.Format(CultureInfo.InvariantCulture, "Enter a number between {0} and {1}.", min, max);

        int value = min - 1;
        while (value < min)
        {
            console.Write(prompt);
            var line = console.ReadLine();

            if (line == null || string.IsNullOrWhiteSpace(line))
                return defaultValue;

            if (!int.TryParse(line.Trim(), CultureInfo.InvariantCulture, out value) || value < min || value > max)
            {
                console.WriteLine(message);
                value = min - 1;
            }
        }

        return value;
    }
}
