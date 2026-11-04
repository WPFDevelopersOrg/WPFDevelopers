using System.Windows;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    ///     HighTitleBarWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HighTitleBarWindow
    {
        public HighTitleBarWindow()
        {
            InitializeComponent();
            ThemeSwitchBtn.IsChecked = ThemeManager.Instance.Resources.IsDarkTheme;
            Activated += (sender, e) => ThemeSwitchBtn.IsChecked = ThemeManager.Instance.Resources.IsDarkTheme;
        }

        private void ThemeSwitchBtn_Click(object sender, RoutedEventArgs e)
        {
            var targetTheme = ThemeManager.Instance.Resources.IsDarkTheme ? ThemeType.Light : ThemeType.Dark;
            ThemeManager.Instance.SwitchTheme(ThemeSwitchBtn, targetTheme);
            App.Theme = ThemeManager.Instance.Resources.Theme;
            ThemeSwitchBtn.IsChecked = ThemeManager.Instance.Resources.IsDarkTheme;
        }
    }
}