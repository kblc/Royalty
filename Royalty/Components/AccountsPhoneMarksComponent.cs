using Helpers;
using Helpers.WPF;
using Royalty.Additional;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Royalty.Components
{
    public class AccountsPhoneMarksComponent : AbstractComponent<RoyaltyServiceWorker.AccountPhoneMarksWorker>,
        IHistoryItemsChange<RoyaltyServiceWorker.AccountService.AccountPhoneMark>
    {
        #region Account

        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register(nameof(Account), typeof(RoyaltyServiceWorker.AccountService.Account),
            typeof(AccountsPhoneMarksComponent), new PropertyMetadata(null, (s, e) => { (s as AccountsPhoneMarksComponent)?.OnAccountChanged(e.NewValue as RoyaltyServiceWorker.AccountService.Account, e.OldValue as RoyaltyServiceWorker.AccountService.Account); }));

        public RoyaltyServiceWorker.AccountService.Account Account
        {
            get { return (RoyaltyServiceWorker.AccountService.Account)GetValue(AccountProperty); }
            set { SetValue(AccountProperty, value); }
        }

        #endregion
        #region HistoryComponent

        public static readonly DependencyProperty HistoryComponentProperty = DependencyProperty.Register(nameof(HistoryComponent), typeof(HistoryComponent),
            typeof(AccountsPhoneMarksComponent), new PropertyMetadata(null, (s, e) =>
            {
                var model = s as AccountsPhoneMarksComponent;
                var newHistoryComponent = e.NewValue as HistoryComponent;
                var oldHistoryComponent = e.OldValue as HistoryComponent;
                if (model != null && newHistoryComponent != oldHistoryComponent)
                {
                    if (oldHistoryComponent != null)
                        oldHistoryComponent.Change -= model.WorkerApplyHistory;
                    if (newHistoryComponent != null)
                        newHistoryComponent.Change += model.WorkerApplyHistory;
                }
            }));

        public HistoryComponent HistoryComponent
        {
            get { return (HistoryComponent)GetValue(HistoryComponentProperty); }
            set { SetValue(HistoryComponentProperty, value); }
        }

        #endregion
        #region PageIndex

        public static readonly DependencyProperty PageIndexProperty = DependencyProperty.Register(nameof(PageIndex), typeof(uint),
            typeof(AccountsPhoneMarksComponent), new PropertyMetadata((uint)0, (s, e) => { (s as AccountsPhoneMarksComponent)?.OnPageIndexChanged((uint)e.NewValue, (uint)e.OldValue); }));

        public uint PageIndex
        {
            get { return (uint)GetValue(PageIndexProperty); }
            set { SetValue(PageIndexProperty, value); }
        }

        #endregion
        #region PageCount

        protected static readonly DependencyPropertyKey ReadOnlyPageCountPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(PageCount), typeof(uint), typeof(AccountsPhoneMarksComponent),
                new FrameworkPropertyMetadata((uint)0,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { (s as AccountsPhoneMarksComponent)?.OnPageCountChanged((uint)e.NewValue, (uint)e.OldValue); })));
        public static readonly DependencyProperty ReadOnlyPageCountProperty = ReadOnlyPageCountPropertyKey.DependencyProperty;

        public uint PageCount
        {
            get { return (uint)GetValue(ReadOnlyPageCountProperty); }
            protected set { SetValue(ReadOnlyPageCountPropertyKey, value); }
        }

        #endregion
        #region Filter

        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(nameof(Filter), typeof(string),
            typeof(AccountsPhoneMarksComponent), new PropertyMetadata(string.Empty, (s, e) => { (s as AccountsPhoneMarksComponent)?.OnFilterChanged((string)e.NewValue, (string)e.OldValue); }));

        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        #endregion
        #region ItemsPerPage

        public static readonly DependencyProperty ItemsPerPageProperty = DependencyProperty.Register(nameof(ItemsPerPage), typeof(uint),
            typeof(AccountsPhoneMarksComponent), new PropertyMetadata(RoyaltyServiceWorker.AccountPhoneMarksWorker.DEFAULT_ITEMS_PER_PAGE, (s, e) => { (s as AccountsPhoneMarksComponent)?.OnItemsPerPageChanged((uint)e.NewValue, (uint)e.OldValue); }));

        public uint ItemsPerPage
        {
            get { return (uint)GetValue(ItemsPerPageProperty); }
            set { SetValue(ItemsPerPageProperty, value); }
        }

        #endregion
        #region DecreasePageIndexCommand

        protected static readonly DependencyPropertyKey ReadOnlyDecreasePageIndexCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(DecreasePageIndexCommand), typeof(DelegateCommand), typeof(AccountsPhoneMarksComponent),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyDecreasePageIndexCommandProperty = ReadOnlyDecreasePageIndexCommandPropertyKey.DependencyProperty;

        public DelegateCommand DecreasePageIndexCommand
        {
            get { return (DelegateCommand)GetValue(ReadOnlyDecreasePageIndexCommandProperty); }
            protected set { SetValue(ReadOnlyDecreasePageIndexCommandPropertyKey, value); }
        }

        #endregion
        #region IncreasePageIndexCommand

        protected static readonly DependencyPropertyKey ReadOnlyIncreasePageIndexCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(IncreasePageIndexCommand), typeof(DelegateCommand), typeof(AccountsPhoneMarksComponent),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyIncreasePageIndexCommandProperty = ReadOnlyIncreasePageIndexCommandPropertyKey.DependencyProperty;

        public DelegateCommand IncreasePageIndexCommand
        {
            get { return (DelegateCommand)GetValue(ReadOnlyIncreasePageIndexCommandProperty); }
            protected set { SetValue(ReadOnlyIncreasePageIndexCommandPropertyKey, value); }
        }

        #endregion

        private NotifyCollectionWatcher<RoyaltyServiceWorker.AccountService.AccountPhoneMark> sourceItems = new NotifyCollectionWatcher<RoyaltyServiceWorker.AccountService.AccountPhoneMark>((x,y) => x.Id == y.Id);
        public IReadOnlyNotifyCollection<RoyaltyServiceWorker.AccountService.AccountPhoneMark> AccountPhoneMarks => sourceItems;

        public AccountsPhoneMarksComponent() {
            DecreasePageIndexCommand = new DelegateCommand(o => { PageIndex -= 1; }, o => IsLoaded && IsActive && PageIndex > 0);
            IncreasePageIndexCommand = new DelegateCommand(o => { PageIndex += 1; }, o => IsLoaded && IsActive && PageCount > 0 && PageIndex < PageCount - 1);
        }

        protected override void OnIsActiveChanged(bool value)
        {
            base.OnIsActiveChanged(value);
            raiseUpdateCommands();
        }

        protected override void OnIsLoadedChanged(bool value)
        {
            base.OnIsLoadedChanged(value);
            raiseUpdateCommands();
        }

        private void raiseUpdateCommands()
        {
            DecreasePageIndexCommand.RaiseCanExecuteChanged();
            IncreasePageIndexCommand.RaiseCanExecuteChanged();
        }

        private void OnAccountChanged(RoyaltyServiceWorker.AccountService.Account newValue, RoyaltyServiceWorker.AccountService.Account oldValue)
        {
            sourceItems.Clear();
            if (this.worker != null)
            {
                this.worker.OnItemsChanged -= Worker_OnItemsChanged;
                this.worker.PropertyChanged -= Worker_PropertyChanged;
            }

            InitializeWorker(newValue != null ? new RoyaltyServiceWorker.AccountPhoneMarksWorker(newValue.Id) : null);

            if (this.worker != null)
            {
                this.worker.OnItemsChanged += Worker_OnItemsChanged;
                this.worker.PropertyChanged += Worker_PropertyChanged;
            }
        }

        private void OnPageIndexChanged(uint newValue, uint oldValue)
        {
            if (this.worker != null)
                this.worker.PageIndex = newValue;
            raiseUpdateCommands();
        }

        private void OnPageCountChanged(uint newValue, uint oldValue)
        {
            raiseUpdateCommands();
        }

        private void OnFilterChanged(string newValue, string oldValue)
        {
            if (this.worker != null)
                this.worker.Filter = newValue;
            raiseUpdateCommands();
        }

        private void OnItemsPerPageChanged(uint newValue, uint oldValue)
        {
            if (this.worker != null)
                this.worker.ItemsPerPage = newValue;
            raiseUpdateCommands();
        }

        private void Worker_OnItemsChanged(object sender, RoyaltyServiceWorker.Additional.ListItemsEventArgs<RoyaltyServiceWorker.AccountService.AccountPhoneMark> e)
            => RunUnderDispatcher(() => { sourceItems.UpdateCollection(e); Change?.Invoke(this, e); });

        private void Worker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            => RunUnderDispatcher(() => {
            var existedProperty = GetType().GetProperty(e.PropertyName);
            var sourcepProperty = typeof(RoyaltyServiceWorker.AccountPhoneMarksWorker).GetProperty(e.PropertyName);
            if (existedProperty != null && existedProperty.CanWrite && sourcepProperty != null && sourcepProperty.CanRead)
            {
                try
                {
                    var value = sourcepProperty.GetValue(sender);
                    if (sourcepProperty.PropertyType != existedProperty.PropertyType)
                        try
                        {
                            value = Convert.ChangeType(value, existedProperty.PropertyType);
                        }
                        catch { }
                    existedProperty.SetValue(this, value, null);
                }
                catch { }
            }
        });

        private void WorkerApplyHistory(object sender, RoyaltyServiceWorker.HistoryService.History e) => worker?.ApplyHistoryChanges(e);

        public event EventHandler<RoyaltyServiceWorker.Additional.ListItemsEventArgs<RoyaltyServiceWorker.AccountService.AccountPhoneMark>> Change;
    }
}
