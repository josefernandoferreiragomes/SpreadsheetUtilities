namespace SpreadsheetUtility.Domain.ValueObjects;

public readonly struct DateRange
{
    public DateTime Start { get; }
    public DateTime End { get; }

    public DateRange(DateTime start, DateTime end)
    {
        if (start > end)
            throw new ArgumentException("Start date must be before or equal to end date.", nameof(start));
        Start = start;
        End = end;
    }

    public bool IsWithin(DateTime date) => date >= Start && date <= End;

    public override string ToString() => $"{Start:yyyy-MM-dd};{End:yyyy-MM-dd}";

    public static DateRange? Parse(string range)
    {
        var dates = range.Split(';');
        if (dates.Length == 2 &&
            DateTime.TryParseExact(dates[0].Trim(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var start) &&
            DateTime.TryParseExact(dates[1].Trim(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var end))
        {
            return new DateRange(start, end);
        }
        return null;
    }
}
