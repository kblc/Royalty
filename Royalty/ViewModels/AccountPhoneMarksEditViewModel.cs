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
    public class AccountPhoneMarksEditViewModel : AbstractActionWithBackViewModel
    {
        #region Account

        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register(nameof(Account), typeof(RoyaltyServiceWorker.AccountService.Account),
            typeof(AccountPhoneMarksEditViewModel), new PropertyMetadata(null, (s, e) => { (s as AccountPhoneMarksEditViewModel)?.OnAccountChanged(e.NewValue as RoyaltyServiceWorker.AccountService.Account, e.OldValue as RoyaltyServiceWorker.AccountService.Account); }));

        public RoyaltyServiceWorker.AccountService.Account Account
        {
            get { return (RoyaltyServiceWorker.AccountService.Account)GetValue(AccountProperty); }
            set { SetValue(AccountProperty, value); }
        }

        #endregion
        #region Marks

        public static readonly DependencyProperty MarksProperty = DependencyProperty.Register(nameof(Marks), typeof(ICollectionView),
            typeof(AccountPhoneMarksEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public ICollectionView Marks
        {
            get { return (ICollectionView)GetValue(MarksProperty); }
            set { SetValue(MarksProperty, value); }
        }

        #endregion

        #region AccountsPhoneMarksComponent

        public static readonly DependencyProperty AccountsPhoneMarksComponentProperty = DependencyProperty.Register(nameof(AccountsPhoneMarksComponent), typeof(AccountsPhoneMarksComponent),
            typeof(AccountPhoneMarksEditViewModel), new PropertyMetadata(null, (s, e) =>
            {
                var model = s as AccountPhoneMarksEditViewModel;
                var newAccountsComponent = e.NewValue as AccountsPhoneMarksComponent;
                var oldAccountsComponent = e.OldValue as AccountsPhoneMarksComponent;
                if (model != null && newAccountsComponent != oldAccountsComponent)
                {
                    model.UpdateAccountsPhoneMarksComponentSource(newAccountsComponent, oldAccountsComponent);
                }
            }));

        public AccountsPhoneMarksComponent AccountsPhoneMarksComponent
        {
            get { return (AccountsPhoneMarksComponent)GetValue(AccountsPhoneMarksComponentProperty); }
            set { SetValue(AccountsPhoneMarksComponentProperty, value); }
        }

        #endregion
        #region AccountPhoneMarks

        private static readonly DependencyPropertyKey AccountPhoneMarksPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(AccountPhoneMarks), typeof(ICollectionView), typeof(AccountPhoneMarksEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyAccountPhoneMarksPropertyKey = AccountPhoneMarksPropertyKey.DependencyProperty;

        public ICollectionView AccountPhoneMarks
        {
            get { return (ICollectionView)GetValue(ReadOnlyAccountPhoneMarksPropertyKey); }
            private set { SetValue(AccountPhoneMarksPropertyKey, value); }
        }

        #endregion

        #region SelectedValue

        public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register(nameof(SelectedValue), typeof(object),
            typeof(AccountPhoneMarksEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public object SelectedValue
        {
            get { return GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        #endregion
        #region Filter

        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(nameof(Filter), typeof(string),
            typeof(AccountPhoneMarksEditViewModel), new PropertyMetadata(string.Empty, (s, e) => { }));

        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        #endregion

        #region RowEditEndingCommand

        private static readonly DependencyPropertyKey ReadOnlyRowEditEndingCommanddPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(RowEditEndingCommand), typeof(DelegateCommand), typeof(AccountPhoneMarksEditViewModel),
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

        private ObservableCollectionWatcher<RoyaltyServiceWorker.AccountService.AccountPhoneMark> localCollection = null;

        public AccountPhoneMarksEditViewModel()
        {
            localCollection = new ObservableCollectionWatcher<AccountPhoneMark>((x, y) => x.Id == y.Id);
            AccountPhoneMarks = CollectionViewSource.GetDefaultView(localCollection);
            AccountPhoneMarks.CollectionChanged += AccountPhoneMarks_CollectionChanged;
            RowEditEndingCommand = new DelegateCommand(o => RowEditEnding(o as System.Windows.Controls.DataGridRowEditEndingEventArgs));
        }

        private void OnAccountChanged(RoyaltyServiceWorker.AccountService.Account newItem, RoyaltyServiceWorker.AccountService.Account oldItem)
        {
            localCollection.Clear();
        }

        private void UpdateAccountsPhoneMarksComponentSource(AccountsPhoneMarksComponent newItem, AccountsPhoneMarksComponent oldItem)
        {
            if (oldItem != null)
            {
                oldItem.Change -= AccountsPhoneMarksComponent_Change;
                BindingOperations.ClearBinding(oldItem, AccountProperty);
                BindingOperations.ClearBinding(oldItem, AbstractComponent.IsActiveProperty);
                DependencyPropertyDescriptor.FromProperty(AbstractComponent.ReadOnlyIsLoadedProperty, newItem.GetType())
                    .RemoveValueChanged(oldItem, AccountsPhoneMarksComponent_IsLoadedChanged);
            }

            localCollection.Clear();

            if (newItem != null)
            {
                localCollection.AddRange(newItem.AccountPhoneMarks);
                newItem.Change += AccountsPhoneMarksComponent_Change;

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

                BindingOperations.SetBinding(newItem, AccountProperty, accountBinding);
                BindingOperations.SetBinding(newItem, AbstractComponent.IsActiveProperty, isActiveBinding);
                DependencyPropertyDescriptor.FromProperty(AbstractComponent.ReadOnlyIsLoadedProperty, newItem.GetType())
                    .AddValueChanged(newItem, AccountsPhoneMarksComponent_IsLoadedChanged);

                this.IsBusy = !AccountsPhoneMarksComponent.IsLoaded;
            }
        }

        private void AccountsPhoneMarksComponent_Change(object sender, ListItemsEventArgs<AccountPhoneMark> e)
        {
            localCollection.UpdateCollection(e);
        }

        private void AccountsPhoneMarksComponent_IsLoadedChanged(object sender, EventArgs e)
        {
            this.IsBusy = !AccountsPhoneMarksComponent.IsLoaded;
        }

        private void AccountPhoneMarks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null && IsActive)
            {
                var oldItems = e.OldItems
                    .OfType<AccountPhoneMark>()
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
                    .OfType<AccountPhoneMark>()
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
                SaveTask(SelectedValue as RoyaltyServiceWorker.AccountService.AccountPhoneMark);
        }

        private Task<bool> SaveTask(RoyaltyServiceWorker.AccountService.AccountPhoneMark item)
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
                        ? client.PutAccountPhoneMark(item)
                        : client.UpdateAccountPhoneMark(item);
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

        private Task<AccountPhoneMark[]> DeleteTask(AccountPhoneMark[] items)
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
                    return client.RemoveAccountPhoneMarkRange(items.Select(i => i.Id).ToList());
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
