using System.Collections.Generic;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Core
{
    public class ThemeManager
    {
        private Resources _cachedResources;
        private static ThemeManager _instance;
        public static ThemeManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ThemeManager();
                }
                return _instance;
            }
        }
        public Color PrimaryColor
        {
            get => (Color)Application.Current?.TryFindResource("WD.PrimaryColor");

        }

        public Brush PrimaryBrush
        {
            get => (Brush)Application.Current?.TryFindResource("WD.PrimaryBrush");

        }
        public Brush BackgroundBrush
        {
            get => (Brush)Application.Current?.TryFindResource("WD.BackgroundBrush");
        }

        public Dictionary<Tuple<ThemeType, Color, double>, SolidColorBrush> PrimaryColorCache = new Dictionary<Tuple<ThemeType, Color, double>, SolidColorBrush>();

        public Dictionary<Color, Dictionary<Tuple<ThemeType, string>, Color>> ColorCache =
            new Dictionary<Color, Dictionary<Tuple<ThemeType, string>, Color>>();

        
        public Resources GetResources()
        {
            if (_cachedResources != null)
                return _cachedResources;

            _cachedResources = (Resources)Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(x => x is Resources);

            return _cachedResources;
        }

        public void SetTheme(ThemeType themeType)
        {
            var resources = GetResources();
            if (resources != null)
                resources.Theme = themeType;
        }

        public void SetColor(Color color) 
        {
            var resources = GetResources();
            if (resources != null)
                resources.Color = color;
        }
    }
}
