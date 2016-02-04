using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helpers;
using System.Runtime.CompilerServices;
using RoyaltyServiceWorker.Additional;
using RoyaltyServiceWorker.Properties;

namespace RoyaltyServiceWorker.Additional
{
    public enum WorkerState
    {
        None,
        Error,
        Started,
        Stoped,
    }

    public abstract class AbstractBaseWorker : Additional.NotifyPropertyChangedBase, IDisposable
    {
        static AbstractBaseWorker()
        {
            //Automapper.Init();
        }

        public const int DefaultConnectionTimeIntervalIsSeconds = 5;

        public System.Runtime.Remoting.Contexts.Context Context { get; } = Thread.CurrentContext;

        protected override void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            Context.DoCallBack(() => base.RaisePropertyChanged(propertyName));
        }
        protected override void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            Context.DoCallBack(() => base.RaisePropertyChanged<T>(propertyExpression));
        }

        protected abstract bool DoStart();
        protected abstract bool DoStop();

        public void Start()
        {
            if (State == WorkerState.Started)
                return;

            State = (DoStart()) ? WorkerState.Started : WorkerState.Error;
        }
        public void Stop()
        {
            if (State == WorkerState.Stoped)
                return;

            State = (DoStop()) ? WorkerState.Stoped : WorkerState.Error;
        }

        private WorkerState state = WorkerState.None;
        public WorkerState State
        {
            get { return state; }
            protected set
            {
                if (state == value)
                    return;
                state = value;
                RaisePropertyChanged();
                RaiseOnStateChanged();
            }
        }

        private bool isLoaded = false;
        public bool IsLoaded
        {
            get { return isLoaded; }
            protected set
            {
                if (isLoaded == value)
                    return;
                isLoaded = value;
                RaisePropertyChanged();
                RaiseOnLoadedChanged();
            }
        }

        private string error = string.Empty;
        public string Error
        {
            get { return error; }
            private set
            {
                if (error == value)
                    return;

                error = value;
                RaisePropertyChanged();
                RaiseOnErrorChanged();
            }
        }

        protected void SetError(string ex)
        {
            Error = string.IsNullOrWhiteSpace(ex) ? null : ex;
        }
        protected void SetError(Exception ex)
        {
            SetError(ex.ToString());
        }

        private CultureInfo serviceCultureInfo = Thread.CurrentThread.CurrentUICulture;
        public CultureInfo ServiceCultureInfo
        {
            get { return serviceCultureInfo; }
            set
            {
                if (serviceCultureInfo == value)
                    return;

                if (value == null)
                    throw new ArgumentNullException(nameof(ServiceCultureInfo));

                serviceCultureInfo = value;
                RaisePropertyChanged();
            }
        }

        private TimeSpan connectionTimeInterval = TimeSpan.FromSeconds(DefaultConnectionTimeIntervalIsSeconds);
        public TimeSpan ConnectionTimeInterval
        {
            get { return connectionTimeInterval; }
            set
            {
                if (connectionTimeInterval == value)
                    return;
                connectionTimeInterval = value;
                RaisePropertyChanged();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void OnDisposing() { }

        protected void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (State == WorkerState.Started)
                        DoStop();
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

        private void RaiseOnLoadedChanged()
        {
            RaiseOnNotification(
            new NotificationItem(
                header: Resources.ResourceManager.GetString(nameof(Resources.NOTIFICATION), serviceCultureInfo),
                message: IsLoaded ? Resources.ResourceManager.GetString(nameof(Resources.LOADED), serviceCultureInfo) : Resources.ResourceManager.GetString(nameof(Resources.UNLOADED), serviceCultureInfo)
            ));
            Context.DoCallBack(() => OnLoadedChanged?.Invoke(this, IsLoaded));
        }
        private void RaiseOnErrorChanged()
        {
            if (!string.IsNullOrWhiteSpace(Error))
                RaiseOnNotification(
                    new NotificationItem(
                        header: Resources.ResourceManager.GetString(nameof(Resources.ERROR), serviceCultureInfo),
                        message: Error,
                        isError: true,
                        created: DateTime.UtcNow
                    ));
            Context.DoCallBack(() => OnErrorChanged?.Invoke(this, Error));
        }
        private void RaiseOnStateChanged()
        {
            if (new[] { WorkerState.Started, WorkerState.Stoped }.Contains(this.State))
                RaiseOnNotification(
                    new NotificationItem(
                        header: Resources.ResourceManager.GetString(nameof(Resources.NOTIFICATION), serviceCultureInfo),
                        message: State == WorkerState.Started ? Resources.ResourceManager.GetString(nameof(Resources.STARTED), serviceCultureInfo) : Resources.ResourceManager.GetString(nameof(Resources.STOPED), serviceCultureInfo)
                    ));
            Context.DoCallBack(() => OnStateChanged?.Invoke(this, State));
        }
        protected void RaiseOnNotification(NotificationItem notification) => Context.DoCallBack(() => OnNotification?.Invoke(this, notification));

        public event EventHandler<bool> OnLoadedChanged;
        public event EventHandler<string> OnErrorChanged;
        public event EventHandler<WorkerState> OnStateChanged;
        public event EventHandler<NotificationItem> OnNotification;
    }
}
