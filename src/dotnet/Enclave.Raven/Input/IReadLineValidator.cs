namespace Enclave.Raven.Input;

/// <summary>
/// Optional validator for the read-line: on Enter, if invalid, the runner re-prompts and shows the error.
/// </summary>
public interface IReadLineValidator
{
    /// <summary>
    /// Validate the current line. If invalid, the runner calls <see cref="ReadLineParams.OnInvalidInput"/> and continues.
    /// </summary>
    /// <param name="line">The line entered (may be empty).</param>
    /// <returns>True if valid (runner returns the line); false and error message to show and re-prompt.</returns>
    (bool IsValid, string? ErrorMessage) Validate(string line);
}
