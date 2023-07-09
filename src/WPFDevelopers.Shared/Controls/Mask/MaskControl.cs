using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class MaskControl : ContentControl
    {
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MaskControl),
                new PropertyMetadata(new CornerRadius(0)));

        private readonly Visual visual;

        static MaskControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MaskControl),
                new FrameworkPropertyMetadata(typeof(MaskControl)));
        }

        public MaskControl(Visual _visual)
        {
            visual = _visual;
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius) GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
    }
}