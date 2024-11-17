using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class RollLoading : LoadingBase
    {
        public static readonly DependencyProperty ForegroundColorProperty =
            DependencyProperty.Register("ForegroundColor", typeof(Color), typeof(RollLoading),
                new PropertyMetadata(Colors.Red));

        static RollLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RollLoading),
                new FrameworkPropertyMetadata(typeof(RollLoading)));
        }

        public Color ForegroundColor
        {
            get => (Color)GetValue(ForegroundColorProperty);
            set => SetValue(ForegroundColorProperty, value);
        }
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }
}