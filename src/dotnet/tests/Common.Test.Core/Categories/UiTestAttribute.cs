using Xunit.v3;

#pragma warning disable IDE0130
namespace Xunit.Categories;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class UiTestAttribute : Attribute, ITraitAttribute
{
    public UiTestAttribute() { }

    public UiTestAttribute(string identifier)
    {
        Identifier = identifier;
    }

    public UiTestAttribute(long identifier)
    {
        Identifier = identifier.ToString();
    }

    public string? Identifier { get; }

    public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits()
    {
        var traits = new List<KeyValuePair<string, string>> { new("Category", "UiTest") };
        if (!string.IsNullOrWhiteSpace(Identifier))
            traits.Add(new("UiTest", Identifier));
        return traits;
    }
}
#pragma warning restore IDE0130
