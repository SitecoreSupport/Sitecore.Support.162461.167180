using System;

namespace Sitecore.Support.ExM.Framework.Formatters
{
  public interface IDateTimeFormatter
  {
    DateTimeFormatterOptions GetDefaultOptions();

    string FormatTime([Utc] DateTime dateTime, [CanBeNull] DateTimeFormatterOptions options = null);

    string FormatShortDate(DateTime dateTime);

    string FormatShortDateTime([Utc] DateTime dateTime, [CanBeNull] DateTimeFormatterOptions options = null);

    string FormatLongDateTime([Utc] DateTime dateTime, [CanBeNull] DateTimeFormatterOptions options = null);

    void TransformDayHour(ref int day, ref int hour);
  }
}