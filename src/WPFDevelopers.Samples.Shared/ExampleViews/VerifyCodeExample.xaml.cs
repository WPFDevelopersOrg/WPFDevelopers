using System.Text;
using System.Windows.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// CheckCodeExample.xaml 的交互逻辑
    /// </summary>
    public partial class VerifyCodeExample : UserControl
    {
        public VerifyCodeExample()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"获取到验证码如下：");
            stringBuilder.AppendLine(VerifyCode1.VerifyCodeText);
            stringBuilder.AppendLine(VerifyCode2.VerifyCodeText);
            stringBuilder.AppendLine(VerifyCode3.VerifyCodeText);
            stringBuilder.AppendLine(VerifyCode4.VerifyCodeText);
            WPFDevelopers.Controls.MessageBox.Show(stringBuilder.ToString(),
                "验证码",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }
    }
}
