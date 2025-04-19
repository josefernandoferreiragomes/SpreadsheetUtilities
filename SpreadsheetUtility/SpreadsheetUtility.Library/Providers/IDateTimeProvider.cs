namespace SpreadsheetUtility.Library.Providers
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
        DateTime Today { get; }
        DateTime UtcNow { get; }
    }
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTime Today => DateTime.Today;
        public DateTime UtcNow => DateTime.UtcNow;
    }

}
