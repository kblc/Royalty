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
    public enum AccountsViewEnum
    {
        Accounts,
        Account,
        AccountSettings,
        AccountSettingsColumns,
        AccountSettingsExportDirectories,
        AccountSettingsImportDirectories,
        AccountSettingsSheduleTimes,
        AccountSeriesOfNumbers,
        AccountAdditionalColumns,
        AccountPhoneMarks,
    }

    public class AccountsViewModel : FrameworkElement
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
        #region AccountsSeriesOfNumbersComponent

        public static readonly DependencyProperty AccountsSeriesOfNumbersComponentProperty = DependencyProperty.Register(nameof(AccountsSeriesOfNumbersComponent), typeof(AccountsSeriesOfNumbersComponent),
            typeof(AccountsViewModel), new PropertyMetadata(null, (s, e) =>
            {
                var model = s as AccountsViewModel;
                var newAccountsComponent = e.NewValue as AccountsSeriesOfNumbersComponent;
                var oldAccountsComponent = e.OldValue as AccountsSeriesOfNumbersComponent;
                if (model != null && newAccountsComponent != oldAccountsComponent)
                {
                    model.UpdateAccountsSeriesOfNumbersComponentSource(newAccountsComponent);
                }
            }));

        public AccountsSeriesOfNumbersComponent AccountsSeriesOfNumbersComponent
        {
            get { return (AccountsSeriesOfNumbersComponent)GetValue(AccountsSeriesOfNumbersComponentProperty); }
            set { SetValue(AccountsSeriesOfNumbersComponentProperty, value); }
        }

        #endregion
        #region AccountsAdditionalColumnsComponent

        public static readonly DependencyProperty AccountsAdditionalColumnsComponentComponentProperty = DependencyProperty.Register(nameof(AccountsAdditionalColumnsComponent), typeof(AccountsAdditionalColumnsComponent),
            typeof(AccountsViewModel), new PropertyMetadata(null, (s, e) =>
            {
                var model = s as AccountsViewModel;
                var newAccountsComponent = e.NewValue as AccountsAdditionalColumnsComponent;
                var oldAccountsComponent = e.OldValue as AccountsAdditionalColumnsComponent;
                if (model != null && newAccountsComponent != oldAccountsComponent)
                {
                    model.UpdateAccountsAdditionalColumnsComponentSource(newAccountsComponent);
                }
            }));

        public AccountsAdditionalColumnsComponent AccountsAdditionalColumnsComponent
        {
            get { return (AccountsAdditionalColumnsComponent)GetValue(AccountsAdditionalColumnsComponentComponentProperty); }
            set { SetValue(AccountsAdditionalColumnsComponentComponentProperty, value); }
        }

        #endregion
        #region AccountsPhoneMarksComponent

        public static readonly DependencyProperty AccountsPhoneMarksComponentProperty = DependencyProperty.Register(nameof(AccountsPhoneMarksComponent), typeof(AccountsPhoneMarksComponent),
            typeof(AccountsViewModel), new PropertyMetadata(null, (s, e) =>
            {
                var model = s as AccountsViewModel;
                var newAccountsComponent = e.NewValue as AccountsPhoneMarksComponent;
                var oldAccountsComponent = e.OldValue as AccountsPhoneMarksComponent;
                if (model != null && newAccountsComponent != oldAccountsComponent)
                {
                    model.UpdateAccountsPhoneMarksComponentSource(newAccountsComponent);
                }
            }));

        public AccountsPhoneMarksComponent AccountsPhoneMarksComponent
        {
            get { return (AccountsPhoneMarksComponent)GetValue(AccountsPhoneMarksComponentProperty); }
            set { SetValue(AccountsPhoneMarksComponentProperty, value); }
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
        #region Marks

        private static readonly DependencyPropertyKey MarksPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(Marks), typeof(ICollectionView), typeof(AccountsViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyMarksPropertyKey = MarksPropertyKey.DependencyProperty;

        public ICollectionView Marks
        {
            get { return (ICollectionView)GetValue(ReadOnlyMarksPropertyKey); }
            private set { SetValue(MarksPropertyKey, value); }
        }

        #endregion
        #region ColumnTypes

        private static readonly DependencyPropertyKey ColumnTypesPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(ColumnTypes), typeof(ICollectionView), typeof(AccountsViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyColumnTypesPropertyKey = ColumnTypesPropertyKey.DependencyProperty;

        public ICollectionView ColumnTypes
        {
            get { return (ICollectionView)GetValue(ReadOnlyColumnTypesPropertyKey); }
            private set { SetValue(ColumnTypesPropertyKey, value); }
        }

        #endregion
        #region AccountForEdit

        private static readonly DependencyPropertyKey AccountForEditPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(AccountForEdit), typeof(RoyaltyServiceWorker.AccountService.Account), typeof(AccountsViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { (s as AccountsViewModel)?.OnAccountForEditChanged((RoyaltyServiceWorker.AccountService.Account)e.NewValue); })));
        public static readonly DependencyProperty ReadOnlyAccountForEditPropertyKey = AccountForEditPropertyKey.DependencyProperty;

        public RoyaltyServiceWorker.AccountService.Account AccountForEdit
        {
            get { return (RoyaltyServiceWorker.AccountService.Account)GetValue(ReadOnlyAccountForEditPropertyKey); }
            private set { SetValue(AccountForEditPropertyKey, value); }
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
        #region SetViewCommand

        private static readonly DependencyPropertyKey ReadOnlySetViewCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(SetViewCommand), typeof(DelegateCommand), typeof(AccountsViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlySetViewCommandProperty = ReadOnlySetViewCommandPropertyKey.DependencyProperty;

        public DelegateCommand SetViewCommand
        {
            get { return (DelegateCommand)GetValue(ReadOnlySetViewCommandProperty); }
            private set { SetValue(ReadOnlySetViewCommandPropertyKey, value); }
        }

        #endregion
        #region View

        private static readonly DependencyPropertyKey ReadOnlyViewPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(View), typeof(AccountsViewEnum), typeof(AccountsViewModel),
                new FrameworkPropertyMetadata(AccountsViewEnum.Accounts,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyViewProperty = ReadOnlyViewPropertyKey.DependencyProperty;

        public AccountsViewEnum View
        {
            get { return (AccountsViewEnum)GetValue(ReadOnlyViewProperty); }
            private set { SetValue(ReadOnlyViewPropertyKey, value); }
        }

        #endregion

        private void UpdateAccountSource(AccountsComponent newComponent)
        {
            if (newComponent == null)
            {
                FilteredAccounts = null;
                Marks = null;
                ColumnTypes = null;
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

            Marks = CollectionViewSource.GetDefaultView(newComponent.Marks);
            ColumnTypes = CollectionViewSource.GetDefaultView(newComponent.ColumnTypes);
        }

        private void UpdateAccountsSeriesOfNumbersComponentSource(AccountsSeriesOfNumbersComponent newComponent) { }

        private void UpdateAccountsAdditionalColumnsComponentSource(AccountsAdditionalColumnsComponent newComponent) { }

        private void UpdateAccountsPhoneMarksComponentSource(AccountsPhoneMarksComponent newComponent) { }

        private void RefreshAccountSource()
        {
            FilteredAccounts?.Refresh();
        }
        private void OnAccountForEditChanged(RoyaltyServiceWorker.AccountService.Account account)
        {
            View = (account == null)
                ? AccountsViewEnum.Accounts
                : AccountsViewEnum.Account;
        }

        public AccountsViewModel()
        {
            SelectAccountCommand = new DelegateCommand((o) =>
            {
                AccountForEdit = o as RoyaltyServiceWorker.AccountService.Account;
                OnAccountForEditChanged(AccountForEdit);
            });
            NewAccountCommand = new DelegateCommand((o) => SelectAccountCommand.Execute(new RoyaltyServiceWorker.AccountService.Account()));
            SetViewCommand = new DelegateCommand(o => View = (AccountsViewEnum)o);
        }
    }
}
