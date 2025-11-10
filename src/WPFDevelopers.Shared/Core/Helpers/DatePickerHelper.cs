using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Helpers
{
    public class DatePickerHelper
    {
        private static readonly Dictionary<DatePicker, TimeSpan> _timeStorage = new Dictionary<DatePicker, TimeSpan>();
        private static readonly Dictionary<DatePicker, TimePicker> _timePickerStorage = new Dictionary<DatePicker, TimePicker>();
        private static readonly Dictionary<DatePicker, DatePickerTextBox> _textBoxStorage = new Dictionary<DatePicker, DatePickerTextBox>();

        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.RegisterAttached("Watermark", typeof(object), typeof(DatePickerHelper),
                new FrameworkPropertyMetadata(string.Empty,
                    OnWatermarkChanged));

        public static readonly DependencyProperty TextTrimmingProperty =
            DependencyProperty.RegisterAttached(
                "TextTrimming", typeof(TextTrimming), typeof(DatePickerHelper),
                new FrameworkPropertyMetadata(
                    default(TextTrimming),
                    FrameworkPropertyMetadataOptions.Inherits,
                    TextTrimmingPropertyChanged));

        public static readonly DependencyProperty IsMonthYearProperty =
       DependencyProperty.RegisterAttached("IsMonthYear", typeof(bool), typeof(DatePickerHelper),
                                           new PropertyMetadata(OnIsMonthYearChanged));

        public static readonly DependencyProperty IsHighlightProperty =
            DependencyProperty.RegisterAttached("IsHighlight", typeof(bool), typeof(DatePickerHelper),
                new PropertyMetadata(false));

        public static readonly DependencyProperty DateTimeFormatProperty =
            DependencyProperty.RegisterAttached(
                "DateTimeFormat",
                typeof(string),
                typeof(DatePickerHelper),
                new PropertyMetadata("yyyy-MM-dd HH:mm:ss"));

        public static readonly DependencyProperty ShowTimeProperty =
            DependencyProperty.RegisterAttached(
                "ShowTime",
                typeof(bool),
                typeof(DatePickerHelper),
                new PropertyMetadata(false, OnShowTimeChanged));

        public static readonly DependencyProperty SelectedDateTimeProperty =
            DependencyProperty.RegisterAttached("SelectedDateTime", typeof(DateTime?), typeof(DatePickerHelper),
                new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnSelectedDateTimeChanged));


        public static object GetWatermark(DependencyObject obj)
        {
            return obj.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(DependencyObject obj, object value)
        {
            obj.SetValue(WatermarkProperty, value);
        }

        public static TextTrimming GetTextTrimming(DependencyObject obj)
        {
            return (TextTrimming)obj.GetValue(TextTrimmingProperty);
        }

        public static void SetTextTrimming(DependencyObject obj, TextTrimming value)
        {
            obj.SetValue(TextTrimmingProperty, value);
        }
        public static bool GetIsMonthYear(DependencyObject dobj)
        {
            return (bool)dobj.GetValue(IsMonthYearProperty);
        }

        public static void SetIsMonthYear(DependencyObject dobj, bool value)
        {
            dobj.SetValue(IsMonthYearProperty, value);
        }

        public static bool GetIsHighlight(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsHighlightProperty);
        }

        public static void SetIsHighlight(DependencyObject obj, bool value)
        {
            obj.SetValue(IsHighlightProperty, value);
        }
        public static string GetDateTimeFormat(DependencyObject obj)
        {
            return (string)obj.GetValue(DateTimeFormatProperty);
        }
        public static void SetDateTimeFormat(DependencyObject obj, string value)
        {
            obj.SetValue(DateTimeFormatProperty, value);
        }
        public static bool GetShowTime(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowTimeProperty);
        }

        public static void SetShowTime(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowTimeProperty, value);
        }

        public static DateTime? GetSelectedDateTime(DependencyObject obj)
        {
            return (DateTime?)obj.GetValue(SelectedDateTimeProperty);
        }

        public static void SetSelectedDateTime(DependencyObject obj, DateTime? value)
        {
            obj.SetValue(SelectedDateTimeProperty, value);
        }

        private static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DatePicker datePicker)
            {
                if (datePicker.IsLoaded)
                {
                    SetWatermarkInternal(datePicker, e.NewValue);
                }
                else
                {
                    RoutedEventHandler loadedHandler = null;
                    loadedHandler = delegate
                    {
                        datePicker.Loaded -= loadedHandler;
                        SetWatermarkInternal(datePicker, e.NewValue);
                    };
                    datePicker.Loaded += loadedHandler;
                }
            }
            else if (d is DatePickerTextBox datePickerTextBox)
            {
                if (datePickerTextBox.IsLoaded)
                {
                    SetDatePickerTextBoxWatermark(datePickerTextBox, e.NewValue);
                }
                else
                {
                    RoutedEventHandler loadedHandler = null;
                    loadedHandler = delegate
                    {
                        datePickerTextBox.Loaded -= loadedHandler;
                        SetDatePickerTextBoxWatermark(datePickerTextBox, e.NewValue);
                    };
                    datePickerTextBox.Loaded += loadedHandler;
                }
            }
        }

        private static void SetDatePickerTextBoxWatermark(DatePickerTextBox d, object value)
        {
            if (d != null)
            {
                var watermarkControl = d.Template.FindName("PART_Watermark", d) as ContentControl;
                if (watermarkControl != null)
                    watermarkControl.Content = value;
            }
        }

        private static void SetWatermarkInternal(DatePicker d, object value)
        {
            var textBox = d.Template.FindName("PART_TextBox", d) as Control;
            if (textBox != null)
            {
                var watermarkControl = textBox.Template.FindName("PART_Watermark", textBox) as ContentControl;
                if (watermarkControl != null)
                    watermarkControl.Content = value;
            }
        }

        private static void TextTrimmingPropertyChanged(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = obj as TextBlock;
            if (textBlock != null)
                textBlock.TextTrimming = (TextTrimming)e.NewValue;
        }

        private static void OnIsMonthYearChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (DatePicker)obj;
            if (ctrl == null) return;
            var syncContext = SynchronizationContext.Current;
            if (syncContext != null)
            {
                syncContext.Post(new SendOrPostCallback((state) =>
                {
                    SetCalendarEventHandlers(ctrl, e);
                }), null);
            }
        }

        private static void SetCalendarEventHandlers(DatePicker datePicker, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;

            if ((bool)e.NewValue)
            {
                datePicker.CalendarOpened += DatePickerOnCalendarOpened;
                datePicker.CalendarClosed += DatePickerOnCalendarClosed;
            }
            else
            {
                datePicker.CalendarOpened -= DatePickerOnCalendarOpened;
                datePicker.CalendarClosed -= DatePickerOnCalendarClosed;
            }
        }

        private static void DatePickerOnCalendarOpened(object sender, RoutedEventArgs routedEventArgs)
        {
            var calendar = GetDatePickerCalendar(sender);
            calendar.DisplayMode = CalendarMode.Year;
            calendar.DisplayModeChanged += CalendarOnDisplayModeChanged;
        }

        private static void DatePickerOnCalendarClosed(object sender, RoutedEventArgs routedEventArgs)
        {
            var datePicker = (DatePicker)sender;
            var calendar = GetDatePickerCalendar(sender);
            datePicker.SelectedDate = calendar.SelectedDate;
            calendar.DisplayModeChanged -= CalendarOnDisplayModeChanged;
        }

        private static void CalendarOnDisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
        {
            var calendar = (Calendar)sender;
            if (calendar.DisplayMode != CalendarMode.Month)
                return;

            calendar.SelectedDate = GetSelectedCalendarDate(calendar.DisplayDate);

            var datePicker = GetCalendarsDatePicker(calendar);
            datePicker.IsDropDownOpen = false;
        }

        private static Calendar GetDatePickerCalendar(object sender)
        {
            var datePicker = (DatePicker)sender;
            var popup = (Popup)datePicker.Template.FindName("PART_Popup", datePicker);
            return ((Calendar)popup.Child);
        }

        private static DatePicker GetCalendarsDatePicker(FrameworkElement child)
        {
            var parent = (FrameworkElement)child.Parent;
            if (parent.Name == "PART_Root")
                return (DatePicker)parent.TemplatedParent;
            return GetCalendarsDatePicker(parent);
        }

        private static DateTime? GetSelectedCalendarDate(DateTime? selectedDate)
        {
            if (!selectedDate.HasValue)
                return null;
            return new DateTime(selectedDate.Value.Year, selectedDate.Value.Month, 1);
        }

        private static void OnSelectedDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DatePicker datePicker)
            {
                if (e.NewValue is DateTime dateTime)
                {
                    if (!IsValidDateTime(dateTime)) return;
                    _timeStorage[datePicker] = dateTime.TimeOfDay;
                    datePicker.SelectedDate = dateTime;
                }
                else if (e.NewValue == null)
                {
                    if (_timeStorage.ContainsKey(datePicker))
                        _timeStorage.Remove(datePicker);
                    datePicker.SelectedDate = null;
                }

            }

        }

        static bool IsValidDateTime(DateTime dateTime)
        {
            return dateTime != DateTime.MinValue &&
                   dateTime != DateTime.MaxValue &&
                   dateTime.Year >= 1900 &&
                   dateTime.Year <= 2100;
        }

        private static void OnShowTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DatePicker datePicker && e.NewValue is bool showTime)
            {
                if (showTime)
                {
                    datePicker.LostFocus += OnDatePicker_LostFocus;
                    datePicker.Loaded += OnDatePicker_Loaded;
                    datePicker.SelectedDateChanged += OnDatePicker_SelectedDateChanged;
                    datePicker.CalendarOpened += OnDatePicker_CalendarOpened;
                }
                else
                {
                    datePicker.Loaded -= OnDatePicker_Loaded;
                    datePicker.SelectedDateChanged -= OnDatePicker_SelectedDateChanged;
                    datePicker.CalendarOpened -= OnDatePicker_CalendarOpened;
                }
            }
        }

        private static void OnDatePicker_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is DatePicker datePicker)
            {
                UpdateDateTime(datePicker);
                Debug.WriteLine($"OnDatePicker_LostFocus datetime:{datePicker.SelectedDate}");
            }
        }

        private static void OnDatePicker_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (sender is DatePicker datePicker)
            {
                UpdateDateTime(datePicker);
                Debug.WriteLine($"OnDatePicker_CalendarClosed datetime:{datePicker.SelectedDate}_lastTime:");
            }
        }

        private static void OnDatePicker_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is DatePicker datePicker)
            {
                var datePickerTextBox = ControlsHelper.FindVisualChild<DatePickerTextBox>(datePicker);
                if (datePickerTextBox != null)
                {
                    _textBoxStorage[datePicker] = datePickerTextBox;
                    UpdateDateTime(datePicker);
                }

                var popup = ControlsHelper.FindVisualChild<Popup>(datePicker);
                if (popup == null) return;

                popup.Opened += (s, args) =>
                {
                    if (_timePickerStorage.ContainsKey(datePicker)) return;

                    var calendar = popup.Child as Calendar;
                    if (calendar == null) return;
                    var calendarItem = ControlsHelper.FindVisualChild<CalendarItem>(calendar);
                    if (calendarItem == null) return;
                    var timePicker = ControlsHelper.FindVisualChild<TimePicker>(calendarItem);
                    if (timePicker != null)
                    {
                        timePicker.SelectedTime = datePicker.SelectedDate;
                        _timePickerStorage[datePicker] = timePicker;
                        timePicker.SelectedTimeChanged -= OnTimePicker_SelectedTimeChanged;
                        timePicker.SelectedTimeChanged += OnTimePicker_SelectedTimeChanged;
                    }
                };

            }
        }

        private static void OnTimePicker_SelectedTimeChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            var timePicker = sender as TimePicker;
            if (timePicker == null) return;
            var datePickerEntry = _timePickerStorage.FirstOrDefault(x => x.Value == timePicker);
            if (datePickerEntry.Key != null && timePicker.SelectedTime.HasValue)
            {
                _timeStorage[datePickerEntry.Key] = timePicker.SelectedTime.Value.TimeOfDay;
                UpdateDateTime(datePickerEntry.Key);
            }
        }

        private static void OnDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DatePicker datePicker)
            {
                UpdateDateTime(datePicker);
            }
        }

        private static void OnDatePicker_CalendarOpened(object sender, RoutedEventArgs e)
        {
            if (sender is DatePicker datePicker)
            {
                UpdateDateTime(datePicker);
            }
        }

        private static void UpdateDateTime(DatePicker datePicker)
        {
            TimeSpan timePart = TimeSpan.Zero;

            if (_timeStorage.TryGetValue(datePicker, out var storedTime))
            {
                timePart = storedTime;
            }
            else if (datePicker.SelectedDate.HasValue && datePicker.SelectedDate.Value.TimeOfDay != TimeSpan.Zero)
            {
                timePart = datePicker.SelectedDate.Value.TimeOfDay;
                _timeStorage[datePicker] = timePart;
            }
            else
            {
                timePart = DateTime.Now.TimeOfDay;
                _timeStorage[datePicker] = timePart;
            }

            if (datePicker.SelectedDate.HasValue)
            {
                var newDateTime = datePicker.SelectedDate.Value.Date + timePart;
                if (datePicker.SelectedDate != newDateTime)
                {
                    datePicker.SelectedDate = newDateTime;
                }
                SetSelectedDateTime(datePicker, newDateTime);
            }

            if (_textBoxStorage.TryGetValue(datePicker, out var textBox) && datePicker.SelectedDate.HasValue)
            {
                textBox.Text = datePicker.SelectedDate.Value.ToString(GetDateTimeFormat(datePicker));
            }

            if (_timePickerStorage.TryGetValue(datePicker, out var timePicker) && datePicker.SelectedDate.HasValue)
            {
                timePicker.SelectedTimeChanged -= OnTimePicker_SelectedTimeChanged;
                timePicker.SelectedTime = datePicker.SelectedDate;
                timePicker.SelectedTimeChanged += OnTimePicker_SelectedTimeChanged;
            }
        }
    }
}