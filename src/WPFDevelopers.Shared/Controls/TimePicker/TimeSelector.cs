using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = ListBoxHourTemplateName, Type = typeof(ListBox))]
    [TemplatePart(Name = ListBoxMinuteTemplateName, Type = typeof(ListBox))]
    [TemplatePart(Name = ListBoxSecondTemplateName, Type = typeof(ListBox))]
    public class TimeSelector : Control
    {
        private const string ListBoxHourTemplateName = "PART_ListBoxHour";
        private const string ListBoxMinuteTemplateName = "PART_ListBoxMinute";
        private const string ListBoxSecondTemplateName = "PART_ListBoxSecond";

        public static readonly RoutedEvent SelectedTimeChangedEvent =
            EventManager.RegisterRoutedEvent("SelectedTimeChanged", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<DateTime?>), typeof(TimeSelector));

        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register("SelectedTime", typeof(DateTime?), typeof(TimeSelector),
                new PropertyMetadata(null, OnSelectedTimeChanged));

        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(TimeSelector), new PropertyMetadata(0d));

        public static readonly DependencyProperty SelectorMarginProperty =
            DependencyProperty.Register("SelectorMargin", typeof(Thickness), typeof(TimeSelector),
                new PropertyMetadata(new Thickness(0)));

        private int _hour, _minute, _second;

        private ListBox _listBoxHour, _listBoxMinute, _listBoxSecond;

        static TimeSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeSelector),
                new FrameworkPropertyMetadata(typeof(TimeSelector)));
        }


        public DateTime? SelectedTime
        {
            get => (DateTime?) GetValue(SelectedTimeProperty);
            set => SetValue(SelectedTimeProperty, value);
        }


        public double ItemHeight
        {
            get => (double) GetValue(ItemHeightProperty);
            set => SetValue(ItemHeightProperty, value);
        }


        public Thickness SelectorMargin
        {
            get => (Thickness) GetValue(SelectorMarginProperty);
            set => SetValue(SelectorMarginProperty, value);
        }


        public event RoutedPropertyChangedEventHandler<DateTime?> SelectedTimeChanged
        {
            add => AddHandler(SelectedTimeChangedEvent, value);
            remove => RemoveHandler(SelectedTimeChangedEvent, value);
        }

        public virtual void OnSelectedTimeChanged(DateTime? oldValue, DateTime? newValue)
        {
            var args = new RoutedPropertyChangedEventArgs<DateTime?>(oldValue, newValue, SelectedTimeChangedEvent);
            RaiseEvent(args);
        }

        private static void OnSelectedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as TimeSelector;
            if (ctrl != null)
                ctrl.OnSelectedTimeChanged((DateTime?) e.OldValue, (DateTime?) e.NewValue);
        }


        private double GetItemHeight(ListBox listBox)
        {
            if (listBox.Items.Count > 0)
            {
                var listBoxItem = listBox.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
                if (listBoxItem != null) return listBoxItem.ActualHeight;
            }

            return 0;
        }

        private int GetFirstNonEmptyItemIndex(ListBox listBox)
        {
            for (var i = 0; i < listBox.Items.Count; i++)
            {
                var item = listBox.Items[i] as string;
                if (!string.IsNullOrWhiteSpace(item))
                    return i;
            }

            return -1;
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var minuteSecondList = Enumerable.Range(0, 60).Select(num => num.ToString("D2"));
            var emptyData = Enumerable.Repeat(string.Empty, 4);
            var result = emptyData.Concat(minuteSecondList).Concat(emptyData);
            _listBoxHour = GetTemplateChild(ListBoxHourTemplateName) as ListBox;
            if (_listBoxHour != null)
            {
                var hours = Enumerable.Range(0, 24).Select(num => num.ToString("D2"));
                _listBoxHour.ItemsSource = emptyData.Concat(hours).Concat(emptyData);
                _listBoxHour.SelectionChanged -= ListBoxHour_SelectionChanged;
                _listBoxHour.SelectionChanged += ListBoxHour_SelectionChanged;
                _listBoxHour.Loaded += (sender, args) =>
                {
                    var h = GetItemHeight(_listBoxHour);
                    if (h <= 0) return;
                    ItemHeight = h;
                    Height = h * 10;
                    var YAxis = GetFirstNonEmptyItemIndex(_listBoxHour) * h;
                    SelectorMargin = new Thickness(0, YAxis, 0, 0);
                };
            }

            _listBoxMinute = GetTemplateChild(ListBoxMinuteTemplateName) as ListBox;
            if (_listBoxMinute != null)
            {
                _listBoxMinute.SelectionChanged -= ListBoxMinute_SelectionChanged;
                _listBoxMinute.SelectionChanged += ListBoxMinute_SelectionChanged;
                _listBoxMinute.ItemsSource = result;
            }

            _listBoxSecond = GetTemplateChild(ListBoxSecondTemplateName) as ListBox;
            if (_listBoxSecond != null)
            {
                _listBoxSecond.SelectionChanged -= ListBoxSecond_SelectionChanged;
                _listBoxSecond.SelectionChanged += ListBoxSecond_SelectionChanged;
                _listBoxSecond.ItemsSource = result;
            }
        }

        private void ListBoxSecond_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_listBoxSecond.SelectedValue.ToString())) return;
            _second = Convert.ToInt32(_listBoxSecond.SelectedValue.ToString());
            SetSelectedTime();
        }

        private void ListBoxMinute_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_listBoxMinute.SelectedValue.ToString())) return;
            _minute = Convert.ToInt32(_listBoxMinute.SelectedValue.ToString());
            SetSelectedTime();
        }

        private void ListBoxHour_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_listBoxHour.SelectedValue.ToString())) return;
            _hour = Convert.ToInt32(_listBoxHour.SelectedValue.ToString());
            SetSelectedTime();
        }

        private void SetSelectedTime()
        {
            var dt = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, _hour, _minute,
                _second);
            SelectedTime = dt;
        }

        public void SetTime()
        {
            if (!SelectedTime.HasValue)
                return;
            _hour = SelectedTime.Value.Hour;
            _minute = SelectedTime.Value.Minute;
            _second = SelectedTime.Value.Second;

            _listBoxHour.SelectionChanged -= ListBoxHour_SelectionChanged;
            var hour = _hour.ToString("D2");
            _listBoxHour.SelectedItem = hour;
            _listBoxHour.SelectionChanged += ListBoxHour_SelectionChanged;

            _listBoxMinute.SelectionChanged -= ListBoxMinute_SelectionChanged;
            var minute = _minute.ToString("D2");
            _listBoxMinute.SelectedItem = minute;
            _listBoxMinute.SelectionChanged += ListBoxMinute_SelectionChanged;

            _listBoxSecond.SelectionChanged -= ListBoxSecond_SelectionChanged;
            var second = _second.ToString("D2");
            _listBoxSecond.SelectedItem = second;
            _listBoxSecond.SelectionChanged += ListBoxSecond_SelectionChanged;

            SetSelectedTime();
        }
    }
}