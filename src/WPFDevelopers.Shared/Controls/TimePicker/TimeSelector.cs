using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = ListBoxHourTemplateName, Type = typeof(ListBox))]
    [TemplatePart(Name = ListBoxMinuteTemplateName, Type = typeof(ListBox))]
    [TemplatePart(Name = ListBoxSecondTemplateName, Type = typeof(ListBox))]
    [TemplatePart(Name = ListBoxPeriodTemplateName, Type = typeof(ListBox))]
    [TemplatePart(Name = SecondColumnTemplateName, Type = typeof(ColumnDefinition))]
    public class TimeSelector : Control
    {
        private const string ListBoxHourTemplateName = "PART_ListBoxHour";
        private const string ListBoxMinuteTemplateName = "PART_ListBoxMinute";
        private const string ListBoxSecondTemplateName = "PART_ListBoxSecond";
        private const string ListBoxPeriodTemplateName = "PART_ListBoxPeriod";
        private const string SecondColumnTemplateName = "PART_SecondColumn";
        private const int VisibleItemCount = 10;
        private const int CenterIndex = 4;
        private int _hour;
        private int _minute;
        private int _second;
        private bool _isRefreshing;
        private ListBox _listBoxHour;
        private ListBox _listBoxMinute;
        private ListBox _listBoxSecond;
        private ListBox _listBoxPeriod;
        private ColumnDefinition _secondColumn;

        public static readonly RoutedEvent SelectedTimeChangedEvent =
            EventManager.RegisterRoutedEvent(
                "SelectedTimeChanged",
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<DateTime?>),
                typeof(TimeSelector));

        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register(
                "SelectedTime",
                typeof(DateTime?),
                typeof(TimeSelector),
                new PropertyMetadata(null, OnSelectedTimeChanged));

        public static readonly DependencyProperty IsTwelveHourFormatProperty =
            DependencyProperty.Register(
                "IsTwelveHourFormat",
                typeof(bool),
                typeof(TimeSelector),
                new PropertyMetadata(false, OnIsTwelveHourFormatChanged));

        public static readonly DependencyProperty ShowSecondsProperty =
            DependencyProperty.Register(
                "ShowSeconds",
                typeof(bool),
                typeof(TimeSelector),
                new PropertyMetadata(true, OnShowSecondsChanged));

        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register(
                "ItemHeight",
                typeof(double),
                typeof(TimeSelector),
                new PropertyMetadata(0d));

        public static readonly DependencyProperty SelectorMarginProperty =
            DependencyProperty.Register(
                "SelectorMargin",
                typeof(Thickness),
                typeof(TimeSelector),
                new PropertyMetadata(new Thickness(0)));

        static TimeSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TimeSelector),
                new FrameworkPropertyMetadata(typeof(TimeSelector))
            );
        }

        public DateTime? SelectedTime
        {
            get => (DateTime?)GetValue(SelectedTimeProperty);
            set => SetValue(SelectedTimeProperty, value);
        }

        public bool IsTwelveHourFormat
        {
            get => (bool)GetValue(IsTwelveHourFormatProperty);
            set => SetValue(IsTwelveHourFormatProperty, value);
        }

        public bool ShowSeconds
        {
            get => (bool)GetValue(ShowSecondsProperty);
            set => SetValue(ShowSecondsProperty, value);
        }

        public double ItemHeight
        {
            get => (double)GetValue(ItemHeightProperty);
            set => SetValue(ItemHeightProperty, value);
        }

        public Thickness SelectorMargin
        {
            get => (Thickness)GetValue(SelectorMarginProperty);
            set => SetValue(SelectorMarginProperty, value);
        }

        public event RoutedPropertyChangedEventHandler<DateTime?> SelectedTimeChanged
        {
            add => AddHandler(SelectedTimeChangedEvent, value);
            remove => RemoveHandler(SelectedTimeChangedEvent, value);
        }

        public virtual void OnSelectedTimeChanged(DateTime? oldValue, DateTime? newValue)
        {
            var args = new RoutedPropertyChangedEventArgs<DateTime?>(
                oldValue,
                newValue,
                SelectedTimeChangedEvent
            );
            RaiseEvent(args);
        }

        private static void OnSelectedTimeChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            if (d is TimeSelector ctrl)
            {
                ctrl.HandleSelectedTimeChanged((DateTime?)e.OldValue, (DateTime?)e.NewValue);
                ctrl.OnSelectedTimeChanged((DateTime?)e.OldValue, (DateTime?)e.NewValue);
            }
        }

        private static void OnIsTwelveHourFormatChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            if (d is TimeSelector ctrl)
            {
                if (ctrl._listBoxPeriod != null)
                    ctrl._listBoxPeriod.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
                ctrl.RefreshHourItems();
                ctrl.RefreshPeriodItems();
            }
        }

        private static void OnShowSecondsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TimeSelector ctrl)
            {
                var showSeconds = (bool)e.NewValue;
                if (ctrl._listBoxSecond != null)
                    ctrl._listBoxSecond.Visibility = showSeconds ? Visibility.Visible : Visibility.Collapsed;
                if (ctrl._secondColumn != null)
                    ctrl._secondColumn.Width = showSeconds ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
                if (!showSeconds)
                    ctrl._second = 0;
                ctrl.RefreshSecondItems();
            }
        }

        private void HandleSelectedTimeChanged(DateTime? oldValue, DateTime? newValue)
        {
            if (_isRefreshing)
                return;

            if (!newValue.HasValue)
                return;

            _hour = newValue.Value.Hour;
            _minute = newValue.Value.Minute;
            _second = ShowSeconds ? newValue.Value.Second : 0;

            if (_listBoxHour != null && _listBoxMinute != null && _listBoxSecond != null)
            {
                RefreshAllColumns();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _listBoxHour = GetTemplateChild(ListBoxHourTemplateName) as ListBox;
            _listBoxMinute = GetTemplateChild(ListBoxMinuteTemplateName) as ListBox;
            _listBoxSecond = GetTemplateChild(ListBoxSecondTemplateName) as ListBox;
            _listBoxPeriod = GetTemplateChild(ListBoxPeriodTemplateName) as ListBox;
            _secondColumn = GetTemplateChild(SecondColumnTemplateName) as ColumnDefinition;

            if (_listBoxHour != null)
            {
                _listBoxHour.SelectionChanged -= ListBoxHour_SelectionChanged;
                _listBoxHour.SelectionChanged += ListBoxHour_SelectionChanged;

                _listBoxHour.Loaded -= ListBox_Loaded;
                _listBoxHour.Loaded += ListBox_Loaded;
            }

            if (_listBoxMinute != null)
            {
                _listBoxMinute.SelectionChanged -= ListBoxMinute_SelectionChanged;
                _listBoxMinute.SelectionChanged += ListBoxMinute_SelectionChanged;
            }

            if (_listBoxSecond != null)
            {
                _listBoxSecond.Visibility = ShowSeconds ? Visibility.Visible : Visibility.Collapsed;
                _listBoxSecond.SelectionChanged -= ListBoxSecond_SelectionChanged;
                _listBoxSecond.SelectionChanged += ListBoxSecond_SelectionChanged;
            }
            if (_secondColumn != null)
                _secondColumn.Width = ShowSeconds ? new GridLength(1, GridUnitType.Star) : new GridLength(0);

            if (_listBoxPeriod != null)
            {
                _listBoxPeriod.Visibility = IsTwelveHourFormat ? Visibility.Visible : Visibility.Collapsed;
                _listBoxPeriod.SelectionChanged -= ListBoxPeriod_SelectionChanged;
                _listBoxPeriod.SelectionChanged += ListBoxPeriod_SelectionChanged;
            }

            if (SelectedTime.HasValue)
            {
                _hour = SelectedTime.Value.Hour;
                _minute = SelectedTime.Value.Minute;
                _second = ShowSeconds ? SelectedTime.Value.Second : 0;
            }
            else
            {
                var now = DateTime.Now;
                _hour = now.Hour;
                _minute = now.Minute;
                _second = ShowSeconds ? now.Second : 0;

                _isRefreshing = true;
                SelectedTime = new DateTime(
                    DateTime.Today.Year,
                    DateTime.Today.Month,
                    DateTime.Today.Day,
                    _hour,
                    _minute,
                    _second
                );
                _isRefreshing = false;
            }

            RefreshAllColumns();
        }

        private void ListBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (_listBoxHour == null)
                return;

            var h = GetItemHeight(_listBoxHour);
            if (h <= 0)
                return;

            ItemHeight = h;
            Height = h * VisibleItemCount;
            SelectorMargin = new Thickness(0, CenterIndex * h, 0, 0);
            if (_listBoxPeriod != null)
                _listBoxPeriod.Height = h * VisibleItemCount;
        }

        private double GetItemHeight(ListBox listBox)
        {
            if (listBox == null)
                return 0;

            if (listBox.Items.Count > 0)
            {
                listBox.UpdateLayout();
                var listBoxItem = listBox.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
                if (listBoxItem != null)
                    return listBoxItem.ActualHeight;
            }

            return 0;
        }

        private List<string> BuildCyclicItems(int current, int maxValue)
        {
            var list = new List<string>(VisibleItemCount);

            for (int i = 0; i < VisibleItemCount; i++)
            {
                int value = (current - CenterIndex + i) % maxValue;
                if (value < 0)
                    value += maxValue;

                list.Add(value.ToString("D2"));
            }

            return list;
        }

        private List<string> BuildTwelveHourItems(int hour)
        {
            var list = new List<string>(VisibleItemCount);
            var current = hour % 12;
            if (current == 0)
                current = 12;

            for (int i = 0; i < VisibleItemCount; i++)
            {
                int value = (current - 1 - CenterIndex + i) % 12;
                if (value < 0)
                    value += 12;
                list.Add((value + 1).ToString("D2"));
            }

            return list;
        }

        private void RefreshAllColumns()
        {
            RefreshHourItems();
            RefreshMinuteItems();
            RefreshSecondItems();
            RefreshPeriodItems();
        }

        private void RefreshSecondItems()
        {
            if (_listBoxSecond == null)
                return;

            _listBoxSecond.SelectionChanged -= ListBoxSecond_SelectionChanged;
            _listBoxSecond.ItemsSource = BuildCyclicItems(_second, 60);
            _listBoxSecond.SelectedIndex = CenterIndex;
            _listBoxSecond.SelectionChanged += ListBoxSecond_SelectionChanged;
        }

        private void RefreshHourItems()
        {
            if (_listBoxHour == null)
                return;

            _listBoxHour.SelectionChanged -= ListBoxHour_SelectionChanged;
            _listBoxHour.ItemsSource = IsTwelveHourFormat
                ? BuildTwelveHourItems(_hour)
                : BuildCyclicItems(_hour, 24);
            _listBoxHour.SelectedIndex = CenterIndex;
            _listBoxHour.SelectionChanged += ListBoxHour_SelectionChanged;
        }

        private void RefreshPeriodItems()
        {
            if (_listBoxPeriod == null || !IsTwelveHourFormat)
                return;

            var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
            var am = string.IsNullOrEmpty(dateTimeFormat.AMDesignator) ? "AM" : dateTimeFormat.AMDesignator;
            var pm = string.IsNullOrEmpty(dateTimeFormat.PMDesignator) ? "PM" : dateTimeFormat.PMDesignator;
            var isPm = _hour >= 12;
            var items = new List<string>(VisibleItemCount);
            for (int i = 0; i < VisibleItemCount; i++)
                items.Add(string.Empty);
            items[CenterIndex] = isPm ? pm : am;
            items[CenterIndex + 1] = isPm ? am : pm;

            _listBoxPeriod.SelectionChanged -= ListBoxPeriod_SelectionChanged;
            _listBoxPeriod.ItemsSource = items;
            _listBoxPeriod.SelectedIndex = CenterIndex;
            _listBoxPeriod.SelectionChanged += ListBoxPeriod_SelectionChanged;
        }

        private void RefreshMinuteItems()
        {
            if (_listBoxMinute == null)
                return;

            _listBoxMinute.SelectionChanged -= ListBoxMinute_SelectionChanged;
            _listBoxMinute.ItemsSource = BuildCyclicItems(_minute, 60);
            _listBoxMinute.SelectedIndex = CenterIndex;
            _listBoxMinute.SelectionChanged += ListBoxMinute_SelectionChanged;
        }

        private void ListBoxSecond_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isRefreshing)
                return;

            if (_listBoxSecond?.SelectedItem == null)
                return;

            if (!int.TryParse(_listBoxSecond.SelectedItem.ToString(), out var value))
                return;

            _second = value;
            UpdateSelectedTimeFromFields();
            RefreshSecondItems();
        }

        private void ListBoxMinute_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isRefreshing)
                return;

            if (_listBoxMinute?.SelectedItem == null)
                return;

            if (!int.TryParse(_listBoxMinute.SelectedItem.ToString(), out var value))
                return;

            _minute = value;
            UpdateSelectedTimeFromFields();
            RefreshMinuteItems();
        }

        private void ListBoxHour_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isRefreshing)
                return;

            if (_listBoxHour?.SelectedItem == null)
                return;

            if (!int.TryParse(_listBoxHour.SelectedItem.ToString(), out var value))
                return;

            if (IsTwelveHourFormat)
            {
                value %= 12;
                if (_hour >= 12)
                    value += 12;
            }
            _hour = value;
            UpdateSelectedTimeFromFields();
            RefreshHourItems();
        }

        private void ListBoxPeriod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isRefreshing || _listBoxPeriod == null || _listBoxPeriod.SelectedIndex < 0)
                return;

            var periodIsPm = _hour >= 12 ^ ((_listBoxPeriod.SelectedIndex - CenterIndex) % 2 != 0);
            var hour = (_hour % 12) + (periodIsPm ? 12 : 0);
            if (hour != _hour)
            {
                _hour = hour;
                UpdateSelectedTimeFromFields();
            }
            RefreshPeriodItems();
        }

        private void UpdateSelectedTimeFromFields()
        {
            _isRefreshing = true;

            SelectedTime = new DateTime(
                DateTime.Today.Year,
                DateTime.Today.Month,
                DateTime.Today.Day,
                _hour,
                _minute,
                _second
            );

            _isRefreshing = false;
        }

        public void SetTime()
        {
            if (!SelectedTime.HasValue)
                return;

            _hour = SelectedTime.Value.Hour;
            _minute = SelectedTime.Value.Minute;
            _second = ShowSeconds ? SelectedTime.Value.Second : 0;
            RefreshAllColumns();
        }
    }
}
