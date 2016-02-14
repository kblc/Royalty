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
    public class AccountSettingsImportDirectoriesEditViewModel : AbstractActionWithBackViewModel
    {
        #region Account

        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register(nameof(Account), typeof(RoyaltyServiceWorker.AccountService.Account),
            typeof(AccountSettingsImportDirectoriesEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public RoyaltyServiceWorker.AccountService.Account Account
        {
            get { return (RoyaltyServiceWorker.AccountService.Account)GetValue(AccountProperty); }
            set { SetValue(AccountProperty, value); }
        }

        #endregion
        #region AccountSettings

        public static readonly DependencyProperty AccountSettingsProperty = DependencyProperty.Register(nameof(AccountSettings), typeof(RoyaltyServiceWorker.AccountService.AccountSettings),
            typeof(AccountSettingsImportDirectoriesEditViewModel), new PropertyMetadata(null, (s, e) => { (s as AccountSettingsImportDirectoriesEditViewModel).OnAccountSettingsChanged(e.NewValue as RoyaltyServiceWorker.AccountService.AccountSettings, e.OldValue as RoyaltyServiceWorker.AccountService.AccountSettings); }));

        public RoyaltyServiceWorker.AccountService.AccountSettings AccountSettings
        {
            get { return (RoyaltyServiceWorker.AccountService.AccountSettings)GetValue(AccountSettingsProperty); }
            set { SetValue(AccountSettingsProperty, value); }
        }

        #endregion
        #region AccountSettingsImportDirectoriesEdit

        private static readonly DependencyPropertyKey ReadOnlyAccountSettingsImportDirectoriesEditPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(AccountSettingsImportDirectoriesEdit), typeof(ICollectionView), typeof(AccountSettingsImportDirectoriesEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyAccountSettingsImportDirectoriesEditProperty = ReadOnlyAccountSettingsImportDirectoriesEditPropertyKey.DependencyProperty;

        public ICollectionView AccountSettingsImportDirectoriesEdit
        {
            get { return (ICollectionView)GetValue(ReadOnlyAccountSettingsImportDirectoriesEditProperty); }
            private set { SetValue(ReadOnlyAccountSettingsImportDirectoriesEditPropertyKey, value); }
        }

        #endregion
        #region SaveCommand

        private static readonly DependencyPropertyKey ReadOnlySaveCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(SaveCommand), typeof(DelegateCommand), typeof(AccountSettingsImportDirectoriesEditViewModel),
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

        private List<RoyaltyServiceWorker.AccountService.AccountSettingsImportDirectory> collectionForEdit = null;

        public AccountSettingsImportDirectoriesEditViewModel()
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
                collectionForEdit = newItem.ImportDirectories?.ToList() ?? new List<RoyaltyServiceWorker.AccountService.AccountSettingsImportDirectory>();
                AccountSettingsImportDirectoriesEdit = CollectionViewSource.GetDefaultView(collectionForEdit);
                newItem.PropertyChanged += AccountSettingsPropertyChanged;
            }
        }

        private void AccountSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AccountSettings.ImportDirectories))
            {
                collectionForEdit = AccountSettings.ImportDirectories?.ToList() ?? new List<RoyaltyServiceWorker.AccountService.AccountSettingsImportDirectory>();
                AccountSettingsImportDirectoriesEdit = CollectionViewSource.GetDefaultView(collectionForEdit);
            }
        }

        protected override void BackCommandExecuted(object o)
        {
            base.BackCommandExecuted(o);
            OnAccountSettingsChanged(AccountSettings, AccountSettings);
        }

        private Task<bool> SaveTask(IList<RoyaltyServiceWorker.AccountService.AccountSettingsImportDirectory> newItems)
        {
            IsBusy = true;
            return Task.Factory.StartNew(() => 
            {
                try
                {
                    foreach(var i in newItems)
                        i.AccountUID = AccountSettings.Id;
                    AccountSettings.ImportDirectories = new List<RoyaltyServiceWorker.AccountService.AccountSettingsImportDirectory>(newItems);
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
