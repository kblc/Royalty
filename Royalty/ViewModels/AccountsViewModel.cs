using Royalty.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Helpers.WPF;
using System.Windows.Input;

namespace Royalty.ViewModels
{
    public class AccountsViewModel : DependencyObject
    {
        #region AccountsComponent

        public static readonly DependencyProperty AccountsComponentProperty = DependencyProperty.Register(nameof(AccountsComponent), typeof(AccountsComponent),
            typeof(AccountsViewModel), new PropertyMetadata(null, (s, e) =>
            {
                var model = s as AccountsViewModel;
                var newAccountsComponent = e.NewValue as AccountsComponent;
                var oldAccountsComponent = e.OldValue as AccountsComponent;
                if (model != null && newAccountsComponent != oldAccountsComponent)
                {
                    model.UpdateAccountSource(newAccountsComponent);
                }
            }));

        public AccountsComponent AccountsComponent
        {
            get { return (AccountsComponent)GetValue(AccountsComponentProperty); }
            set { SetValue(AccountsComponentProperty, value); }
        }

        #endregion
        #region ShowHidden

        public static readonly DependencyProperty ShowHiddenProperty = DependencyProperty.Register(nameof(ShowHidden), typeof(bool),
            typeof(AccountsViewModel), new PropertyMetadata(false, (s, e) =>
            {
                var model = s as AccountsViewModel;
                model?.RefreshAccountSource();
            }));

        public bool ShowHidden
        {
            get { return (bool)GetValue(ShowHiddenProperty); }
            set { SetValue(ShowHiddenProperty, value); }
        }

        #endregion
        #region FilteredAccounts

        private static readonly DependencyPropertyKey FilteredAccountsPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(FilteredAccounts), typeof(ICollectionView), typeof(AccountsViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyFilteredAccountsPropertyKey = FilteredAccountsPropertyKey.DependencyProperty;

        public ICollectionView FilteredAccounts
        {
            get { return (ICollectionView)GetValue(ReadOnlyFilteredAccountsPropertyKey); }
            private set { SetValue(FilteredAccountsPropertyKey, value); }
        }

        #endregion
        #region AccountsForEditSettings

        private static readonly DependencyPropertyKey AccountsForEditSettingsPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(AccountsForEditSettings), typeof(RoyaltyServiceWorker.AccountService.Account), typeof(AccountsViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyAccountsForEditSettingsPropertyKey = AccountsForEditSettingsPropertyKey.DependencyProperty;

        public RoyaltyServiceWorker.AccountService.Account AccountsForEditSettings
        {
            get { return (RoyaltyServiceWorker.AccountService.Account)GetValue(ReadOnlyAccountsForEditSettingsPropertyKey); }
            private set { SetValue(AccountsForEditSettingsPropertyKey, value); }
        }

        #endregion

        private void UpdateAccountSource(AccountsComponent newComponent)
        {
            if (newComponent == null)
            {
                FilteredAccounts = null;
                return;
            }

            FilteredAccounts = CollectionViewSource.GetDefaultView(newComponent.Accounts);
            FilteredAccounts.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            FilteredAccounts.Filter = (o) =>
            {
                var item = o as RoyaltyServiceWorker.AccountService.Account;
                if (item != null)
                    return !item.IsHidden || ShowHidden;
                return false;
            };
        }
        private void RefreshAccountSource()
        {
            FilteredAccounts?.Refresh();
        }

        private DelegateCommand selectAccountSettingsCommand = null;
        public ICommand SelectAccountSettingsCommand
        {
            get
            {
                return selectAccountSettingsCommand ?? (selectAccountSettingsCommand = new DelegateCommand((o) => 
                    {
                        AccountsForEditSettings = o as RoyaltyServiceWorker.AccountService.Account;
                    }));
            }
        }

        private DelegateCommand newAccountSettingsCommand = null;
        public ICommand NewAccountSettingsCommand
        {
            get
            {
                return newAccountSettingsCommand ?? (newAccountSettingsCommand = new DelegateCommand((o) =>
                {
                    AccountsForEditSettings = new RoyaltyServiceWorker.AccountService.Account();
                }));
            }
        }
    }
}
