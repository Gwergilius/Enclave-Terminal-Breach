using Xunit.Sdk;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Xunit.Categories;

[TraitDiscoverer(AcceptanceTestDiscoverer.DiscovererTypeName, AssemblyHelper.AssemblyName)]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class AcceptanceTestAttribute : Attribute, ITraitAttribute
{
    public AcceptanceTestAttribute()
    {

    }
    public AcceptanceTestAttribute(string identifier)
    {
        Identifier = identifier;
    }

    public AcceptanceTestAttribute(long identifier)
    {
        Identifier = identifier.ToString();
    }

    public string? Identifier { get; }
}
#pragma warning restore IDE0130 // Namespace does not match folder structure







