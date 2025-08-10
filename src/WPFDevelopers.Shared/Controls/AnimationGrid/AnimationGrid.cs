using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace WPFDevelopers.Controls
{
    public class AnimationGrid : Grid
    {
        private readonly object _syncLock = new object();

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(AnimationGrid),
                new PropertyMetadata(null, OnItemsSourceChanged));

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(AnimationGrid),
                new PropertyMetadata(null));

        private readonly Dictionary<object, FrameworkElement> _itemMap = new Dictionary<object, FrameworkElement>();
        private readonly HashSet<object> _visibleItems = new HashSet<object>();

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AnimationGrid panel)
            {
                panel.InitializeItems();
            }
        }

        private void InitializeItems()
        {
            Children.Clear();
            ColumnDefinitions.Clear();
            _itemMap.Clear();
            _visibleItems.Clear();

            if (ItemsSource == null || ItemTemplate == null) return;

            foreach (var item in ItemsSource)
            {
                var content = (FrameworkElement)ItemTemplate.LoadContent();
                content.DataContext = item;
                content.Visibility = Visibility.Collapsed;
                content.Width = 0;
                _itemMap[item] = content;
                Children.Add(content);
            }
            if (_itemMap.Count > 0)
            {
                var first = _itemMap.First();
                _visibleItems.Add(first.Key);
                first.Value.Visibility = Visibility.Visible;
                UpdateLayoutAnimated();
            }
        }

        public void ShowItem(object item)
        {
            lock (_syncLock)
            {
                if (!_itemMap.ContainsKey(item))
                    return;
                if (_visibleItems.Contains(item))
                    _visibleItems.Remove(item);
                else
                    _visibleItems.Add(item);
                _itemMap[item].Visibility = Visibility.Visible;
                UpdateLayoutAnimated();
            }
        }

        private void UpdateLayoutAnimated()
        {
            ColumnDefinitions.Clear();
            var visibleCount = Math.Max(1, _visibleItems.Count);
            var width = this.Width;
            var targetWidth = ActualWidth / visibleCount;
            int index = 0;
            foreach (var item in _itemMap.Keys)
            {
                ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                var element = _itemMap[item];
                SetColumn(element, index);
                if (_visibleItems.Contains(item))
                {
                    AnimateWidth(element, targetWidth);
                }
                else
                {
                    AnimateWidth(element, 0, () => element.Visibility = Visibility.Collapsed);
                }
                index++;
            }
        }

        private void AnimateWidth(FrameworkElement element, double targetWidth, Action completed = null)
        {
            var anim = new DoubleAnimation
            {
                To = targetWidth,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            if (completed != null)
            {
                anim.Completed += delegate { completed(); };
            }
            element.BeginAnimation(WidthProperty, anim);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (_visibleItems.Count > 0)
            {
                UpdateLayoutAnimated();
            }
        }
    }
}
