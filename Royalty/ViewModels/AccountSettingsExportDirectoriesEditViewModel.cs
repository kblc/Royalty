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
    public class AccountSettingsExportDirectoriesEditViewModel : AbstractActionWithBackViewModel
    {
        #region Account

        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register(nameof(Account), typeof(RoyaltyServiceWorker.AccountService.Account),
            typeof(AccountSettingsExportDirectoriesEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public RoyaltyServiceWorker.AccountService.Account Account
        {
            get { return (RoyaltyServiceWorker.AccountService.Account)GetValue(AccountProperty); }
            set { SetValue(AccountProperty, value); }
        }

        #endregion
        #region Marks

        public static readonly DependencyProperty MarksProperty = DependencyProperty.Register(nameof(Marks), typeof(ICollectionView),
            typeof(AccountSettingsExportDirectoriesEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public ICollectionView Marks
        {
            get { return (ICollectionView)GetValue(MarksProperty); }
            set { SetValue(MarksProperty, value); }
        }

        #endregion
        #region AccountSettings

        public static readonly DependencyProperty AccountSettingsProperty = DependencyProperty.Register(nameof(AccountSettings), typeof(RoyaltyServiceWorker.AccountService.AccountSettings),
            typeof(AccountSettingsExportDirectoriesEditViewModel), new PropertyMetadata(null, (s, e) => { (s as AccountSettingsExportDirectoriesEditViewModel).OnAccountSettingsChanged(e.NewValue as RoyaltyServiceWorker.AccountService.AccountSettings, e.OldValue as RoyaltyServiceWorker.AccountService.AccountSettings); }));

        public RoyaltyServiceWorker.AccountService.AccountSettings AccountSettings
        {
            get { return (RoyaltyServiceWorker.AccountService.AccountSettings)GetValue(AccountSettingsProperty); }
            set { SetValue(AccountSettingsProperty, value); }
        }

        #endregion
        #region AccountSettingsExportDirectoriesEdit

        private static readonly DependencyPropertyKey ReadOnlyAccountSettingsExportDirectoriesEditPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(AccountSettingsExportDirectoriesEdit), typeof(ICollectionView), typeof(AccountSettingsExportDirectoriesEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyAccountSettingsExportDirectoriesEditProperty = ReadOnlyAccountSettingsExportDirectoriesEditPropertyKey.DependencyProperty;

        public ICollectionView AccountSettingsExportDirectoriesEdit
        {
            get { return (ICollectionView)GetValue(ReadOnlyAccountSettingsExportDirectoriesEditProperty); }
            private set { SetValue(ReadOnlyAccountSettingsExportDirectoriesEditPropertyKey, value); }
        }

        #endregion
        #region SaveCommand

        private static readonly DependencyPropertyKey ReadOnlySaveCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(SaveCommand), typeof(DelegateCommand), typeof(AccountSettingsExportDirectoriesEditViewModel),
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

        private List<RoyaltyServiceWorker.AccountService.AccountSettingsExportDirectory> collectionForEdit = null;

        public AccountSettingsExportDirectoriesEditViewModel()
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
                collectionForEdit = newItem.ExportDirectories?.ToList() ?? new List<RoyaltyServiceWorker.AccountService.AccountSettingsExportDirectory>();
                AccountSettingsExportDirectoriesEdit = CollectionViewSource.GetDefaultView(collectionForEdit);
                newItem.PropertyChanged += AccountSettingsPropertyChanged;
            }
        }

        private void AccountSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AccountSettings.ExportDirectories))
            {
                collectionForEdit = AccountSettings.ExportDirectories?.ToList() ?? new List<RoyaltyServiceWorker.AccountService.AccountSettingsExportDirectory>();
                AccountSettingsExportDirectoriesEdit = CollectionViewSource.GetDefaultView(collectionForEdit);
            }
        }

        protected override void BackCommandExecuted(object o)
        {
            base.BackCommandExecuted(o);
            OnAccountSettingsChanged(AccountSettings, AccountSettings);
        }

        private Task<bool> SaveTask(IList<RoyaltyServiceWorker.AccountService.AccountSettingsExportDirectory> newItems)
        {
            IsBusy = true;
            return Task.Factory.StartNew(() => 
            {
                try
                {
                    foreach(var i in newItems)
                        i.AccountUID = AccountSettings.Id;
                    AccountSettings.ExportDirectories = new List<RoyaltyServiceWorker.AccountService.AccountSettingsExportDirectory>(newItems);
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
