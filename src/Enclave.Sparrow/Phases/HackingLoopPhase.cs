using System.Diagnostics.CodeAnalysis;
using Enclave.Echelon.Core.Services;
using Enclave.Sparrow.IO;
using Enclave.Sparrow.Models;

namespace Enclave.Sparrow.Phases;

/// <summary>
/// Hacking loop: suggest guess, read match count, narrow candidates until win (SPARROW-Requirements §3).
/// </summary>
public sealed class HackingLoopPhase([NotNull] IGameSession session, [NotNull] IConsoleIO console, [NotNull] IPasswordSolver solver) : IHackingLoopPhase
{
    private readonly IGameSession _session = session;
    private readonly IConsoleIO _console = console;
    private readonly IPasswordSolver _solver = solver;

    /// <inheritdoc />
    public void Run()
    {
        var wordLength = _session.WordLength ?? 0;
        if (wordLength <= 0 || _session.Count == 0)
        {
            _console.WriteLine("No candidates. Exiting.");
            return;
        }

        while (true)
        {
            var guess = _solver.GetBestGuess(_session);
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
        _console.WriteLine($"{_session.Count} candidate(s):");
        _console.WriteLine(CandidateListFormatter.Format(_session, wordLength));
    }

    private void NarrowCandidates(Echelon.Core.Models.Password guess, int matchCount)
    {
        var narrowed = _solver.NarrowCandidates(_session, guess, matchCount);
        _session.Clear();
        foreach (var p in narrowed)
            _session.Add(p);
    }

    private int ReadMatchCount(Echelon.Core.Models.Password guess)
    {
        var wordLength = guess.Word.Length;
        _console.WriteLine();
        _console.WriteLine($"Suggested guess: `{guess.Word}`");
        return _console.ReadInt(0, wordLength, wordLength, "Match count? ");
    }
}
