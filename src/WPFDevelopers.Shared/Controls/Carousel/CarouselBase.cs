using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace WPFDevelopers.Controls
{
    public abstract class CarouselBase : Control
    {
        private static readonly Type _typeofSelf = typeof(CarouselBase);

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), _typeofSelf,
                new PropertyMetadata(null, OnItemsSourceChanged));

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(nameof(SelectedIndex), typeof(int), _typeofSelf,
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedIndexChanged));

        public static readonly DependencyProperty AutoPlayProperty =
            DependencyProperty.Register(nameof(AutoPlay), typeof(bool), _typeofSelf,
                new PropertyMetadata(false, OnAutoPlayChanged));

        public static readonly DependencyProperty AutoPlayIntervalProperty =
            DependencyProperty.Register(nameof(AutoPlayInterval), typeof(TimeSpan), _typeofSelf,
                new PropertyMetadata(TimeSpan.FromSeconds(3)));

        public static readonly RoutedEvent ItemClickEvent =
            EventManager.RegisterRoutedEvent(nameof(ItemClick), RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), _typeofSelf);

        public static readonly DependencyProperty ItemClickCommandProperty =
            DependencyProperty.Register(nameof(ItemClickCommand), typeof(ICommand), _typeofSelf,
                new PropertyMetadata(null));

        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(nameof(DisplayMemberPath), typeof(string), _typeofSelf,
                new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), _typeofSelf,
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedItemChanged));

        public static readonly RoutedEvent SelectedItemChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(SelectedItemChanged), RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<object>), _typeofSelf);

        private DispatcherTimer _autoPlayTimer;
        private bool _hasItemsSource;
        private static readonly ConcurrentDictionary<string, PropertyInfo> _propertyCache =
            new ConcurrentDictionary<string, PropertyInfo>();

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        public bool AutoPlay
        {
            get => (bool)GetValue(AutoPlayProperty);
            set => SetValue(AutoPlayProperty, value);
        }

        public TimeSpan AutoPlayInterval
        {
            get => (TimeSpan)GetValue(AutoPlayIntervalProperty);
            set => SetValue(AutoPlayIntervalProperty, value);
        }

        public event RoutedEventHandler ItemClick
        {
            add => AddHandler(ItemClickEvent, value);
            remove => RemoveHandler(ItemClickEvent, value);
        }

        public ICommand ItemClickCommand
        {
            get => (ICommand)GetValue(ItemClickCommandProperty);
            set => SetValue(ItemClickCommandProperty, value);
        }

        public string DisplayMemberPath
        {
            get => (string)GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public event RoutedPropertyChangedEventHandler<object> SelectedItemChanged
        {
            add => AddHandler(SelectedItemChangedEvent, value);
            remove => RemoveHandler(SelectedItemChangedEvent, value);
        }

        protected CarouselBase()
        {
            Loaded += CarouselBase_Loaded;
            Unloaded += CarouselBase_Unloaded;
        }

        private void CarouselBase_Loaded(object sender, RoutedEventArgs e)
        {
            if (AutoPlay)
                StartAutoPlay();
        }

        private void CarouselBase_Unloaded(object sender, RoutedEventArgs e)
        {
            StopAutoPlay();
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (CarouselBase)d;

            if (e.OldValue is INotifyCollectionChanged oldNcc)
                oldNcc.CollectionChanged -= ctrl.OnItemsCollectionChanged;
            if (e.NewValue is INotifyCollectionChanged newNcc)
                newNcc.CollectionChanged += ctrl.OnItemsCollectionChanged;

            ctrl._hasItemsSource = e.NewValue != null;
            ctrl.OnItemsSourceChangedCore(e.OldValue, e.NewValue);
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (CarouselBase)d;
            ctrl.OnSelectedIndexChangedCore((int)e.OldValue, (int)e.NewValue);
        }

        private static void OnAutoPlayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (CarouselBase)d;
            if ((bool)e.NewValue)
                ctrl.StartAutoPlay();
            else
                ctrl.StopAutoPlay();
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (CarouselBase)d;
            var args = new RoutedPropertyChangedEventArgs<object>(e.OldValue, e.NewValue, SelectedItemChangedEvent);
            ctrl.RaiseEvent(args);
        }

        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnItemsSourceCollectionChanged(e);
        }

        protected virtual void OnItemsSourceChangedCore(object oldValue, object newValue) { }

        protected virtual void OnSelectedIndexChangedCore(int oldIndex, int newIndex) { }

        protected virtual void OnItemsSourceCollectionChanged(NotifyCollectionChangedEventArgs e) { }

        protected virtual void OnAutoPlayTick() { }

        protected void RaiseItemClick(object clickedItem)
        {
            var args = new RoutedEventArgs(ItemClickEvent, clickedItem);
            RaiseEvent(args);

            if (ItemClickCommand?.CanExecute(clickedItem) == true)
                ItemClickCommand.Execute(clickedItem);
        }

        protected void StartAutoPlay()
        {
            if (!AutoPlay) return;

            StopAutoPlay();
            _autoPlayTimer = new DispatcherTimer
            {
                Interval = AutoPlayInterval
            };
            _autoPlayTimer.Tick += (s, e) => OnAutoPlayTick();
            _autoPlayTimer.Start();
        }

        protected void StopAutoPlay()
        {
            _autoPlayTimer?.Stop();
            _autoPlayTimer = null;
        }

        protected bool IsAutoPlaying => _autoPlayTimer != null;

        protected bool HasItemsSource => _hasItemsSource;

        protected IList<object> GetItemsFromSource()
        {
            var list = new List<object>();
            if (ItemsSource != null)
            {
                foreach (var item in ItemsSource)
                    list.Add(item);
            }
            return list;
        }

        protected string GetDisplayValue(object item, string propertyPath)
        {
            if (item == null || string.IsNullOrEmpty(propertyPath))
                return null;
            var key = item.GetType().FullName + "|" + propertyPath;
            if (!_propertyCache.TryGetValue(key, out var prop))
            {
                prop = item.GetType().GetProperty(propertyPath);
                if (prop != null)
                    _propertyCache[key] = prop;
            }
            return prop?.GetValue(item, null)?.ToString();
        }
    }
}
