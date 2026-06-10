namespace SpreadsheetUtility.Infrastructure.Providers;

public class DateTimeProvider : Application.Ports.IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
    public DateTime Today => DateTime.Today;
    public DateTime UtcNow => DateTime.UtcNow;
}
