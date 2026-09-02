using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WPFDevelopers.Helpers;

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

        private bool _themeAnimating;

        public void SwitchTheme(FrameworkElement source, ThemeType targetTheme)
        {
            if (_themeAnimating) return;
            _themeAnimating = true;

            var window = Application.Current.MainWindow;
            if (window == null) { _themeAnimating = false; return; }

            var layoutRoot = ControlsHelper.FindVisualChild<Grid>(window);
            if (layoutRoot == null)
            {
                _themeAnimating = false;
                return;
            }

            double w = layoutRoot.ActualWidth;
            double h = layoutRoot.ActualHeight;
            if (w <= 0 || h <= 0)
            {
                _themeAnimating = false;
                return;
            }

            var clickPoint = source.TranslatePoint(
                new Point(source.ActualWidth / 2, source.ActualHeight / 2), layoutRoot);

            double maxRadius = Math.Sqrt(
                Math.Pow(Math.Max(clickPoint.X, w - clickPoint.X), 2) +
                Math.Pow(Math.Max(clickPoint.Y, h - clickPoint.Y), 2));

            var currentBg = BackgroundBrush as SolidColorBrush;
            var overlayColor = currentBg?.Color ?? Colors.Black;

            var overlay = new Border
            {
                Background = new SolidColorBrush(overlayColor),
                IsHitTestVisible = false
            };
            Grid.SetRow(overlay, 0);
            Grid.SetRowSpan(overlay, 2);
            Grid.SetColumn(overlay, 0);
            Grid.SetColumnSpan(overlay, 3);

            var circle = new EllipseGeometry { Center = clickPoint, RadiusX = 0, RadiusY = 0 };
            var rect = new RectangleGeometry(new Rect(0, 0, w, h));
            overlay.Clip = new CombinedGeometry(GeometryCombineMode.Exclude, rect, circle);

            layoutRoot.Children.Add(overlay);

            SetTheme(targetTheme);

            double duration = 400;
            var easing = new CubicEase { EasingMode = EasingMode.EaseOut };
            var animX = new DoubleAnimation(0, maxRadius, TimeSpan.FromMilliseconds(duration)) { EasingFunction = easing };
            var animY = new DoubleAnimation(0, maxRadius, TimeSpan.FromMilliseconds(duration)) { EasingFunction = easing };

            animX.Completed += (s, args) =>
            {
                layoutRoot.Children.Remove(overlay);
                overlay.Clip = null;
                _themeAnimating = false;
            };

            circle.BeginAnimation(EllipseGeometry.RadiusXProperty, animX);
            circle.BeginAnimation(EllipseGeometry.RadiusYProperty, animY);
        }
    }
}
