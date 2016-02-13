using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Royalty.ViewModels
{
    public abstract class AbstractActionActivationViewModel : AbstractActionViewModel
    {
        #region IsActive

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(nameof(IsActive), typeof(bool),
            typeof(AbstractActionActivationViewModel), new PropertyMetadata(false, (s, e) => { (s as AbstractActionActivationViewModel)?.OnIsActiveChanged((bool)e.NewValue);  }));

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        #endregion

        protected virtual void OnIsActiveChanged(bool newValue) => RaiseCommands();
    }
}
