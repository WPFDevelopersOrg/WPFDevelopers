using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Controls;
using MessageBox = WPFDevelopers.Controls.MessageBox;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// NotifyIconExample.xaml 的交互逻辑
    /// </summary>
    public partial class NotifyIconExample : UserControl
    {
        private int _clickCount;
        private int _doubleClickCount;

        public NotifyIconExample()
        {
            InitializeComponent();
        }

        private void NotifyIcon_Click(object sender, RoutedEventArgs e)
        {
            _clickCount++;
            ClickCountText.Text = _clickCount.ToString();
        }

        private void NotifyIcon_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            _doubleClickCount++;
            DoubleClickCountText.Text = _doubleClickCount.ToString();
        }

        private void BtnNotifyIconMessage_Click(object sender, RoutedEventArgs e)
        {
            var msg = "Welcome to WPFDevelopers ";
            if (!string.IsNullOrWhiteSpace(MyTextBox.Text))
                msg = MyTextBox.Text;
            NotifyIcon.ShowBalloonTip("Message", msg, NotifyIconInfoType.None);
        }

        private void IsTwink_Checked(object sender, RoutedEventArgs e)
        {
            NotifyIconEvents.IsTwink = true;
        }

        private void UnIsTwink_Checked(object sender, RoutedEventArgs e)
        {
            NotifyIconEvents.IsTwink = false;
        }

        private void ContextContent_Send_Click(object sender, RoutedEventArgs e)
        {
            NotifyIcon.ShowBalloonTip("来自 ContextContent", "右键面板发送的消息", NotifyIconInfoType.Info);
        }

        private void ContextContent_Twink_Click(object sender, RoutedEventArgs e)
        {
            NotifyIconEvents.IsTwink = !NotifyIconEvents.IsTwink;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("WPFDevelopers NotifyIcon 示例", "关于", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            NotifyIconEvents.Dispose();
        }
    }
}
