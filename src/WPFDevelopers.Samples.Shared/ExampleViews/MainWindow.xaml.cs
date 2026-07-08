using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using WPFDevelopers.Controls;
using WPFDevelopers.Samples.ExampleViews;
using WPFDevelopers.Samples.Helpers;
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
                CreateGrayscale(0);
            }
               
            else
            {
                menuItemGrayscale.Header = "开启灰度";
                CreateGrayscale(1);

            }
        }
        void CreateGrayscale(double to)
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

        public ICommand CircleMenuItemClickCommand => new RelayCommand(param =>
        {
            var item = param as CircleMenuItem;
            if (item == null) return;
            var name = item.Name.ToString();
            if (string.IsNullOrWhiteSpace(name)) return;
            switch (name)
            {
                case "ScreenShot":
                    Application.Current.MainWindow.WindowState = WindowState.Minimized;
                    var screenCaptureExt = new ScreenCaptureExt();
                    screenCaptureExt.SnapCanceled += ScreenCaptureExt_SnapCanceled;
                    screenCaptureExt.SnapCompleted += ScreenCaptureExt_SnapCompleted;
                    break;
                case "Github":
                    var uri = "https://github.com/WPFDevelopersOrg/WPFDevelopers";
                    if (uri.StartsWith("http://") || uri.StartsWith("https://"))
                    {
                        Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
                    }
                    break;
                case "Grayscale":
                    var isEnable = GrayscaleSvg.Source.Contains("Enable");
                    GrayscaleSvg.Source = isEnable
               ? "pack://application:,,,/WPFDevelopers.Samples;component/Resources/Svg/DisableGrayscale.svg"
               : "pack://application:,,,/WPFDevelopers.Samples;component/Resources/Svg/EnableGrayscale.svg";
                    if(isEnable)
                        CreateGrayscale(0);
                    else
                        CreateGrayscale(1);
                    break;
                case "Theme":
                    var isDark = ThemeSvg.Source.Contains("Moon");
                    var theme = isDark ? ThemeType.Dark : ThemeType.Light;
                    if (App.Theme == theme) return;
                    App.Theme = theme;
                    ThemeManager.Instance.SetTheme(theme);
                    SetThemeSource();
                    break;
                case "Code":
                    CodeDrawer.IsOpen = !CodeDrawer.IsOpen;
                    break;
                case "ClearToast":
                    Toast.ClearAll();
                    break;
                default:
                    break;
            }
        });

        void SetThemeSource()
        {
            App.Theme = ThemeManager.Instance.Resources.Theme;
            if (App.Theme == ThemeType.Dark)
                ThemeSvg.Source = "pack://application:,,,/WPFDevelopers.Samples;component/Resources/Svg/Sun.svg";
            else
                ThemeSvg.Source = "pack://application:,,,/WPFDevelopers.Samples;component/Resources/Svg/Moon.svg";
        }

        private void ScreenCaptureExt_SnapCanceled()
        {
            Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        private void ScreenCaptureExt_SnapCompleted(System.Windows.Media.Imaging.BitmapSource bitmap)
        {

            Application.Current.MainWindow.WindowState = WindowState.Normal;
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
