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
        #region AccountsForEdit

        private static readonly DependencyPropertyKey AccountsForEditPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(AccountsForEdit), typeof(RoyaltyServiceWorker.AccountService.Account), typeof(AccountsViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyAccountsForEditPropertyKey = AccountsForEditPropertyKey.DependencyProperty;

        public RoyaltyServiceWorker.AccountService.Account AccountsForEdit
        {
            get { return (RoyaltyServiceWorker.AccountService.Account)GetValue(ReadOnlyAccountsForEditPropertyKey); }
            private set { SetValue(AccountsForEditPropertyKey, value); }
        }

        #endregion
        #region SelectAccountCommand

        private static readonly DependencyPropertyKey ReadOnlySelectAccountCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(SelectAccountCommand), typeof(DelegateCommand), typeof(AccountsViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlySelectAccountCommandProperty = ReadOnlySelectAccountCommandPropertyKey.DependencyProperty;

        public DelegateCommand SelectAccountCommand
        {
            get { return (DelegateCommand)GetValue(ReadOnlySelectAccountCommandProperty); }
            private set { SetValue(ReadOnlySelectAccountCommandPropertyKey, value); }
        }

        #endregion
        #region NewAccountCommand

        private static readonly DependencyPropertyKey ReadOnlyNewAccountCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(NewAccountCommand), typeof(DelegateCommand), typeof(AccountsViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyNewAccountCommandProperty = ReadOnlyNewAccountCommandPropertyKey.DependencyProperty;

        public DelegateCommand NewAccountCommand
        {
            get { return (DelegateCommand)GetValue(ReadOnlyNewAccountCommandProperty); }
            private set { SetValue(ReadOnlyNewAccountCommandPropertyKey, value); }
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

        public AccountsViewModel()
        {
            SelectAccountCommand = new DelegateCommand((o) => AccountsForEdit = o as RoyaltyServiceWorker.AccountService.Account);
            NewAccountCommand = new DelegateCommand((o) => AccountsForEdit = new RoyaltyServiceWorker.AccountService.Account());
        }
    }
}
