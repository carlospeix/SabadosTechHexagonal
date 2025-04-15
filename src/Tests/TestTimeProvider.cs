using Model.Ports.Driven;

namespace Tests;

public class TestTimeProvider : ITimeProvider
{
    private DateTime utcNow;

    public TestTimeProvider(DateTime initialTime)
    {
        utcNow = initialTime;
    }

    public DateTime UtcNow => utcNow;

    public void TravelBy(TimeSpan timeSpan)
    {
        utcNow = utcNow.Add(timeSpan);
    }

    public void TravelTo(DateTime utcTime)
    {
        utcNow = utcTime;
    }
}
