using System.Diagnostics.CodeAnalysis;
using Enclave.Phosphor;
using Enclave.Raven.Configuration;
using Enclave.Shared.Models;

namespace Enclave.Raven.Phases;

/// <summary>
/// Data-input phase: loads candidates from file when WordListPath is set, otherwise prompts until empty line (RAVEN-Requirements §2).
/// </summary>
public sealed class DataInputPhase(
    [NotNull] IGameSession session,
    [NotNull] IPhosphorWriter writer,
    [NotNull] IPhosphorReader reader,
    [NotNull] RavenOptions options) : IDataInputPhase
{
    private readonly IGameSession _session = session;
    private readonly IPhosphorWriter _writer = writer;
    private readonly IPhosphorReader _reader = reader;
    private readonly RavenOptions _options = options;

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

        _writer.WriteLine(Prompts.Initial);
        var line = _reader.ReadLine();

        while (line != null && !string.IsNullOrWhiteSpace(line))
        {
            ProcessInputLine(line);

            WriteCandidateCountAndList();

            _writer.WriteLine(Prompts.More);
            line = _reader.ReadLine();
        }
    }

    private void LoadCandidatesFromFile(string path)
    {
        if (!File.Exists(path))
        {
            _writer.WriteLine($"Word list file not found: {path}");
            return;
        }

        foreach (var line in File.ReadLines(path))
        {
            foreach (var token in GetTokens(line))
            {
                var result = _session.Add(token);
                if (result.IsFailed)
                    _writer.WriteLine(result.Errors[0].Message);
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
                    _writer.WriteLine(result.Errors[0].Message);
            }
            else
            {
                var result = _session.Add(token);
                if (result.IsFailed)
                    _writer.WriteLine(result.Errors[0].Message);
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
        _writer.WriteLine();
        _writer.WriteLine($"{n} candidate(s):");
        if (n > 0 && len > 0)
            _writer.WriteLine(CandidateListFormatter.Format(_session, len));
        _writer.WriteLine();
    }
}
