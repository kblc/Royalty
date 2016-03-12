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
    public class AccountEditViewModel : AbstractActionWithBackViewModel
    {
        #region Account

        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register(nameof(Account), typeof(RoyaltyServiceWorker.AccountService.Account),
            typeof(AccountEditViewModel), new PropertyMetadata(null, (s, e) => { (s as AccountEditViewModel)?.OnAccountChanged(e.NewValue as RoyaltyServiceWorker.AccountService.Account, e.OldValue as RoyaltyServiceWorker.AccountService.Account); }));

        public RoyaltyServiceWorker.AccountService.Account Account
        {
            get { return (RoyaltyServiceWorker.AccountService.Account)GetValue(AccountProperty); }
            set { SetValue(AccountProperty, value); }
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
        #region SettingsViewCommand

        public static readonly DependencyProperty SettingsViewCommandProperty = DependencyProperty.Register(nameof(SettingsViewCommand), typeof(ICommand),
            typeof(AccountEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public ICommand SettingsViewCommand
        {
            get { return (ICommand)GetValue(SettingsViewCommandProperty); }
            set { SetValue(SettingsViewCommandProperty, value); }
        }

        #endregion
        #region SettingsViewCommandParameter

        public static readonly DependencyProperty SettingsViewCommandParameterProperty = DependencyProperty.Register(nameof(SettingsViewCommandParameter), typeof(object),
            typeof(AccountEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public object SettingsViewCommandParameter
        {
            get { return (object)GetValue(SettingsViewCommandParameterProperty); }
            set { SetValue(SettingsViewCommandParameterProperty, value); }
        }

        #endregion
        #region SettingsViewInternalCommand

        private static readonly DependencyPropertyKey ReadOnlySettingsViewInternalCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(SettingsViewInternalCommand), typeof(DelegateCommand), typeof(AccountEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlySettingsViewInternalCommandProperty = ReadOnlySettingsViewInternalCommandPropertyKey.DependencyProperty;

        public DelegateCommand SettingsViewInternalCommand
        {
            get { return (DelegateCommand)GetValue(ReadOnlySettingsViewInternalCommandProperty); }
            private set { SetValue(ReadOnlySettingsViewInternalCommandPropertyKey, value); }
        }

        #endregion
        #region SeriesOfNumbersViewCommand

        public static readonly DependencyProperty SeriesOfNumbersViewCommandProperty = DependencyProperty.Register(nameof(SeriesOfNumbersViewCommand), typeof(ICommand),
            typeof(AccountEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public ICommand SeriesOfNumbersViewCommand
        {
            get { return (ICommand)GetValue(SeriesOfNumbersViewCommandProperty); }
            set { SetValue(SeriesOfNumbersViewCommandProperty, value); }
        }

        #endregion
        #region SeriesOfNumbersViewCommandParameter

        public static readonly DependencyProperty SeriesOfNumbersViewCommandParameterProperty = DependencyProperty.Register(nameof(SeriesOfNumbersViewCommandParameter), typeof(object),
            typeof(AccountEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public object SeriesOfNumbersViewCommandParameter
        {
            get { return GetValue(SeriesOfNumbersViewCommandParameterProperty); }
            set { SetValue(SeriesOfNumbersViewCommandParameterProperty, value); }
        }

        #endregion
        #region AdditionalColumnsViewCommand

        public static readonly DependencyProperty AdditionalColumnsViewCommandProperty = DependencyProperty.Register(nameof(AdditionalColumnsViewCommand), typeof(ICommand),
            typeof(AccountEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public ICommand AdditionalColumnsViewCommand
        {
            get { return (ICommand)GetValue(AdditionalColumnsViewCommandProperty); }
            set { SetValue(AdditionalColumnsViewCommandProperty, value); }
        }

        #endregion
        #region AdditionalColumnsViewCommandParameter

        public static readonly DependencyProperty AdditionalColumnsViewCommandParameterProperty = DependencyProperty.Register(nameof(AdditionalColumnsViewCommandParameter), typeof(object),
            typeof(AccountEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public object AdditionalColumnsViewCommandParameter
        {
            get { return GetValue(AdditionalColumnsViewCommandParameterProperty); }
            set { SetValue(AdditionalColumnsViewCommandParameterProperty, value); }
        }

        #endregion
        #region PhoneMarksViewCommand

        public static readonly DependencyProperty PhoneMarksViewCommandProperty = DependencyProperty.Register(nameof(PhoneMarksViewCommand), typeof(ICommand),
            typeof(AccountEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public ICommand PhoneMarksViewCommand
        {
            get { return (ICommand)GetValue(PhoneMarksViewCommandProperty); }
            set { SetValue(PhoneMarksViewCommandProperty, value); }
        }

        #endregion
        #region PhoneMarksViewCommandParameter

        public static readonly DependencyProperty PhoneMarksViewCommandParameterProperty = DependencyProperty.Register(nameof(PhoneMarksViewCommandParameter), typeof(object),
            typeof(AccountEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public object PhoneMarksViewCommandParameter
        {
            get { return GetValue(PhoneMarksViewCommandParameterProperty); }
            set { SetValue(PhoneMarksViewCommandParameterProperty, value); }
        }

        #endregion
        #region ImportQueueRecordsViewCommand

        public static readonly DependencyProperty ImportQueueRecordsViewCommandProperty = DependencyProperty.Register(nameof(ImportQueueRecordsViewCommand), typeof(ICommand),
            typeof(AccountEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public ICommand ImportQueueRecordsViewCommand
        {
            get { return (ICommand)GetValue(ImportQueueRecordsViewCommandProperty); }
            set { SetValue(ImportQueueRecordsViewCommandProperty, value); }
        }

        #endregion
        #region ImportQueueRecordsViewCommandParameter

        public static readonly DependencyProperty ImportQueueRecordsViewCommandParameterProperty = DependencyProperty.Register(nameof(ImportQueueRecordsViewCommandParameter), typeof(object),
            typeof(AccountEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public object ImportQueueRecordsViewCommandParameter
        {
            get { return GetValue(ImportQueueRecordsViewCommandParameterProperty); }
            set { SetValue(ImportQueueRecordsViewCommandParameterProperty, value); }
        }

        #endregion

        public AccountEditViewModel()
        {
            SaveCommand = new DelegateCommand(o => {
                SaveTask(AccountEdit).ContinueWith((res) => 
                {
                    if (res.Result)
                        BackInternalCommand?.Execute(null);
                }, GetCancellationToken(), TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.FromCurrentSynchronizationContext());
            }, o => !IsBusy);
            DeleteCommand = new DelegateCommand(o => {
                DeleteTask(AccountEdit).ContinueWith((res) =>
                {
                    if (res.Result)
                        BackInternalCommand?.Execute(null);
                }, GetCancellationToken(), TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.FromCurrentSynchronizationContext());
            }, o => !IsBusy && (Account?.Id ?? Guid.Empty) != Guid.Empty);
            SettingsViewInternalCommand = new DelegateCommand(o => SettingsViewCommand?.Execute(o), o => !IsBusy && (Account?.Id ?? Guid.Empty) != Guid.Empty);
        }

        private void OnAccountChanged(RoyaltyServiceWorker.AccountService.Account newItem, RoyaltyServiceWorker.AccountService.Account oldItem)
        {
            IsBusy = false;
            Error = null;
            RaiseCommands();
            RestoreFromSource(newItem);
            if (newItem != null)
                newItem.PropertyChanged += AccountPropertyChanged;
            if (oldItem != null)
                oldItem.PropertyChanged -= AccountPropertyChanged;
        }

        private void AccountPropertyChanged(object sender, PropertyChangedEventArgs e) => RestoreFromSource((RoyaltyServiceWorker.AccountService.Account)sender);

        private void RestoreFromSource(RoyaltyServiceWorker.AccountService.Account source)
        {
            if (AccountEdit == null)
                AccountEdit = new RoyaltyServiceWorker.AccountService.Account();
            if (source != null)
                AccountEdit.CopyObjectFrom(source);
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
            base.RaiseCommands();
            SaveCommand?.RaiseCanExecuteChanged();
            DeleteCommand?.RaiseCanExecuteChanged();
            SettingsViewInternalCommand?.RaiseCanExecuteChanged();
        }
    }
}
