using Helpers;
using Helpers.WPF;
using Royalty.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

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

        #region SaveCommand

        private static readonly DependencyPropertyKey ReadOnlySaveCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(SaveCommand), typeof(DelegateCommand), typeof(AccountSeriesOfNumbersEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlySaveCommandProperty = ReadOnlySaveCommandPropertyKey.DependencyProperty;

        public DelegateCommand SaveCommand
        {
            get { return (DelegateCommand)GetValue(ReadOnlySaveCommandProperty); }
            private set { SetValue(ReadOnlySaveCommandPropertyKey, value); }
        }

        #endregion
        #region DeleteCommand

        private static readonly DependencyPropertyKey ReadOnlyDeleteCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(DeleteCommand), typeof(DelegateCommand), typeof(AccountSeriesOfNumbersEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyDeleteCommandProperty = ReadOnlyDeleteCommandPropertyKey.DependencyProperty;

        public DelegateCommand DeleteCommand
        {
            get { return (DelegateCommand)GetValue(ReadOnlyDeleteCommandProperty); }
            private set { SetValue(ReadOnlyDeleteCommandPropertyKey, value); }
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

        public AccountSeriesOfNumbersEditViewModel()
        {
            RowEditEndingCommand = new DelegateCommand(o => RowEditEnding(o as System.Windows.Controls.DataGridRowEditEndingEventArgs));
        }

        private void OnAccountChanged(RoyaltyServiceWorker.AccountService.Account newItem, RoyaltyServiceWorker.AccountService.Account oldItem) { }

        private void UpdateAccountsSeriesOfNumbersComponentSource(AccountsSeriesOfNumbersComponent newItem, AccountsSeriesOfNumbersComponent oldItem)
        {
            if (oldItem != null)
            {
                BindingOperations.ClearBinding(oldItem, AccountsSeriesOfNumbersComponent.AccountProperty);
                BindingOperations.ClearBinding(oldItem, AccountsSeriesOfNumbersComponent.IsActiveProperty);
                DependencyPropertyDescriptor.FromProperty(AbstractComponent.ReadOnlyIsLoadedProperty, newItem.GetType())
                    .RemoveValueChanged(oldItem, AccountsSeriesOfNumbersComponent_IsLoadedChanged);
            }

            if (AccountSeriesOfNumbers != null)
            {
                AccountSeriesOfNumbers.CollectionChanged -= AccountSeriesOfNumbers_CollectionChanged;
                AccountSeriesOfNumbers = null;
            }

            if (newItem != null)
            {
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

                AccountSeriesOfNumbers = CollectionViewSource.GetDefaultView(newItem.AccountSeriesOfNumbersRecords);
                AccountSeriesOfNumbers.CollectionChanged += AccountSeriesOfNumbers_CollectionChanged;

                this.IsBusy = !AccountsSeriesOfNumbersComponent.IsLoaded;
            }
        }

        private void AccountsSeriesOfNumbersComponent_IsLoadedChanged(object sender, EventArgs e)
        {
            this.IsBusy = !AccountsSeriesOfNumbersComponent.IsLoaded;
        }

        private void AccountSeriesOfNumbers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                var oldItems = e.OldItems
                    .OfType<RoyaltyServiceWorker.AccountService.AccountSeriesOfNumbersRecord>()
                    .Where(i => i.Id != default(long))
                    .ToArray();
                if (oldItems.Length > 0)
                {
                    DeleteTask(oldItems)
                        .ContinueWith(res => 
                        {
                            if (res.Result.Length > 0)
                            {
                                //AccountsSeriesOfNumbersComponent.AccountSeriesOfNumbersRecords
                            }

                        }, GetCancellationToken(), TaskContinuationOptions.AttachedToParent, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            if (e.NewItems != null)
            {
                var newItems = e.NewItems
                    .OfType<RoyaltyServiceWorker.AccountService.AccountSeriesOfNumbersRecord>()
                    .Where(i => i.Id == default(long))
                    .ToArray();
                if (newItems.Length > 0)
                {
                    foreach (var i in newItems)
                    {
                        i.AccountUID = this.Account?.Id;
                    }
                    //try to insert items (and delete it on error)
                }
            }
        }

        private void RowEditEnding(System.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            if (SelectedValue == null || e == null)
                return;

            if (e.EditAction == System.Windows.Controls.DataGridEditAction.Commit)
            {
                SaveTask(SelectedValue as RoyaltyServiceWorker.AccountService.AccountSeriesOfNumbersRecord);
            }
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

        private Task<RoyaltyServiceWorker.AccountService.AccountSeriesOfNumbersRecord[]> DeleteTask(RoyaltyServiceWorker.AccountService.AccountSeriesOfNumbersRecord[] items)
        {
            IsBusy = true;

            var client = new RoyaltyServiceWorker.AccountService.AccountServiceClient();
            var task = client.RemoveSeriesOfNumbersRangeAsync(items.Select(i => i.Id).ToList());
            var taskRes = task.ContinueWith((res) =>
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
                    try { client.Close(); } catch { }
                    IsBusy = false;
                }
            }, GetCancellationToken(), TaskContinuationOptions.AttachedToParent, TaskScheduler.FromCurrentSynchronizationContext());

            return taskRes;
        }

        protected override void RaiseCommands()
        {
            base.RaiseCommands();
            SaveCommand?.RaiseCanExecuteChanged();
            DeleteCommand?.RaiseCanExecuteChanged();
        }
    }
}
