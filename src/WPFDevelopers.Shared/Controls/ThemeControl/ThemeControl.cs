using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public class ThemeControl : Control
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<ThemeModel>), typeof(ThemeControl),
                new PropertyMetadata(null));

        static ThemeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ThemeControl),
                new FrameworkPropertyMetadata(typeof(ThemeControl)));
        }

        public ObservableCollection<ThemeModel> ItemsSource
        {
            get => (ObservableCollection<ThemeModel>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ItemsSource = new ObservableCollection<ThemeModel>();
            ItemsSource.Add(new ThemeModel
            {
                Color = "#409EFF",
                ResourcePath = "pack://application:,,,/WPFDevelopers;component/Themes/Light.Blue.xaml"
            });
            ItemsSource.Add(new ThemeModel
            {
                Color = "#FF033E",
                ResourcePath = "pack://application:,,,/WPFDevelopers;component/Themes/Light.Red.xaml"
            });
            ItemsSource.Add(new ThemeModel
            {
                Color = "#A21BFC",
                ResourcePath = "pack://application:,,,/WPFDevelopers;component/Themes/Light.Purple.xaml"
            });
            ItemsSource.Add(new ThemeModel
            {
                Color = "#FE9426",
                ResourcePath = "pack://application:,,,/WPFDevelopers;component/Themes/Light.Orange.xaml"
            });
            ItemsSource.Add(new ThemeModel
            {
                Color = "#00B050",
                ResourcePath = "pack://application:,,,/WPFDevelopers;component/Themes/Light.Green.xaml"
            });
            ItemsSource.Add(new ThemeModel
            {
                Color = "#FF007F",
                ResourcePath = "pack://application:,,,/WPFDevelopers;component/Themes/Light.Pink.xaml"
            });
            if (ThemeCache.ThemesDictCache.Count > 0)
                foreach (var item in ThemeCache.ThemesDictCache)
                    if (ItemsSource.Any(x => x.Color != item.Key))
                        ItemsSource.Add(item.Value);
            SelectChecked();
            ItemsSource.CollectionChanged += ItemsSource_CollectionChanged;
            foreach (var theme in ItemsSource)
                theme.PropertyChanged += Theme_PropertyChanged;
        }

        private void SelectChecked()
        {
            var existsTheme = ItemsSource.FirstOrDefault(y =>
                Application.Current.Resources.MergedDictionaries.ToList().Exists(j =>
                    j.Source != null && y.ResourcePath.Contains(j.Source.AbsoluteUri)));

            if (existsTheme != null)
                existsTheme.IsChecked = true;
        }

        private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ThemeModel item in e.NewItems)
                    {
                        if (!ThemeCache.ThemesDictCache.ContainsKey(item.Color))
                            ThemeCache.ThemesDictCache.Add(item.Color, item);
                        item.PropertyChanged += Theme_PropertyChanged;
                        SelectChecked();
                        if (!item.IsChecked) return;
                        ReviseTheme(item);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (ThemeModel item in e.NewItems)
                        if (ThemeCache.ThemesDictCache.ContainsKey(item.Color))
                            ThemeCache.ThemesDictCache.Remove(item.Color);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
            }
        }


        private void Theme_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ThemeModel.IsChecked))
            {
                var theme = sender as ThemeModel;
                if (!theme.IsChecked) return;
                ReviseTheme(theme);
            }
        }

        private void ReviseTheme(ThemeModel theme)
        {
            if (theme == null) return;

            var old = ItemsSource.FirstOrDefault(x => x.IsChecked && x.Color != theme.Color);
            if (old != null)
            {
                ItemsSource.Where(y => !y.Color.Equals(theme.Color) && y.IsChecked).ToList()
                    .ForEach(h => h.IsChecked = false);
                var existingResourceDictionary =
                    Application.Current.Resources.MergedDictionaries.FirstOrDefault(x =>
                        x.Source != null && x.Source.Equals(old.ResourcePath));
                if (existingResourceDictionary != null)
                    Application.Current.Resources.MergedDictionaries.Remove(existingResourceDictionary);
                var newResource =
                    Application.Current.Resources.MergedDictionaries.FirstOrDefault(x =>
                        x.Source != null && x.Source.Equals(theme.ResourcePath));
                if (newResource != null) return;
                var newResourceDictionary = new ResourceDictionary { Source = new Uri(theme.ResourcePath) };
                Application.Current.Resources.MergedDictionaries.Insert(0, newResourceDictionary);
                ControlsHelper.ThemeRefresh();
            }
        }
    }

    public class ThemeCache
    {
        public static Dictionary<string, ThemeModel> ThemesDictCache = new Dictionary<string, ThemeModel>();
    }
}