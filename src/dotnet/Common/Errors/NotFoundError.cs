using FluentResults;

namespace Enclave.Common.Errors;

public class NotFoundError(string message) : Error(message)
{
    public NotFoundError() : this("Resource not found")
    {
    }

    public NotFoundError(IEnumerable<IError> reasons) : this()
    {
        foreach (var reason in reasons)
        {
            Reasons.Add(reason);
        }
    }

    public NotFoundError(IEnumerable<string> messages) : this()
    {
        foreach (var reason in messages)
        {
            Reasons.Add(new Error(reason));
        }
    }
}
