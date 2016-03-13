using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Royalty.ViewModels
{
    public abstract class AbstractViewModel : FrameworkElement
    {
        #region CancellationToken

        public static readonly DependencyProperty CancellationTokenProperty = DependencyProperty.Register(nameof(CancellationToken), typeof(CancellationToken?),
            typeof(AbstractViewModel), new PropertyMetadata(null, (s, e) => { (s as AbstractViewModel)?.RaiseCommands(); }));

        public CancellationToken? CancellationToken
        {
            get { return (CancellationToken?)GetValue(CancellationTokenProperty); }
            set { SetValue(CancellationTokenProperty, value); }
        }

        #endregion

        protected void RunUnderDispatcher(Action a)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, a);
        }

        protected CancellationToken GetCancellationToken()
        {
            return CancellationToken ?? System.Threading.CancellationToken.None;
        }

        protected virtual void RaiseCommands() { }
    }

    public abstract class AbstractActionViewModel : AbstractViewModel
    {
        #region IsBusy

        protected static readonly DependencyPropertyKey ReadOnlyIsBusyPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(IsBusy), typeof(bool), typeof(AbstractActionViewModel),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { (s as AbstractActionViewModel)?.RaiseCommands(); })));
        public static readonly DependencyProperty ReadOnlyIsBusyProperty = ReadOnlyIsBusyPropertyKey.DependencyProperty;

        public bool IsBusy
        {
            get { return (bool)GetValue(ReadOnlyIsBusyProperty); }
            protected set { SetValue(ReadOnlyIsBusyPropertyKey, value); }
        }

        #endregion
        #region Error

        private static readonly DependencyPropertyKey ReadOnlyErrorPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(Error), typeof(string), typeof(AbstractActionViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { (s as AbstractActionViewModel)?.RaiseCommands(); })));
        public static readonly DependencyProperty ReadOnlyErrorProperty = ReadOnlyErrorPropertyKey.DependencyProperty;

        public string Error
        {
            get { return (string)GetValue(ReadOnlyErrorProperty); }
            protected set { SetValue(ReadOnlyErrorPropertyKey, value); }
        }

        #endregion
    }
}
