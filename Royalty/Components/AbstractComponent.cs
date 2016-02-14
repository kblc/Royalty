﻿using Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Royalty.Components
{
    public abstract class AbstractComponent : DependencyObject
    {
        #region IsLoaded

        protected static readonly DependencyPropertyKey ReadOnlyIsLoadedPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(IsLoaded), typeof(bool), typeof(AbstractComponent),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyIsLoadedProperty = ReadOnlyIsLoadedPropertyKey.DependencyProperty;

        public bool IsLoaded
        {
            get { return (bool)GetValue(ReadOnlyIsLoadedProperty); }
            protected set { SetValue(ReadOnlyIsLoadedPropertyKey, value); }
        }

        #endregion
        #region Error

        protected static readonly DependencyPropertyKey ReadOnlyErrorPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(Error), typeof(string), typeof(AbstractComponent),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyErrorProperty = ReadOnlyErrorPropertyKey.DependencyProperty;

        public string Error
        {
            get { return (string)GetValue(ReadOnlyErrorProperty); }
            protected set { SetValue(ReadOnlyErrorPropertyKey, value); }
        }

        #endregion
        #region State

        protected static readonly DependencyPropertyKey ReadOnlyStatePropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(State), typeof(RoyaltyServiceWorker.Additional.WorkerState), typeof(AbstractComponent),
                new FrameworkPropertyMetadata(RoyaltyServiceWorker.Additional.WorkerState.None,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyStateProperty = ReadOnlyErrorPropertyKey.DependencyProperty;

        public RoyaltyServiceWorker.Additional.WorkerState State
        {
            get { return (RoyaltyServiceWorker.Additional.WorkerState)GetValue(ReadOnlyStateProperty); }
            protected set { SetValue(ReadOnlyStatePropertyKey, value); }
        }
        #endregion
        #region ConnectionTimeInterval

        public static readonly DependencyProperty ConnectionTimeIntervalProperty = DependencyProperty.Register(nameof(ConnectionTimeInterval), typeof(TimeSpan),
            typeof(AbstractComponent), new PropertyMetadata(TimeSpan.FromSeconds(RoyaltyServiceWorker.Additional.AbstractBaseWorker.DefaultConnectionTimeIntervalIsSeconds), (s, e) =>
            { (s as AbstractComponent)?.OnConnectionTimeIntervalChanged((TimeSpan)e.NewValue); }));

        public TimeSpan ConnectionTimeInterval
        {
            get { return (TimeSpan)GetValue(ConnectionTimeIntervalProperty); }
            set { SetValue(ConnectionTimeIntervalProperty, value); }
        }

        protected abstract void OnConnectionTimeIntervalChanged(TimeSpan value);

        #endregion
        #region ServiceCultureInfo

        public static readonly DependencyProperty ServiceCultureInfoProperty = DependencyProperty.Register(nameof(ServiceCultureInfo), typeof(CultureInfo),
            typeof(AbstractComponent), new PropertyMetadata(System.Threading.Thread.CurrentThread.CurrentUICulture, (s, e) =>
            { (s as AbstractComponent).OnServiceCultureInfoChanged((CultureInfo)e.NewValue); }));

        public CultureInfo ServiceCultureInfo
        {
            get { return (CultureInfo)GetValue(ServiceCultureInfoProperty); }
            set { SetValue(ServiceCultureInfoProperty, value); }
        }

        protected abstract void OnServiceCultureInfoChanged(CultureInfo value);
        #endregion
        #region IsActive

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(nameof(IsActive), typeof(bool),
            typeof(AbstractComponent), new PropertyMetadata(false, (s, e) =>
            { (s as AbstractComponent)?.OnIsActiveChanged((bool)e.NewValue); }));

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        protected abstract void OnIsActiveChanged(bool value);
        #endregion
    }

    public abstract class AbstractComponent<TWorker> : AbstractComponent
        where TWorker : RoyaltyServiceWorker.Additional.AbstractBaseWorker
    {
        protected TWorker worker { get; private set; }

        public AbstractComponent() { }

        public AbstractComponent(TWorker worker) 
        {
            InitializeWorker(worker);
        }

        protected void InitializeWorker(TWorker worker)
        {
            if (this.worker != null)
            {
                this.worker.OnErrorChanged -= Worker_OnErrorChanged;
                this.worker.OnLoadedChanged -= Worker_OnLoadedChanged;
                this.worker.OnStateChanged -= Worker_OnStateChanged;
                this.worker.Dispose();
                this.worker = null;
            }

            if (worker != null)
            {
                Error = null;
                State = RoyaltyServiceWorker.Additional.WorkerState.None;

                this.worker = worker;
                this.worker.OnErrorChanged += Worker_OnErrorChanged;
                this.worker.OnLoadedChanged += Worker_OnLoadedChanged;
                this.worker.OnStateChanged += Worker_OnStateChanged;
                this.worker.ServiceCultureInfo = ServiceCultureInfo;
                if (IsActive)
                    this.worker.Start();
            }
        }

        private void Worker_OnStateChanged(object sender, RoyaltyServiceWorker.Additional.WorkerState e) => RunUnderDispatcher(() => State = e);
        private void Worker_OnLoadedChanged(object sender, bool e) => RunUnderDispatcher(() => IsLoaded = e);
        private void Worker_OnErrorChanged(object sender, string e) => RunUnderDispatcher(() => Error = e);

        protected void RunUnderDispatcher(Action a)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, a);
        }

        protected override void OnConnectionTimeIntervalChanged(TimeSpan value)
        {
            if (worker != null)
                worker.ConnectionTimeInterval = value;
        }
        protected override void OnServiceCultureInfoChanged(CultureInfo value)
        {
            if (worker != null)
                worker.ServiceCultureInfo = value;
        }
        protected override void OnIsActiveChanged(bool value)
        {
            if (value)
                worker?.Start();
            else
                worker?.Stop();
        }
    }
}
