using System.Globalization;
using System.Windows;
using WPFDevelopers;

namespace WPFDevelopers.Samples
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            LanguageManager.Instance.ChangeLanguage(new CultureInfo("en-US"));
            base.OnStartup(e);
        }

        public static double Wdith
        {
            get { return SystemParameters.WorkArea.Width / 1.5; }
        }
        public static double Height
        {
            get { return SystemParameters.WorkArea.Height / 1.5; }
        }
        public static Window CurrentMainWindow
        {
            get { return Current.MainWindow; }
        }
        public static ThemeType Theme { get; set; }

    }
}
