using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Client.Converters
{
    class BoolToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            switch ((bool?)value)
            {
                case true:
                    return Brushes.Green;
                case false:
                    return Brushes.Red;
                default:
                    return Brushes.Lavender;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            Brush val = (Brush)value;
            if (val == Brushes.Green)
            {
                return true;
            }
            if (val == Brushes.Red)
            {
                return false;
            }
            return null;
        }
    }
}
