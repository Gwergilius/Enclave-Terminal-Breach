using Moq;

namespace Enclave.Common.Test.Core;

public static class MockExtensions
{
    public static Mock<T> AsMock<T>(this T mocked) where T : class => Mock.Get(mocked);
}







