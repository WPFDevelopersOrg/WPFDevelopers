using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.Controls
{
    public partial class ThemeSettings : UserControl
    {
        private bool _synchronizingTheme;
        private bool _synchronizingLanguage;

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
            ThemeManager.Instance.Resources.ThemeModeChanged -= OnThemeModeChanged;
            ThemeManager.Instance.Resources.ThemeModeChanged += OnThemeModeChanged;
            LanguageManager.Instance.PropertyChanged -= OnLanguageChanged;
            LanguageManager.Instance.PropertyChanged += OnLanguageChanged;
            SynchronizeTheme();
            SynchronizeLanguage();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            WPFDevelopers.Resources.ThemeChanged -= OnThemeChanged;
            ThemeManager.Instance.Resources.ThemeModeChanged -= OnThemeModeChanged;
            LanguageManager.Instance.PropertyChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LanguageManager.CurrentCulture))
                SynchronizeLanguage();
        }

        private void SynchronizeLanguage()
        {
            _synchronizingLanguage = true;
            try
            {
                var localizer = SampleLanguageManager.Instance;
                localizer.SetLocalizedText(PersonalizationLabel, TextBlock.TextProperty, "个性化");
                localizer.SetLocalizedText(ThemeColorLabel, TextBlock.TextProperty, "主题色");
                localizer.SetLocalizedText(LanguageLabel, TextBlock.TextProperty, "Languages");
                var culture = LanguageManager.Instance.CurrentCulture;
                LanguageSelector.SelectedIndex = -1;
                for (var i = 0; i < LanguageSelector.Items.Count; i++)
                {
                    var item = (ComboBoxItem)LanguageSelector.Items[i];
                    if (string.Equals(
                        new CultureInfo((string)item.Tag).TwoLetterISOLanguageName,
                        culture.TwoLetterISOLanguageName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        LanguageSelector.SelectedIndex = i;
                        break;
                    }
                }
            }
            finally
            {
                _synchronizingLanguage = false;
            }
        }

        private void LanguageSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_synchronizingLanguage || !(LanguageSelector.SelectedItem is ComboBoxItem item))
                return;

            var culture = new CultureInfo((string)item.Tag);
            if (!Equals(LanguageManager.Instance.CurrentCulture, culture))
                LanguageManager.Instance.ChangeLanguage(culture);
        }

        private void OnThemeChanged(ThemeType theme)
        {
            SynchronizeTheme();
        }

        private void OnThemeModeChanged(ThemeMode mode)
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
                var index = (int)resources.ThemeMode;
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
            var mode = (ThemeMode)ThemeSelector.SelectedIndex;
            ThemeManager.Instance.SwitchThemeMode(ThemeSelector, mode);
            SynchronizeTheme();
        }
    }
}
