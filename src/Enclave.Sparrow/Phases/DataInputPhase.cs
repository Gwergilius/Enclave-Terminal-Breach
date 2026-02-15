using Enclave.Echelon.Core.Models;
using Enclave.Sparrow.IO;
using Enclave.Sparrow.Session;

namespace Enclave.Sparrow.Phases;

/// <summary>
/// Data-input phase: prompts for password candidates until empty line; fills session (SPARROW-Requirements §2).
/// </summary>
public sealed class DataInputPhase(IGameSession session, IConsoleIO console) : IDataInputPhase
{
    private readonly IGameSession _session = session ?? throw new ArgumentNullException(nameof(session));
    private readonly IConsoleIO _console = console ?? throw new ArgumentNullException(nameof(console));
    private static readonly StringComparer _caseInsensitive = StringComparer.OrdinalIgnoreCase;

    /// <inheritdoc />
    public void Run()
    {
        _console.WriteLine("Enter password candidates:");
        var line = _console.ReadLine();

        while (line != null && !string.IsNullOrWhiteSpace(line))
        {
            ProcessInputLine(line);

            WriteCandidateCountAndList();

            _console.WriteLine("Enter more password candidates (empty line to finish):");
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
                RemoveWord(token);
            }
            else
            {
                AddWord(token);
            }
        }
    }

    private static IEnumerable<string> GetTokens(string line)
    {
        return line.Split((string[]?)null, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrEmpty(t));
    }

    private bool RemoveCandidate(string word)
    {
        for (var i = 0; i < _session.Candidates.Count; i++)
        {
            if (_caseInsensitive.Equals(_session.Candidates[i].Word, word))
            {
                _session.Candidates.RemoveAt(i);
                if (_session.Candidates.Count == 0)
                {
                    _session.WordLength = null;
                }
                return true;
            }
        }
        return false;
    }

    private void AddWord(string word)
    {
        Password password;
        try
        {
            password = new Password(word);
        }
        catch (ArgumentException ex)
        {
            _console.WriteLine($"Invalid word (skipped): {ex.Message}");
            return;
        }

        var len = password.Word.Length;

        if (_session.WordLength.HasValue && _session.WordLength.Value != len)
        {
            _console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Word length must be {0}. Skipping: {1}", _session.WordLength.Value, password.Word));
            return;
        }

        if (_session.Candidates.Any(c => _caseInsensitive.Equals(c.Word, password.Word)))
        {
            _console.WriteLine($"Already in list (ignored): {password.Word}");
            return;
        }

        _session.WordLength ??= len;
        _session.Candidates.Add(password);
    }

    private bool RemoveWord(string token)
    {
        var toRemove = token[1..].Trim();
        if (string.IsNullOrEmpty(toRemove)) return false;
        var removed = RemoveCandidate(toRemove);
        if (!removed)
            _console.WriteLine($"Not in list (ignored): {toRemove}");
        return true;
    }

    private void WriteCandidateCountAndList()
    {
        var n = _session.Candidates.Count;
        var len = _session.WordLength ?? 0;
        _console.WriteLine();
        _console.WriteLine($"{n} candidate(s):");
        if (n > 0 && len > 0)
            _console.WriteLine(CandidateListFormatter.Format(_session.Candidates.ToList(), len));
        _console.WriteLine();
    }
}
