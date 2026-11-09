using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Controls;
using MessageBox = WPFDevelopers.Controls.MessageBox;

namespace WPFDevelopers.Samples.ExampleViews
{
    public partial class NotifyIconExample : UserControl
    {
        private NotifyIcon _notifyIcon;
        private FrameworkElement _trayPanel;
        private int _clickCount;
        private int _doubleClickCount;

        public NotifyIconExample()
        {
            InitializeComponent();
        }

        private void NotifyIconExample_Loaded(object sender, RoutedEventArgs e)
        {
            LanguageManager.Instance.PropertyChanged -= LanguageManager_PropertyChanged;
            LanguageManager.Instance.PropertyChanged += LanguageManager_PropertyChanged;
            if (_notifyIcon == null)
                CreateIcon();
            UpdateTrayTranslations();
        }

        private void NotifyIconExample_Unloaded(object sender, RoutedEventArgs e)
        {
            LanguageManager.Instance.PropertyChanged -= LanguageManager_PropertyChanged;
            RemoveIcon();
        }

        private void LanguageManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LanguageManager.CurrentCulture))
                UpdateTrayTranslations();
        }

        private void UpdateTrayTranslations()
        {
            if (_notifyIcon == null) return;
            var localizer = SampleLanguageManager.Instance;
            _notifyIcon.Title = localizer.TranslateText("WPFDevelopers - NotifyIcon 示例");
            localizer.LocalizeTree(_notifyIcon.ContextMenu);
            localizer.LocalizeTree(_trayPanel);
        }

        private void TrayMenu_Opened(object sender, RoutedEventArgs e)
        {
            UpdateTrayTranslations();
        }

        private void CreateIcon()
        {
            _notifyIcon = (NotifyIcon)((DataTemplate)Resources["TrayIconTemplate"]).LoadContent();
            _trayPanel = (FrameworkElement)((DataTemplate)Resources["TrayPanelTemplate"]).LoadContent();
            _notifyIcon.ContextMenu.Opened += TrayMenu_Opened;
            UpdateTrayTranslations();
            TrayIconHost.Children.Add(_notifyIcon);
            ApplyMenuMode();
            InteractionPanel.IsEnabled = true;
            ToggleIconButton.Content = "移除示例图标";
            IconStatusText.Text = "示例图标已创建（1 个）";
            FeedbackText.Text = "已创建示例图标，可在系统托盘中单击、双击或右键操作。";
        }

        private void RemoveIcon()
        {
            if (_notifyIcon == null) return;
            _notifyIcon.ContextMenu.Opened -= TrayMenu_Opened;
            _notifyIcon.Dispose();
            TrayIconHost.Children.Clear();
            _notifyIcon = null;
            _trayPanel = null;
            TwinkCheckBox.IsChecked = false;
            InteractionPanel.IsEnabled = false;
            ToggleIconButton.Content = "创建示例图标";
            IconStatusText.Text = "示例图标已移除";
            FeedbackText.Text = "示例图标和闪烁计时器已释放，主窗口托盘图标不受影响。";
        }

        private void ToggleIcon_Click(object sender, RoutedEventArgs e)
        {
            if (_notifyIcon == null)
                CreateIcon();
            else
                RemoveIcon();
        }

        private void ApplyMenuMode()
        {
            _notifyIcon.ContextContent = MenuMode.SelectedIndex == 1 ? _trayPanel : null;
        }

        private void MenuMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_notifyIcon == null) return;
            ApplyMenuMode();
            FeedbackText.Text = MenuMode.SelectedIndex == 1
                ? "右键示例图标将显示自定义面板。"
                : "右键示例图标将显示标准菜单。";
        }

        private void NotifyIcon_Click(object sender, RoutedEventArgs e)
        {
            ClickCountText.Text = (++_clickCount).ToString();
            FeedbackText.Text = "收到托盘图标左键单击事件。";
        }

        private void NotifyIcon_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            DoubleClickCountText.Text = (++_doubleClickCount).ToString();
            FeedbackText.Text = "收到托盘图标双击事件（双击前的第一次单击也会计数）。";
        }

        private void BtnNotifyIconMessage_Click(object sender, RoutedEventArgs e)
        {
            if (_notifyIcon == null)
            {
                FeedbackText.Text = "请先创建示例图标。";
                return;
            }
            if (string.IsNullOrWhiteSpace(MessageTitle.Text) || string.IsNullOrWhiteSpace(MessageContent.Text))
            {
                FeedbackText.Text = "请先填写通知标题和内容。";
                return;
            }
            if (!(MessageType.SelectedItem is ComboBoxItem item) || !(item.Tag is NotifyIconInfoType type))
            {
                FeedbackText.Text = "请先选择通知类型。";
                return;
            }
            _notifyIcon.ShowBalloonTips(MessageTitle.Text, MessageContent.Text, type);
            FeedbackText.Text = "已向 Windows 提交通知；若未显示，请检查系统通知设置和勿扰模式。";
        }

        private void Twink_Changed(object sender, RoutedEventArgs e)
        {
            if (_notifyIcon == null) return;
            _notifyIcon.IsTwink = TwinkCheckBox.IsChecked == true;
            FeedbackText.Text = _notifyIcon.IsTwink ? "已开启图标闪烁。" : "已停止闪烁并恢复图标。";
        }

        private void ContextContent_Twink_Click(object sender, RoutedEventArgs e)
        {
            TwinkCheckBox.IsChecked = TwinkCheckBox.IsChecked != true;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            FeedbackText.Text = "已打开关于对话框。";
            var localizer = SampleLanguageManager.Instance;
            MessageBox.Show(
                localizer.TranslateText("本示例使用一个 NotifyIcon 演示点击事件、右键菜单、气泡消息和图标闪烁。"),
                localizer.TranslateText("关于 NotifyIcon 示例"),
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
