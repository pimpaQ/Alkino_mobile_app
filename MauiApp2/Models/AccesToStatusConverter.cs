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
                if( acces == 0)
                {
                    return "Ожидание подтверждения";
                }
                else if (acces == 1)
                {
                    return "Запись подтверждена";
                }
                else
                {
                    return "Запись отклонена";
                }
                
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
