namespace Persistence;

public class UnitOfWork(ApplicationContext applicationContext) : IDisposable
{
    private readonly ApplicationContext applicationContext = applicationContext;
    private bool disposed = false;

    public async Task SaveChangesAsync()
    {
        await applicationContext.SaveChangesAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                applicationContext.Dispose();
            }
            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
