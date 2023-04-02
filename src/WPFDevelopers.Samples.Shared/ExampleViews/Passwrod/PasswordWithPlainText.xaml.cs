using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFDevelopers.Samples.Passwrod
{
    /// <summary>
    /// PasswordWithPlainText.xaml 的交互逻辑
    /// </summary>
    public partial class PasswordWithPlainText : UserControl
    {
        public PasswordWithPlainText()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 图标
        /// </summary>
        public static readonly DependencyProperty IconImageProperty =
                             DependencyProperty.Register("IconImage", typeof(ImageSource), typeof(PasswordWithPlainText), new PropertyMetadata(new BitmapImage(new Uri("pack://application:,,,/WPFDevelopers.Samples;component/Resources/Images/Passwrod/Lock_48px.png"))));
        /// <summary>
        /// 图标
        /// </summary>
        public ImageSource IconImage
        {
            get { return (ImageSource)GetValue(IconImageProperty); }
            set { SetValue(IconImageProperty, value); }
        }
        /// <summary>
        /// 文本框提示文字
        /// </summary>
        public string Hint
        {
            get { return (string)GetValue(HintProperty); }
            set { SetValue(HintProperty, value); }
        }
        public static readonly DependencyProperty HintProperty =
            DependencyProperty.Register("Hint", typeof(string), typeof(PasswordWithPlainText), new PropertyMetadata(null));
        /// <summary>
        /// 获取或设置水印背景的水平对齐方式
        /// </summary>
        public AlignmentX AlignmentX
        {
            get { return (AlignmentX)GetValue(AlignmentXProperty); }
            set { SetValue(AlignmentXProperty, value); }
        }

        public static readonly DependencyProperty AlignmentXProperty =
            DependencyProperty.Register("AlignmentX", typeof(AlignmentX), typeof(PasswordWithPlainText), new PropertyMetadata(AlignmentX.Left));
        /// <summary>
        /// 获取或设置密码
        /// </summary>
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(PasswordWithPlainText), new PropertyMetadata(null));
        /// <summary>
        /// 获取或设置光标颜色
        /// </summary>
        public Brush CaretBrush
        {
            get { return (Brush)GetValue(CaretBrushProperty); }
            set { SetValue(CaretBrushProperty, value); }
        }

        public static readonly DependencyProperty CaretBrushProperty =
            DependencyProperty.Register("CaretBrush", typeof(Brush), typeof(PasswordWithPlainText), new PropertyMetadata(Brushes.Black));


        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(PasswordWithPlainText),
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

        /// <summary>
        /// 切换成明文
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
       
        private void ImageToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            pwdCiphertext.Visibility = Visibility.Collapsed;
            tbPlainText.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// 切换成密文
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            pwdCiphertext.Visibility = Visibility.Visible;
            tbPlainText.Visibility = Visibility.Collapsed;
        }
        private void pwdCiphertext_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox pwd = sender as PasswordBox;
            if (pwd != null)
            {
                if (!string.IsNullOrEmpty(pwd.Password))
                {
                    pwd.Background = Brushes.Transparent;
                }
                else
                {
                    pwd.Background = (Brush)FindResource("HelpBrush");
                }
            }
        }

       
    }
}
