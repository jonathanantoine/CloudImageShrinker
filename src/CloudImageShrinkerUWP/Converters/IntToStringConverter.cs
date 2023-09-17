using System;
using Windows.UI.Xaml.Data;

namespace CloudImageShrinkerUWP.Converters
{
    public class IntToStringConverter : IValueConverter
    {
        /// <summary>
        /// Convert from int to XAML string. Removes zero as defaultvalue in XAML</summary>
        /// 
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((int)value == 0)
            {
                return string.Empty;
            }
            return value;
        }

        /// <summary>
        /// Convert from XAML string to int. Return 0 instead of null or empty string or if parsing is not successful
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (!String.IsNullOrEmpty((string)value))
            {
                bool canParse = int.TryParse((string)value, out var valueAsInt);

                if (canParse)
                {
                    return valueAsInt;
                }
            }
            return 0;
        }
    }
}