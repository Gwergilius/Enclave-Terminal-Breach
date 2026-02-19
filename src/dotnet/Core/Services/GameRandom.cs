namespace Enclave.Echelon.Core.Services;

/// <summary>
/// Provides random number generation for non-security purposes (e.g. game suggestions ). 
/// 
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Security",
    "S2245:Using pseudorandom number generators is security-sensitive",
    Justification = "Non-security use: game suggestion randomization only.")]
public sealed class GameRandom : IRandom
{
    private readonly Random _random;

    private GameRandom(Random? random)
    {
        _random = random ?? new Random();
    }

    public GameRandom(int seed) : this(new Random(seed))
    {
    }

    public GameRandom() : this((Random?)null)
    {
    }

    public double Next() => _random.NextDouble();
    public int Next(int minValue, int maxValue) => _random.Next(minValue, maxValue);
    public int Next(int maxValue) => _random.Next(maxValue);

#if SHUFFLE
    public void Shuffle<T>(IList<T> list)
    {
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = _random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
#endif
}
