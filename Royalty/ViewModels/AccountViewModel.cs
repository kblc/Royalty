using Royalty.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Royalty.ViewModels
{
    public class AccountViewModel : FrameworkElement
    {
        #region Account

        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register(nameof(Account), typeof(RoyaltyServiceWorker.AccountService.Account),
            typeof(AccountViewModel), new PropertyMetadata(null, (s, e) =>
            {
                var model = s as AccountViewModel;
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
        #region EditCommand

        public static readonly DependencyProperty EditCommandProperty = DependencyProperty.Register(nameof(EditCommand), typeof(ICommand),
            typeof(AccountViewModel), new PropertyMetadata(null, (s, e) => { }));

        public ICommand EditCommand
        {
            get { return (ICommand)GetValue(EditCommandProperty); }
            set { SetValue(EditCommandProperty, value); }
        }

        #endregion
        #region EditCommandParameter

        public static readonly DependencyProperty EditCommandParameterProperty = DependencyProperty.Register(nameof(EditCommandParameter), typeof(object),
            typeof(AccountViewModel), new PropertyMetadata(null, (s, e) => { }));

        public object EditCommandParameter
        {
            get { return GetValue(EditCommandParameterProperty); }
            set { SetValue(EditCommandParameterProperty, value); }
        }

        #endregion

        private void UpdateAccount(RoyaltyServiceWorker.AccountService.Account newItem, RoyaltyServiceWorker.AccountService.Account oldItem)
        {
        }
    }
}
