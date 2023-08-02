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

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(nameof(Data), typeof(Geometry), typeof(PathIcon));

        public PackIconKind Kind
        {
            get { return (PackIconKind)GetValue(KindProperty); }
            set { SetValue(KindProperty, value); }
        }

        public Geometry Data
        {
            get { return (Geometry)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        private static void OnKindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pathIcon = (PathIcon)d;
            var kind = (string)e.NewValue;
            if (!string.IsNullOrWhiteSpace(kind))
            {
                kind = $"WD.{kind}Geometry";
                pathIcon.Data = (Geometry)pathIcon.FindResource(kind);
            }
            else
                pathIcon.Data = null;
        }

        static PathIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathIcon), new FrameworkPropertyMetadata(typeof(PathIcon)));
        }
    }
}
