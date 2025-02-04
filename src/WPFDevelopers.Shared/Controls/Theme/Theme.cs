using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public class Theme : ListBox
    {
        private readonly Dictionary<string, string> themeMapping = new Dictionary<string, string>
        {
            { "#409EFF", "pack://application:,,,/WPFDevelopers;component/Themes/Light.Blue.xaml" },
            { "#FF033E", "pack://application:,,,/WPFDevelopers;component/Themes/Light.Red.xaml" },
            { "#A21BFC", "pack://application:,,,/WPFDevelopers;component/Themes/Light.Purple.xaml" },
            { "#FE9426", "pack://application:,,,/WPFDevelopers;component/Themes/Light.Orange.xaml" },
            { "#00B050", "pack://application:,,,/WPFDevelopers;component/Themes/Light.Green.xaml" },
            { "#FF007F", "pack://application:,,,/WPFDevelopers;component/Themes/Light.Pink.xaml" }
        };
        public ObservableCollection<ThemeItem> ThemeItems { get; set; }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Theme), new PropertyMetadata(Orientation.Horizontal));


        static Theme()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Theme),
                new FrameworkPropertyMetadata(typeof(Theme)));
        }

        public Theme()
        {
            SelectionMode = SelectionMode.Single;
            ThemeItems = new ObservableCollection<ThemeItem>();
            foreach (var mapping in themeMapping)
            {
                ThemeItems.Add(new ThemeItem
                {
                    Background = GetColorBrush(mapping.Key),
                    Content = mapping.Value
                });
            }
            ItemsSource = ThemeItems;
            var existsTheme = ThemeItems.FirstOrDefault(y =>
              Application.Current.Resources.MergedDictionaries.ToList().Exists(j =>
                  j.Source != null && y.Content.ToString().Contains(j.Source.AbsoluteUri)));
            if (existsTheme == null) return;
            SelectedItem = existsTheme;
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ThemeItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ThemeItem();
        }

        private SolidColorBrush GetColorBrush(string colorHex)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorHex));
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            if(e.RemovedItems == null || e.RemovedItems.Count == 0) return;
            var removeItem = e.RemovedItems[0] as ThemeItem;
            ThemeRefresh(removeItem);
        }

        public void AddThemeItem(string backgroundColor, string content)
        {
            var newItem = new ThemeItem
            {
                Background = GetColorBrush(backgroundColor),
                Content = content
            };
            ThemeItems.Add(newItem);
        }

        private void ThemeRefresh(ThemeItem removeItem)
        {
            if (removeItem == null) return;
            if (SelectedItem == null) return;
            var item = SelectedItem as ThemeItem;
            var resourcePath = item.Content.ToString();
            var items = ItemsSource.Cast<ThemeItem>();
            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            var existingResourceDictionary = mergedDictionaries.FirstOrDefault(x =>
                        x.Source != null && x.Source.Equals(removeItem.Content.ToString()));
            if (existingResourceDictionary != null)
                mergedDictionaries.Remove(existingResourceDictionary);
            if (mergedDictionaries.Any(x => x.Source != null && x.Source.Equals(resourcePath)))
                return;
            var newResourceDictionary = new ResourceDictionary { Source = new Uri(resourcePath) };
            mergedDictionaries.Insert(0, newResourceDictionary);
            ControlsHelper.ThemeRefresh();
        }
    }
}