using Xunit.Abstractions;
using Xunit.Sdk;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Xunit.Categories;

public class AcceptanceTestDiscoverer : ITraitDiscoverer
{
    internal const string DiscovererTypeName = AssemblyHelper.Namespace + "." + nameof(AcceptanceTestDiscoverer);
    private static readonly string TraitName = nameof(AcceptanceTestAttribute).Replace(nameof(Attribute), string.Empty);

    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        var identifier = traitAttribute.GetNamedArgument<string>(nameof(AcceptanceTestAttribute.Identifier));

        yield return new KeyValuePair<string, string>(AssemblyHelper.Category, TraitName);

        if (!string.IsNullOrWhiteSpace(identifier))
            yield return new KeyValuePair<string, string>(TraitName, identifier);
    }
}
#pragma warning restore IDE0130 // Namespace does not match folder structure







