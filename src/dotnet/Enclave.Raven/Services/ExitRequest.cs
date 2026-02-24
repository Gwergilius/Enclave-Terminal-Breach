namespace Enclave.Raven.Services;

/// <summary>
/// Default implementation of <see cref="IExitRequest"/> using a volatile flag.
/// </summary>
public sealed class ExitRequest : IExitRequest
{
    private volatile bool _exitRequested;

    /// <inheritdoc />
    public bool IsExitRequested => _exitRequested;

    /// <inheritdoc />
    public void RequestExit() => _exitRequested = true;
}
