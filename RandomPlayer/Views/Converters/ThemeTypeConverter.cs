using RandomPlayer.Models.Theme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RandomPlayer.Views.Converters
{
    [ValueConversion(typeof(ThemeType), typeof(bool))]
    public class ThemeTypeConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean.");

            if (parameter == null)
                throw new InvalidOperationException("The parameter can't be null.");

            if (value is ThemeType && (ThemeType)value == (ThemeType)Enum.Parse(typeof(ThemeType), parameter.ToString()))
                return true;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(ThemeType))
                throw new InvalidOperationException("The target must be a ThemeType.");

            // Always return the type of the selected value.
            return (ThemeType)Enum.Parse(typeof(ThemeType), parameter.ToString());
        }
        #endregion
    }
}
