using Enclave.Echelon.Core.Models;

namespace Enclave.Echelon.Core.Services;

/// <summary>
/// Picks a guess uniformly at random from the candidates (no score). Lore: SPARROW HOUSE gambit.
/// Overrides only <see cref="GetBestGuesses"/> (returns a single random candidate); uses base for <see cref="GetBestGuess"/>, <see cref="CalculateInformationScore"/>, and <see cref="NarrowCandidates"/>.
/// </summary>
public class HouseGambitPasswordSolver(IRandom random) : PasswordSolverBase
{
    private readonly IRandom _random = random;

    /// <inheritdoc />
    public override SolverLevel Level => SolverLevel.HouseGambit;

    /// <inheritdoc />
    public override IReadOnlyList<Password> GetBestGuesses(IEnumerable<Password> candidates)
    {
        var list = candidates?.ToList() ?? throw new ArgumentNullException(nameof(candidates));
        if (list.Count <= 1) return list;
        var pick = list[_random.Next(list.Count)];
        return [pick];
    }
}
