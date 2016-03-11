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
    public class AccountsComponent : AbstractComponent<RoyaltyServiceWorker.AccountWorker>
    {
        #region HistoryComponent

        public static readonly DependencyProperty HistoryComponentProperty = DependencyProperty.Register(nameof(HistoryComponent), typeof(HistoryComponent),
            typeof(AccountsComponent), new PropertyMetadata(null, (s, e) =>
            {
                var model = s as AccountsComponent;
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

        private NotifyCollection<RoyaltyServiceWorker.AccountService.Account> accounts = new NotifyCollection<RoyaltyServiceWorker.AccountService.Account>();
        public IReadOnlyNotifyCollection<RoyaltyServiceWorker.AccountService.Account> Accounts => accounts;

        private NotifyCollection<RoyaltyServiceWorker.AccountService.Mark> marks = new NotifyCollection<RoyaltyServiceWorker.AccountService.Mark>();
        public IReadOnlyNotifyCollection<RoyaltyServiceWorker.AccountService.Mark> Marks => marks;

        private NotifyCollection<RoyaltyServiceWorker.AccountService.ColumnType> columnTypes = new NotifyCollection<RoyaltyServiceWorker.AccountService.ColumnType>();
        public IReadOnlyNotifyCollection<RoyaltyServiceWorker.AccountService.ColumnType> ColumnTypes => columnTypes;

        private NotifyCollection<RoyaltyServiceWorker.AccountService.ImportQueueRecordState> importQueueRecordStates = new NotifyCollection<RoyaltyServiceWorker.AccountService.ImportQueueRecordState>();
        public IReadOnlyNotifyCollection<RoyaltyServiceWorker.AccountService.ImportQueueRecordState> ImportQueueRecordStates => importQueueRecordStates;

        public AccountsComponent() : base(new RoyaltyServiceWorker.AccountWorker())
        {
            accounts.Clear();
            var accountItems = worker.GetAccounts();
            foreach(var item in accountItems)
                accounts.Add(item);
            marks.Clear();
            var markItems = worker.GetMarks();
            foreach (var item in markItems)
                marks.Add(item);
            columnTypes.Clear();
            var columnTypeItems = worker.GetColumnTypes();
            foreach (var item in columnTypeItems)
                columnTypes.Add(item);
            marks.Clear();
            var importQueueRecordStatesItems = worker.GetImportQueueRecordStates();
            foreach (var item in importQueueRecordStatesItems)
                importQueueRecordStates.Add(item);

            worker.OnAccountsChanged += (_,e) => RunUnderDispatcher(() => WorkerOnAccountsChanged(e));
            worker.OnMarksChanged += (_, e) => RunUnderDispatcher(() => WorkerOnMarksChanged(e));
            worker.OnColumnTypesChanged += (_, e) => RunUnderDispatcher(() => WorkerOnColumnTypesChanged(e));
            worker.OnImportQueueRecordStatesChanged += (_, e) => RunUnderDispatcher(() => WorkerOnImportQueueRecordStatesChanged(e));
        }

        private void WorkerApplyHistory(object sender, RoyaltyServiceWorker.HistoryService.History e) => worker.ApplyHistoryChanges(e);

        private void WorkerOnAccountsChanged(RoyaltyServiceWorker.Additional.ListItemsEventArgs<RoyaltyServiceWorker.AccountService.Account> e)
        {
            if (e.Action == RoyaltyServiceWorker.Additional.ChangeAction.Add)
                e.Items.ToList().ForEach(i => accounts.Add(i));
                        
            if (e.Action == RoyaltyServiceWorker.Additional.ChangeAction.Change)
                e.Items.ToList().ForEach(i => accounts.FirstOrDefault(a => a.Id == i.Id)?.CopyObjectFrom(i));

            if (e.Action == RoyaltyServiceWorker.Additional.ChangeAction.Remove)
                e.Items.ToList().ForEach(i => accounts.Remove(accounts.FirstOrDefault(a => a.Id == i.Id)));
        }

        private void WorkerOnMarksChanged(RoyaltyServiceWorker.Additional.ListItemsEventArgs<RoyaltyServiceWorker.AccountService.Mark> e)
        {
            if (e.Action == RoyaltyServiceWorker.Additional.ChangeAction.Add)
                e.Items.ToList().ForEach(i => marks.Add(i));

            if (e.Action == RoyaltyServiceWorker.Additional.ChangeAction.Change)
                e.Items.ToList().ForEach(i => marks.FirstOrDefault(a => a.Id == i.Id).CopyObjectFrom(i));

            if (e.Action == RoyaltyServiceWorker.Additional.ChangeAction.Remove)
                e.Items.ToList().ForEach(i => marks.Remove(marks.FirstOrDefault(a => a.Id == i.Id)));
        }

        private void WorkerOnColumnTypesChanged(RoyaltyServiceWorker.Additional.ListItemsEventArgs<RoyaltyServiceWorker.AccountService.ColumnType> e)
        {
            if (e.Action == RoyaltyServiceWorker.Additional.ChangeAction.Add)
                e.Items.ToList().ForEach(i => columnTypes.Add(i));

            if (e.Action == RoyaltyServiceWorker.Additional.ChangeAction.Change)
                e.Items.ToList().ForEach(i => columnTypes.FirstOrDefault(a => a.Id == i.Id).CopyObjectFrom(i));

            if (e.Action == RoyaltyServiceWorker.Additional.ChangeAction.Remove)
                e.Items.ToList().ForEach(i => columnTypes.Remove(columnTypes.FirstOrDefault(a => a.Id == i.Id)));
        }

        private void WorkerOnImportQueueRecordStatesChanged(RoyaltyServiceWorker.Additional.ListItemsEventArgs<RoyaltyServiceWorker.AccountService.ImportQueueRecordState> e)
        {
            if (e.Action == RoyaltyServiceWorker.Additional.ChangeAction.Add)
                e.Items.ToList().ForEach(i => importQueueRecordStates.Add(i));

            if (e.Action == RoyaltyServiceWorker.Additional.ChangeAction.Change)
                e.Items.ToList().ForEach(i => importQueueRecordStates.FirstOrDefault(a => a.Id == i.Id).CopyObjectFrom(i));

            if (e.Action == RoyaltyServiceWorker.Additional.ChangeAction.Remove)
                e.Items.ToList().ForEach(i => importQueueRecordStates.Remove(importQueueRecordStates.FirstOrDefault(a => a.Id == i.Id)));
        }
    }
}
