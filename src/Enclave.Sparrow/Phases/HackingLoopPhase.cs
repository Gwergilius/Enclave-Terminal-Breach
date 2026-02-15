using Enclave.Echelon.Core.Services;
using Enclave.Sparrow.IO;
using Enclave.Sparrow.Session;

namespace Enclave.Sparrow.Phases;

/// <summary>
/// Hacking loop: suggest guess, read match count, narrow candidates until win (SPARROW-Requirements §3).
/// </summary>
public sealed class HackingLoopPhase(IGameSession session, IConsoleIO console, IPasswordSolver solver) : IHackingLoopPhase
{
    private readonly IGameSession _session = session ?? throw new ArgumentNullException(nameof(session));
    private readonly IConsoleIO _console = console ?? throw new ArgumentNullException(nameof(console));
    private readonly IPasswordSolver _solver = solver ?? throw new ArgumentNullException(nameof(solver));

    /// <inheritdoc />
    public void Run()
    {
        var wordLength = _session.WordLength ?? 0;
        if (wordLength <= 0 || _session.Candidates.Count == 0)
        {
            _console.WriteLine("No candidates. Exiting.");
            return;
        }

        while (true)
        {
            var guess = _solver.GetBestGuess(_session.Candidates);
            if (guess == null)
            {
                _console.WriteLine("No candidates left. Exiting.");
                return;
            }

            int matchCount = ReadMatchCount(guess);

            if (matchCount == wordLength)
            {
                _console.WriteLine();
                _console.WriteLine("Correct. Terminal cracked.");
                return;
            }

            NarrowCandidates(guess, matchCount);
            WriteCandidates(wordLength);
        }
    }

    private void WriteCandidates(int wordLength)
    {
        _console.WriteLine();
        _console.WriteLine($"{_session.Candidates.Count} candidate(s):");
        _console.WriteLine(CandidateListFormatter.Format(_session.Candidates.ToList(), wordLength));
    }

    private void NarrowCandidates(Echelon.Core.Models.Password guess, int matchCount)
    {
        var narrowed = _solver.NarrowCandidates(_session.Candidates, guess, matchCount);
        _session.Candidates.Clear();
        foreach (var p in narrowed)
            _session.Candidates.Add(p);
    }

    private int ReadMatchCount(Echelon.Core.Models.Password guess)
    {
        int wordLength = guess.Word.Length;  
        _console.WriteLine();
        _console.WriteLine($"Suggested guess: `{guess.Word}`");
        _console.Write("Match count? ");

        int matchCount = -1;
        while (matchCount < 0)
        {
            var line = _console.ReadLine();

            if (line == null)
            {
                matchCount = wordLength;
            }
            else if (!int.TryParse(line.Trim(), CultureInfo.InvariantCulture, out matchCount) || matchCount < 0 || matchCount > wordLength)
            {
                _console.WriteLine($"Enter a number between 0 and {wordLength}.");
            }
        }

        return matchCount;
    }
}
