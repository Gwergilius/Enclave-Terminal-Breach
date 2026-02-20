namespace Enclave.Common.Services;

public interface IStorageService
{
    Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default);
    Task SetStringAsync(string key, string? value, CancellationToken cancellationToken = default);
}
