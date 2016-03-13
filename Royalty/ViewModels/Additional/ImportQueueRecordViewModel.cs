using RoyaltyServiceWorker.AccountService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        private void OnImportQueueRecordUpdated(ImportQueueRecord newValue, ImportQueueRecord oldValue)
        {
            if (oldValue != null)
                oldValue.PropertyChanged -= ImportQueueRecord_PropertyChanged;
            if (newValue != null)
                newValue.PropertyChanged += ImportQueueRecord_PropertyChanged;
        }

        private void ImportQueueRecord_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ImportQueueRecord.HasError))
            {

            }
        }
    }
}
