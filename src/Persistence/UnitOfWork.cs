namespace Persistence
{
    public class UnitOfWork(ApplicationContext applicationContext) : IDisposable
    {
        private bool disposed = false;
        private readonly ApplicationContext applicationContext = applicationContext;

        public void SaveChanges()
        {
            applicationContext.SaveChanges();
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
}
