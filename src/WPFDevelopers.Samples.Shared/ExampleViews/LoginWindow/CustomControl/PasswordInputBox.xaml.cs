using System.Windows;
using System.Windows.Media.Imaging;

namespace WPFDevelopers.Samples.ExampleViews.LoginWindow.CustomControl
{
    public partial class PasswordInputBox : InputBoxBase
    {
        public PasswordInputBox()
        {
            InitializeComponent();
        }

        #region 接口实现

        protected override void ApplyText()
        {
            if (PasswordBox01.Password != Text)
                PasswordBox01.Password = Text;
        }
        protected override void ApplyPlaceHolder()
        {
            Hint.Text = PlaceHolder;
        }

        protected override void ApplyIcon(BitmapImage icon)
        {
            ImageIcon.Source = icon;
        }

        #endregion

        private void PasswordBox01_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Text = PasswordBox01.Password;
            // 显示或隐藏“清除按钮”与“占位文本”
            Clear.Visibility = Text == "" ? Visibility.Hidden : Visibility.Visible;
            Hint.Visibility = Text == "" ? Visibility.Visible : Visibility.Hidden;
        }
    }
}