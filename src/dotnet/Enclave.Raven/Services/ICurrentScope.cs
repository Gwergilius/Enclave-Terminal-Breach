using Microsoft.Extensions.DependencyInjection;

namespace Enclave.Raven.Services;

/// <summary>
/// Holds the current <see cref="IServiceScope"/> used for resolving scoped services (e.g. phases).
/// <see cref="ResetScope"/> disposes the current scope and creates a new one so that the next resolution gets fresh scoped instances.
/// </summary>
public interface ICurrentScope
{
    /// <summary>Gets the current scope; creates one on first access.</summary>
    IServiceScope CurrentScope { get; }

    /// <summary>Disposes the current scope and creates a new one; the next <see cref="CurrentScope"/> access returns the new scope.</summary>
    void ResetScope();
}