namespace Enclave.Raven.Services;

/// <summary>
/// Default implementation of <see cref="ICurrentScope"/>: lazily creates the first scope from <see cref="IServiceScopeFactory"/>, and replaces it when <see cref="ResetScope"/> is called.
/// </summary>
/// <remarks>Initializes with the root scope factory.</remarks>
public sealed class CurrentScopeHolder(IServiceScopeFactory scopeFactory) : ICurrentScope
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory 
        ?? throw new ArgumentNullException(nameof(scopeFactory));
    private IServiceScope? _scope;

    /// <inheritdoc />
    public IServiceScope CurrentScope => _scope ??= _scopeFactory.CreateScope();

    /// <inheritdoc />
    public void ResetScope()
    {
        _scope?.Dispose();
        _scope = _scopeFactory.CreateScope();
    }
}
