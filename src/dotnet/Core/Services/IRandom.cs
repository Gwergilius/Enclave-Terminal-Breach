namespace Enclave.Echelon.Core.Services;

public interface IRandom
{
    double Next();
    int Next(int maxValue);
    int Next(int minValue, int maxValue);
}
