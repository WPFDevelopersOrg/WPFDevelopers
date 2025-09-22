using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace WPFDevelopers
{
    public class ThemeManager
    {

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

        public Resources Resources { get; set; }

        public Color PrimaryColor
        {
            get => Resources.TryFindResource<Color>("WD.PrimaryColor");

        }

        public Brush PrimaryBrush
        {
            get => Resources.TryFindResource<Brush>("WD.PrimaryBrush");

        }
        public Brush BackgroundBrush
        {
            get => Resources.TryFindResource<Brush>("WD.BackgroundBrush");
        }
        public Brush PrimaryTextBrush
        {
            get => Resources.TryFindResource<Brush>("WD.PrimaryTextBrush");

        }

        public Dictionary<Tuple<ThemeType, Color, double>, SolidColorBrush> PrimaryColorCache = new Dictionary<Tuple<ThemeType, Color, double>, SolidColorBrush>();

        public Dictionary<Color, Dictionary<Tuple<ThemeType, string>, Color>> ColorCache =
            new Dictionary<Color, Dictionary<Tuple<ThemeType, string>, Color>>();

        public void SetTheme(ThemeType themeType)
        {
            if (Resources != null)
                Resources.Theme = themeType;
        }

        public void SetColor(Color color) 
        {
            if (Resources != null)
                Resources.Color = color;
        }
    }
}
