using RoyaltyServiceWorker.AccountService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Royalty.Converters
{
    public class ImportQueueRecordStateGetterMultiValueConverter : FrameworkElement, IValueConverter
    {
        #region States

        public static readonly DependencyProperty StatesProperty = DependencyProperty.Register(nameof(States), typeof(ICollectionView),
            typeof(ImportQueueRecordStateGetterMultiValueConverter), new PropertyMetadata(null, (s, e) => { }));

        public ICollectionView States
        {
            get { return (ICollectionView)GetValue(StatesProperty); }
            set { SetValue(StatesProperty, value); }
        }

        #endregion

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (States != null && States.SourceCollection != null && value != null)
            {
                var id = System.Convert.ToInt64(value);
                var state = States.SourceCollection.OfType<ImportQueueRecordState>().FirstOrDefault(s => s.Id == id);
                if (state != null)
                    return state.Name;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
