using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// BarrageExample.xaml 的交互逻辑
    /// </summary>
    public partial class IPEditBoxExample : UserControl
    {
        public IPEditBoxExample()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var text = myIPEditBox.Text;
            if (!string.IsNullOrWhiteSpace(text))
                Message.Push(text, MessageBoxImage.Information);
        }
    }
}
