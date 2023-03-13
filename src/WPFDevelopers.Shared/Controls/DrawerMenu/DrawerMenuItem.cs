using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class DrawerMenuItem : ListBoxItem
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(DrawerMenuItem),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(object), typeof(DrawerMenuItem),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SelectionIndicatorColorProperty =
            DependencyProperty.Register("SelectionIndicatorColor", typeof(Brush), typeof(DrawerMenuItem),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SelectionCommandProperty =
            DependencyProperty.Register("SelectionCommand", typeof(ICommand), typeof(DrawerMenuItem),
                new PropertyMetadata(null));

        static DrawerMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DrawerMenuItem),
                new FrameworkPropertyMetadata(typeof(DrawerMenuItem)));
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }


        public object Icon
        {
            get => (object)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public Brush SelectionIndicatorColor
        {
            get => (Brush)GetValue(SelectionIndicatorColorProperty);
            set => SetValue(SelectionIndicatorColorProperty, value);
        }

        public ICommand SelectionCommand
        {
            get => (ICommand)GetValue(SelectionCommandProperty);
            set => SetValue(SelectionCommandProperty, value);
        }
    }
}