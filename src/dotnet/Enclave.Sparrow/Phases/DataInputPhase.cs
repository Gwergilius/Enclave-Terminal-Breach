using System.Diagnostics.CodeAnalysis;
using Enclave.Sparrow.IO;
using Enclave.Sparrow.Models;

namespace Enclave.Sparrow.Phases;

/// <summary>
/// Data-input phase: prompts for password candidates until empty line; fills session (SPARROW-Requirements §2).
/// </summary>
public sealed class DataInputPhase([NotNull] IGameSession session, [NotNull] IConsoleIO console) : IDataInputPhase
{
    private readonly IGameSession _session = session;
    private readonly IConsoleIO _console = console;
    private static class Prompts
    {
        public const string Initial = "Enter password candidates:";
        public const string More = "Enter more password candidates (empty line to finish):";
    }

    /// <inheritdoc />
    public void Run()
    {
        _console.WriteLine(Prompts.Initial);
        var line = _console.ReadLine();

        while (line != null && !string.IsNullOrWhiteSpace(line))
        {
            ProcessInputLine(line);

            WriteCandidateCountAndList();

            _console.WriteLine(Prompts.More);
            line = _console.ReadLine();
        }
    }

    private void ProcessInputLine(string line)
    {
        var tokens = GetTokens(line);

        foreach (var token in tokens)
        {
            if (token.StartsWith('-'))
            {
                var result = _session.Remove(token[1..].Trim());
                if (result.IsFailed)
                    _console.WriteLine(result.Errors[0].Message);
            }
            else
            {
                var result = _session.Add(token);
                if (result.IsFailed)
                    _console.WriteLine(result.Errors[0].Message);
            }
        }
    }

    private static IEnumerable<string> GetTokens(string line)
    {
        return line.Split((string[]?)null, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrEmpty(t));
    }

    private void WriteCandidateCountAndList()
    {
        var n = _session.Count;
        var len = _session.WordLength ?? 0;
        _console.WriteLine();
        _console.WriteLine($"{n} candidate(s):");
        if (n > 0 && len > 0)
            _console.WriteLine(CandidateListFormatter.Format(_session, len));
        _console.WriteLine();
    }
}
