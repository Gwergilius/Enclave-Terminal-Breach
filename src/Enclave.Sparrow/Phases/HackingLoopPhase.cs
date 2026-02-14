using System.Globalization;
using Enclave.Echelon.Core.Models;
using Enclave.Echelon.Core.Services;
using Enclave.Sparrow.IO;
using Enclave.Sparrow.Session;

namespace Enclave.Sparrow.Phases;

/// <summary>
/// Hacking loop: suggest guess, read match count, narrow candidates until win (SPARROW-Requirements §3).
/// </summary>
public sealed class HackingLoopPhase : IHackingLoopPhase
{
    private readonly IGameSession _session;
    private readonly IConsoleIO _console;
    private readonly IPasswordSolver _solver;

    public HackingLoopPhase(IGameSession session, IConsoleIO console, IPasswordSolver solver)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _solver = solver ?? throw new ArgumentNullException(nameof(solver));
    }

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

            _console.WriteLine();
            _console.WriteLine($"Suggested guess: `{guess.Word}`");
            _console.Write("Match count? ");
            var line = _console.ReadLine();
            if (line == null) return;

            if (!int.TryParse(line.Trim(), CultureInfo.InvariantCulture, out var matchCount) || matchCount < 0 || matchCount > wordLength)
            {
                _console.WriteLine("Enter a number between 0 and " + wordLength + ".");
                continue;
            }

            if (matchCount == wordLength)
            {
                _console.WriteLine();
                _console.WriteLine("Correct. Terminal cracked.");
                return;
            }

            var narrowed = _solver.NarrowCandidates(_session.Candidates, guess, matchCount);
            _session.Candidates.Clear();
            foreach (var p in narrowed)
                _session.Candidates.Add(p);

            _console.WriteLine();
            _console.WriteLine($"{_session.Candidates.Count} candidate(s):");
            _console.WriteLine(CandidateListFormatter.Format(_session.Candidates.ToList(), wordLength));
        }
    }
}
