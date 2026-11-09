using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WPFDevelopers;
using WPFDevelopers.Controls;
using WPFDevelopers.Helpers;
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
            VersionText.Text = "v" + typeof(MainWindow).BaseType.Assembly.GetName().Version.ToString(3);
            Loaded += OnMainWindow_Loaded;
            Closing += MainWindow_Closing;
            WPFDevelopers.Resources.ThemeChanged += OnThemeChanged;
            LanguageManager.Instance.PropertyChanged += LanguageManager_PropertyChanged;
            Closed += (sender, e) => WPFDevelopers.Resources.ThemeChanged -= OnThemeChanged;
            Closed += (sender, e) => LanguageManager.Instance.PropertyChanged -= LanguageManager_PropertyChanged;
            UpdateShellDisplayNames();
        }

        private void OnMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetThemeSource();
            ThemeColorPicker.SelectedColor = ThemeManager.Instance.PrimaryColor;
        }

        private void OnThemeChanged(ThemeType theme)
        {
            SetThemeSource();
        }

        private void LanguageManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LanguageManager.CurrentCulture))
            {
                UpdateShellDisplayNames();
            }
        }

        private void UpdateShellDisplayNames()
        {
            var localizer = SampleLanguageManager.Instance;
            ToolTipService.SetToolTip(HomeButton, localizer.TranslateText("首页 / Home"));
            ColorPickerApplyButton.Content = localizer.TranslateText("Apply");
            ColorPickerCancelButton.Content = localizer.TranslateText("Cancel");
            WpfNotifyIcon.Title = localizer.TranslateText("WPF开发者");
            menuItemGrayscale.Header = localizer.TranslateText(grayscaleEffect.Factor == 1 ? "开启灰度" : "关闭灰度");
            menuItemSendMessage.Header = localizer.TranslateText("托盘消息");
            menuItemTwink.Header = localizer.TranslateText("闪烁");
            menuItemAbout.Header = localizer.TranslateText("关于");
            menuItemQuit.Header = localizer.TranslateText("退出");
        }

        private void TrayMenu_Opened(object sender, RoutedEventArgs e)
        {
            UpdateShellDisplayNames();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateMenu.SelectedIndex = -1;
        }

        public void ToggleTheme(FrameworkElement source)
        {
            var targetTheme = ThemeManager.Instance.Resources.Theme == ThemeType.Dark ? ThemeType.Light : ThemeType.Dark;
            ThemeManager.Instance.SwitchTheme(source, targetTheme);
            SetThemeSource();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var localizer = SampleLanguageManager.Instance;
            if (MessageBox.Show(
                    localizer.TranslateText("是否退出当前系统?"),
                    localizer.TranslateText("询问"),
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Question) != MessageBoxResult.OK)
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
            new AboutWindow().MaskShowDialog();
        }
        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            var localizer = SampleLanguageManager.Instance;
            NotifyIcon.ShowBalloonTip(
                localizer.TranslateText("Message"),
                localizer.TranslateText("Welcome to WPFDevelopers"),
                NotifyIconInfoType.None);
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
                menuItemGrayscale.Header = SampleLanguageManager.Instance.TranslateText("关闭灰度");
                CreateGrayscale(0);
            }

            else
            {
                menuItemGrayscale.Header = SampleLanguageManager.Instance.TranslateText("开启灰度");
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
            ColorSwitchBtn.IsChecked = false;
            ThemeManager.Instance.SetColor(ThemeColorPicker.SelectedColor);
        }

        private void ColorPicker_Cancel(object sender, RoutedEventArgs e)
        {
            ColorSwitchBtn.IsChecked = false;
        }
    }
}
