using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// ShakeExample.xaml 的交互逻辑
    /// </summary>
    public partial class ShakeExample : UserControl
    {
        public ShakeExample()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ControlsHelper.WindowShake();
        }
    }
}
