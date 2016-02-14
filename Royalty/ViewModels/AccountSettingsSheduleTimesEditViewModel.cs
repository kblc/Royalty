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
    public class AccountSettingsSheduleTimesEditViewModel : AbstractActionWithBackViewModel
    {
        #region Account

        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register(nameof(Account), typeof(RoyaltyServiceWorker.AccountService.Account),
            typeof(AccountSettingsSheduleTimesEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public RoyaltyServiceWorker.AccountService.Account Account
        {
            get { return (RoyaltyServiceWorker.AccountService.Account)GetValue(AccountProperty); }
            set { SetValue(AccountProperty, value); }
        }

        #endregion
        #region AccountSettings

        public static readonly DependencyProperty AccountSettingsProperty = DependencyProperty.Register(nameof(AccountSettings), typeof(RoyaltyServiceWorker.AccountService.AccountSettings),
            typeof(AccountSettingsSheduleTimesEditViewModel), new PropertyMetadata(null, (s, e) => { (s as AccountSettingsSheduleTimesEditViewModel).OnAccountSettingsChanged(e.NewValue as RoyaltyServiceWorker.AccountService.AccountSettings, e.OldValue as RoyaltyServiceWorker.AccountService.AccountSettings); }));

        public RoyaltyServiceWorker.AccountService.AccountSettings AccountSettings
        {
            get { return (RoyaltyServiceWorker.AccountService.AccountSettings)GetValue(AccountSettingsProperty); }
            set { SetValue(AccountSettingsProperty, value); }
        }

        #endregion
        #region AccountSettingsSheduleTimesEdit

        private static readonly DependencyPropertyKey ReadOnlyAccountSettingsSheduleTimesEditPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(AccountSettingsSheduleTimesEdit), typeof(ICollectionView), typeof(AccountSettingsSheduleTimesEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyAccountSettingsSheduleTimesEditProperty = ReadOnlyAccountSettingsSheduleTimesEditPropertyKey.DependencyProperty;

        public ICollectionView AccountSettingsSheduleTimesEdit
        {
            get { return (ICollectionView)GetValue(ReadOnlyAccountSettingsSheduleTimesEditProperty); }
            private set { SetValue(ReadOnlyAccountSettingsSheduleTimesEditPropertyKey, value); }
        }

        #endregion
        #region SaveCommand

        private static readonly DependencyPropertyKey ReadOnlySaveCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(SaveCommand), typeof(DelegateCommand), typeof(AccountSettingsSheduleTimesEditViewModel),
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

        private List<RoyaltyServiceWorker.AccountService.AccountSettingsSheduleTime> collectionForEdit = null;

        public AccountSettingsSheduleTimesEditViewModel()
        {
            SaveCommand = new DelegateCommand(o => {
                SaveTask(collectionForEdit).ContinueWith((res) => { if (res.Result) Back(); }
                , GetCancellationToken(), TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.FromCurrentSynchronizationContext());
            }, o => !IsBusy);
        }

        private void OnAccountSettingsChanged(RoyaltyServiceWorker.AccountService.AccountSettings newItem, RoyaltyServiceWorker.AccountService.AccountSettings oldItem)
        {
            if (oldItem != null)
            {
                oldItem.PropertyChanged -= AccountSettingsPropertyChanged;
            }
            if (newItem != null)
            {
                collectionForEdit = newItem.SheduleTimes?.ToList() ?? new List<RoyaltyServiceWorker.AccountService.AccountSettingsSheduleTime>();
                AccountSettingsSheduleTimesEdit = CollectionViewSource.GetDefaultView(collectionForEdit);
                newItem.PropertyChanged += AccountSettingsPropertyChanged;
            }
        }

        private void AccountSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AccountSettings.SheduleTimes))
            {
                collectionForEdit = AccountSettings.SheduleTimes?.ToList() ?? new List<RoyaltyServiceWorker.AccountService.AccountSettingsSheduleTime>();
                AccountSettingsSheduleTimesEdit = CollectionViewSource.GetDefaultView(collectionForEdit);
            }
        }

        protected override void BackCommandExecuted(object o)
        {
            base.BackCommandExecuted(o);
            OnAccountSettingsChanged(AccountSettings, AccountSettings);
        }

        private Task<bool> SaveTask(IList<RoyaltyServiceWorker.AccountService.AccountSettingsSheduleTime> newItems)
        {
            IsBusy = true;
            return Task.Factory.StartNew(() => 
            {
                try
                {
                    foreach(var i in newItems)
                        i.AccountUID = AccountSettings.Id;
                    AccountSettings.SheduleTimes = new List<RoyaltyServiceWorker.AccountService.AccountSettingsSheduleTime>(newItems);
                    return true;
                }
                catch (Exception ex)
                {
                    Error = ex.ToString();
                    return false;
                }
                finally
                {
                    IsBusy = false;
                }
            }, GetCancellationToken(), TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        protected override void RaiseCommands()
        {
            base.RaiseCommands();
            SaveCommand?.RaiseCanExecuteChanged();
        }
    }
}
