using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class MapPin : ContentControl
    {
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(MapPin),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty IsActivatedProperty =
            DependencyProperty.Register(nameof(IsActivated), typeof(bool), typeof(MapPin),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty AnchorPointProperty =
            DependencyProperty.Register(nameof(AnchorPoint), typeof(Point), typeof(MapPin),
                new FrameworkPropertyMetadata(new Point(0.5, 1.0), FrameworkPropertyMetadataOptions.AffectsArrange));

        static MapPin()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MapPin), new FrameworkPropertyMetadata(typeof(MapPin)));
        }

        public MapPin()
        {
            IsHitTestVisible = false;
            Focusable = false;
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public bool IsActivated
        {
            get { return (bool)GetValue(IsActivatedProperty); }
            set { SetValue(IsActivatedProperty, value); }
        }

        public Point AnchorPoint
        {
            get { return (Point)GetValue(AnchorPointProperty); }
            set { SetValue(AnchorPointProperty, value); }
        }
    }
}
