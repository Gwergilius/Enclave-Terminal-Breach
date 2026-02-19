using Xunit.Sdk;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Xunit.Categories;

[TraitDiscoverer(UiTestDiscoverer.DiscovererTypeName, AssemblyHelper.AssemblyName)]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class UiTestAttribute : Attribute, ITraitAttribute
{
    public UiTestAttribute()
    {

    }
    public UiTestAttribute(string identifier)
    {
        Identifier = identifier;
    }

    public UiTestAttribute(long identifier)
    {
        Identifier = identifier.ToString();
    }

    public string? Identifier { get; }
}
#pragma warning restore IDE0130 // Namespace does not match folder structure







