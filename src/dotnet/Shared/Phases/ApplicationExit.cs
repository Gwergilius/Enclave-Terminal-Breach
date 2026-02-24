using FluentResults;

namespace Enclave.Shared.Phases;

/// <summary>
/// Error indicating that the application requested a normal exit (e.g. user chose Exit or Ctrl+C).
/// Similar in intent to <c>OperationCanceledException</c>; used so the runner can distinguish normal exit from failure and avoid dumping error messages.
/// </summary>
public sealed class ApplicationExit(string message) : Error(message)
{
    /// <summary>Default message for normal application exit.</summary>
    public const string DefaultMessage = "Application exit requested.";

    /// <summary>Initializes with the default message.</summary>
    public ApplicationExit() : this(DefaultMessage) { }
}
