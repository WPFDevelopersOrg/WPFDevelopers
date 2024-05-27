using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class AnimationNavigationBar3D : ItemsControl
    {
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(int), typeof(AnimationNavigationBar3D),
                new PropertyMetadata(null));

        static AnimationNavigationBar3D()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimationNavigationBar3D),
                new FrameworkPropertyMetadata(typeof(AnimationNavigationBar3D)));
        }


        public int Columns
        {
            get => (int)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Columns = Items.Count;
        }
    }
}