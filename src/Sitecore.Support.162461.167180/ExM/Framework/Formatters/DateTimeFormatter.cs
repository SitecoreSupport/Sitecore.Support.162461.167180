namespace Sitecore.Support.ExM.Framework.Formatters
{
  using System;
  using Sitecore.Configuration;
  using Sitecore.Diagnostics;

  public class DateTimeFormatter : IDateTimeFormatter
  {
    private TimeZoneInfo _primaryTimeZoneInfo;
    private TimeZoneInfo _secondaryTimeZoneInfo;
    private bool? _displayDatesInUtc;

    [UsedImplicitly]
    public string Template { get; set; } = "<span title=\"{0}\">{1}</span>";

    [UsedImplicitly]
    public DateTimeFormatterOptions DefaultOptions { get; set; } = new DateTimeFormatterOptions();

    protected internal virtual bool DisplayDatesInUtc
    {
      get
      {
        if (_displayDatesInUtc.HasValue)
        {
          return _displayDatesInUtc.Value;
        }

        _displayDatesInUtc = Settings.GetBoolSetting("Analytics.Reports.DisplayDatesInUtc", false);
        return _displayDatesInUtc.Value;
      }
    }

    protected virtual TimeZoneInfo PrimaryTimeZoneInfo => _primaryTimeZoneInfo
        ?? (_primaryTimeZoneInfo = DisplayDatesInUtc ? TimeZoneInfo.Utc : Settings.ServerTimeZone);

    protected virtual TimeZoneInfo SecondaryTimeZoneInfo => _secondaryTimeZoneInfo
        ?? (_secondaryTimeZoneInfo = !DisplayDatesInUtc ? TimeZoneInfo.Utc : Settings.ServerTimeZone);

    public virtual string FormatTime([Utc] DateTime dateTime, DateTimeFormatterOptions options = null)
    {
      options = options ?? DefaultOptions;

      if (!options.RichFormatting || PrimaryTimeZoneInfo.GetUtcOffset(dateTime) == SecondaryTimeZoneInfo.GetUtcOffset(dateTime))
      {
        return FormatTime(dateTime, options, DateMode.Primary, PrimaryTimeZoneInfo);
      }

      return string.Format(
          Template,
          FormatTime(dateTime, options, DateMode.Secondary, SecondaryTimeZoneInfo),
          FormatTime(dateTime, options, DateMode.Primary, PrimaryTimeZoneInfo));
    }

    public virtual string FormatShortDate([Utc] DateTime dateTime)
    {
      dateTime = GetEffectiveDate(dateTime, DateMode.Primary);

      return DateUtil.FormatDateTime(dateTime, "d", Context.Culture);
    }

    public virtual string FormatShortDateTime([Utc] DateTime dateTime, DateTimeFormatterOptions options = null)
    {
      options = options ?? DefaultOptions;

      if (!options.RichFormatting || PrimaryTimeZoneInfo.GetUtcOffset(dateTime) == SecondaryTimeZoneInfo.GetUtcOffset(dateTime))
      {
        return FormatShortDateTime(dateTime, options, DateMode.Primary, PrimaryTimeZoneInfo);
      }

      return string.Format(
          Template,
          FormatShortDateTime(dateTime, options, DateMode.Secondary, SecondaryTimeZoneInfo),
          FormatShortDateTime(dateTime, options, DateMode.Primary, PrimaryTimeZoneInfo));
    }

    public virtual string FormatLongDateTime([Utc] DateTime dateTime, DateTimeFormatterOptions options = null)
    {
      options = options ?? DefaultOptions;

      if (!options.RichFormatting || PrimaryTimeZoneInfo.GetUtcOffset(dateTime) == SecondaryTimeZoneInfo.GetUtcOffset(dateTime))
      {
        return FormatLongDateTime(dateTime, options, DateMode.Primary, PrimaryTimeZoneInfo);
      }

      return string.Format(
          Template,
          FormatLongDateTime(dateTime, options, DateMode.Secondary, SecondaryTimeZoneInfo),
          FormatLongDateTime(dateTime, options, DateMode.Primary, PrimaryTimeZoneInfo));
    }

    public virtual DateTime GetEffectiveDate([Utc] DateTime dateTime, DateMode dateMode)
    {
      if (dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue)
      {
        return dateTime;
      }

      if (dateTime.Kind != DateTimeKind.Utc)
      {
        throw new ArgumentException("DateTimeKind must be Utc");
      }

      if (DisplayDatesInUtc)
      {
        return dateMode == DateMode.Primary ? dateTime : DateUtil.ToServerTime(dateTime);
      }

      return dateMode == DateMode.Primary ? DateUtil.ToServerTime(dateTime) : dateTime;
    }
    public void TransformDayHour(ref int day, ref int hour)
    {
      if (DisplayDatesInUtc)
      {
        return;
      }

      TimeSpan utcOffset = PrimaryTimeZoneInfo.GetUtcOffset(GetDateTime());

      // We have lost the number of minutes, we'll have to make do with adding whole hours
      hour = hour + utcOffset.Hours;
      if (hour >= 24)
      {
        // The time has moved onto the next day.
        hour -= 24;
        day++;

        if (day == 7)
        {
          // The day has moved onto next week, reset to first day now
          day = 0;
        }
      }
      else if (hour < 0)
      {
        // The time has moved back to the previous day
        hour += 24;
        day--;

        if (day == -1)
        {
          // The day has moved back to the previous week, reset to last day now.
          day = 6;
        }
      }
    }

    /// <inheritdoc />
    public DateTimeFormatterOptions GetDefaultOptions()
    {
      return DefaultOptions.Clone();
    }

    protected virtual string FormatTime([Utc] DateTime dateTime, [NotNull] DateTimeFormatterOptions options, DateMode dateMode, [NotNull] TimeZoneInfo timeZoneInfo)
    {
      Assert.ArgumentNotNull(options, nameof(options));

      DateTime effectiveDate = GetEffectiveDate(dateTime, dateMode);
      string utcOffset = dateMode == DateMode.Secondary || options.ShowUtcOffset ? FormatUtcOffset(timeZoneInfo, effectiveDate) : string.Empty;

      return $"{DateUtil.FormatDateTime(effectiveDate, "t", Context.Culture)}{utcOffset}";
    }

    protected virtual string FormatShortDateTime([Utc] DateTime dateTime, [NotNull] DateTimeFormatterOptions options, DateMode dateMode, [NotNull] TimeZoneInfo timeZoneInfo)
    {
      Assert.ArgumentNotNull(options, nameof(options));
      Assert.ArgumentNotNull(timeZoneInfo, nameof(timeZoneInfo));

      DateTime effectiveDate = GetEffectiveDate(dateTime, dateMode);
      string utcOffset = dateMode == DateMode.Secondary || options.ShowUtcOffset ? FormatUtcOffset(timeZoneInfo, effectiveDate) : string.Empty;

      return $"{DateUtil.FormatShortDateTime(effectiveDate, Context.Culture)}{utcOffset}";
    }

    protected virtual string FormatLongDateTime([Utc] DateTime dateTime, [NotNull] DateTimeFormatterOptions options, DateMode dateMode, [NotNull] TimeZoneInfo timeZoneInfo)
    {
      Assert.ArgumentNotNull(options, nameof(options));
      Assert.ArgumentNotNull(timeZoneInfo, nameof(timeZoneInfo));

      DateTime effectiveDate = GetEffectiveDate(dateTime, dateMode);
      string utcOffset = dateMode == DateMode.Secondary || options.ShowUtcOffset ? FormatUtcOffset(timeZoneInfo, effectiveDate) : string.Empty;

      return $"{DateUtil.FormatLongDateTime(effectiveDate, Context.Culture)}{utcOffset}";
    }

    protected virtual string FormatUtcOffset([NotNull] TimeZoneInfo timeZoneInfo, DateTime dateTime)
    {
      Assert.ArgumentNotNull(timeZoneInfo, nameof(timeZoneInfo));

      if (ReferenceEquals(timeZoneInfo, TimeZoneInfo.Utc))
      {
        return " (UTC)";
      }

      TimeSpan utcOffset = timeZoneInfo.GetUtcOffset(dateTime);
      string sign = utcOffset < TimeSpan.Zero ? "-" : "+";
      return $" (UTC {sign}{Math.Abs(utcOffset.Hours):00}:{Math.Abs(utcOffset.Minutes):00})";
    }
    protected internal virtual DateTime GetDateTime()
    {
      return DateTime.Now;
    }
  }
}