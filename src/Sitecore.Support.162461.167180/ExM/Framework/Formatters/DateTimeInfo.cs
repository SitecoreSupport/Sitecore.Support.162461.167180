using System;
using Newtonsoft.Json;
using Sitecore.Configuration;

namespace Sitecore.Support.ExM.Framework.Formatters
{
  [UsedImplicitly]
  public class DateTimeInfo
  {
    static DateTimeInfo()
    {
      DisplayDatesInUtc = Settings.GetBoolSetting("Analytics.Reports.DisplayDatesInUtc", false);
      TimeZoneInfo = DisplayDatesInUtc ? TimeZoneInfo.Utc : Settings.ServerTimeZone;
    }

    public DateTimeInfo()
    {
      AMDesignator = Context.Culture.DateTimeFormat.AMDesignator;
      PMDesignator = Context.Culture.DateTimeFormat.PMDesignator;
      TimeSeparator = Context.Culture.DateTimeFormat.TimeSeparator;
      HourPattern = GetHourPattern();
    }

    [JsonProperty]
    public static bool DisplayDatesInUtc { get; }

    [JsonProperty]
    public static TimeZoneInfo TimeZoneInfo { get; }

    public TimeSpan UtcOffset => DisplayDatesInUtc
        ? TimeZoneInfo.Utc.BaseUtcOffset
        : TimeZoneInfo.GetUtcOffset(DateUtil.ToServerTime(DateTime.UtcNow));

    public string AMDesignator { get; }

    public string PMDesignator { get; }

    public string TimeSeparator { get; }

    public string HourPattern { get; }

    [NotNull]
    private string GetHourPattern()
    {
      string shortTimePattern = Context.Culture.DateTimeFormat.ShortTimePattern;
      string[] arr = shortTimePattern.Split(new[] { TimeSeparator }, StringSplitOptions.None);
      return arr[0];
    }
  }
}
