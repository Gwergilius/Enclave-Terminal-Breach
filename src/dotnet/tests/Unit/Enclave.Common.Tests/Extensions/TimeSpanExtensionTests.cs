using Enclave.Common.Extensions;
using Enclave.Common.Test.Core;

namespace Enclave.Common.Tests.Extensions;

[UnitTest, TestOf(nameof(TimeSpanExtensions))]
public class TimeSpanExtensionInlineDatas
{
    public static readonly IEnumerable<object?[]> ParseTimeFromUnitInlineDatas = new (string, string?, TimeSpan)[]
    {
        ("500 ms",  "500 ms",       TimeSpan.FromMilliseconds(500)),
        ("1.5 sec", "1.5 sec",      TimeSpan.FromSeconds(1.5)),
        ("null",    null,           TimeSpan.Zero),
        ("empty",   string.Empty,   TimeSpan.Zero)
    }
    .ToTestData();

    [Theory, MemberData(nameof(ParseTimeFromUnitInlineDatas))]
    public void ParseTimeFromUnitInlineData(string testName, string timeWithUnit, TimeSpan expected)
    {
        // Arrange
        // Act
        // Assert
        var actual = timeWithUnit.ParseTimeUnit();
        actual.ShouldBe(expected, customMessage: testName);
    }

    public static readonly IEnumerable<object?[]> MillisecsInlineDatas = new (string, object, TimeSpan)[]
    {
        ("integer",         500,    TimeSpan.FromMilliseconds(500)),
        ("double",          250.3,  TimeSpan.FromMilliseconds(250.3)),
        ("int string",      "300",  TimeSpan.FromMilliseconds(300)),
        ("double string",   "250.3",TimeSpan.FromMilliseconds(250.3)),
    }
    .ToTestData();

    public static readonly IEnumerable<object?[]> SecondsInlineDatas = new (string, object, TimeSpan)[]
    {
        ("integer",         500,        TimeSpan.FromSeconds(500)),
        ("double",          250.3,      TimeSpan.FromSeconds(250.3)),
        ("int string",      "300",      TimeSpan.FromSeconds(300)),
        ("double string",   "250.3",    TimeSpan.FromSeconds(250.3)),
    }
    .ToTestData();

    public static readonly IEnumerable<object?[]> MinutesInlineDatas = new (string, object, TimeSpan)[]
    {
        ("integer", 500, TimeSpan.FromMinutes(500)),
        ("double", 250.3, TimeSpan.FromMinutes(250.3)),
        ("int string", "300", TimeSpan.FromMinutes(300)),
        ("double string", "250.3", TimeSpan.FromMinutes(250.3)),
    }
    .ToTestData();

    public static readonly IEnumerable<object?[]> HoursInlineDatas = new (string, object, TimeSpan)[]
    {
        ("integer", 500, TimeSpan.FromHours(500)),
        ("double", 250.3, TimeSpan.FromHours(250.3)),
        ("int string", "300", TimeSpan.FromHours(300)),
        ("double string", "250.3", TimeSpan.FromHours(250.3)),
    }
    .ToTestData();

    public static readonly IEnumerable<object?[]> ElapsedDateInlineDatas = new (string, DateTime, string, int)[]
    {
        ("'2022/01/25', '2020/01/25'", DateTime.Parse("2022/01/25"), "2020/01/25", 731),
        ("'2022/03/25', '2020/03/25'", DateTime.Parse("2022/03/25"), "2020/03/25", 730),
    }
    .ToTestData();

    [Theory, MemberData(nameof(MillisecsInlineDatas))]
    public void MillisecsTests(string testCaseName, object millisecs, TimeSpan expected)
    {
        string typeName = millisecs.GetType().Name;
        TimeSpan value = TimeSpan.Zero;
        switch (typeName)
        {
            case nameof(Double):
                value = ((double)millisecs).Millisecs();
                break;
            case nameof(Int32):
                value = ((int)millisecs).Millisecs();
                break;
            case nameof(String):
                value = ((string)millisecs).Millisecs();
                break;
            default:
                throw new NotImplementedException($"{typeName}.Millisecs() is not implemented");
        }
        value.ShouldBe(expected, customMessage: testCaseName);
    }

    [Theory, MemberData(nameof(SecondsInlineDatas))]
    public void SecondsTests( string testName, object minutes, TimeSpan expected)
    {
        string typeName = minutes.GetType().Name;
        TimeSpan value = TimeSpan.Zero;
        switch (typeName)
        {
            case nameof(Double):
                value = ((double)minutes).Seconds();
                break;
            case nameof(Int32):
                value = ((int)minutes).Seconds();
                break;
            case nameof(String):
                value = ((string)minutes).Seconds();
                break;
            default:
                throw new NotImplementedException($"{typeName}.Seconds() is not implemented");
        }

        value.ShouldBe(expected, customMessage: testName);
    }

    [Theory, MemberData(nameof(MinutesInlineDatas))]
    public void MinutesTests( string testName, object seconds, TimeSpan expected)
    {
        string typeName = seconds.GetType().Name;
        TimeSpan value = TimeSpan.Zero;
        switch (typeName)
        {
            case nameof(Double):
                value = ((double)seconds).Minutes();
                break;
            case nameof(Int32):
                value = ((int)seconds).Minutes();
                break;
            case nameof(String):
                value = ((string)seconds).Minutes();
                break;
            default:
                throw new NotImplementedException($"{typeName}.Minutes() is not implemented");
        }

        value.ShouldBe(expected, customMessage: testName);
    }

    [Theory, MemberData(nameof(HoursInlineDatas))]
    public void HoursTests( string testName, object hours, TimeSpan expected)
    {
        string typeName = hours.GetType().Name;
        TimeSpan value = TimeSpan.Zero;
        switch (typeName)
        {
            case nameof(Double):
                value = ((double)hours).Hours();
                break;
            case nameof(Int32):
                value = ((int)hours).Hours();
                break;
            case nameof(String):
                value = ((string)hours).Hours();
                break;
            default:
                throw new NotImplementedException($"{typeName}.Hours() is not implemented");
        }

        value.ShouldBe(expected, customMessage: testName);
    }

    [Theory, MemberData(nameof(ElapsedDateInlineDatas))]
    public void ElapsedDaysTest( string testName, DateTime now, string dateFrom, int expected)
    {
        var actual = now.ElapsedDays(dateFrom);
        actual.ShouldBe(expected, customMessage: testName);
    }

    [Theory]
    [InlineData("500 mS",       "Unknown unit")]
    [InlineData("1.5 Sec",      "Unknown unit")]
    [InlineData("3 minutes",    "Unknown unit")]
    [InlineData("500-ms",       "Invalid format")]
    [InlineData("1,5 sec",      "Bad numeric format")]
    public void ParseTimeUnit_BadFormatTests(string timeUnit, string reason)
    {
        Action act = () => timeUnit.ParseTimeUnit();

        var exception = act.ShouldThrow<FormatException>();
        exception.Message.ShouldEndWith("was not in a correct format.", customMessage: reason);
    }
}
