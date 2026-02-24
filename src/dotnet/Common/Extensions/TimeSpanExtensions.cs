using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using Enclave.Common.Helpers;

namespace Enclave.Common.Extensions;

public static class TimeSpanExtensions
{
    #region Regex groups
    private const string ValueGroup = nameof(ValueGroup);
    private const string UnitGroup = nameof(UnitGroup);
    #endregion

    #region Millisecs
    /// <summary>
    /// Creates TimeSpan from a value representing milliseconds
    /// </summary>
    /// <param name="n">Quantity of milliseconds converted to TimeSpan</param>
    /// <returns></returns>
    public static TimeSpan Millisecs(this int n) => TimeSpan.FromMilliseconds(n);
    /// <summary>
    /// Creates TimeSpan from a value representing milliseconds
    /// </summary>
    /// <param name="n">Quantity of milliseconds converted to TimeSpan</param>
    /// <returns></returns>
    public static TimeSpan Millisecs(this double n) => TimeSpan.FromMilliseconds(n);
    /// <summary>
    /// Creates TimeSpan from a value representing milliseconds
    /// </summary>
    /// <param name="s">String representing the quantity of milliseconds converted to TimeSpan (either int or double)</param>
    /// <returns></returns>
    public static TimeSpan Millisecs(this string s) => TimeSpan.FromMilliseconds(double.Parse(s, CultureInfo.InvariantCulture));
    #endregion Millisecs

    #region Seconds
    /// <summary>
    /// Creates TimeSpan from a value representing seconds
    /// </summary>
    /// <param name="n">Quantity of seconds converted to TimeSpan</param>
    /// <returns></returns>
    public static TimeSpan Seconds(this int n) => TimeSpan.FromSeconds(n);
    /// <summary>
    /// Creates TimeSpan from a value representing seconds
    /// </summary>
    /// <param name="n">Quantity of seconds converted to TimeSpan</param>
    /// <returns></returns>
    public static TimeSpan Seconds(this double n) => TimeSpan.FromSeconds(n);
    /// <summary>
    /// Creates TimeSpan from a value representing seconds
    /// </summary>
    /// <param name="s">>String representing the quantity of seconds converted to TimeSpan (either int or double)</param>
    /// <returns></returns>
    public static TimeSpan Seconds(this string s) => TimeSpan.FromSeconds(double.Parse(s, CultureInfo.InvariantCulture));
    #endregion Seconds

    #region Minutes
    /// <summary>
    /// Creates TimeSpan from a value representing minutes
    /// </summary>
    /// <param name="n">Quantity of minutes converted to TimeSpan</param>
    /// <returns></returns>
    public static TimeSpan Minutes(this int n) => TimeSpan.FromMinutes(n);
    /// <summary>
    /// Creates TimeSpan from a value representing minutes
    /// </summary>
    /// <param name="n">Quantity of minutes converted to TimeSpan</param>
    /// <returns></returns>
    public static TimeSpan Minutes(this double n) => TimeSpan.FromMinutes(n);
    /// <summary>
    /// Creates TimeSpan from a value representing minutes
    /// </summary>
    /// <param name="s">String representing the quantity of minutes converted to TimeSpan (either int or double)</param>
    /// <returns></returns>
    public static TimeSpan Minutes(this string s) => TimeSpan.FromMinutes(double.Parse(s, CultureInfo.InvariantCulture));
    #endregion Minutes

    #region Hours
    /// <summary>
    /// Creates TimeSpan from a value representing hours
    /// </summary>
    /// <param name="n">Quantity of hours converted to TimeSpan</param>
    /// <returns></returns>
    public static TimeSpan Hours(this int n) => TimeSpan.FromHours(n);
    /// <summary>
    /// Creates TimeSpan from a value representing hours
    /// </summary>
    /// <param name="n">Quantity of hours converted to TimeSpan</param>
    /// <returns></returns>
    public static TimeSpan Hours(this double n) => TimeSpan.FromHours(n);
    /// <summary>
    /// Creates TimeSpan from a value representing hours
    /// </summary>
    /// <param name="s">String representing the quantity of hours converted to TimeSpan (either int or double)</param>
    /// <returns></returns>
    public static TimeSpan Hours(this string s) => TimeSpan.FromHours(double.Parse(s, CultureInfo.InvariantCulture));
    #endregion Hours

    #region ElapsedDays
    /// <summary>
    /// Calculate the elapsed days between two dates
    /// </summary>
    /// <param name="now">actual date</param>
    /// <param name="dateFrom">base date</param>
    /// <returns>Elapsed days from <paramref name="dateFrom"/> and <paramref name="now"/></returns>
    /// <remarks>
    /// Note, that <paramref name="dateFrom"/> is represented as string. It could be useful when the method is called from a configuration section.
    /// </remarks>
    public static int ElapsedDays(this DateTime now, string dateFrom) => now.ElapsedDays(DateTime.Parse(dateFrom, CultureInfo.InvariantCulture));

    /// <summary>
    /// Calculate the elapsed days between two dates
    /// </summary>
    /// <param name="now">actual date</param>
    /// <param name="dateFrom">base date</param>
    /// <returns>Elapsed days from <paramref name="dateFrom"/> and <paramref name="now"/></returns>
    public static int ElapsedDays(this DateTime now, DateTime dateFrom) => (int)(now - dateFrom).TotalDays;
    #endregion ElapsedDays

    #region ParseTimeUnit
    private static readonly Regex _timeUnitPattern = new(
        $@"^(?<{ValueGroup}>[0-9]*\.?[0-9]*)\s*(?<{UnitGroup}>ms|sec|s|min|m|h)$",
        RegexOptions.Compiled,
        matchTimeout: 1.Seconds()); //Defense against regex DoS with untrusted input; pattern is simple and should never take more than a second to match

    private static readonly Dictionary<string, Func<double, TimeSpan>> _timeConverters = new()
    {
        ["ms"] = TimeSpan.FromMilliseconds,
        ["sec"] = TimeSpan.FromSeconds,
        ["s"] = TimeSpan.FromSeconds,
        ["min"] = TimeSpan.FromMinutes,
        ["m"] = TimeSpan.FromMinutes,
        ["h"] = TimeSpan.FromHours,
    };

    /// <summary>
    /// Parse a string into a TimeSpan, where the string contains a number and a unit
    /// </summary>
    /// <param name="timeWithUnit"></param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    /// <example>"1 sec" should returns TimeSpan.FromSeconds(1)</example>
    /// <example>"500 ms" should returns TimeSpan.FromMilliseconds(500)</example>
    /// <remarks>Known units are: `ms`, `sec` (or `s`), `min` (or `m`), and `h`</remarks>
    public static TimeSpan ParseTimeUnit(this string timeWithUnit, TimeSpan? defaultValue = null)
    {
        defaultValue ??= TimeSpan.Zero;
        timeWithUnit = (timeWithUnit ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(timeWithUnit))
            return defaultValue!.Value;

        var match = _timeUnitPattern.Match(timeWithUnit);
        if(!match.Success)
        {
            throw new FormatException($"TimeUnit string '{timeWithUnit}' was not in a correct format.");
        }

        var value = match.Groups[ValueGroup].Value;
        var unit = match.Groups[UnitGroup].Value.ToLower();
        return _timeConverters[unit](double.Parse(value, CultureInfo.InvariantCulture));
    }
    #endregion ParseTimeUnit

    private static readonly Waiter _defaultWaiter = new();

    /// <summary>
    /// Waits for the given delay, honouring cancellation. Implemented via <see cref="Waiter"/>; use <see cref="Waiter"/> in tests with a mock delay.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Verified by code review; Waiter is tested with injectable delay.")]
    public static Task SleepAsync(this TimeSpan delay, CancellationToken cancellationToken)
        => _defaultWaiter.SleepAsync(delay, cancellationToken);

    /// <summary>
    /// Waits for the given number of milliseconds, honouring cancellation. Delegates to <see cref="SleepAsync(TimeSpan, CancellationToken)"/>.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Verified by code review; Waiter is tested with injectable delay.")]
    public static Task Delay(this int delayMs, CancellationToken cancellationToken)
        => delayMs.Millisecs().SleepAsync(cancellationToken);

}
