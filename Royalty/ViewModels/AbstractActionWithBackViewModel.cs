using Helpers.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Royalty.ViewModels
{
    public abstract class AbstractActionWithBackViewModel : AbstractActionActivationViewModel
    {
        #region BackCommand

        public static readonly DependencyProperty BackCommandProperty = DependencyProperty.Register(nameof(BackCommand), typeof(ICommand),
            typeof(AbstractActionWithBackViewModel), new PropertyMetadata(null, (s, e) => { }));

        public ICommand BackCommand
        {
            get { return (ICommand)GetValue(BackCommandProperty); }
            set { SetValue(BackCommandProperty, value); }
        }

        #endregion
        #region BackCommandParameter

        public static readonly DependencyProperty BackCommandParameterProperty = DependencyProperty.Register(nameof(BackCommandParameter), typeof(object),
            typeof(AbstractActionWithBackViewModel), new PropertyMetadata(null, (s, e) => { }));

        public object BackCommandParameter
        {
            get { return GetValue(BackCommandParameterProperty); }
            set { SetValue(BackCommandParameterProperty, value); }
        }

        #endregion
        #region BackInternalCommand

        private static readonly DependencyPropertyKey ReadOnlyBackInternalCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(BackInternalCommand), typeof(DelegateCommand), typeof(AbstractActionWithBackViewModel),
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

        public AbstractActionWithBackViewModel()
        {
            BackInternalCommand = new DelegateCommand(o => { BackCommandExecuted(o); BackCommand?.Execute(o); }, o => CanBackCommandExecuted(o));
        }

        protected virtual void BackCommandExecuted(object o) { }
        protected virtual bool CanBackCommandExecuted(object o) => true;

        protected void Back()
        {
            BackInternalCommand?.Execute(BackCommandParameter);
        }

        protected override void RaiseCommands()
        {
            base.RaiseCommands();
            BackInternalCommand?.RaiseCanExecuteChanged();
        }
    }
}
