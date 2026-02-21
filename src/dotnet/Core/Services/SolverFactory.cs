namespace Enclave.Echelon.Core.Services;

/// <summary>
/// Selects the <see cref="IPasswordSolver"/> from the configured collection by matching <see cref="ISolverConfiguration.Level"/>.
/// </summary>
public sealed class SolverFactory(IEnumerable<IPasswordSolver> solvers, ISolverConfiguration config) : ISolverFactory
{
    /// <summary>
    /// Default level used when the configured level is invalid or no solver is registered for it.
    /// </summary>
    public static SolverLevel DefaultLevel => SolverLevel.Default;

    private readonly List<IPasswordSolver> _solvers = (solvers ?? throw new ArgumentNullException(nameof(solvers))).ToList();
    private readonly ISolverConfiguration _config = config ?? throw new ArgumentNullException(nameof(config));
    private IPasswordSolver? _cached;

    /// <inheritdoc />
    public IPasswordSolver GetSolver()
    {
        if (_cached != null)
            return _cached;

        var requested = SolverLevel.IsDefined(_config.Level) ? _config.Level : DefaultLevel;
        _cached = _solvers.FirstOrDefault(s => s.Level == requested)
            ?? _solvers.FirstOrDefault(s => s.Level == DefaultLevel);        
        if (_cached == null)
            throw new InvalidOperationException($"No IPasswordSolver registered for level {requested} or default {DefaultLevel}.");
        return _cached;
    }
}
