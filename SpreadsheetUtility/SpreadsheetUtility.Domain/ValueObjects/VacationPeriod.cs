namespace SpreadsheetUtility.Domain.ValueObjects;

public readonly struct VacationPeriod
{
    public DateRange? Range { get; }

    public VacationPeriod(DateRange? range)
    {
        Range = range;
    }

    public bool IsVacation(DateTime date) => Range.HasValue && Range.Value.IsWithin(date);

    public override string ToString() => Range?.ToString() ?? "";

    public static VacationPeriod? FromString(string value)
    {
        var range = DateRange.Parse(value);
        return range.HasValue ? new VacationPeriod(range) : null;
    }
}
