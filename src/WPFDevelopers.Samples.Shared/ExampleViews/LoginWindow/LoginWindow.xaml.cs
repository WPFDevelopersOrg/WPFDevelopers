using System.Windows;
using WPFDevelopers.Samples.ExampleViews.LoginWindow;
#if NET40
using Microsoft.Windows.Shell;
#else
using System.Windows.Shell;
# endif

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
#if NET40
            var windowChrome = new WindowChrome
            {
                CaptionHeight = 100,
                GlassFrameThickness = new Thickness(1)
            };
#else
            var windowChrome = new WindowChrome
            {
                CaptionHeight = 100,
                GlassFrameThickness = new Thickness(1),
                UseAeroCaptionButtons = false
            };
#endif
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
