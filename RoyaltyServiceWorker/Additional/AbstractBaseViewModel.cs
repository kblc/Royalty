using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RoyaltyServiceWorker.Additional
{
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExpr = propertyExpression.Body as MemberExpression;
            if (memberExpr == null)
                throw new ArgumentException("propertyExpression should represent access to a member");
            string memberName = memberExpr.Member.Name;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    public abstract class NotifyDisposablePropertyChangedBase : NotifyPropertyChangedBase, IDisposable
    {
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected abstract void DisposeManaged();

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    DisposeManaged();
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion
    }

    public abstract class AbstractBaseViewModel : NotifyPropertyChangedBase, IDisposable
    {
        public AbstractBaseViewModel()
        {
            Init();
        }

        public System.Runtime.Remoting.Contexts.Context Context { get; } = Thread.CurrentContext;

        protected abstract void Init();

        private bool isLoaded = false;
        public bool IsLoaded
        {
            get { return isLoaded; }
            protected set
            {
                if (isLoaded == value)
                    return;
                isLoaded = value;
                Context.DoCallBack(() =>
                {
                    RaisePropertyChanged(() => IsLoaded);
                    IsLoadedChanged?.Invoke(this, value);
                });
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void OnDisposing() { }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    OnDisposing();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        public event EventHandler<bool> IsLoadedChanged;

        protected Task ExecuteCommandAsDispatcher(Action action, Action<Task> checkAction = null)
        {
            var ui = SynchronizationContext.Current != null ? TaskScheduler.FromCurrentSynchronizationContext() : null;
            Task res = null;
            if (ui != null)
            {
                res = Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, ui);
                if (checkAction != null)
                    res = res.ContinueWith(checkAction, ui);
            }
            else
            {
                res = Task.Factory.StartNew(action);
                if (checkAction != null)
                    res = res.ContinueWith(checkAction);
            }

            return res;
        }
        protected Task ExecuteEndOfCommandAsDispatcher(Action action, Action<Task> checkAction = null)
        {
            if (SynchronizationContext.Current == null)
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            var ui = SynchronizationContext.Current != null ? TaskScheduler.FromCurrentSynchronizationContext() : null;
            Task res = null;
            if (ui != null)
            {
                res = Task.Factory.StartNew(action);
                if (checkAction != null)
                    res = res.ContinueWith(checkAction, ui);
            }
            else
            {
                res = Task.Factory.StartNew(action);
                if (checkAction != null)
                    res = res.ContinueWith(checkAction);
            }

            return res;
        }
    }
}
