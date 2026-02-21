using System.Diagnostics.CodeAnalysis;
using Enclave.Echelon.Core.Services;
using Enclave.Phosphor;
using Enclave.Shared.Models;

namespace Enclave.Raven.Phases;

/// <summary>
/// Hacking loop: suggest guess, read match count, narrow candidates until win (RAVEN-Requirements §3).
/// </summary>
public sealed class HackingLoopPhase(
    [NotNull] IGameSession session,
    [NotNull] IPhosphorWriter writer,
    [NotNull] IPhosphorReader reader,
    [NotNull] ISolverFactory solverFactory) : IHackingLoopPhase
{
    private readonly IGameSession _session = session;
    private readonly IPhosphorWriter _writer = writer;
    private readonly IPhosphorReader _reader = reader;
    private readonly IPasswordSolver _solver = solverFactory.GetSolver();

    /// <inheritdoc />
    public void Run()
    {
        var wordLength = _session.WordLength ?? 0;
        if (wordLength <= 0 || _session.Count == 0)
        {
            _writer.WriteLine("No candidates. Exiting.");
            return;
        }

        while (true)
        {
            var guess = _solver.GetBestGuess(_session);
            if (guess == null)
            {
                _writer.WriteLine("No candidates left. Exiting.");
                return;
            }

            int matchCount = ReadMatchCount(guess);

            if (matchCount == wordLength)
            {
                _writer.WriteLine();
                _writer.WriteLine("Correct. Terminal cracked.");
                return;
            }

            NarrowCandidates(guess, matchCount);
            WriteCandidates(wordLength);
        }
    }

    private void WriteCandidates(int wordLength)
    {
        _writer.WriteLine();
        _writer.WriteLine($"{_session.Count} candidate(s):");
        _writer.WriteLine(CandidateListFormatter.Format(_session, wordLength));
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
        _writer.WriteLine();
        _writer.WriteLine($"Suggested guess: `{guess.Word}`");
        return _reader.ReadInt(_writer, 0, wordLength, wordLength, "Match count? ");
    }
}
