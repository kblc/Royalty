using Helpers;
using Helpers.WPF;
using Royalty.Additional;
using Royalty.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using RoyaltyServiceWorker.AccountService;
using RoyaltyServiceWorker.Additional;

namespace Royalty.ViewModels
{
    public class AccountSeriesOfNumbersEditViewModel : AbstractActionWithBackViewModel
    {
        #region Account

        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register(nameof(Account), typeof(RoyaltyServiceWorker.AccountService.Account),
            typeof(AccountSeriesOfNumbersEditViewModel), new PropertyMetadata(null, (s, e) => { (s as AccountSeriesOfNumbersEditViewModel)?.OnAccountChanged(e.NewValue as RoyaltyServiceWorker.AccountService.Account, e.OldValue as RoyaltyServiceWorker.AccountService.Account); }));

        public RoyaltyServiceWorker.AccountService.Account Account
        {
            get { return (RoyaltyServiceWorker.AccountService.Account)GetValue(AccountProperty); }
            set { SetValue(AccountProperty, value); }
        }

        #endregion
        #region AccountsSeriesOfNumbersComponent

        public static readonly DependencyProperty AccountsSeriesOfNumbersComponentProperty = DependencyProperty.Register(nameof(AccountsSeriesOfNumbersComponent), typeof(AccountsSeriesOfNumbersComponent),
            typeof(AccountSeriesOfNumbersEditViewModel), new PropertyMetadata(null, (s, e) =>
            {
                var model = s as AccountSeriesOfNumbersEditViewModel;
                var newAccountsComponent = e.NewValue as AccountsSeriesOfNumbersComponent;
                var oldAccountsComponent = e.OldValue as AccountsSeriesOfNumbersComponent;
                if (model != null && newAccountsComponent != oldAccountsComponent)
                {
                    model.UpdateAccountsSeriesOfNumbersComponentSource(newAccountsComponent, oldAccountsComponent);
                }
            }));

        public AccountsSeriesOfNumbersComponent AccountsSeriesOfNumbersComponent
        {
            get { return (AccountsSeriesOfNumbersComponent)GetValue(AccountsSeriesOfNumbersComponentProperty); }
            set { SetValue(AccountsSeriesOfNumbersComponentProperty, value); }
        }

        #endregion
        #region AccountSeriesOfNumbers

        private static readonly DependencyPropertyKey AccountSeriesOfNumbersPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(AccountSeriesOfNumbers), typeof(ICollectionView), typeof(AccountSeriesOfNumbersEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyAccountSeriesOfNumbersPropertyKey = AccountSeriesOfNumbersPropertyKey.DependencyProperty;

        public ICollectionView AccountSeriesOfNumbers
        {
            get { return (ICollectionView)GetValue(ReadOnlyAccountSeriesOfNumbersPropertyKey); }
            private set { SetValue(AccountSeriesOfNumbersPropertyKey, value); }
        }

        #endregion

        #region SelectedValue

        public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register(nameof(SelectedValue), typeof(object),
            typeof(AccountSeriesOfNumbersEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public object SelectedValue
        {
            get { return GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        #endregion

        #region RowEditEndingCommand

        private static readonly DependencyPropertyKey ReadOnlyRowEditEndingCommanddPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(RowEditEndingCommand), typeof(DelegateCommand), typeof(AccountSeriesOfNumbersEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyRowEditEndingCommandProperty = ReadOnlyRowEditEndingCommanddPropertyKey.DependencyProperty;

        public DelegateCommand RowEditEndingCommand
        {
            get { return (DelegateCommand)GetValue(ReadOnlyRowEditEndingCommandProperty); }
            private set { SetValue(ReadOnlyRowEditEndingCommanddPropertyKey, value); }
        }

        #endregion

        protected override void OnIsActiveChanged(bool newValue)
        {
            base.OnIsActiveChanged(newValue);
            localCollection.Clear();
        }

        private ObservableCollectionWatcher<RoyaltyServiceWorker.AccountService.AccountSeriesOfNumbersRecord> localCollection = null;

        public AccountSeriesOfNumbersEditViewModel()
        {
            localCollection = new ObservableCollectionWatcher<AccountSeriesOfNumbersRecord>((x, y) => x.Id == y.Id);
            AccountSeriesOfNumbers = CollectionViewSource.GetDefaultView(localCollection);
            AccountSeriesOfNumbers.CollectionChanged += AccountSeriesOfNumbers_CollectionChanged;
            RowEditEndingCommand = new DelegateCommand(o => RowEditEnding(o as System.Windows.Controls.DataGridRowEditEndingEventArgs));
        }

        private void OnAccountChanged(RoyaltyServiceWorker.AccountService.Account newItem, RoyaltyServiceWorker.AccountService.Account oldItem)
        {
            localCollection.Clear();
        }

        private void UpdateAccountsSeriesOfNumbersComponentSource(AccountsSeriesOfNumbersComponent newItem, AccountsSeriesOfNumbersComponent oldItem)
        {
            if (oldItem != null)
            {
                oldItem.Change -= AccountsSeriesOfNumbersComponent_Change;
                BindingOperations.ClearBinding(oldItem, AccountsSeriesOfNumbersComponent.AccountProperty);
                BindingOperations.ClearBinding(oldItem, AbstractComponent.IsActiveProperty);
                DependencyPropertyDescriptor.FromProperty(AbstractComponent.ReadOnlyIsLoadedProperty, newItem.GetType())
                    .RemoveValueChanged(oldItem, AccountsSeriesOfNumbersComponent_IsLoadedChanged);
            }

            localCollection.Clear();

            if (newItem != null)
            {
                localCollection.AddRange(newItem.AccountSeriesOfNumbersRecords);
                newItem.Change += AccountsSeriesOfNumbersComponent_Change;

                var accountBinding = new Binding() {
                    Source = this,
                    Path = new PropertyPath(AccountProperty.Name),
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };

                var isActiveBinding = new Binding() {
                    Source = this,
                    Path = new PropertyPath(IsActiveProperty.Name),
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };

                BindingOperations.SetBinding(newItem, AccountsSeriesOfNumbersComponent.AccountProperty, accountBinding);
                BindingOperations.SetBinding(newItem, AbstractComponent.IsActiveProperty, isActiveBinding);
                DependencyPropertyDescriptor.FromProperty(AbstractComponent.ReadOnlyIsLoadedProperty, newItem.GetType())
                    .AddValueChanged(newItem, AccountsSeriesOfNumbersComponent_IsLoadedChanged);

                this.IsBusy = !AccountsSeriesOfNumbersComponent.IsLoaded;
            }
        }

        private void AccountsSeriesOfNumbersComponent_Change(object sender, ListItemsEventArgs<AccountSeriesOfNumbersRecord> e)
        {
            localCollection.UpdateCollection(e);
        }

        private void AccountsSeriesOfNumbersComponent_IsLoadedChanged(object sender, EventArgs e)
        {
            this.IsBusy = !AccountsSeriesOfNumbersComponent.IsLoaded;
        }

        private void AccountSeriesOfNumbers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null && IsActive && !localCollection.IsCollectionUpdating)
            {
                var oldItems = e.OldItems
                    .OfType<AccountSeriesOfNumbersRecord>()
                    .Where(i => i.Id != default(long))
                    .ToArray();
                if (oldItems.Length > 0)
                {
                    DeleteTask(oldItems)
                        .ContinueWith(res =>
                        {
                            if (res.Result.Length > 0)
                                localCollection.AddRange(res.Result);
                        }, GetCancellationToken(), TaskContinuationOptions.AttachedToParent, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            #region Set account UID for new Items
            if (e.NewItems != null)
            {
                var newItems = e.NewItems
                    .OfType<AccountSeriesOfNumbersRecord>()
                    .Where(i => i.Id == default(long))
                    .ToArray();
                foreach (var i in newItems)
                    i.AccountUID = this.Account?.Id;
            }
            #endregion
        }

        private void RowEditEnding(System.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            if (SelectedValue == null || e == null)
                return;

            if (e.EditAction == System.Windows.Controls.DataGridEditAction.Commit)
                SaveTask(SelectedValue as RoyaltyServiceWorker.AccountService.AccountSeriesOfNumbersRecord);
        }

        private Task<bool> SaveTask(RoyaltyServiceWorker.AccountService.AccountSeriesOfNumbersRecord item)
        {
            var startTask = Task.Factory.StartNew(() => 
            {
                IsBusy = true;
            }, GetCancellationToken(), TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());

            var clientTask = startTask.ContinueWith((t) => 
            {
                var client = new RoyaltyServiceWorker.AccountService.AccountServiceClient();
                try
                {
                    return item.Id == default(long)
                        ? client.PutSeriesOfNumbers(item)
                        : client.UpdateSeriesOfNumbers(item);
                }
                finally
                {
                    try { client.Close(); } catch { }
                }
            }, GetCancellationToken(), TaskContinuationOptions.AttachedToParent, TaskScheduler.Default);

            var taskRes = clientTask.ContinueWith((res) => 
            {
                try
                {
                    if (res.Result.Error != null)
                        throw new Exception(res.Result.Error);

                    item.CopyObjectFrom(res.Result.Value);
                    
                    return true;
                }
                catch(Exception ex)
                {
                    Error = ex.ToString();
                    return false;
                }
                finally
                {
                    IsBusy = false;
                }
            }, GetCancellationToken(), TaskContinuationOptions.AttachedToParent, TaskScheduler.FromCurrentSynchronizationContext());

            return taskRes;
        }

        private Task<AccountSeriesOfNumbersRecord[]> DeleteTask(AccountSeriesOfNumbersRecord[] items)
        {
            var startTask = Task.Factory.StartNew(() =>
            {
                IsBusy = true;
            }, GetCancellationToken(), TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());

            var clientTask = startTask.ContinueWith((t) =>
            {
                var client = new RoyaltyServiceWorker.AccountService.AccountServiceClient();
                try
                {
                    return client.RemoveSeriesOfNumbersRange(items.Select(i => i.Id).ToList());
                }
                finally
                {
                    try { client.Close(); } catch { }
                }
            }, GetCancellationToken(), TaskContinuationOptions.AttachedToParent, TaskScheduler.Default);

            var taskRes = clientTask.ContinueWith((res) =>
            {
                try
                {
                    if (res.Result.Error != null)
                        throw new Exception(res.Result.Error);

                    return items
                        .Where(i => !res.Result.Values.Contains(i.Id))
                        .ToArray();
                }
                catch (Exception ex)
                {
                    Error = ex.ToString();
                    return items;
                }
                finally
                {
                    IsBusy = false;
                }
            }, GetCancellationToken(), TaskContinuationOptions.AttachedToParent, TaskScheduler.FromCurrentSynchronizationContext());

            return taskRes;
        }

        protected override void RaiseCommands()
        {
            base.RaiseCommands();
            RowEditEndingCommand?.RaiseCanExecuteChanged();
        }
    }
}
