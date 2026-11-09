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
            SampleLanguageManager.Instance.RegisterExampleView(this);
        }
        private void btnInformation_Click(object sender, RoutedEventArgs e)
        {
            var localizer = SampleLanguageManager.Instance;
            MessageBox.Show(localizer.TranslateText("文件删除成功。"), localizer.TranslateText("消息"),
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnWarning_Click(object sender, RoutedEventArgs e)
        {
            var localizer = SampleLanguageManager.Instance;
            MessageBox.Show(localizer.TranslateText("执行此操作可能导致文件无法打开！"),
                localizer.TranslateText("警告"), MessageBoxImage.Warning);
        }

        private void btnError_Click(object sender, RoutedEventArgs e)
        {
            var localizer = SampleLanguageManager.Instance;
            MessageBox.Show(localizer.TranslateText("当前文件不存在。"), localizer.TranslateText("错误"),
                MessageBoxImage.Error);
        }

        private void btnQuestion_Click(object sender, RoutedEventArgs e)
        {
            var localizer = SampleLanguageManager.Instance;
            MessageBox.Show(localizer.TranslateText("当前文件不存在,是否继续?"), localizer.TranslateText("询问"),
                MessageBoxButton.OKCancel, MessageBoxImage.Question);
        }
        private void btnMsgBoxYesNoCancel_Click(object sender, RoutedEventArgs e)
        {
            var localizer = SampleLanguageManager.Instance;
            var result = MessageBox.Show(localizer.TranslateText("当前文件不存在,是否继续?"),
                localizer.TranslateText("询问"), MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
                Toast.Push(localizer.TranslateText("Yes"), ToastImage.Success);
        }
    }
}
