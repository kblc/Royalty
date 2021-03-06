﻿using Helpers;
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
    public class AccountsSeriesOfNumbersComponent : AbstractComponent<RoyaltyServiceWorker.AccountSeriesOfNumbersWorker>,
        IHistoryItemsChange<RoyaltyServiceWorker.AccountService.AccountSeriesOfNumbersRecord>
    {
        #region Account

        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register(nameof(Account), typeof(RoyaltyServiceWorker.AccountService.Account),
            typeof(AccountsSeriesOfNumbersComponent), new PropertyMetadata(null, (s, e) => { (s as AccountsSeriesOfNumbersComponent)?.OnAccountChanged(e.NewValue as RoyaltyServiceWorker.AccountService.Account, e.OldValue as RoyaltyServiceWorker.AccountService.Account); }));

        public RoyaltyServiceWorker.AccountService.Account Account
        {
            get { return (RoyaltyServiceWorker.AccountService.Account)GetValue(AccountProperty); }
            set { SetValue(AccountProperty, value); }
        }

        #endregion
        #region HistoryComponent

        public static readonly DependencyProperty HistoryComponentProperty = DependencyProperty.Register(nameof(HistoryComponent), typeof(HistoryComponent),
            typeof(AccountsSeriesOfNumbersComponent), new PropertyMetadata(null, (s, e) =>
            {
                var model = s as AccountsSeriesOfNumbersComponent;
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

        private NotifyCollectionWatcher<RoyaltyServiceWorker.AccountService.AccountSeriesOfNumbersRecord> accountSeriesOfNumbersRecords = new NotifyCollectionWatcher<RoyaltyServiceWorker.AccountService.AccountSeriesOfNumbersRecord>((x,y) => x.Id == y.Id);
        public IReadOnlyNotifyCollection<RoyaltyServiceWorker.AccountService.AccountSeriesOfNumbersRecord> AccountSeriesOfNumbersRecords => accountSeriesOfNumbersRecords;

        public AccountsSeriesOfNumbersComponent() { }

        private void OnAccountChanged(RoyaltyServiceWorker.AccountService.Account newValue, RoyaltyServiceWorker.AccountService.Account oldValue)
        {
            accountSeriesOfNumbersRecords.Clear();
            if (this.worker != null)
                this.worker.OnItemsChanged -= Worker_OnItemsChanged;

            InitializeWorker(newValue != null ? new RoyaltyServiceWorker.AccountSeriesOfNumbersWorker(newValue.Id) : null);

            if (this.worker != null)
                this.worker.OnItemsChanged += Worker_OnItemsChanged;
        }

        private void Worker_OnItemsChanged(object sender, RoyaltyServiceWorker.Additional.ListItemsEventArgs<RoyaltyServiceWorker.AccountService.AccountSeriesOfNumbersRecord> e)
            => RunUnderDispatcher(() => { accountSeriesOfNumbersRecords.UpdateCollection(e); Change?.Invoke(this, e); });

        private void WorkerApplyHistory(object sender, RoyaltyServiceWorker.HistoryService.History e) => worker?.ApplyHistoryChanges(e);

        public event EventHandler<RoyaltyServiceWorker.Additional.ListItemsEventArgs<RoyaltyServiceWorker.AccountService.AccountSeriesOfNumbersRecord>> Change;
    }
}
