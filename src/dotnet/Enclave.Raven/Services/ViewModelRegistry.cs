using Enclave.Common.Errors;
using Enclave.Raven.Screens;
using FluentResults;

namespace Enclave.Raven.Services;

/// <summary>
/// Default implementation of <see cref="IViewModelRegistry"/>: holds ViewModels from the current scope
/// (injected via <see cref="IEnumerable{IScreenViewModel}"/>) and returns the matching instance by name.
/// </summary>
public sealed class ViewModelRegistry : IViewModelRegistry
{
    private readonly IReadOnlyDictionary<string, IScreenViewModel> _nameToViewModel;

    /// <param name="viewModels">All ViewModel instances from the current scope.</param>
    public ViewModelRegistry(IEnumerable<IScreenViewModel> viewModels)
    {
        ArgumentNullException.ThrowIfNull(viewModels);
        _nameToViewModel = viewModels.ToDictionary(vm => vm.Name, StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public Result<IScreenViewModel> GetViewModel(string name)
    {
        if (_nameToViewModel.TryGetValue(name, out var vm))
            return Result.Ok(vm);
        return Result.Fail<IScreenViewModel>(new NotFoundError($"ViewModel '{name}' is not registered."));
    }
}
