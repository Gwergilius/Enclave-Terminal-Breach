
using Enclave.Common.Errors;
using FluentResults;

namespace Enclave.Common.Extensions;

public static class ResourceExtensions
{
    public static Result<IEnumerable<T>> GetJsonResource<T>(this Type referenceType, string resourcePath)
        => referenceType.Assembly.GetJsonResource<T>(resourcePath);

    /// <summary>
    /// Returns a list of objects from a json resource file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assembly"></param>
    /// <param name="resourcePath"></param>
    /// <returns></returns>
    public static Result<IEnumerable<T>> GetJsonResource<T>(this Assembly assembly, string resourcePath)
    {
        var jsonData = assembly.GetResourceString(resourcePath)!;
        if (!jsonData.IsSuccess)
        {
            return Result.Fail(new NotFoundError(jsonData.Errors));
        }
        IEnumerable<T> data = JsonSerializer.Deserialize<List<T>>(jsonData.Value)!;
        return Result.Ok(data);
    }

    public static Result<string> GetResourceString(this Type referenceType, string resourcePath)
        => referenceType.Assembly.GetResourceString(resourcePath);

    public static Result<string> GetResourceString(this Assembly assembly, string resourcePath)
    {
        var result = assembly.GetResourceStream(resourcePath);
        if (!result.IsSuccess)
        {
            return Result.Fail(new NotFoundError(result.Errors));
        }

        using var stream = result.Value;
        using var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();
        return Result.Ok(content);
    }

    public static Result<Stream> GetResourceStream(this Type referenceType, string resourcePath)
        => referenceType.Assembly.GetResourceStream(resourcePath);

    private static Dictionary<char, char> _replacements= new ()
    {
        { '\\', '.' },
        { '/', '.' },
        { '-', '_' },
    };
    public static Result<Stream> GetResourceStream(this Assembly assembly, string resourcePath)
    {
        if (string.IsNullOrWhiteSpace(resourcePath))
        {
            throw new ArgumentNullException(nameof(resourcePath), "Resource name cannot be null or empty");
        }

        string resourceName = resourcePath;
        foreach(var rule in _replacements)
        {
            resourceName = resourceName.Replace(rule.Key, rule.Value);
        }

        var actualName = assembly.GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith(resourceName, StringComparison.InvariantCultureIgnoreCase));

        if(actualName == null)
        {
             return ResourceNotFound(resourcePath, assembly);
        }

        var stream = assembly.GetManifestResourceStream(actualName);
        return Result.Ok(stream!);
    }

    private static Result ResourceNotFound(string resourcePath, Assembly assembly)
    {
        var messages = new List<string>
        {
            resourcePath,
            "Resource not found",
            "Available resources are:"
        };
        messages.AddRange(assembly.GetManifestResourceNames());
        return Result.Fail(new NotFoundError(messages));
    }
}
