using System.Windows;
using WPFDevelopers.Samples.ExampleViews.LoginWindow;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindowExample : WindowBase
    {
        public LoginWindowExample()
        {
            InitializeComponent();
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            SignUpDialog dialog = new SignUpDialog
            {
                Owner = this
            };
            dialog.ShowDialog();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitWindow();
        }
    }
}
