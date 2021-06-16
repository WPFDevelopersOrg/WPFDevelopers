using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WpfPasswrod.CustomControls
{
    public class ImageToggleButton : ToggleButton
    {
        static ImageToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageToggleButton), new FrameworkPropertyMetadata(typeof(ImageToggleButton)));
        }
        public static readonly DependencyProperty NormalImageProperty =
                        DependencyProperty.Register("NormalImage", typeof(ImageSource), typeof(ImageToggleButton), new PropertyMetadata(null));
        public ImageSource NormalImage
        {
            get { return (ImageSource)GetValue(NormalImageProperty); }
            set { SetValue(NormalImageProperty, value); }
        }

        public static readonly DependencyProperty UnImageProperty =
                       DependencyProperty.Register("UnImage", typeof(ImageSource), typeof(ImageToggleButton), new PropertyMetadata(null));
        public ImageSource UnImage
        {
            get { return (ImageSource)GetValue(UnImageProperty); }
            set { SetValue(UnImageProperty, value); }
        }


        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ImageToggleButton),
                                                                            new PropertyMetadata(new CornerRadius(0, 0, 0, 0), OnCornerRadiusChanged));
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }
        private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CornerRadius cornerRadius = new CornerRadius();
            cornerRadius = (CornerRadius)e.NewValue;
        }
    }
}
