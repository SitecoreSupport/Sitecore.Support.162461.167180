namespace Sitecore.Support.ExM.Framework.Formatters
{
  public class DateTimeFormatterOptions
  {
    public bool RichFormatting { get; set; } = true;

    public bool ShowUtcOffset { get; set; } = true;

    public DateTimeFormatterOptions Clone()
    {
      return new DateTimeFormatterOptions
      {
        RichFormatting = RichFormatting,
        ShowUtcOffset = ShowUtcOffset
      };
    }
  }
}
