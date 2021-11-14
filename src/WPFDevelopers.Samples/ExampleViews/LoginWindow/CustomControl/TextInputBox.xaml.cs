using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WPFDevelopers.Samples.ExampleViews.LoginWindow.CustomControl
{
    public partial class TextInputBox : InputBoxBase
    {
        public TextInputBox()
        {
            InitializeComponent();
        }

        #region 接口实现

        protected override void ApplyText()
        {
            TextBox01.Text = Text;
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

        private void TextBox01_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = TextBox01.Text;
            // 显示或隐藏“清除按钮”与“占位文本”
            Clear.Visibility = Text == "" ? Visibility.Hidden : Visibility.Visible;
            Hint.Visibility = Text == "" ? Visibility.Visible : Visibility.Hidden;
        }
    }
}