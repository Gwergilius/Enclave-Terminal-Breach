namespace Enclave.Raven.Screens;

/// <summary>
/// Contract for a screen-level ViewModel: drives state transitions, timing, and navigation
/// for one full-screen UI context (boot, data-input, hacking loop).
/// </summary>
/// <remarks>
/// Responsibilities: async orchestration, input handling, invalidation, navigation.
/// Must not perform direct terminal writes — rendering belongs to the paired component.
/// </remarks>
public interface IScreenViewModel
{
    /// <summary>Unique identifier used by <see cref="Enclave.Shared.Phases.INavigationService"/> to address this screen.</summary>
    string Name { get; }

    /// <summary>Runs the screen until it navigates away (by calling <see cref="Enclave.Shared.Phases.INavigationService.NavigateTo"/>).</summary>
    Task RunAsync(CancellationToken ct = default);
}
