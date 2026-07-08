using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using WPFDevelopers.Controls;
using WPFDevelopers.Samples.ExampleViews;
using MessageBox = WPFDevelopers.Controls.MessageBox;

namespace WPFDevelopers.Samples
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow 
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnMainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private void OnMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetThemeSource();
            ThemeColorPicker.SelectedColor = ThemeManager.Instance.PrimaryColor;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("是否退出当前系统?", "询问", MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
            {
                e.Cancel = true;
            }
            WpfNotifyIcon?.Dispose();
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow().Show();
        }
        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            NotifyIcon.ShowBalloonTip("Message", " Welcome to WPFDevelopers ", NotifyIconInfoType.None);
        }
        private void Twink_Click(object sender, RoutedEventArgs e)
        {
            WpfNotifyIcon.IsTwink = !WpfNotifyIcon.IsTwink;
            menuItemTwink.IsChecked = WpfNotifyIcon.IsTwink;
        }
        private void Grayscale_Click(object sender, RoutedEventArgs e)
        {
            if(grayscaleEffect.Factor == 1)
            {
                menuItemGrayscale.Header = "关闭灰度";
                Create(0);
            }
               
            else
            {
                menuItemGrayscale.Header = "开启灰度";
                Create(1);

            }
        }
        void Create(double to)
        {
            var sineEase = new SineEase() { EasingMode = EasingMode.EaseOut };
            var doubleAnimation = new DoubleAnimation
            {
                To = to,
                Duration = TimeSpan.FromMilliseconds(1000),
                EasingFunction = sineEase
            };
            grayscaleEffect.BeginAnimation(GrayscaleEffect.FactorProperty, doubleAnimation);
        }

        public void SetWindowGrayscale(double factor)
        {
            grayscaleEffect.BeginAnimation(GrayscaleEffect.FactorProperty, null);
            grayscaleEffect.Factor = factor;
        }

        public void IsTwink()
        {
            WpfNotifyIcon.IsTwink = !WpfNotifyIcon.IsTwink;
            menuItemTwink.IsChecked = WpfNotifyIcon.IsTwink;
        }
        private void ColorPicker_Apply(object sender, RoutedEventArgs e)
        {
            ThemeSwitchBtn.IsChecked = false;
            ThemeManager.Instance.SetColor(ThemeColorPicker.SelectedColor);
        }

        private void ColorPicker_Cancel(object sender, RoutedEventArgs e)
        {
            ThemeSwitchBtn.IsChecked = false;
        }
    }
}
