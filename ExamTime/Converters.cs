using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace ExamTime
{
    public class BooleanNotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool was)
                return !was;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool was)
                return !was;
            else
                return true;
        }
    }
}