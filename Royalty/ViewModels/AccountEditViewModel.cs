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
    public enum AccountEditViewModelView
    {
        Account,
        Settings
    }

    public class AccountEditViewModel : AbstractActionViewModel
    {
        #region Account

        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register(nameof(Account), typeof(RoyaltyServiceWorker.AccountService.Account),
            typeof(AccountEditViewModel), new PropertyMetadata(null, (s, e) =>
            {
                var model = s as AccountEditViewModel;
                if (model != null)
                {
                    var newItem = e.NewValue as RoyaltyServiceWorker.AccountService.Account;
                    var oldItem = e.OldValue as RoyaltyServiceWorker.AccountService.Account;
                    if (newItem != oldItem)
                        model.UpdateAccount(newItem, oldItem);
                }
            }));

        public RoyaltyServiceWorker.AccountService.Account Account
        {
            get { return (RoyaltyServiceWorker.AccountService.Account)GetValue(AccountProperty); }
            set { SetValue(AccountProperty, value); }
        }

        #endregion
        #region BackCommand

        public static readonly DependencyProperty BackCommandProperty = DependencyProperty.Register(nameof(BackCommand), typeof(ICommand),
            typeof(AccountEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public ICommand BackCommand
        {
            get { return (ICommand)GetValue(BackCommandProperty); }
            set { SetValue(BackCommandProperty, value); }
        }

        #endregion
        #region BackCommandParameterProperty

        public static readonly DependencyProperty BackCommandParameterProperty = DependencyProperty.Register(nameof(BackCommandParameter), typeof(object),
            typeof(AccountEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public object BackCommandParameter
        {
            get { return GetValue(BackCommandParameterProperty); }
            set { SetValue(BackCommandParameterProperty, value); }
        }

        #endregion
        #region AccountEdit

        private static readonly DependencyPropertyKey ReadOnlyAccountEditPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(AccountEdit), typeof(RoyaltyServiceWorker.AccountService.Account), typeof(AccountEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyAccountEditProperty = ReadOnlyAccountEditPropertyKey.DependencyProperty;

        public RoyaltyServiceWorker.AccountService.Account AccountEdit
        {
            get { return (RoyaltyServiceWorker.AccountService.Account)GetValue(ReadOnlyAccountEditProperty); }
            private set { SetValue(ReadOnlyAccountEditPropertyKey, value); }
        }

        #endregion
        #region SaveCommand

        private static readonly DependencyPropertyKey ReadOnlySaveCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(SaveCommand), typeof(DelegateCommand), typeof(AccountEditViewModel),
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
            = DependencyProperty.RegisterReadOnly(nameof(DeleteCommand), typeof(DelegateCommand), typeof(AccountEditViewModel),
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
        #region SetSettingsViewCommand

        private static readonly DependencyPropertyKey ReadOnlySetSettingsViewCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(SetSettingsViewCommand), typeof(DelegateCommand), typeof(AccountEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlySetSettingsViewCommandProperty = ReadOnlySetSettingsViewCommandPropertyKey.DependencyProperty;

        public DelegateCommand SetSettingsViewCommand
        {
            get { return (DelegateCommand)GetValue(ReadOnlySetSettingsViewCommandProperty); }
            private set { SetValue(ReadOnlySetSettingsViewCommandPropertyKey, value); }
        }

        #endregion
        #region SetAccountViewCommand

        private static readonly DependencyPropertyKey ReadOnlySetAccountViewCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(SetAccountViewCommand), typeof(DelegateCommand), typeof(AccountEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlySetAccountViewCommandProperty = ReadOnlySetAccountViewCommandPropertyKey.DependencyProperty;

        public DelegateCommand SetAccountViewCommand
        {
            get { return (DelegateCommand)GetValue(ReadOnlySetAccountViewCommandProperty); }
            private set { SetValue(ReadOnlySetAccountViewCommandPropertyKey, value); }
        }

        #endregion
        #region View

        private static readonly DependencyPropertyKey ReadOnlyViewPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(View), typeof(AccountEditViewModelView), typeof(AccountEditViewModel),
                new FrameworkPropertyMetadata(AccountEditViewModelView.Account,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyViewProperty = ReadOnlyViewPropertyKey.DependencyProperty;

        public AccountEditViewModelView View
        {
            get { return (AccountEditViewModelView)GetValue(ReadOnlyViewProperty); }
            private set { SetValue(ReadOnlyViewPropertyKey, value); }
        }

        #endregion
        #region BackEvent

        public static readonly RoutedEvent BackEvent = EventManager.RegisterRoutedEvent(nameof(Back), RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(AccountEditViewModel));

        public event RoutedEventHandler Back
        {
            add { AddHandler(BackEvent, value); }
            remove { RemoveHandler(BackEvent, value); }
        }

        #endregion

        public AccountEditViewModel()
        {
            SaveCommand = new DelegateCommand(o => {
                SaveTask(AccountEdit).ContinueWith((res) => 
                {
                    if (res.Result)
                        BackCommand?.Execute(BackCommandParameter);
                }, GetCancellationToken(), TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.FromCurrentSynchronizationContext());
            }, o => !IsBusy);
            DeleteCommand = new DelegateCommand(o => {
                DeleteTask(AccountEdit).ContinueWith((res) =>
                {
                    if (res.Result)
                        BackCommand?.Execute(BackCommandParameter);
                }, GetCancellationToken(), TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.FromCurrentSynchronizationContext());
            }, o => !IsBusy && (Account?.Id ?? Guid.Empty) != Guid.Empty);
            SetAccountViewCommand = new DelegateCommand(o => View = AccountEditViewModelView.Account);
            SetSettingsViewCommand = new DelegateCommand(o => View = AccountEditViewModelView.Settings, o => !IsBusy && (Account?.Id ?? Guid.Empty) != Guid.Empty);
        }

        private void UpdateAccount(RoyaltyServiceWorker.AccountService.Account newItem, RoyaltyServiceWorker.AccountService.Account oldItem)
        {
            IsBusy = false;
            Error = null;
            RaiseCommands();
            AccountEdit = new RoyaltyServiceWorker.AccountService.Account();
            if (newItem != null)
            {
                AccountEdit.CopyObjectFrom(newItem);
                newItem.PropertyChanged += AccountPropertyChanged;
            }
            if (oldItem != null)
            {
                oldItem.PropertyChanged -= AccountPropertyChanged;
            }
        }

        private void AccountPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            AccountEdit.CopyObjectFrom((RoyaltyServiceWorker.AccountService.Account)sender);
        }

        private Task<bool> SaveTask(RoyaltyServiceWorker.AccountService.Account item)
        {
            IsBusy = true;

            var client = new RoyaltyServiceWorker.AccountService.AccountServiceClient();
            var task = Account.Id == Guid.Empty
                ? client.PutAsync(item)
                : client.UpdateAsync(item);

            var taskRes = task.ContinueWith((res) => 
            {
                try
                {
                    if (res.Result.Error != null)
                        throw new Exception(res.Result.Error);

                    if (Account != null)
                    {
                        Account.CopyObjectFrom(res.Result.Value);
                    } else
                    {
                        Account = res.Result.Value;
                    }
                    return true;
                }
                catch(Exception ex)
                {
                    Error = ex.ToString();
                    return false;
                }
                finally
                {
                    try { client.Close(); } catch { }
                    IsBusy = false;
                }
            }, GetCancellationToken(), TaskContinuationOptions.AttachedToParent, TaskScheduler.FromCurrentSynchronizationContext());

            return taskRes;
        }

        private Task<bool> DeleteTask(RoyaltyServiceWorker.AccountService.Account item)
        {
            IsBusy = true;

            var client = new RoyaltyServiceWorker.AccountService.AccountServiceClient();
            var task = client.RemoveAsync(item.Id);
            var taskRes = task.ContinueWith((res) =>
            {
                try
                {
                    if (res.Result.Error != null)
                        throw new Exception(res.Result.Error);
                    return true;
                }
                catch (Exception ex)
                {
                    Error = ex.ToString();
                    return false;
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
            SaveCommand?.RaiseCanExecuteChanged();
            DeleteCommand?.RaiseCanExecuteChanged();
            SetAccountViewCommand?.RaiseCanExecuteChanged();
            SetSettingsViewCommand?.RaiseCanExecuteChanged();
        }
    }
}
