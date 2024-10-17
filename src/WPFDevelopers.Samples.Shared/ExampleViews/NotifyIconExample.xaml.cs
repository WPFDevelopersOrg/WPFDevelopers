using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// NotifyIconExample.xaml 的交互逻辑
    /// </summary>
    public partial class NotifyIconExample : UserControl
    {
        public NotifyIconExample()
        {
            InitializeComponent();
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
            NotifyIconTwink();
        }
        private void UnIsTwink_Checked(object sender, RoutedEventArgs e)
        {
            NotifyIconTwink();
        }
        void NotifyIconTwink()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow == null) return;
            mainWindow.IsTwink();
        }
    }
}
