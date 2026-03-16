using Enclave.Shared.Models;

namespace Enclave.Ghost.Services;

/// <summary>
/// Singleton application state: current screen and active game session.
/// Components subscribe to <see cref="StateChanged"/> to re-render on navigation.
/// </summary>
public sealed class GhostAppState
{
    public GhostScreen CurrentScreen { get; private set; } = GhostScreen.Boot;

    /// <summary>Active candidate session. Reset to a fresh instance on each navigation to <see cref="GhostScreen.DataInput"/>.</summary>
    public IGameSession Session { get; private set; } = new GameSession();

    /// <summary>Raised after <see cref="NavigateTo"/> changes the current screen.</summary>
    public event Action? StateChanged;

    /// <summary>Navigate to the given screen. Navigating to DataInput also resets the session for a new round.</summary>
    public void NavigateTo(GhostScreen screen)
    {
        if (screen == GhostScreen.DataInput)
            Session = new GameSession();

        CurrentScreen = screen;
        StateChanged?.Invoke();
    }
}
