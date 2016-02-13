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
    public class AccountSettingsEditViewModel : AbstractActionWithBackViewModel
    {
        #region Account

        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register(nameof(Account), typeof(RoyaltyServiceWorker.AccountService.Account),
            typeof(AccountSettingsEditViewModel), new PropertyMetadata(null, (s, e) =>
            {
                var model = s as AccountSettingsEditViewModel;
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
        #region AccountSettingsEdit

        private static readonly DependencyPropertyKey ReadOnlyAccountSettingsEditPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(AccountSettingsEdit), typeof(RoyaltyServiceWorker.AccountService.AccountSettings), typeof(AccountSettingsEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyAccountSettingsEditProperty = ReadOnlyAccountSettingsEditPropertyKey.DependencyProperty;

        public RoyaltyServiceWorker.AccountService.AccountSettings AccountSettingsEdit
        {
            get { return (RoyaltyServiceWorker.AccountService.AccountSettings)GetValue(ReadOnlyAccountSettingsEditProperty); }
            private set { SetValue(ReadOnlyAccountSettingsEditPropertyKey, value); }
        }

        #endregion
        #region SaveCommand

        private static readonly DependencyPropertyKey ReadOnlySaveCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(SaveCommand), typeof(DelegateCommand), typeof(AccountSettingsEditViewModel),
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

        #region SettingsColumnsViewCommand

        public static readonly DependencyProperty SettingsColumnsViewCommandProperty = DependencyProperty.Register(nameof(SettingsColumnsViewCommand), typeof(ICommand),
            typeof(AccountSettingsEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public ICommand SettingsColumnsViewCommand
        {
            get { return (ICommand)GetValue(SettingsColumnsViewCommandProperty); }
            set { SetValue(SettingsColumnsViewCommandProperty, value); }
        }

        #endregion
        #region SettingsColumnsViewCommandParameter

        public static readonly DependencyProperty SettingsColumnsViewCommandParameterProperty = DependencyProperty.Register(nameof(SettingsColumnsViewCommandParameter), typeof(object),
            typeof(AccountSettingsEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public object SettingsColumnsViewCommandParameter
        {
            get { return GetValue(SettingsColumnsViewCommandParameterProperty); }
            set { SetValue(SettingsColumnsViewCommandParameterProperty, value); }
        }

        #endregion
        #region SettingsExportDirectoriesViewCommand

        public static readonly DependencyProperty SettingsExportDirectoriesViewCommandProperty = DependencyProperty.Register(nameof(SettingsExportDirectoriesViewCommand), typeof(ICommand),
            typeof(AccountSettingsEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public ICommand SettingsExportDirectoriesViewCommand
        {
            get { return (ICommand)GetValue(SettingsExportDirectoriesViewCommandProperty); }
            set { SetValue(SettingsExportDirectoriesViewCommandProperty, value); }
        }

        #endregion
        #region SettingsExportDirectoriesViewCommandParameter

        public static readonly DependencyProperty SettingsExportDirectoriesViewCommandParameterProperty = DependencyProperty.Register(nameof(SettingsExportDirectoriesViewCommandParameter), typeof(object),
            typeof(AccountSettingsEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public object SettingsExportDirectoriesViewCommandParameter
        {
            get { return GetValue(SettingsExportDirectoriesViewCommandParameterProperty); }
            set { SetValue(SettingsExportDirectoriesViewCommandParameterProperty, value); }
        }

        #endregion
        #region SettingsImportDirectoriesViewCommand

        public static readonly DependencyProperty SettingsImportDirectoriesViewCommandProperty = DependencyProperty.Register(nameof(SettingsImportDirectoriesViewCommand), typeof(ICommand),
            typeof(AccountSettingsEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public ICommand SettingsImportDirectoriesViewCommand
        {
            get { return (ICommand)GetValue(SettingsImportDirectoriesViewCommandProperty); }
            set { SetValue(SettingsImportDirectoriesViewCommandProperty, value); }
        }

        #endregion
        #region SettingsImportDirectoriesViewCommandParameter

        public static readonly DependencyProperty SettingsImportDirectoriesViewCommandParameterProperty = DependencyProperty.Register(nameof(SettingsImportDirectoriesViewCommandParameter), typeof(object),
            typeof(AccountSettingsEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public object SettingsImportDirectoriesViewCommandParameter
        {
            get { return GetValue(SettingsImportDirectoriesViewCommandParameterProperty); }
            set { SetValue(SettingsImportDirectoriesViewCommandParameterProperty, value); }
        }

        #endregion
        #region SettingsSheduleTimesViewCommand

        public static readonly DependencyProperty SettingsSheduleTimesViewCommandProperty = DependencyProperty.Register(nameof(SettingsSheduleTimesViewCommand), typeof(ICommand),
            typeof(AccountSettingsEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public ICommand SettingsSheduleTimesViewCommand
        {
            get { return (ICommand)GetValue(SettingsSheduleTimesViewCommandProperty); }
            set { SetValue(SettingsSheduleTimesViewCommandProperty, value); }
        }

        #endregion
        #region SettingsSheduleTimesViewCommandParameter

        public static readonly DependencyProperty SettingsSheduleTimesViewCommandParameterProperty = DependencyProperty.Register(nameof(SettingsSheduleTimesViewCommandParameter), typeof(object),
            typeof(AccountSettingsEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public object SettingsSheduleTimesViewCommandParameter
        {
            get { return GetValue(SettingsSheduleTimesViewCommandParameterProperty); }
            set { SetValue(SettingsSheduleTimesViewCommandParameterProperty, value); }
        }

        #endregion

        public AccountSettingsEditViewModel()
        {
            SaveCommand = new DelegateCommand(o => {
                SaveTask(AccountSettingsEdit).ContinueWith((res) => { if (res.Result) Back(); }
                , GetCancellationToken(), TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.FromCurrentSynchronizationContext());
            }, o => !IsBusy);
        }

        private void UpdateAccount(RoyaltyServiceWorker.AccountService.Account newItem, RoyaltyServiceWorker.AccountService.Account oldItem)
        {
            IsBusy = false;
            Error = null;
            RaiseCommands();
            if (newItem != null)
                newItem.PropertyChanged += AccountPropertyChanged;
            if (oldItem != null)
                oldItem.PropertyChanged -= AccountPropertyChanged;
            UpdateAccountSettings(newItem?.Settings, oldItem?.Settings);
        }

        private void UpdateAccountSettings(RoyaltyServiceWorker.AccountService.AccountSettings newItem, RoyaltyServiceWorker.AccountService.AccountSettings oldItem)
        {
            if (newItem != null)
            {
                AccountSettingsEdit = new RoyaltyServiceWorker.AccountService.AccountSettings();
                AccountSettingsEdit.CopyObjectFrom(newItem);
                newItem.PropertyChanged += AccountSettingsPropertyChanged;
            }
            if (oldItem != null)
            {
                oldItem.PropertyChanged -= AccountSettingsPropertyChanged;
            }
        }

        private void AccountSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            AccountSettingsEdit?.CopyObjectFrom(Account?.Settings);
        }

        private void AccountPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RoyaltyServiceWorker.AccountService.Account.Settings))
                UpdateAccountSettings(Account?.Settings, AccountSettingsEdit);
        }

        protected override void BackCommandExecuted(object o)
        {
            base.BackCommandExecuted(o);
            UpdateAccountSettings(Account.Settings, AccountSettingsEdit);
        }

        private Task<bool> SaveTask(RoyaltyServiceWorker.AccountService.AccountSettings item)
        {
            IsBusy = true;

            var client = new RoyaltyServiceWorker.AccountService.AccountServiceClient();
            var task = client.SettingsUpdateAsync(item);

            var taskRes = task.ContinueWith((res) => 
            {
                try
                {
                    if (res.Result.Error != null)
                        throw new Exception(res.Result.Error);
                    Account.Settings.CopyObjectFrom(res.Result.Value);
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

        protected override void RaiseCommands()
        {
            base.RaiseCommands();
            SaveCommand?.RaiseCanExecuteChanged();
        }
    }
}
