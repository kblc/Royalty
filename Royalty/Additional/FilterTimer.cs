using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Royalty.Additional
{
    public class FilterTimer<T> : IDisposable
    {
        private Timer timer;
        private TimeSpan waitInterval;
        private Action<T> fireAction;
        private T filter;

        public FilterTimer(TimeSpan waitInterval, Action<T> fireAction)
        {
            if (waitInterval == null)
                throw new ArgumentNullException(nameof(waitInterval));
            if (fireAction == null)
                throw new ArgumentNullException(nameof(fireAction));
            this.waitInterval = waitInterval;
            this.fireAction = fireAction;
        }

        public T Filter {
            get { return filter; }
            set
            {
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }
                filter = value;
                timer = new Timer((o) => 
                {
                    fireAction((T)o);
                }, filter, (int)waitInterval.TotalMilliseconds, Timeout.Infinite);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }
                disposedValue = true;
            }
        }

         ~FilterTimer()
        {
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
