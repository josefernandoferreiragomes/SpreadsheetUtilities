namespace SpreadsheetUtility.Application.Ports;

public interface IDateTimeProvider
{
    DateTime Now { get; }
    DateTime Today { get; }
    DateTime UtcNow { get; }
}
