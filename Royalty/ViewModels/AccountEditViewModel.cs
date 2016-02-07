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
            = DependencyProperty.RegisterReadOnly(nameof(SaveCommand), typeof(ICommand), typeof(AccountEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlySaveCommandProperty = ReadOnlySaveCommandPropertyKey.DependencyProperty;

        public ICommand SaveCommand
        {
            get { return (ICommand)GetValue(ReadOnlySaveCommandProperty); }
            private set { SetValue(ReadOnlySaveCommandPropertyKey, value); }
        }

        #endregion
        #region DeleteCommand

        private static readonly DependencyPropertyKey ReadOnlyDeleteCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(DeleteCommand), typeof(ICommand), typeof(AccountEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyDeleteCommandProperty = ReadOnlyDeleteCommandPropertyKey.DependencyProperty;

        public ICommand DeleteCommand
        {
            get { return (ICommand)GetValue(ReadOnlyDeleteCommandProperty); }
            private set { SetValue(ReadOnlyDeleteCommandPropertyKey, value); }
        }

        #endregion

        private DelegateCommand saveCommand;
        private DelegateCommand deleteCommand;
        public AccountEditViewModel()
        {
            SaveCommand = saveCommand = new DelegateCommand(o => {
                SaveTask(AccountEdit).ContinueWith((res) => 
                {
                    if (res.Result)
                        BackCommand?.Execute(BackCommandParameter);
                }, GetCancellationToken(), TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.FromCurrentSynchronizationContext());
            }, o => !IsBusy);
            DeleteCommand = deleteCommand = new DelegateCommand(o => {
                DeleteTask(AccountEdit).ContinueWith((res) =>
                {
                    if (res.Result)
                        BackCommand?.Execute(BackCommandParameter);
                }, GetCancellationToken(), TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.FromCurrentSynchronizationContext());
            }, o => !IsBusy && (Account?.Id ?? Guid.Empty) != Guid.Empty);
        }

        private void UpdateAccount(RoyaltyServiceWorker.AccountService.Account newItem, RoyaltyServiceWorker.AccountService.Account oldItem)
        {
            IsBusy = false;
            Error = null;
            RaiseCommands();
            AccountEdit = new RoyaltyServiceWorker.AccountService.Account();
            if (newItem != null)
                AccountEdit.CopyObjectFrom(newItem);
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
            saveCommand?.RaiseCanExecuteChanged();
            deleteCommand?.RaiseCanExecuteChanged();
        }
    }
}
