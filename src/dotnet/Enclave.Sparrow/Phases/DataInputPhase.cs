using System.Diagnostics.CodeAnalysis;
using Enclave.Sparrow.Configuration;
using Enclave.Shared.IO;
using Enclave.Shared.Models;

namespace Enclave.Sparrow.Phases;

/// <summary>
/// Data-input phase: loads candidates from file when WordListPath is set, otherwise prompts until empty line (SPARROW-Requirements §2).
/// </summary>
public sealed class DataInputPhase(
    [NotNull] IGameSession session,
    [NotNull] IConsoleIO console,
    [NotNull] SparrowOptions options) : IDataInputPhase
{
    private readonly IGameSession _session = session;
    private readonly IConsoleIO _console = console;
    private readonly SparrowOptions _options = options;

    private static class Prompts
    {
        public const string Initial = "Enter password candidates:";
        public const string More = "Enter more password candidates (empty line to finish):";
    }

    /// <inheritdoc />
    public void Run()
    {
        if (!string.IsNullOrWhiteSpace(_options.WordListPath))
        {
            LoadCandidatesFromFile(_options.WordListPath!);
            WriteCandidateCountAndList();
            return;
        }

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

    private void LoadCandidatesFromFile(string path)
    {
        if (!File.Exists(path))
        {
            _console.WriteLine($"Word list file not found: {path}");
            return;
        }

        foreach (var line in File.ReadLines(path))
        {
            foreach (var token in GetTokens(line))
            {
                var result = _session.Add(token);
                if (result.IsFailed)
                    _console.WriteLine(result.Errors[0].Message);
            }
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
