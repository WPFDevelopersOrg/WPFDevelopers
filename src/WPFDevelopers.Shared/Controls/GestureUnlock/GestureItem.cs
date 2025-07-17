using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class GestureItem : Control
    {
        public static readonly DependencyProperty NumberProperty =
            DependencyProperty.Register("Number", typeof(int), typeof(GestureItem), new PropertyMetadata(0));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(GestureItem), new PropertyMetadata(false));

        static GestureItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GestureItem),
                new FrameworkPropertyMetadata(typeof(GestureItem)));
        }

        public int Number
        {
            get => (int) GetValue(NumberProperty);
            set => SetValue(NumberProperty, value);
        }

        public bool IsSelected
        {
            get => (bool) GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
    }
}