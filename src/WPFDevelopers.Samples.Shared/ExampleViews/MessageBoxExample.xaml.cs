using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Controls;
using MessageBox = WPFDevelopers.Controls.MessageBox;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// MessageBoxExample.xaml 的交互逻辑
    /// </summary>
    public partial class MessageBoxExample : UserControl
    {
        public MessageBoxExample()
        {
            InitializeComponent();
        }
        private void btnInformation_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("文件删除成功。", "消息", MessageBoxButton.OK, MessageBoxImage.Information, buttonRadius: 4);
        }

        private void btnWarning_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("执行此操作可能导致文件无法打开！", "警告", MessageBoxImage.Warning);
        }

        private void btnError_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("当前文件不存在。", "错误", MessageBoxImage.Error);
        }

        private void btnQuestion_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("当前文件不存在,是否继续?", "询问", MessageBoxButton.OKCancel, MessageBoxImage.Question, buttonRadius: 4);
        }
        private void btnMsgBoxYesNoCancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("当前文件不存在,是否继续?", "询问", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
                Toast.Push(result.ToString(),ToastImage.Success);
        }
    }
}
