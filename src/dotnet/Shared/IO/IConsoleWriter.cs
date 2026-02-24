using System.Text;
using Enclave.Common.Models;

namespace Enclave.Shared.IO;

public interface IConsoleWriter
{
    void Write(string? value);
    void WriteLine(string? value = null);
}
