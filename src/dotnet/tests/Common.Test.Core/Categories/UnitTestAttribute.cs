using Xunit.v3;

#pragma warning disable IDE0130
namespace Xunit.Categories;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class UnitTestAttribute : Attribute, ITraitAttribute
{
    public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits() =>
        [new KeyValuePair<string, string>("Category", "UnitTest")];
}
#pragma warning restore IDE0130
