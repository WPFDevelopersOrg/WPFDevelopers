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
#if NETFRAMEWORK
            // Older framework targets default to protocols rejected by modern HTTPS image hosts.
            // 3072 is TLS 1.2, whose enum name is unavailable in the .NET 4.0 reference assemblies.
            if (System.Net.ServicePointManager.SecurityProtocol != (System.Net.SecurityProtocolType)0)
                System.Net.ServicePointManager.SecurityProtocol |= (System.Net.SecurityProtocolType)3072;
#endif
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
