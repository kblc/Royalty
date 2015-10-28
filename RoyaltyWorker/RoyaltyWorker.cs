using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoyaltyRepository;

namespace RoyaltyWorker
{
    public class RoyaltyWorker : IDisposable //IRoyaltyWorker, 
    {
        /// <summary>
        /// Royalty repository
        /// </summary>
        public Repository Repository { get; private set; }

        /// <summary>
        /// Create new worker instance
        /// </summary>
        /// <param name="repository">Royalty repository</param>
        public RoyaltyWorker(Repository repository)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            this.Repository = repository;
        }

        

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Repository != null)
                    {
                        Repository.Dispose();
                        Repository = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~RoyaltyWorker() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
