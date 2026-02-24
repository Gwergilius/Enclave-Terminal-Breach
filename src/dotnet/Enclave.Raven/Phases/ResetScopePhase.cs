using System.Diagnostics.CodeAnalysis;
using Enclave.Raven.Services;
using Enclave.Shared.Phases;
using FluentResults;

namespace Enclave.Raven.Phases;

/// <summary>
/// Resets the current scope and navigates to the target phase name passed in <paramref name="args"/> (e.g. <c>Run("DataInput")</c>).
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Thin orchestration; scope and navigation are tested via integration.")]
public sealed class ResetScopePhase(
    ICurrentScope scopeHolder,
    INavigationService navigation) : IResetScopePhase
{
    /// <inheritdoc />
    public string Name => "ResetScope";

    /// <inheritdoc />
    public Result Run(params object[] args)
    {
        var targetPhaseName = args is { Length: > 0 } && args[0] is string s
            ? s
            : "DataInput";
        scopeHolder.ResetScope();
        return navigation.NavigateTo(targetPhaseName);
    }
}