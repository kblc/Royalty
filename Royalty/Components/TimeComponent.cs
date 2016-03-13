using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Royalty.Components
{
    public class TimeComponent : DependencyObject, IDisposable
    {
        #region Current

        protected static readonly DependencyPropertyKey CurrentPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(Current), typeof(DateTime), typeof(TimeComponent),
                new FrameworkPropertyMetadata(DateTime.Now,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyCurrentPropertyKey = CurrentPropertyKey.DependencyProperty;

        public DateTime Current
        {
            get { return (DateTime)GetValue(ReadOnlyCurrentPropertyKey); }
            protected set { SetValue(CurrentPropertyKey, value); }
        }

        #endregion

        private Timer updateCurrentTimer;

        public TimeComponent()
        {
            updateCurrentTimer = new Timer((o) => {
                Dispatcher.Invoke(() => { Current = DateTime.Now; });
            }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                if (updateCurrentTimer != null)
                {
                    updateCurrentTimer.Dispose();
                    updateCurrentTimer = null;
                }

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~TimeComponent()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
