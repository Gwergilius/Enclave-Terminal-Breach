using Xunit.v3;

#pragma warning disable IDE0130
namespace Xunit.Categories;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class PerformanceTestAttribute : Attribute, ITraitAttribute
{
    public PerformanceTestAttribute() { }

    public PerformanceTestAttribute(string identifier)
    {
        Identifier = identifier;
    }

    public PerformanceTestAttribute(long identifier)
    {
        Identifier = identifier.ToString();
    }

    public string? Identifier { get; }

    public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits()
    {
        var traits = new List<KeyValuePair<string, string>> { new("Category", "PerformanceTest") };
        if (!string.IsNullOrWhiteSpace(Identifier))
            traits.Add(new("PerformanceTest", Identifier));
        return traits;
    }
}
#pragma warning restore IDE0130
