using Xunit.v3;

#pragma warning disable IDE0130
namespace Xunit.Categories;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class TestOfAttribute : Attribute, ITraitAttribute
{
    public TestOfAttribute(string subject)
    {
        Subject = subject;
    }

    public string Subject { get; }

    public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits() =>
        [new KeyValuePair<string, string>("Subject", Subject)];
}
#pragma warning restore IDE0130
