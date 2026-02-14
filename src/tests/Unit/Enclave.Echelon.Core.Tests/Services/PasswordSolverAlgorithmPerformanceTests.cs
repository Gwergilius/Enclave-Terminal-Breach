using Enclave.Common.Extensions;
using Enclave.Echelon.Core.Models;
using Enclave.Echelon.Core.Services;
using Xunit.Abstractions;

namespace Enclave.Echelon.Core.Tests.Services;

/// <summary>
/// Performance / convergence tests for the password solver algorithm.
/// Uses the embedded word list (Resources/words.txt). Success = explicit guess hit (response == word length).
/// Test passes if ≥90% of runs find the secret in at most 4 steps (20 runs per difficulty).
/// </summary>
[PerformanceTest, TestOf(nameof(TieBreakerPasswordSolver))]
public class PasswordSolverAlgorithmPerformanceTests
{
    private static readonly Lazy<IReadOnlyList<string>> Words = new(LoadWordsFromCore);
    private readonly IPasswordSolver _solver = new TieBreakerPasswordSolver();
    private readonly ITestOutputHelper _output;

    public PasswordSolverAlgorithmPerformanceTests(ITestOutputHelper output) => _output = output;

    /// <summary>Terminal level and allowed password length range (min, max) per Minigame.md.</summary>
    public static IEnumerable<object[]> DifficultyLevels =>
    [
        ["Very Easy", 4, 5],
        ["Easy", 6, 8],
        ["Average", 9, 10],
        ["Hard", 11, 12],
        ["Very Hard", 13, 15],
    ];

    /// <summary>Solver variants for comparison: TieBreaker (current), BestBucket (RAVEN), HouseGambit (SPARROW).</summary>
    public static IEnumerable<object[]> SolverVariants =>
    [
        ["TieBreaker"],
        ["BestBucket"],
        ["HouseGambit"],
    ];

    /// <summary>All (difficulty, solver) combinations for comparison tests; same seeds used per scenario.</summary>
    public static IEnumerable<object[]> DifficultyAndSolverCombinations =>
        from d in DifficultyLevels
        from s in SolverVariants
        select new[] { d[0], d[1], d[2], s[0] };

    private static IPasswordSolver CreateSolver(string solverName, int seed)
    {
        return solverName switch
        {
            "TieBreaker" => new TieBreakerPasswordSolver(),
            "BestBucket" => new BestBucketPasswordSolver(seed),
            "HouseGambit" => new HouseGambitPasswordSolver(seed),
            _ => throw new ArgumentOutOfRangeException(nameof(solverName), solverName, null)
        };
    }

    /// <summary>At least 90% of runs must find the secret by explicit guess (response == word length) in at most 4 steps.</summary>
    [Theory]
    [MemberData(nameof(DifficultyLevels))]
    public void Solver_FindsSecretWithinFourSteps_InAtLeastNinetyPercentOfRuns(string difficultyName, int minLength, int maxLength)
    {
        const int runsPerDifficulty = 20;
        const int maxSteps = 4;
        const double minSuccessRate = 0.90; // 90%
        var wordList = Words.Value;
        var rnd = new Random(Seed: 17); // fixed seed for reproducible 90% convergence check

        var validLengths = Enumerable.Range(minLength, maxLength - minLength + 1)
            .Where(len => wordList.Count(w => w.Length == len) >= 18).ToList();
        validLengths.Count.ShouldBeGreaterThan(0,
            $"Difficulty {difficultyName}: no length in [{minLength},{maxLength}] has at least 18 words in the list");

        var successCount = 0;
        for (var run = 0; run < runsPerDifficulty; run++)
        {
            var length = validLengths[rnd.Next(validLengths.Count)];
            var ofLength = wordList.Where(w => w.Length == length).ToList();

            var chosen = ofLength.OrderBy(_ => rnd.Next()).Take(18).ToList();
            var candidates = chosen.Select(w => new Password(w)).ToList();
            var secret = candidates[rnd.Next(candidates.Count)];

            var steps = 0;
            var remaining = candidates.ToList();
            var foundByGuess = false;

            while (remaining.Count > 0 && steps < maxSteps)
            {
                var guess = _solver.GetBestGuess(remaining);
                guess.ShouldNotBeNull($"Run {run + 1}/{runsPerDifficulty} ({difficultyName}): no guess returned with {remaining.Count} candidates");

                var response = secret.GetMatchCount(guess);
                steps++;

                if (response == secret.Word.Length)
                {
                    guess.Word.ShouldBe(secret.Word);
                    foundByGuess = true;
                    break;
                }

                remaining = _solver.NarrowCandidates(remaining, guess, response).ToList();
            }

            if (foundByGuess)
                successCount++;
        }

        var rate = successCount / (double)runsPerDifficulty;
        _output.WriteLine($"{difficultyName}: {successCount}/{runsPerDifficulty} = {rate:P0}");
        rate.ShouldBeGreaterThanOrEqualTo(minSuccessRate,
            $"{difficultyName}: at least 90% of runs must find secret in ≤{maxSteps} steps (got {successCount}/{runsPerDifficulty} = {rate:P0})");
    }

    /// <summary>
    /// Adversarial performance test: no fixed secret; the "system" always chooses the response that leaves
    /// the most candidates in play. The secret is only determined when a single candidate remains.
    /// Success = explicit guess hit in at most 4 steps. At least 80% of runs must succeed.
    /// Statistics: success rate and, for failed runs, how many steps were needed to find the secret.
    /// </summary>
    [Theory]
    [MemberData(nameof(DifficultyLevels))]
    public void Solver_Adversarial_AtLeastEightyPercentFindSecretWithinFourSteps_WithFailureStepStats(
        string difficultyName, int minLength, int maxLength)
    {
        const int runsPerDifficulty = 20;
        const int maxSteps = 4;
        // Very Easy (4–5 letters) is harder in adversarial mode due to fewer distinct outcomes; allow 65%.
        var minSuccessRate = difficultyName == "Very Easy" ? 0.65 : 0.80;
        var wordList = Words.Value;
        var rnd = new Random(Seed: 31); // fixed seed for reproducible adversarial stats

        var validLengths = Enumerable.Range(minLength, maxLength - minLength + 1)
            .Where(len => wordList.Count(w => w.Length == len) >= 18).ToList();
        validLengths.Count.ShouldBeGreaterThan(0,
            $"Difficulty {difficultyName}: no length in [{minLength},{maxLength}] has at least 18 words in the list");

        var successCount = 0;
        var failedRunSteps = new List<int>();

        for (var run = 0; run < runsPerDifficulty; run++)
        {
            var length = validLengths[rnd.Next(validLengths.Count)];
            var ofLength = wordList.Where(w => w.Length == length).ToList();
            var chosen = ofLength.OrderBy(_ => rnd.Next()).Take(18).ToList();
            var candidates = chosen.Select(w => new Password(w)).ToList();

            var (foundWithin4, totalStepsToFind) = RunOneAdversarialGameWith(_solver, candidates);

            if (foundWithin4)
                successCount++;
            else
                failedRunSteps.Add(totalStepsToFind);
        }

        var rate = successCount / (double)runsPerDifficulty;
        _output.WriteLine($"{difficultyName} (adversarial): {successCount}/{runsPerDifficulty} = {rate:P0} success in ≤{maxSteps} steps");
        if (failedRunSteps.Count > 0)
        {
            var minS = failedRunSteps.Min();
            var maxS = failedRunSteps.Max();
            var avgS = failedRunSteps.Average();
            _output.WriteLine($"  Failed runs ({failedRunSteps.Count}): steps to find = min {minS}, max {maxS}, avg {avgS:F1}");
            var bySteps = failedRunSteps.GroupBy(s => s).OrderBy(g => g.Key);
            foreach (var g in bySteps)
                _output.WriteLine($"    {g.Count()} run(s) needed {g.Key} steps");
        }

        rate.ShouldBeGreaterThanOrEqualTo(minSuccessRate,
            $"{difficultyName} (adversarial): required {minSuccessRate:P0} find secret in ≤{maxSteps} steps (got {successCount}/{runsPerDifficulty} = {rate:P0})");
    }

    /// <summary>
    /// Comparison test: random secret, same seed (17) for all solvers. Runs 20 games per (difficulty, solver).
    /// Outputs success rate for inclusion in solver comparison doc; no strict success threshold.
    /// </summary>
    [Theory]
    [MemberData(nameof(DifficultyAndSolverCombinations))]
    public void SolverComparison_RandomSecret_OutputsSuccessRate(
        string difficultyName, int minLength, int maxLength, string solverName)
    {
        const int runsPerDifficulty = 20;
        const int maxSteps = 4;
        const int gameSeed = 17;
        var solver = CreateSolver(solverName, gameSeed);
        var wordList = Words.Value;
        var rnd = new Random(gameSeed);

        var validLengths = Enumerable.Range(minLength, maxLength - minLength + 1)
            .Where(len => wordList.Count(w => w.Length == len) >= 18).ToList();
        validLengths.Count.ShouldBeGreaterThan(0,
            $"Difficulty {difficultyName}: no length in [{minLength},{maxLength}] has at least 18 words");

        var successCount = 0;
        for (var run = 0; run < runsPerDifficulty; run++)
        {
            var length = validLengths[rnd.Next(validLengths.Count)];
            var ofLength = wordList.Where(w => w.Length == length).ToList();
            var chosen = ofLength.OrderBy(_ => rnd.Next()).Take(18).ToList();
            var candidates = chosen.Select(w => new Password(w)).ToList();
            var secret = candidates[rnd.Next(candidates.Count)];

            var steps = 0;
            var remaining = candidates.ToList();
            var foundByGuess = false;

            while (remaining.Count > 0 && steps < maxSteps)
            {
                var guess = solver.GetBestGuess(remaining);
                if (guess == null) break;
                var response = secret.GetMatchCount(guess);
                steps++;
                if (response == secret.Word.Length) { foundByGuess = true; break; }
                remaining = solver.NarrowCandidates(remaining, guess, response).ToList();
            }

            if (foundByGuess) successCount++;
        }

        var rate = successCount / (double)runsPerDifficulty;
        _output.WriteLine($"RandomSecret | {difficultyName} | {solverName} | {successCount}/{runsPerDifficulty} = {rate:P0}");
    }

    /// <summary>
    /// Comparison test: adversarial, same seed (31) for all solvers. Runs 20 games per (difficulty, solver).
    /// Outputs success rate and failed-run step stats for inclusion in solver comparison doc.
    /// </summary>
    [Theory]
    [MemberData(nameof(DifficultyAndSolverCombinations))]
    public void SolverComparison_Adversarial_OutputsSuccessRateAndFailureSteps(
        string difficultyName, int minLength, int maxLength, string solverName)
    {
        const int runsPerDifficulty = 20;
        const int gameSeed = 31;
        var solver = CreateSolver(solverName, gameSeed);
        var wordList = Words.Value;
        var rnd = new Random(gameSeed);

        var validLengths = Enumerable.Range(minLength, maxLength - minLength + 1)
            .Where(len => wordList.Count(w => w.Length == len) >= 18).ToList();
        validLengths.Count.ShouldBeGreaterThan(0,
            $"Difficulty {difficultyName}: no length in [{minLength},{maxLength}] has at least 18 words");

        var successCount = 0;
        var failedRunSteps = new List<int>();

        for (var run = 0; run < runsPerDifficulty; run++)
        {
            var length = validLengths[rnd.Next(validLengths.Count)];
            var ofLength = wordList.Where(w => w.Length == length).ToList();
            var chosen = ofLength.OrderBy(_ => rnd.Next()).Take(18).ToList();
            var candidates = chosen.Select(w => new Password(w)).ToList();

            var (foundWithin4, totalStepsToFind) = RunOneAdversarialGameWith(solver, candidates);

            if (foundWithin4) successCount++;
            else failedRunSteps.Add(totalStepsToFind);
        }

        var rate = successCount / (double)runsPerDifficulty;
        var failInfo = failedRunSteps.Count > 0
            ? $"; failed steps min={failedRunSteps.Min()} max={failedRunSteps.Max()} avg={failedRunSteps.Average():F1}"
            : "";
        _output.WriteLine($"Adversarial | {difficultyName} | {solverName} | {successCount}/{runsPerDifficulty} = {rate:P0}{failInfo}");
    }

    /// <summary>
    /// Runs one adversarial game with the given solver. Returns (found in ≤4 steps, total steps to find).
    /// </summary>
    private (bool foundWithin4, int totalStepsToFind) RunOneAdversarialGameWith(
        IPasswordSolver solver, List<Password> candidates)
    {
        var remaining = candidates.ToList();
        var steps = 0;
        var lastResponse = (int?)null;
        Password? lastGuess = null;

        while (remaining.Count > 1)
        {
            var guess = solver.GetBestGuess(remaining);
            if (guess == null) break;

            var buckets = remaining
                .GroupBy(c => guess.GetMatchCount(c))
                .ToDictionary(g => g.Key, g => g.ToList());

            var worstMatchCount = buckets
                .OrderByDescending(kv => kv.Value.Count)
                .ThenByDescending(kv => kv.Key)
                .Select(kv => kv.Key)
                .First();

            lastGuess = guess;
            lastResponse = worstMatchCount;
            remaining = buckets[worstMatchCount];
            steps++;
        }

        if (remaining.Count == 1 && lastGuess != null && lastResponse.HasValue)
        {
            var totalStepsToFind = (lastResponse.Value == lastGuess.Word.Length) ? steps : steps + 1;
            return (totalStepsToFind <= 4, totalStepsToFind);
        }

        return (false, steps);
    }

    /// <summary>
    /// Loads the word list from the Core assembly embedded resource using ResourceExtensions.
    /// The list contains only valid words (4–15 chars); no filtering applied.
    /// </summary>
    private static IReadOnlyList<string> LoadWordsFromCore()
    {
        var result = typeof(TieBreakerPasswordSolver).Assembly.GetResourceString("words.txt");
        result.IsSuccess.ShouldBeTrue("Word list resource missing or failed: " + string.Join("; ", result.Errors.Select(e => e.Message)));

        var words = result.Value!
            .Split(["\r\n", "\n", "\r"], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim().ToUpperInvariant())
            .Where(w => w.Length > 0)
            .ToList();

        words.Count.ShouldBeGreaterThan(0, "Word list must not be empty");
        return words;
    }
}
