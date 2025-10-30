using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// WindowExample.xaml 的交互逻辑
    /// </summary>
    public partial class WindowExample : UserControl
    {
        public WindowExample()
        {
            InitializeComponent();
        }

        private void BtnHighTitleBar_Click(object sender, RoutedEventArgs e)
        {
            new HighTitleBarWindow().MaskShowDialog();
        }
        private void BtnNormal_Click(object sender, RoutedEventArgs e)
        {
            new NormalWindow().MaskShowDialog();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new ToolWindow().MaskShowDialog();
        }

        private void ButtonNone_Click(object sender, RoutedEventArgs e)
        {
            new NoneTitleBarWindow().MaskShowDialog();
        }
    }
}
