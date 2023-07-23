using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class PathIcon : Control
    {
        public static readonly DependencyProperty KindProperty =
            DependencyProperty.Register(nameof(Kind), typeof(string), typeof(PathIcon),
                new PropertyMetadata(string.Empty, OnKindChanged));

        public static readonly DependencyProperty PathDataProperty =
            DependencyProperty.Register(nameof(PathData), typeof(Geometry), typeof(PathIcon));

        public string Kind
        {
            get { return (string)GetValue(KindProperty); }
            set { SetValue(KindProperty, value); }
        }

        public Geometry PathData
        {
            get { return (Geometry)GetValue(PathDataProperty); }
            set { SetValue(PathDataProperty, value); }
        }

        private static void OnKindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pathIcon = (PathIcon)d;
            var kind = (string)e.NewValue;
            if (!string.IsNullOrWhiteSpace(kind))
            {
                kind = $"WD.{kind}Geometry";
                pathIcon.PathData = (Geometry)pathIcon.FindResource(kind);
            }
            else
                pathIcon.PathData = null;
        }

        static PathIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathIcon), new FrameworkPropertyMetadata(typeof(PathIcon)));
        }
    }
}
