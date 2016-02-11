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
    public enum AccountSettingsEditViewModelView
    {
        Settings,
        Columns,
        ExportDirectories,
        ImportDirectories,
        SheduleTimes
    }

    public class AccountSettingsEditViewModel : AbstractActionViewModel
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
        #region BackCommand

        public static readonly DependencyProperty BackCommandProperty = DependencyProperty.Register(nameof(BackCommand), typeof(ICommand),
            typeof(AccountSettingsEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public ICommand BackCommand
        {
            get { return (ICommand)GetValue(BackCommandProperty); }
            set { SetValue(BackCommandProperty, value); }
        }

        #endregion
        #region BackCommandParameter

        public static readonly DependencyProperty BackCommandParameterProperty = DependencyProperty.Register(nameof(BackCommandParameter), typeof(object),
            typeof(AccountSettingsEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public object BackCommandParameter
        {
            get { return GetValue(BackCommandParameterProperty); }
            set { SetValue(BackCommandParameterProperty, value); }
        }

        #endregion
        #region BackInternalCommand

        private static readonly DependencyPropertyKey ReadOnlyBackInternalCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(BackInternalCommand), typeof(DelegateCommand), typeof(AccountSettingsEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyBackInternalCommandProperty = ReadOnlyBackInternalCommandPropertyKey.DependencyProperty;

        public DelegateCommand BackInternalCommand
        {
            get { return (DelegateCommand)GetValue(ReadOnlyBackInternalCommandProperty); }
            private set { SetValue(ReadOnlyBackInternalCommandPropertyKey, value); }
        }

        #endregion
        #region BackInternalCommandParameter

        private static readonly DependencyPropertyKey ReadOnlyBackInternalCommandParameterPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(BackInternalCommandParameter), typeof(object), typeof(AccountSettingsEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyBackInternalCommandParameterProperty = ReadOnlyBackInternalCommandParameterPropertyKey.DependencyProperty;

        public object BackInternalCommandParameter
        {
            get { return (object)GetValue(ReadOnlyBackInternalCommandParameterProperty); }
            private set { SetValue(ReadOnlyBackInternalCommandParameterPropertyKey, value); }
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
        #region SetSettingsViewCommand

        private static readonly DependencyPropertyKey ReadOnlySetSettingsViewCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(SetSettingsViewCommand), typeof(DelegateCommand), typeof(AccountSettingsEditViewModel),
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
        #region SetColumnsViewCommand

        private static readonly DependencyPropertyKey ReadOnlySetColumnsViewCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(SetColumnsViewCommand), typeof(DelegateCommand), typeof(AccountSettingsEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlySetColumnsViewCommandProperty = ReadOnlySetColumnsViewCommandPropertyKey.DependencyProperty;

        public DelegateCommand SetColumnsViewCommand
        {
            get { return (DelegateCommand)GetValue(ReadOnlySetColumnsViewCommandProperty); }
            private set { SetValue(ReadOnlySetColumnsViewCommandPropertyKey, value); }
        }

        #endregion
        #region SetExportDirectoriesViewCommand

        private static readonly DependencyPropertyKey ReadOnlySetExportDirectoriesViewCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(SetExportDirectoriesViewCommand), typeof(DelegateCommand), typeof(AccountSettingsEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlySetExportDirectoriesViewCommandProperty = ReadOnlySetExportDirectoriesViewCommandPropertyKey.DependencyProperty;

        public DelegateCommand SetExportDirectoriesViewCommand
        {
            get { return (DelegateCommand)GetValue(ReadOnlySetExportDirectoriesViewCommandProperty); }
            private set { SetValue(ReadOnlySetExportDirectoriesViewCommandPropertyKey, value); }
        }

        #endregion
        #region SetImportDirectoriesViewCommand

        private static readonly DependencyPropertyKey ReadOnlySetImportDirectoriesViewCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(SetImportDirectoriesViewCommand), typeof(DelegateCommand), typeof(AccountSettingsEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlySetImportDirectoriesViewCommandProperty = ReadOnlySetImportDirectoriesViewCommandPropertyKey.DependencyProperty;

        public DelegateCommand SetImportDirectoriesViewCommand
        {
            get { return (DelegateCommand)GetValue(ReadOnlySetImportDirectoriesViewCommandProperty); }
            private set { SetValue(ReadOnlySetImportDirectoriesViewCommandPropertyKey, value); }
        }

        #endregion
        #region SetSheduleTimesViewCommand

        private static readonly DependencyPropertyKey ReadOnlySetSheduleTimesViewCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(SetSheduleTimesViewCommand), typeof(DelegateCommand), typeof(AccountSettingsEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlySetSheduleTimesViewCommandProperty = ReadOnlySetSheduleTimesViewCommandPropertyKey.DependencyProperty;

        public DelegateCommand SetSheduleTimesViewCommand
        {
            get { return (DelegateCommand)GetValue(ReadOnlySetSheduleTimesViewCommandProperty); }
            private set { SetValue(ReadOnlySetSheduleTimesViewCommandPropertyKey, value); }
        }

        #endregion
        #region View

        private static readonly DependencyPropertyKey ReadOnlyViewPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(View), typeof(AccountSettingsEditViewModelView), typeof(AccountSettingsEditViewModel),
                new FrameworkPropertyMetadata(AccountSettingsEditViewModelView.Settings,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyViewProperty = ReadOnlyViewPropertyKey.DependencyProperty;

        public AccountSettingsEditViewModelView View
        {
            get { return (AccountSettingsEditViewModelView)GetValue(ReadOnlyViewProperty); }
            private set { SetValue(ReadOnlyViewPropertyKey, value); }
        }

        #endregion

        public AccountSettingsEditViewModel()
        {
            BackInternalCommand = new DelegateCommand(o =>
            {
                UpdateAccountSettings(Account.Settings, AccountSettingsEdit);
                BackCommand?.Execute(BackCommandParameter);
            });
            SaveCommand = new DelegateCommand(o => {
                SaveTask(AccountSettingsEdit).ContinueWith((res) => 
                {
                    if (res.Result)
                        BackInternalCommand?.Execute(BackInternalCommandParameter);
                }, GetCancellationToken(), TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.FromCurrentSynchronizationContext());
            }, o => !IsBusy);
            SetSettingsViewCommand = new DelegateCommand(o => View = AccountSettingsEditViewModelView.Settings);
            SetColumnsViewCommand = new DelegateCommand(o => View = AccountSettingsEditViewModelView.Columns);
            SetExportDirectoriesViewCommand = new DelegateCommand(o => View = AccountSettingsEditViewModelView.ExportDirectories);
            SetImportDirectoriesViewCommand = new DelegateCommand(o => View = AccountSettingsEditViewModelView.ImportDirectories);
            SetSheduleTimesViewCommand = new DelegateCommand(o => View = AccountSettingsEditViewModelView.SheduleTimes);
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
            SaveCommand?.RaiseCanExecuteChanged();
            SetSettingsViewCommand?.RaiseCanExecuteChanged();
            SetColumnsViewCommand?.RaiseCanExecuteChanged();
            SetExportDirectoriesViewCommand?.RaiseCanExecuteChanged();
            SetImportDirectoriesViewCommand?.RaiseCanExecuteChanged();
            SetSheduleTimesViewCommand?.RaiseCanExecuteChanged();
        }
    }
}
