using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class PathIcon : Control
    {
        public static readonly DependencyProperty KindProperty =
            DependencyProperty.Register(nameof(Kind), typeof(PackIconKind?), typeof(PathIcon),
                new PropertyMetadata(null, OnKindChanged));

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(nameof(Data), typeof(Geometry), typeof(PathIcon));

        public PackIconKind? Kind
        {
            get { return (PackIconKind?)GetValue(KindProperty); }
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
            if (e.NewValue is PackIconKind kind)
                pathIcon.SetResourceReference(DataProperty, $"WD.{kind}Geometry");
            else
                pathIcon.ClearValue(DataProperty);
        }

        static PathIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathIcon), new FrameworkPropertyMetadata(typeof(PathIcon)));
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ForegroundProperty)
            {
                if (Parent is null && ReadLocalValue(ForegroundProperty) == DependencyProperty.UnsetValue)
                    SetResourceReference(ForegroundProperty, "WD.PrimaryTextBrush");
            }
        }
    }
}
