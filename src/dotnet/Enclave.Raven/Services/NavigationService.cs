using FluentResults;
using Enclave.Shared.Phases;

namespace Enclave.Raven.Services;

/// <summary>
/// RAVEN implementation of <see cref="INavigationService"/>: normal phases get <see cref="Result.Ok"/>; <see cref="NavigateTo"/>("Exit", ...) returns <see cref="ApplicationExit"/> or the optional <see cref="Result"/> from the first argument.
/// </summary>
public sealed class NavigationService : INavigationService
{
    private string? _nextPhase;
    private object[] _nextPhaseArgs = [];

    /// <inheritdoc />
    public string? NextPhase => _nextPhase;

    /// <inheritdoc />
    public IReadOnlyList<object> NextPhaseArgs => _nextPhaseArgs;

    /// <inheritdoc />
    public Result NavigateTo(string phaseName, params object[] args)
    {
        _nextPhase = phaseName ?? throw new ArgumentNullException(nameof(phaseName));
        _nextPhaseArgs = args ?? [];

        if (string.Equals(phaseName, "Exit", StringComparison.OrdinalIgnoreCase))
        {
            if (args is { Length: > 0 } && args[0] is Result exitResult)
                return exitResult;
            return Result.Fail(new ApplicationExit());
        }

        return Result.Ok();
    }
}
