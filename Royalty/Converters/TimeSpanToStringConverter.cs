using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Royalty.Converters
{
    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value is TimeSpan)
                {
                    return ((TimeSpan)value).ToString();
                }
                if (value is TimeSpan?)
                {
                    return ((TimeSpan?)value).Value.ToString();
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace((string)value))
            {
                if (targetType == typeof(TimeSpan))
                    return default(TimeSpan);
                if (targetType == typeof(TimeSpan?))
                    return null;
            }

            var ts = TimeSpan.Parse((string)value, culture);
            if (targetType == typeof(TimeSpan))
                return ts;
            if (targetType == typeof(TimeSpan?))
                return (TimeSpan?)ts;

            return null;
        }
    }
}
