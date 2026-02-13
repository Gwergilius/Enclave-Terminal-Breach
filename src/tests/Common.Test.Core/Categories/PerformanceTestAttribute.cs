using Xunit.Sdk;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Xunit.Categories;

[TraitDiscoverer(PerformanceTestDiscoverer.DiscovererTypeName, AssemblyHelper.AssemblyName)]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class PerformanceTestAttribute : Attribute, ITraitAttribute
{
    public PerformanceTestAttribute()
    {
    }

    public PerformanceTestAttribute(string identifier)
    {
        Identifier = identifier;
    }

    public PerformanceTestAttribute(long identifier)
    {
        Identifier = identifier.ToString();
    }

    public string? Identifier { get; }
}
#pragma warning restore IDE0130 // Namespace does not match folder structure
