using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WPFDevelopers.Core;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Samples.ExampleViews.Basics
{
    public partial class ThemeExample : UserControl
    {
        public ThemeExample()
        {
            InitializeComponent();
            SampleLanguageManager.Instance.RegisterExampleView(this);
        }
        
        private void GithubHyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void GiteeHyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            BrowserHelper.OpenUrl(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        private void QQHyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var uri = new Uri(@"https://qm.qq.com/cgi-bin/qm/qr?k=f2zl3nvoetItho8kGfe1eys0jDkqvvcL&jump_from=webapi");
            BrowserHelper.OpenUrl(uri);
            e.Handled = true;
        }

    }
}
