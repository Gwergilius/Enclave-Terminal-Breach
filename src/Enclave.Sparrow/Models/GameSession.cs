using System.Collections;
using Enclave.Common.Errors;
using Enclave.Echelon.Core.Errors;
using Enclave.Echelon.Core.Models;
using Enclave.Sparrow.Errors;
using FluentResults;

namespace Enclave.Sparrow.Models;

/// <summary>
/// Default implementation of <see cref="IGameSession"/>. Holds the mutable candidate list and word length for one console run.
/// </summary>
public sealed class GameSession : IGameSession
{
    private static readonly StringComparer _caseInsensitive = StringComparer.OrdinalIgnoreCase;

    /// <summary>
    /// Current list of password candidates. Data-input phase adds/removes; hacking phase replaces with narrowed list after each guess.
    /// </summary>
    private IList<Password> Candidates { get; } = [];

    public Password this[int index]
    {
        get => Candidates[index]; 
        set => SetCandidate(index, value);
    }


    /// <inheritdoc />
    public int? WordLength { get; set; }

    public int Count => Candidates.Count;

    public bool IsReadOnly => Candidates.IsReadOnly;

    public void Add(Password password)
    {
        Result result = AddCandidate(password);
        if (result.IsFailed)
        {
            throw new InvalidOperationException(result.Errors[0].Message);
        }
    }

    /// <inheritdoc />
    public Result Add(string word)
    {
        Password password;
        try
        {
            password = new Password(word);
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(new InvalidPassword(word, ex.Message));
        }

        return AddCandidate(password);
    }


    public void Clear()
    {
        Candidates.Clear();
        WordLength = null;
    }

    public bool Contains(Password item) => Candidates.Contains(item);

    public void CopyTo(Password[] array, int arrayIndex) => Candidates.CopyTo(array, arrayIndex);

    public IEnumerator<Password> GetEnumerator() => Candidates.GetEnumerator();

    public int IndexOf(Password item) => Candidates.IndexOf(item);

    public void Insert(int index, Password item)
    {
        throw new NotImplementedException();
    }

    public void RemoveAt(int index) => RemoveCandidate(Candidates[index]);

    public bool Remove(Password item) => RemoveCandidate(item);

    /// <inheritdoc />
    public Result Remove(string word)
    {
        if (string.IsNullOrEmpty(word))
            return Result.Fail(new InvalidPassword("No word specified", word));
        if (!RemoveCandidate(new Password(word)))
            return Result.Fail(new NotFoundError($"Not in list (ignored): {word}"));
        return Result.Ok();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private FluentResults.Result AddCandidate(Password password)
    {
        var len = password.Word.Length;

        if (WordLength.HasValue && WordLength.Value != len)
        {
            var message = string.Format(CultureInfo.InvariantCulture, "Word length must be {0}. Skipping: {1}", WordLength.Value, password.Word);
            return Result.Fail(new InvalidPassword(password.Word, message));
        }

        if (Candidates.Any(c => _caseInsensitive.Equals(c.Word, password.Word)))
        {
            return Result.Fail(new DuplicatedPassword(password.Word));
        }

        WordLength ??= len;
        Candidates.Add(password);
        return Result.Ok();
    }

    private bool RemoveCandidate(Password word)
    {
        int index = Candidates.IndexOf(word);
        if (index < 0)
        {
            return false;
        }

        Candidates.RemoveAt(index);

        if (Candidates.Count == 0)
        {
            WordLength = null;
        }
        return true;
    }

    private void SetCandidate(int index, Password value)
    {
        if (index < 0 || index >= Candidates.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"Index must be between 0 and {Candidates.Count - 1}");

        if (value?.Word == null)
            throw new ArgumentNullException(nameof(value), "Password cannot be null");

        Candidates[index] = value;
    }
}
