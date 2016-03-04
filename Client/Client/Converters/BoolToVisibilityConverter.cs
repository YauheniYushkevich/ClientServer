using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace Client.Converters
{
    class BoolToVisibilityConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            switch ((Visibility)value)
            {
                case Visibility.Collapsed:
                    return false;
                case Visibility.Visible:
                    return true;
                default:
                    return false;
            }
        }
    }
}
