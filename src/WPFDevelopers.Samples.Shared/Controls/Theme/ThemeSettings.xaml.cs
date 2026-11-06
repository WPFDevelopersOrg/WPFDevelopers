using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.Controls
{
    public partial class ThemeSettings : UserControl
    {
        private bool _synchronizingTheme;

        public ThemeSettings()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            WPFDevelopers.Resources.ThemeChanged -= OnThemeChanged;
            WPFDevelopers.Resources.ThemeChanged += OnThemeChanged;
            SynchronizeTheme();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            WPFDevelopers.Resources.ThemeChanged -= OnThemeChanged;
        }

        private void OnThemeChanged(ThemeType theme)
        {
            SynchronizeTheme();
        }

        private void SynchronizeTheme()
        {
            var resources = ThemeManager.Instance.Resources;
            if (resources == null) return;
            _synchronizingTheme = true;
            try
            {
                var index = resources.IsDarkTheme ? 1 : 0;
                ThemeSelector.SelectedIndex = index;
                for (var i = 0; i < ThemeSelector.Items.Count; i++)
                    ((SegmentedItem)ThemeSelector.Items[i]).IsChecked = i == index;
                App.Theme = resources.Theme;
            }
            finally
            {
                _synchronizingTheme = false;
            }
        }

        private void ThemeSelector_ItemClick(object sender, RoutedEventArgs e)
        {
            if (_synchronizingTheme) return;
            var theme = ThemeSelector.SelectedIndex == 0 ? ThemeType.Light : ThemeType.Dark;
            ThemeManager.Instance.SwitchTheme(ThemeSelector, theme);
            SynchronizeTheme();
        }
    }
}
