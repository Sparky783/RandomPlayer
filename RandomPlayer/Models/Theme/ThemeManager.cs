using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RandomPlayer.Models.Theme
{
    /// <summary>
    /// Theme manager used to choose th display color for the application.
    /// </summary>
    public class ThemeManager : ResourceDictionary
    {
        private Dictionary<ThemeType, Uri> _theme = new Dictionary<ThemeType, Uri>();
        
        public ThemeManager()
        {
            // Initialize theme manager
            App.Current.Resources.MergedDictionaries.Clear();
            App.Current.Resources.MergedDictionaries.Add(this);
            InitThemes();

            // Load the default theme
            ChangeTheme(ThemeType.Light);
        }

        #region Methods
        /// <summary>
        /// Change the current theme.
        /// </summary>
        /// <param name="type"></param>
        public void ChangeTheme(ThemeType type)
        {
            if (!_theme.ContainsKey(type))
                throw new ArgumentOutOfRangeException("type", "The theme type does not exist.");

            Source = _theme[type];
        }

        /// <summary>
        /// Convert the theme type to an int value.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static public int ConvertThemeTypeToInt(ThemeType type)
        {
            return (int)type;
        }

        /// <summary>
        /// Convert an int value to a theme type.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static public ThemeType ConvertIntToThemeType(int value)
        {
            return (ThemeType)value;
        }
        #endregion

        /// <summary>
        /// Initialize all themes for user.
        /// </summary>
        private void InitThemes()
        {
            _theme.Add(ThemeType.Light, new Uri(@"Views\Dictionaries\ThemeLight.xaml", UriKind.Relative));
            _theme.Add(ThemeType.Dark, new Uri(@"Views\Dictionaries\ThemeDark.xaml", UriKind.Relative));
        }
    }
}
