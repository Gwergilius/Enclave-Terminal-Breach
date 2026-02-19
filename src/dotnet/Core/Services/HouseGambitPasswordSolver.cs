using Enclave.Common.Extensions;
using Enclave.Echelon.Core.Models;

namespace Enclave.Echelon.Core.Services;

/// <summary>
/// Picks a guess uniformly at random from the candidates (no score). Lore: SPARROW HOUSE gambit.
/// Overrides only <see cref="GetBestGuesses"/> (returns a single random candidate); uses base for <see cref="GetBestGuess"/>, <see cref="CalculateInformationScore"/>, and <see cref="NarrowCandidates"/>.
/// </summary>
/// <remarks>If no <see cref="Random"/> is provided, uses a new <see cref="Random"/> instance (non-deterministic).</remarks>
public class HouseGambitPasswordSolver(Random? random = null) : PasswordSolverBase
{
    private readonly Random _random = random.Enforce();

    /// <summary>Creates a solver that picks guesses at random using the given seed for reproducibility.</summary>
    public HouseGambitPasswordSolver(int seed) : this(new Random(seed)) { }

    /// <inheritdoc />
    public override IReadOnlyList<Password> GetBestGuesses(IEnumerable<Password> candidates)
    {
        var list = candidates?.ToList() ?? throw new ArgumentNullException(nameof(candidates));
        if (list.Count <= 1) return list;
        var pick = list[_random.Next(list.Count)];
        return [pick];
    }
}
