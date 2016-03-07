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
    public class AccountsAdditionalColumnsComponent : AbstractComponent<RoyaltyServiceWorker.AccountAdditionalColumnsWorker>,
        IHistoryItemsChange<RoyaltyServiceWorker.AccountService.AccountDataRecordAdditionalColumn>
    {
        #region Account

        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register(nameof(Account), typeof(RoyaltyServiceWorker.AccountService.Account),
            typeof(AccountsAdditionalColumnsComponent), new PropertyMetadata(null, (s, e) => { (s as AccountsAdditionalColumnsComponent)?.OnAccountChanged(e.NewValue as RoyaltyServiceWorker.AccountService.Account, e.OldValue as RoyaltyServiceWorker.AccountService.Account); }));

        public RoyaltyServiceWorker.AccountService.Account Account
        {
            get { return (RoyaltyServiceWorker.AccountService.Account)GetValue(AccountProperty); }
            set { SetValue(AccountProperty, value); }
        }

        #endregion
        #region HistoryComponent

        public static readonly DependencyProperty HistoryComponentProperty = DependencyProperty.Register(nameof(HistoryComponent), typeof(HistoryComponent),
            typeof(AccountsAdditionalColumnsComponent), new PropertyMetadata(null, (s, e) =>
            {
                var model = s as AccountsAdditionalColumnsComponent;
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

        private NotifyCollectionWatcher<RoyaltyServiceWorker.AccountService.AccountDataRecordAdditionalColumn> accountDataRecordAdditionalColumns = new NotifyCollectionWatcher<RoyaltyServiceWorker.AccountService.AccountDataRecordAdditionalColumn>((x,y) => x.Id == y.Id);
        public IReadOnlyNotifyCollection<RoyaltyServiceWorker.AccountService.AccountDataRecordAdditionalColumn> AccountDataRecordAdditionalColumns => accountDataRecordAdditionalColumns;

        public AccountsAdditionalColumnsComponent() { }

        private void OnAccountChanged(RoyaltyServiceWorker.AccountService.Account newValue, RoyaltyServiceWorker.AccountService.Account oldValue)
        {
            accountDataRecordAdditionalColumns.Clear();
            if (this.worker != null)
                this.worker.OnItemsChanged -= Worker_OnItemsChanged;

            InitializeWorker(newValue != null ? new RoyaltyServiceWorker.AccountAdditionalColumnsWorker(newValue.Id) : null);

            if (this.worker != null)
                this.worker.OnItemsChanged += Worker_OnItemsChanged;
        }

        private void Worker_OnItemsChanged(object sender, RoyaltyServiceWorker.Additional.ListItemsEventArgs<RoyaltyServiceWorker.AccountService.AccountDataRecordAdditionalColumn> e)
            => RunUnderDispatcher(() => { accountDataRecordAdditionalColumns.UpdateCollection(e); Change?.Invoke(this, e); });

        private void WorkerApplyHistory(object sender, RoyaltyServiceWorker.HistoryService.History e) => worker?.ApplyHistoryChanges(e);

        public event EventHandler<RoyaltyServiceWorker.Additional.ListItemsEventArgs<RoyaltyServiceWorker.AccountService.AccountDataRecordAdditionalColumn>> Change;
    }
}
