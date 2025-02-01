using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// NavMenu3DExample.xaml 的交互逻辑
    /// </summary>
    public partial class NavMenu3DExample : UserControl
    {
        public NavMenu3DExample()
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

        private void NavMenu3D_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PART_NavMenu3D.SelectedItem == null) return;
            var item = PART_NavMenu3D.SelectedItem as NavMenu3DItem;
            if (item.Tag.ToString() != null)
                Message.Push(item.Tag.ToString());
        }
    }
}
