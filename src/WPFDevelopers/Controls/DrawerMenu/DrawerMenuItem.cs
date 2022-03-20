using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class DrawerMenuItem : ListBoxItem
    {
        static DrawerMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DrawerMenuItem), new FrameworkPropertyMetadata(typeof(DrawerMenuItem)));
        }
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(DrawerMenuItem), new PropertyMetadata(string.Empty));


        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(DrawerMenuItem), new PropertyMetadata(null));

        public Brush SelectionIndicatorColor
        {
            get { return (Brush)GetValue(SelectionIndicatorColorProperty); }
            set { SetValue(SelectionIndicatorColorProperty, value); }
        }

        public static readonly DependencyProperty SelectionIndicatorColorProperty =
            DependencyProperty.Register("SelectionIndicatorColor", typeof(Brush), typeof(DrawerMenuItem), new PropertyMetadata(DrawingContextHelper.Brush));

        public ICommand SelectionCommand
        {
            get { return (ICommand)GetValue(SelectionCommandProperty); }
            set { SetValue(SelectionCommandProperty, value); }
        }

        public static readonly DependencyProperty SelectionCommandProperty =
            DependencyProperty.Register("SelectionCommand", typeof(ICommand), typeof(DrawerMenuItem), new PropertyMetadata(null));
    }
}
