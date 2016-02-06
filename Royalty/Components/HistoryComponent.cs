using Helpers;
using Royalty.Additional;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Royalty.Components
{
    public class HistoryComponent : AbstractComponent<RoyaltyServiceWorker.HistoryWorker>
    {
        #region IsConnecting

        private static readonly DependencyPropertyKey ReadOnlyIsConnectingPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(IsConnecting), typeof(bool), typeof(HistoryComponent),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyIsConnectingProperty = ReadOnlyIsConnectingPropertyKey.DependencyProperty;

        public bool IsConnecting
        {
            get { return (bool)GetValue(ReadOnlyIsConnectingProperty); }
            private set { SetValue(ReadOnlyIsConnectingPropertyKey, value); }
        }

        #endregion
        #region IsWaiting

        private static readonly DependencyPropertyKey ReadOnlyIsWaitingPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(IsWaiting), typeof(bool), typeof(HistoryComponent),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyIsWaitingProperty = ReadOnlyIsWaitingPropertyKey.DependencyProperty;

        public bool IsWaiting
        {
            get { return (bool)GetValue(ReadOnlyIsWaitingProperty); }
            private set { SetValue(ReadOnlyIsWaitingPropertyKey, value); }
        }

        #endregion

        public HistoryComponent() : base(new RoyaltyServiceWorker.HistoryWorker())
        {
            worker.OnConnectingChanged += (s, e) => RunUnderDispatcher(() => IsConnecting = e);
            worker.OnWaitingChanged += (s, e) => RunUnderDispatcher(() => IsWaiting = e);
            worker.Changed += (_,e) => RunUnderDispatcher(() => Change?.Invoke(this, e));
        }

        public EventHandler<RoyaltyServiceWorker.HistoryService.History> Change;
    }
}
