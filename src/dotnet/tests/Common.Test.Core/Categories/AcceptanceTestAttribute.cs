using Xunit.v3;

#pragma warning disable IDE0130
namespace Xunit.Categories;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class AcceptanceTestAttribute : Attribute, ITraitAttribute
{
    public AcceptanceTestAttribute() { }

    public AcceptanceTestAttribute(string identifier)
    {
        Identifier = identifier;
    }

    public AcceptanceTestAttribute(long identifier)
    {
        Identifier = identifier.ToString();
    }

    public string? Identifier { get; }

    public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits()
    {
        var traits = new List<KeyValuePair<string, string>> { new("Category", "AcceptanceTest") };
        if (!string.IsNullOrWhiteSpace(Identifier))
            traits.Add(new("AcceptanceTest", Identifier));
        return traits;
    }
}
#pragma warning restore IDE0130
