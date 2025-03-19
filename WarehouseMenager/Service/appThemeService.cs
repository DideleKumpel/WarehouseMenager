using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WarehouseMenager.Service
{
    internal class appThemeService
    {
        private static appThemeService _instance;

        public static appThemeService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new appThemeService();
                }
                return _instance;
            }
        }

        private ResourceDictionary _currentThemeDictionary;

        // Konstruktory prywatne w Singletonach
        private appThemeService() { }
        public void ToggleTheme()
        {
            try
            {
                string currentTheme = GetCurrentThemeName();

                string newThemeName = currentTheme == "LightTheme" ? "DarkTheme" : "LightTheme";

                string themePath = string.Format("/Resources/Themes/{0}.xaml", newThemeName);

                var newTheme = new ResourceDictionary
                {
                    Source = new Uri(themePath, UriKind.Relative)
                };

                if (_currentThemeDictionary != null)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(_currentThemeDictionary);
                }

                Application.Current.Resources.MergedDictionaries.Add(newTheme);

                _currentThemeDictionary = newTheme;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to switch theme: " + ex.Message);
            }
        }


        private string GetCurrentThemeName()
            {
                if (_currentThemeDictionary != null && _currentThemeDictionary.Source != null)
                {
                    var uri = _currentThemeDictionary.Source.ToString();
    
                    if (uri.Contains("LightTheme.xaml"))
                       return "LightTheme";

                   if (uri.Contains("DarkTheme.xaml"))
                        return "DarkTheme";
                }

               return "LightTheme";
            }
    }
}

