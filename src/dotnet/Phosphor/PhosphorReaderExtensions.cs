using System.Globalization;
using Enclave.Shared.IO;

namespace Enclave.Phosphor;

/// <summary>
/// Extension methods for <see cref="IPhosphorReader"/> when used with <see cref="IPhosphorWriter"/> for prompt and error output.
/// Enables validated integer input in PHOSPHOR (Raven, Blazor, MAUI) without requiring a single read-write console type.
/// </summary>
public static class PhosphorReaderExtensions
{
    public static int ReadInt(
        this IConsoleIO console,
        int min,
        int max,
        int defaultValue,
        string prompt = "Enter value: ",
        string? errorMessage = null)
        => console.ReadInt(console, min, max, defaultValue, prompt, errorMessage);
    public static int ReadInt(
        this IPhosphorReader reader,
        IPhosphorWriter writer,
        int min,
        int max,
        int defaultValue,
        string prompt = "Enter value: ",
        string? errorMessage = null)
        => ReadInt((IConsoleReader)reader, (IConsoleWriter)writer, min, max, defaultValue, prompt, errorMessage);

    /// <summary>
    /// Prompts until a valid integer in [min, max] is entered.
    /// Returns <paramref name="defaultValue"/> when ReadLine returns null (e.g. EOF).
    /// </summary>
    /// <param name="reader">Reader for ReadLine.</param>
    /// <param name="writer">Writer for prompt and error messages.</param>
    /// <param name="min">Minimum allowed value (inclusive).</param>
    /// <param name="max">Maximum allowed value (inclusive).</param>
    /// <param name="defaultValue">Value returned when input is null.</param>
    /// <param name="prompt">Optional prompt text before reading.</param>
    /// <param name="errorMessage">Optional error message when input is invalid.</param>
    /// <returns>An integer in [min, max].</returns>
    public static int ReadInt(
        this IConsoleReader reader,
        IConsoleWriter writer,
        int min,
        int max,
        int defaultValue,
        string prompt = "Enter value: ",
        string? errorMessage = null)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentNullException.ThrowIfNull(writer);
        var message = errorMessage ?? string.Format(CultureInfo.InvariantCulture, "Enter a number between {0} and {1}.", min, max);

        int value = min - 1;
        while (value < min)
        {
            writer.Write(prompt);
            var line = reader.ReadLine();

            if (string.IsNullOrWhiteSpace(line))
                return defaultValue;

            if (!int.TryParse(line.Trim(), CultureInfo.InvariantCulture, out value) || value < min || value > max)
            {
                writer.WriteLine(message);
                value = min - 1;
            }
        }

        return value;
    }
}
