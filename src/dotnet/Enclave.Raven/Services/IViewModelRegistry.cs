using Enclave.Raven.Screens;
using FluentResults;

namespace Enclave.Raven.Services;

/// <summary>
/// Provides the <see cref="IScreenViewModel"/> instance for a given screen name (from the current scope).
/// </summary>
public interface IViewModelRegistry
{
    /// <summary>Gets the ViewModel for the given screen name (e.g. "BootScreen").</summary>
    Result<IScreenViewModel> GetViewModel(string name);
}
