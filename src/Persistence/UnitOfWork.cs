namespace Persistence
{
    public class UnitOfWork : IDisposable
    {
        private bool disposed = false;
        private readonly ApplicationContext applicationContext;

        public UnitOfWork(ApplicationContext applicationContext) => this.applicationContext = applicationContext;

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
