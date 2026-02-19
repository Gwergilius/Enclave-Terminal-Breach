using Xunit;

namespace Enclave.Common.Test.Core;

public static class TestDataExtensions
{
    public static IEnumerable<object?[]> ToTestDataFromObject(this IEnumerable<object> testCases)
        => testCases.ToTestDataFromObject<object>();

    /// <summary>
    /// Creates testdata for a method receiving parameters coming of the properties of a test class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="testCases"></param>
    /// <returns></returns>
    public static IEnumerable<object?[]> ToTestDataFromObject<T>(this IEnumerable<T> testCases)
    {
        var properties = typeof(T).GetProperties();
        foreach (var testCase in testCases)
        {
            var propValues = new List<object?>();
            foreach (var property in properties)
            {
                var value = property.GetValue(testCase);
                propValues.Add(value);
            }
            yield return propValues.ToArray();
        }
    }

    public static IEnumerable<object?[]> ToTestData(this IEnumerable<object> testCases)
        => testCases.ToTestData<object>();

    /// <summary>
    /// Creates testdata for a method receiving a single parmeter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="testCases"></param>
    /// <returns></returns>
    /// <seealso cref="ToTestDataFromObject{T}(IEnumerable{T})"/>
    public static IEnumerable<object?[]> ToTestData<T>(this IEnumerable<T> testCases)
        => testCases.Select(t => Array(t));

    public static IEnumerable<object?[]> ToTestData<T1, T2>(this IEnumerable<(T1, T2)> testCases)
        => testCases.Select(t => Array(t.Item1, t.Item2));

    public static IEnumerable<object?[]> ToTestData<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> testCases)
        => testCases.Select(t => Array(t.Item1, t.Item2, t.Item3));

    public static IEnumerable<object?[]> ToTestData<T1, T2, T3, T4>(this IEnumerable<(T1, T2, T3, T4)> testCases)
        => testCases.Select(t => Array(t.Item1, t.Item2, t.Item3, t.Item4));

    public static TheoryData<T> ToTheoryData<T>(this IEnumerable<T> testCases)
    {
        var data = new TheoryData<T>();
        foreach (var testCase in testCases)
        {
            data.Add(testCase);
        }
        return data;
    }

    public static TheoryData<T1, T2> ToTheoryData<T1, T2>(this IEnumerable<(T1, T2)> testCases)
    {
        var data = new TheoryData<T1, T2>();
        foreach (var testCase in testCases)
        {
            data.Add(testCase.Item1, testCase.Item2);
        }
        return data;
    }

    public static TheoryData<T1, T2, T3> ToTheoryData<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> testCases)
    {
        var data = new TheoryData<T1, T2, T3>();
        foreach (var testCase in testCases)
        {
            data.Add(testCase.Item1, testCase.Item2, testCase.Item3);
        }
        return data;
    }
    public static TheoryData<T1, T2, T3, T4> ToTheoryData<T1, T2, T3, T4>(this IEnumerable<(T1, T2, T3, T4)> testCases)
    {
        var data = new TheoryData<T1, T2, T3, T4>();
        foreach (var testCase in testCases)
        {
            data.Add(testCase.Item1, testCase.Item2, testCase.Item3, testCase.Item4);
        }
        return data;
    }

    static object?[] Array(params object?[] args) => args;
}

