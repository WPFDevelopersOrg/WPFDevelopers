using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPFDevelopers.Controls
{
    public class DefaultLoading : Control
    {
        public static readonly DependencyProperty StrokeArrayProperty =
            DependencyProperty.Register("StrokeArray", typeof(DoubleCollection), typeof(DefaultLoading)
                , new PropertyMetadata(new DoubleCollection { 20, 100 }));
        public static DefaultLoading Default = new DefaultLoading();
        static DefaultLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DefaultLoading),
                new FrameworkPropertyMetadata(typeof(DefaultLoading)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
        public DoubleCollection StrokeArray
        {
            get => (DoubleCollection)GetValue(StrokeArrayProperty);
            set => SetValue(StrokeArrayProperty, value);
        }
    }
}
