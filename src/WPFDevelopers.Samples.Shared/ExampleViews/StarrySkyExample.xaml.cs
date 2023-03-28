using System.Windows.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// StarrySkyExample.xaml 的交互逻辑
    /// </summary>
    public partial class StarrySkyExample : UserControl
    {
        public StarrySkyExample()
        {
            InitializeComponent();
        }

        private void btn_render_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            myStarrySky.InitStar();
        }
    }
}
