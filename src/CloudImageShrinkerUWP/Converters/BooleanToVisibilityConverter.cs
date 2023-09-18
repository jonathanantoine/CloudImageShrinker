using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace CloudImageShrinkerUWP.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (bool.TryParse(value?.ToString(), out bool converted))
            {
                return converted ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}