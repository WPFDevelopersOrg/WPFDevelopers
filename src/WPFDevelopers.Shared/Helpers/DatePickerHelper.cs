using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace WPFDevelopers.Helpers
{
    public class DatePickerHelper
    {
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
            return (TextTrimming) obj.GetValue(TextTrimmingProperty);
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
        private static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var datePicker = (DatePicker) d;
            if (datePicker == null)
                return;
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
                textBlock.TextTrimming = (TextTrimming) e.NewValue;
        }

        private static void OnIsMonthYearChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            var datePicker = (DatePicker)dobj;

            Application.Current.Dispatcher
                .BeginInvoke(DispatcherPriority.Loaded,
                             new Action<DatePicker, DependencyPropertyChangedEventArgs>(SetCalendarEventHandlers),
                             datePicker, e);
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
    }
}