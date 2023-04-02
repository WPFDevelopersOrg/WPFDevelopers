using System.Windows.Controls;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// MaskExample.xaml 的交互逻辑
    /// </summary>
    public partial class MaskExample : UserControl
    {
        public MaskExample()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new AboutWindow().MaskShowDialog();
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            new AboutWindow().MaskShow();
        }
    }
}
