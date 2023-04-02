#if NET40
using Microsoft.Windows.Shell;
#else
using System.Windows.Shell;
# endif
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    ///     Window1.xaml 的交互逻辑
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
#if NET40
            var windowChrome = new WindowChrome
            {
                CaptionHeight = 160,
                GlassFrameThickness = new Thickness(1)
            };
            WindowChrome.SetIsHitTestVisibleInChrome(CloseButton, true);
#else
            var windowChrome = new WindowChrome
            {
                CaptionHeight = 160,
                GlassFrameThickness = new Thickness(1),
                UseAeroCaptionButtons = false
            };
             WindowChrome.SetIsHitTestVisibleInChrome(CloseButton, true);
#endif
        }

        private void GithubHyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void GiteeHyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void QQHyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var uri = new Uri(@"https://qm.qq.com/cgi-bin/qm/qr?k=f2zl3nvoetItho8kGfe1eys0jDkqvvcL&jump_from=webapi");
            Process.Start(new ProcessStartInfo(uri.AbsoluteUri));
            e.Handled = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}