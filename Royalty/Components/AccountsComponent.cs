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

        public AccountsComponent() : base(new RoyaltyServiceWorker.AccountWorker())
        {
            var items = worker.GetAccounts();
            foreach(var item in items)
                accounts.Add(item);
            worker.OnAccountsChanged += (_,e) => RunUnderDispatcher(() => WorkerOnAccountsChanged(e));
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
    }
}
