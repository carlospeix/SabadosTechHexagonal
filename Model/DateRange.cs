namespace Model;

public class DateRange
{
    public DateRange(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    public DateTime Start { get; init; }
    public DateTime End { get; init; }

    public static DateRange Infinity => new(DateTime.MinValue, DateTime.MaxValue);

    public bool Includes(DateTime moment)
    {
        return moment >= Start && moment <= End;
    }
}
