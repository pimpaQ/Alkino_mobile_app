using System;
using System.Globalization;

namespace MauiApp1.Models
{
    public class AccesToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int acces)
            {
                return acces == 0 ? "Ожидание подтверждения" : "Запись подтверждена";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
