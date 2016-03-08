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
    public class AccountAdditionalColumnsEditViewModel : AbstractActionWithBackViewModel
    {
        #region Account

        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register(nameof(Account), typeof(RoyaltyServiceWorker.AccountService.Account),
            typeof(AccountAdditionalColumnsEditViewModel), new PropertyMetadata(null, (s, e) => { (s as AccountAdditionalColumnsEditViewModel)?.OnAccountChanged(e.NewValue as RoyaltyServiceWorker.AccountService.Account, e.OldValue as RoyaltyServiceWorker.AccountService.Account); }));

        public RoyaltyServiceWorker.AccountService.Account Account
        {
            get { return (RoyaltyServiceWorker.AccountService.Account)GetValue(AccountProperty); }
            set { SetValue(AccountProperty, value); }
        }

        #endregion
        #region AccountsAdditionalColumnsComponent

        public static readonly DependencyProperty AccountsAdditionalColumnsComponentProperty = DependencyProperty.Register(nameof(AccountsAdditionalColumnsComponent), typeof(AccountsAdditionalColumnsComponent),
            typeof(AccountAdditionalColumnsEditViewModel), new PropertyMetadata(null, (s, e) =>
            {
                var model = s as AccountAdditionalColumnsEditViewModel;
                var newAccountsComponent = e.NewValue as AccountsAdditionalColumnsComponent;
                var oldAccountsComponent = e.OldValue as AccountsAdditionalColumnsComponent;
                if (model != null && newAccountsComponent != oldAccountsComponent)
                {
                    model.UpdateAccountsAdditionalColumnsComponentSource(newAccountsComponent, oldAccountsComponent);
                }
            }));

        public AccountsAdditionalColumnsComponent AccountsAdditionalColumnsComponent
        {
            get { return (AccountsAdditionalColumnsComponent)GetValue(AccountsAdditionalColumnsComponentProperty); }
            set { SetValue(AccountsAdditionalColumnsComponentProperty, value); }
        }

        #endregion
        #region AccountAdditionalColumns

        private static readonly DependencyPropertyKey AccountAccountAdditionalColumnsPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(AccountAdditionalColumns), typeof(ICollectionView), typeof(AccountAdditionalColumnsEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyAccountAdditionalColumnsPropertyKey = AccountAccountAdditionalColumnsPropertyKey.DependencyProperty;

        public ICollectionView AccountAdditionalColumns
        {
            get { return (ICollectionView)GetValue(ReadOnlyAccountAdditionalColumnsPropertyKey); }
            private set { SetValue(AccountAccountAdditionalColumnsPropertyKey, value); }
        }

        #endregion

        #region SelectedValue

        public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register(nameof(SelectedValue), typeof(object),
            typeof(AccountAdditionalColumnsEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public object SelectedValue
        {
            get { return GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        #endregion

        #region RowEditEndingCommand

        private static readonly DependencyPropertyKey ReadOnlyRowEditEndingCommanddPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(RowEditEndingCommand), typeof(DelegateCommand), typeof(AccountAdditionalColumnsEditViewModel),
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

        private ObservableCollectionWatcher<RoyaltyServiceWorker.AccountService.AccountDataRecordAdditionalColumn> localCollection = null;

        public AccountAdditionalColumnsEditViewModel()
        {
            localCollection = new ObservableCollectionWatcher<AccountDataRecordAdditionalColumn>((x, y) => x.Id == y.Id);
            AccountAdditionalColumns = CollectionViewSource.GetDefaultView(localCollection);
            AccountAdditionalColumns.CollectionChanged += AccountAdditionalColumns_CollectionChanged;
            RowEditEndingCommand = new DelegateCommand(o => RowEditEnding(o as System.Windows.Controls.DataGridRowEditEndingEventArgs));
        }

        private void OnAccountChanged(RoyaltyServiceWorker.AccountService.Account newItem, RoyaltyServiceWorker.AccountService.Account oldItem)
         => localCollection.Clear();

        private void UpdateAccountsAdditionalColumnsComponentSource(AccountsAdditionalColumnsComponent newItem, AccountsAdditionalColumnsComponent oldItem)
        {
            if (oldItem != null)
            {
                oldItem.Change -= AccountsAdditionalColumnsComponent_Change;
                BindingOperations.ClearBinding(oldItem, AccountsAdditionalColumnsComponent.AccountProperty);
                BindingOperations.ClearBinding(oldItem, AbstractComponent.IsActiveProperty);
                DependencyPropertyDescriptor.FromProperty(AbstractComponent.ReadOnlyIsLoadedProperty, newItem.GetType())
                    .RemoveValueChanged(oldItem, AccountsAdditionalColumnsComponent_IsLoadedChanged);
            }

            localCollection.Clear();

            if (newItem != null)
            {
                localCollection.AddRange(newItem.AccountDataRecordAdditionalColumns);
                newItem.Change += AccountsAdditionalColumnsComponent_Change;

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

                BindingOperations.SetBinding(newItem, AccountsAdditionalColumnsComponent.AccountProperty, accountBinding);
                BindingOperations.SetBinding(newItem, AbstractComponent.IsActiveProperty, isActiveBinding);
                DependencyPropertyDescriptor.FromProperty(AbstractComponent.ReadOnlyIsLoadedProperty, newItem.GetType())
                    .AddValueChanged(newItem, AccountsAdditionalColumnsComponent_IsLoadedChanged);

                this.IsBusy = !AccountsAdditionalColumnsComponent.IsLoaded;
            }
        }

        private void AccountsAdditionalColumnsComponent_Change(object sender, ListItemsEventArgs<AccountDataRecordAdditionalColumn> e)
            => localCollection.UpdateCollection(e);

        private void AccountsAdditionalColumnsComponent_IsLoadedChanged(object sender, EventArgs e)
            => this.IsBusy = !AccountsAdditionalColumnsComponent.IsLoaded;
        
        private void AccountAdditionalColumns_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null && IsActive && !localCollection.IsCollectionUpdating)
            {
                var oldItems = e.OldItems
                    .OfType<AccountDataRecordAdditionalColumn>()
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
                    .OfType<AccountDataRecordAdditionalColumn>()
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
                SaveTask(SelectedValue as RoyaltyServiceWorker.AccountService.AccountDataRecordAdditionalColumn);
        }

        private Task<bool> SaveTask(RoyaltyServiceWorker.AccountService.AccountDataRecordAdditionalColumn item)
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
                        ? client.PutAdditionalColumn(item)
                        : client.UpdateAdditionalColumn(item);
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

        private Task<AccountDataRecordAdditionalColumn[]> DeleteTask(AccountDataRecordAdditionalColumn[] items)
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
                    return client.RemoveAdditionalColumnsRange(items.Select(i => i.Id).ToList());
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
