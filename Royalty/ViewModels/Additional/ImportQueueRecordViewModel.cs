using Helpers.WPF;
using RoyaltyServiceWorker.AccountService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Royalty.ViewModels.Additional
{
    public class ImportQueueRecordViewModel : AbstractViewModel
    {
        #region ImportQueueRecord

        public static readonly DependencyProperty ImportQueueRecordProperty = DependencyProperty.Register(nameof(ImportQueueRecord), typeof(ImportQueueRecord),
            typeof(ImportQueueRecordViewModel), new PropertyMetadata(null, (s, e) => { (s as ImportQueueRecordViewModel)?.OnImportQueueRecordUpdated((ImportQueueRecord)e.NewValue, (ImportQueueRecord)e.OldValue); }));

        public ImportQueueRecord ImportQueueRecord
        {
            get { return (ImportQueueRecord)GetValue(ImportQueueRecordProperty); }
            set { SetValue(ImportQueueRecordProperty, value); }
        }

        #endregion
        #region HasError

        protected static readonly DependencyPropertyKey HasErrorProperty
            = DependencyProperty.RegisterReadOnly(nameof(HasError), typeof(bool), typeof(ImportQueueRecordViewModel),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { (s as ImportQueueRecordViewModel)?.RaiseCommands(); })));
        public static readonly DependencyProperty ReadOnlyHasErrorProperty = HasErrorProperty.DependencyProperty;

        public bool HasError
        {
            get { return (bool)GetValue(ReadOnlyHasErrorProperty); }
            protected set { SetValue(HasErrorProperty, value); }
        }

        #endregion
        #region DeleteCommandInternal

        private static readonly DependencyPropertyKey DeleteCommandInternalPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(DeleteCommandInternal), typeof(DelegateCommand), typeof(ImportQueueRecordViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyDeleteCommandInternalPropertyKey = DeleteCommandInternalPropertyKey.DependencyProperty;

        public DelegateCommand DeleteCommandInternal
        {
            get { return (DelegateCommand)GetValue(ReadOnlyDeleteCommandInternalPropertyKey); }
            private set { SetValue(DeleteCommandInternalPropertyKey, value); }
        }

        #endregion
        #region DeleteCommand

        public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand),
            typeof(ImportQueueRecordViewModel), new PropertyMetadata(null, (s, e) => { (s as ImportQueueRecordViewModel)?.RaiseCommands(); }));

        public ICommand DeleteCommand
        {
            get { return (ICommand)GetValue(DeleteCommandProperty); }
            set { SetValue(DeleteCommandProperty, value); }
        }

        #endregion

        public ImportQueueRecordViewModel()
        {
            DeleteCommandInternal = new DelegateCommand((o) => DeleteCommand?.Execute(o), 
                o => {
                    var record = (o as ImportQueueRecord) ?? ImportQueueRecord;
                    return DeleteCommand != null 
                        && record != null 
                        && record.FileInfoes != null 
                        && record.FileInfoes.All(fi => !fi.Started.HasValue);
            });
        }

        private void OnImportQueueRecordUpdated(ImportQueueRecord newValue, ImportQueueRecord oldValue)
        {
            if (oldValue != null)
                oldValue.PropertyChanged -= ImportQueueRecord_PropertyChanged;
            if (newValue != null)
                newValue.PropertyChanged += ImportQueueRecord_PropertyChanged;
            RaiseCommands();
        }

        private void ImportQueueRecord_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaiseCommands();
        }

        protected override void RaiseCommands()
        {
            base.RaiseCommands();
            DeleteCommandInternal?.RaiseCanExecuteChanged();
        }

    }
}
