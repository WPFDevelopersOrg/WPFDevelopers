using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WPFDevelopers.Controls;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Samples.ExampleViews.Basics
{
    public partial class ThemeExample : UserControl
    {
        public ThemeExample()
        {
            InitializeComponent();
            Loaded += ThemeExample_Loaded;
        }

        private void ThemeExample_Loaded(object sender, RoutedEventArgs e)
        {
            App.Theme = ThemeManager.Instance.Resources.Theme;
            PART_Segmented.SelectedIndex = ThemeManager.Instance.Resources.Theme == ThemeType.Light ? 0 : 1;
        }
        
        private void ComboBoxLanguages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ComboBox cBox)) return;
            if (!(cBox.SelectedItem is ComboBoxItem item)) return;
            var tag = item.Tag.ToString();
            if (LanguageManager.Instance.CurrentCulture.Name != tag)
                LanguageManager.Instance.ChangeLanguage(new CultureInfo(tag));
        }
        private void GithubHyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void GiteeHyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            BrowserHelper.OpenUrl(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        private void QQHyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var uri = new Uri(@"https://qm.qq.com/cgi-bin/qm/qr?k=f2zl3nvoetItho8kGfe1eys0jDkqvvcL&jump_from=webapi");
            BrowserHelper.OpenUrl(uri);
            e.Handled = true;
        }

        private void Segmented_ItemClick(object sender, RoutedEventArgs e)
        {
            var segmented = sender as Segmented;
            var theme = segmented.SelectedIndex == 0 ? ThemeType.Light : ThemeType.Dark;
            App.Theme = theme;
            ThemeManager.Instance.SwitchTheme(PART_Segmented, theme);
        }
    }
}
