using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// AnimationNavigationBar3DExample.xaml 的交互逻辑
    /// </summary>
    public partial class AnimationNavigationBar3DExample : UserControl
    {
        public AnimationNavigationBar3DExample()
        {
            InitializeComponent();
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
    }
}
