using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class NormalLoading: Control
    {
        public static readonly DependencyProperty StrokeArrayProperty =
            DependencyProperty.Register("StrokeArray", typeof(DoubleCollection), typeof(NormalLoading)
                , new PropertyMetadata(new DoubleCollection {20, 100}));

        static NormalLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NormalLoading),
                new FrameworkPropertyMetadata(typeof(NormalLoading)));
        }

        public DoubleCollection StrokeArray
        {
            get => (DoubleCollection) GetValue(StrokeArrayProperty);
            set => SetValue(StrokeArrayProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }
}