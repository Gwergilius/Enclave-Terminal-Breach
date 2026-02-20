using Enclave.Echelon.Core.Models;
using Enclave.Echelon.Core.Services;

namespace Enclave.Echelon.Core.Tests.Services;

/// <summary>
/// Unit tests for <see cref="BestBucketPasswordSolver"/> (best score only, random tie-break). Same contract as base; best-score set matches TieBreaker for these examples.
/// For DANTA/DHOBI/LILTS/OAKUM/ALEFS all five share the same best score, so all five are returned.
/// </summary>
[UnitTest, TestOf(nameof(BestBucketPasswordSolver))]
public class BestBucketPasswordSolverTests : PasswordSolverTestsBase
{
    private const int Seed = 42;

    private static readonly Dictionary<string, HashSet<Password>> _acceptableGuesses = new()
    {
        ["TERMS"] = [.. "TERMS TEXAS TIRES".Split().Select(w => new Password(w))],
        ["SALES"] = [.. "SALES".Split().Select(w => new Password(w))],
        ["DANTA"] = [.. "DANTA DHOBI LILTS OAKUM ALEFS".Split().Select(w => new Password(w))]
    };

    protected override IPasswordSolver Solver { get; } = new BestBucketPasswordSolver(new GameRandom(Seed));
    protected override Dictionary<string, HashSet<Password>> AcceptableGuesses => _acceptableGuesses;
    protected override Dictionary<string, HashSet<Password>> ExpectedBestGuesses => _acceptableGuesses;
}
