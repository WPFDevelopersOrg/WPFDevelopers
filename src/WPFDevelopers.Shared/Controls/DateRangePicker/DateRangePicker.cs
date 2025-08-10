using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = PopupTemplateName, Type = typeof(Popup))]
    [TemplatePart(Name = StartCalendarTemplateName, Type = typeof(Calendar))]
    [TemplatePart(Name = EndCalendarTemplateName, Type = typeof(Calendar))]
    [TemplatePart(Name = TextBoxStartTemplateName, Type = typeof(DatePickerTextBox))]
    [TemplatePart(Name = TextBoxEndTemplateName, Type = typeof(DatePickerTextBox))]
    public class DateRangePicker : Control
    {
        private const string PopupTemplateName = "PART_Popup";
        private const string StartCalendarTemplateName = "PART_StartCalendar";
        private const string EndCalendarTemplateName = "PART_EndCalendar";
        private const string TextBoxStartTemplateName = "PART_TextBoxStart";
        private const string TextBoxEndTemplateName = "PART_TextBoxEnd";

        public static readonly DependencyProperty StartWatermarkProperty =
            DependencyProperty.Register("StartWatermark",
                typeof(string),
                typeof(DateRangePicker),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty EndWatermarkProperty =
            DependencyProperty.Register("EndWatermark",
                typeof(string),
                typeof(DateRangePicker),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty StartDateProperty =
            DependencyProperty.Register("StartDate", typeof(DateTime?), typeof(DateRangePicker),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                    OnStartDateChanged));

        public static readonly DependencyProperty EndDateProperty =
            DependencyProperty.Register("EndDate", typeof(DateTime?), typeof(DateRangePicker),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                    OnEndDateChanged));

        public static readonly DependencyProperty DateFormatFormatProperty =
            DependencyProperty.Register("DateFormat", typeof(string), typeof(DateRangePicker),
                new PropertyMetadata("yyy-MM-dd"));

        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(DateRangePicker),
                new UIPropertyMetadata(SystemParameters.PrimaryScreenHeight / 2.5, OnMaxDropDownHeightChanged));

        public static readonly DependencyProperty IsSyncingDisplayDateProperty =
            DependencyProperty.Register(nameof(IsSyncingDisplayDate), typeof(bool), typeof(DateRangePicker),
                new PropertyMetadata(false, OnIsSyncingDisplayDateChanged));

        public static readonly DependencyProperty MinDateProperty =
    DependencyProperty.Register("MinDate", typeof(DateTime?), typeof(DateRangePicker),
        new PropertyMetadata(null, OnMinDateChanged));

        public static readonly DependencyProperty MaxDateProperty =
            DependencyProperty.Register("MaxDate", typeof(DateTime?), typeof(DateRangePicker),
                    new PropertyMetadata(null, OnMaxDateChanged));

        private int _clickCount;

        private HwndSource _hwndSource;
        private bool _isHandlingSelectionChange;
        private Popup _popup;
        private Calendar _startCalendar, _endCalendar;
        private IEnumerable<CalendarDayButton> _startCalendarDayButtons, _endCalendarDayButtons;
        private DateTime? _startDate, _endDate;
        private DatePickerTextBox _textBoxStart, _textBoxEnd;
        private Window _window;

        static DateRangePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateRangePicker),
                new FrameworkPropertyMetadata(typeof(DateRangePicker)));
        }

        public string StartWatermark
        {
            get => (string)GetValue(StartWatermarkProperty);
            set => SetValue(StartWatermarkProperty, value);
        }

        public string EndWatermark
        {
            get => (string)GetValue(EndWatermarkProperty);
            set => SetValue(EndWatermarkProperty, value);
        }


        public DateTime? StartDate
        {
            get => (DateTime?)GetValue(StartDateProperty);
            set => SetValue(StartDateProperty, value);
        }

        public DateTime? EndDate
        {
            get => (DateTime?)GetValue(EndDateProperty);
            set => SetValue(EndDateProperty, value);
        }

        public string DateFormat
        {
            get => (string)GetValue(DateFormatFormatProperty);
            set => SetValue(DateFormatFormatProperty, value);
        }

        public bool IsSyncingDisplayDate
        {
            get => (bool)GetValue(IsSyncingDisplayDateProperty);
            set => SetValue(IsSyncingDisplayDateProperty, value);
        }

        public DateTime? MinDate
        {
            get => (DateTime?)GetValue(MinDateProperty);
            set => SetValue(MinDateProperty, value);
        }

        public DateTime? MaxDate
        {
            get => (DateTime?)GetValue(MaxDateProperty);
            set => SetValue(MaxDateProperty, value);
        }

        private static void OnStartDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as DateRangePicker;
            if (ctrl != null && ctrl._textBoxStart != null)
            {
                ctrl._textBoxStart.Text = e.NewValue?.ToString();
                ctrl.UpdateStartDateFromTextBox();
            }
        }

        private static void OnEndDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as DateRangePicker;
            if (ctrl != null && ctrl._textBoxEnd != null)
            {
                ctrl._textBoxEnd.Text = e.NewValue?.ToString();
                ctrl.UpdateEndDateFromTextBox();
            }
        }

        private static void OnMaxDropDownHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as DateRangePicker;
            if (ctrl != null)
                ctrl.OnMaxDropDownHeightChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual void OnMaxDropDownHeightChanged(double oldValue, double newValue)
        {
        }

        private static void OnIsSyncingDisplayDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as DateRangePicker;
            if (ctrl != null)
                ctrl.OnIsSyncingDisplayDateChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnIsSyncingDisplayDateChanged(bool oldValue, bool newValue)
        {
        }

        private static void OnMinDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (DateRangePicker)d;
            if (control._startCalendar != null)
                OnMinAndMaxChanged(control);
        }

        private static void OnMaxDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (DateRangePicker)d;
            if (control._endCalendar != null)
                OnMinAndMaxChanged(control);
        }

        static void OnMinAndMaxChanged(DateRangePicker control)
        {
            control._startCalendar.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, control.MinDate.Value.AddDays(-1)));
            control._endCalendar.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, control.MinDate.Value.AddDays(-1)));
            control._startCalendar.BlackoutDates.Add(new CalendarDateRange(control.MaxDate.Value.AddDays(1), DateTime.MaxValue));
            control._endCalendar.BlackoutDates.Add(new CalendarDateRange(control.MaxDate.Value.AddDays(1), DateTime.MaxValue));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _window = Window.GetWindow(this);
            if (_window != null)
            {
                if (_window.IsInitialized)
                {
                    WindowSourceInitialized(_window, EventArgs.Empty);
                }
                else
                {
                    _window.SourceInitialized -= WindowSourceInitialized;
                    _window.SourceInitialized += WindowSourceInitialized;
                }
            }

            _popup = (Popup)GetTemplateChild(PopupTemplateName);
            if (_popup != null)
            {
                _popup.Focusable = true;
                _popup.PlacementTarget = this;
                _popup.Closed += OnPopup_Closed;
                _popup.Closed -= OnPopup_Closed;
                _popup.Opened -= OnPopup_Opened;
                _popup.Opened += OnPopup_Opened;
            }

            AddHandler(PreviewMouseUpEvent, new MouseButtonEventHandler(OnPreviewMouseUp), true);

            _startCalendar = (Calendar)GetTemplateChild(StartCalendarTemplateName);
            if (_startCalendar != null)
            {
                _startCalendar.PreviewMouseUp -= OnCalendar_PreviewMouseUp;
                _startCalendar.PreviewMouseUp += OnCalendar_PreviewMouseUp;
                _startCalendar.DisplayDateChanged -= OnStartCalendar_DisplayDateChanged;
                _startCalendar.DisplayDateChanged += OnStartCalendar_DisplayDateChanged;
                _startCalendar.PreviewMouseUp -= OnStartCalendar_PreviewMouseUp;
                _startCalendar.PreviewMouseUp += OnStartCalendar_PreviewMouseUp;
            }

            _endCalendar = (Calendar)GetTemplateChild(EndCalendarTemplateName);
            if (_endCalendar != null)
            {
                _endCalendar.PreviewMouseUp -= OnCalendar_PreviewMouseUp;
                _endCalendar.PreviewMouseUp += OnCalendar_PreviewMouseUp;
                _endCalendar.DisplayDateChanged -= OnEndCalendar_DisplayDateChanged;
                _endCalendar.DisplayDateChanged += OnEndCalendar_DisplayDateChanged;
                _endCalendar.PreviewMouseUp -= OnEndCalendar_PreviewMouseUp;
                _endCalendar.PreviewMouseUp += OnEndCalendar_PreviewMouseUp;
            }

            if (_startCalendar != null)
            {
                if (MinDate != null)
                    _startCalendar.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, MinDate.Value.AddDays(-1)));
                if (MaxDate != null)
                    _startCalendar.BlackoutDates.Add(new CalendarDateRange(MaxDate.Value.AddDays(1), DateTime.MaxValue));
            }
            if (_endCalendar != null)
            {
                if (MinDate != null)
                    _endCalendar.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, MinDate.Value.AddDays(-1)));
                if (MaxDate != null)
                    _endCalendar.BlackoutDates.Add(new CalendarDateRange(MaxDate.Value.AddDays(1), DateTime.MaxValue));
            }
            var now = MinDate == null ? DateTime.Now : MinDate.Value;
            var firstDayOfNextMonth = new DateTime(now.Year, now.Month, 1).AddMonths(1);
            _startCalendar.DisplayDate = now;
            _startCalendar.DisplayDateEnd = firstDayOfNextMonth.AddDays(-1);
            _endCalendar.DisplayDate = firstDayOfNextMonth;
            _endCalendar.DisplayDateStart = firstDayOfNextMonth;
            var window = Window.GetWindow(this);
            if (window != null)
                window.MouseDown += OnWindow_MouseDown;

            _startCalendarDayButtons = GetCalendarDayButtons(_startCalendar);
            _endCalendarDayButtons = GetCalendarDayButtons(_endCalendar);
            _textBoxStart = (DatePickerTextBox)GetTemplateChild(TextBoxStartTemplateName);
            if (_textBoxStart != null)
                _textBoxStart.TextChanged += TextBoxStart_TextChanged;
            _textBoxEnd = (DatePickerTextBox)GetTemplateChild(TextBoxEndTemplateName);
            if (_textBoxEnd != null)
                _textBoxEnd.TextChanged += TextBoxEnd_TextChanged;
            Loaded += DateRangePicker_Loaded;
        }

        private void OnEndCalendar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isHandlingSelectionChange)
                return;
            _isHandlingSelectionChange = true;
            try
            {
                var calendar = sender as Calendar;
                if (e.OriginalSource is FrameworkElement fe)
                {
                    var dayButton = ControlsHelper.FindParent<CalendarDayButton>(fe);
                    if (dayButton != null && dayButton.DataContext is DateTime clickedDate)
                        if (!calendar.SelectedDates.Contains(clickedDate))
                        {
                            var dateTime = clickedDate;
                            _startCalendar.SelectedDates.Clear();
                            ResetDate(dateTime);
                        }
                }

                SetSelectedDates();
            }
            finally
            {
                _isHandlingSelectionChange = false;
            }
        }

        private void OnStartCalendar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isHandlingSelectionChange)
                return;
            _isHandlingSelectionChange = true;
            try
            {
                var calendar = sender as Calendar;
                if (e.OriginalSource is FrameworkElement fe)
                {
                    var dayButton = ControlsHelper.FindParent<CalendarDayButton>(fe);
                    if (dayButton != null && dayButton.DataContext is DateTime clickedDate)
                        if (!calendar.SelectedDates.Contains(clickedDate))
                        {
                            var dateTime = clickedDate;
                            _endCalendar.SelectedDates.Clear();
                            ResetDate(dateTime);
                        }
                }

                SetSelectedDates();
            }
            finally
            {
                _isHandlingSelectionChange = false;
            }
        }

        private void OnBorder_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Button button)
                return;
            if (_popup != null && !_popup.IsOpen)
                _popup.IsOpen = true;
        }

        private void TextBoxEnd_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateEndDateFromTextBox();
        }

        private void UpdateEndDateFromTextBox()
        {
            if (_textBoxEnd != null)
            {
                if (string.IsNullOrWhiteSpace(_textBoxEnd.Text))
                {
                    _endDate = null;
                    SetIsHighlightFalse(_startCalendarDayButtons);
                    return;
                }

                if (DateTime.TryParse(_textBoxEnd.Text, out var dateTime))
                {
                    if (EndDate.HasValue && dateTime.ToString(DateFormat) == EndDate.Value.ToString(DateFormat))
                        return;
                    if (StartDate.HasValue && dateTime < StartDate.Value.Date)
                    {
                        EndDate = _endDate;
                        _textBoxEnd.Text = _endDate.Value.ToString(DateFormat);
                        return;
                    }

                    SetIsHighlightFalse(_endCalendarDayButtons);
                    EndDate = dateTime;
                    PopupOpened();
                }
                else
                {
                    EndDate = _endDate;
                    _textBoxEnd.Text = _endDate.Value.ToString(DateFormat);
                }
            }
        }

        private void TextBoxStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateStartDateFromTextBox();
        }

        private void UpdateStartDateFromTextBox()
        {
            if (_textBoxStart != null)
            {
                if (string.IsNullOrWhiteSpace(_textBoxStart.Text))
                {
                    _startDate = null;
                    SetIsHighlightFalse(_startCalendarDayButtons);
                    return;
                }

                if (DateTime.TryParse(_textBoxStart.Text, out var dateTime))
                {
                    if (StartDate.HasValue && dateTime.ToString(DateFormat) == StartDate.Value.ToString(DateFormat))
                        return;
                    if (EndDate.HasValue && dateTime < EndDate.Value.Date)
                    {
                        StartDate = _startDate;
                        _textBoxStart.Text = _startDate.Value.ToString(DateFormat);
                        return;
                    }

                    SetIsHighlightFalse(_startCalendarDayButtons);
                    StartDate = dateTime;
                    PopupOpened();
                }
                else
                {
                    StartDate = _startDate;
                    _textBoxStart.Text = _startDate.Value.ToString(DateFormat);
                }
            }
        }

        private void DateRangePicker_Loaded(object sender, RoutedEventArgs e)
        {
            if (_textBoxStart != null && StartDate.HasValue)
                _textBoxStart.Text = StartDate.Value.ToString(DateFormat);
            if (_textBoxEnd != null && EndDate.HasValue)
                _textBoxEnd.Text = EndDate.Value.ToString(DateFormat);
        }

        private void OnPopup_Closed(object sender, EventArgs e)
        {
            _window.PreviewMouseDown -= OnWindowPreviewMouseDown;
        }

        private void WindowSourceInitialized(object sender, EventArgs e)
        {
            var window = sender as Window;
            if (window != null)
            {
                _hwndSource = PresentationSource.FromVisual(window) as HwndSource;
                if (_hwndSource != null) _hwndSource.AddHook(WndProc);
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_NCLBUTTONDOWN = 0x00A1;
            if (msg == WM_NCLBUTTONDOWN)
            {
                _popup.IsOpen = false;
                _startDate = null;
                _endDate = null;
                _startCalendar.SelectedDate = null;
                _endCalendar.SelectedDate = null;
            }

            return IntPtr.Zero;
        }

        private void OnPopup_Opened(object sender, EventArgs e)
        {
            _window.PreviewMouseDown += OnWindowPreviewMouseDown;
            PopupOpened();
        }

        private void OnWindowPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsMouseOver)
            {
                _popup.IsOpen = false;
                _startDate = null;
                _endDate = null;
                _startCalendar.SelectedDate = null;
                _endCalendar.SelectedDate = null;
            }
        }

        private void PopupOpened()
        {
            if (IsSyncingDisplayDate)
            {
                var btnNext = GetButtonFromCalendar(_startCalendar, true);
                if (btnNext != null)
                    btnNext.Visibility = Visibility.Collapsed;
                var prevBtn = GetButtonFromCalendar(_endCalendar, false);
                if (prevBtn != null)
                    prevBtn.Visibility = Visibility.Collapsed;
            }

            if (StartDate.HasValue)
                _startDate = StartDate.Value;
            if (EndDate.HasValue)
                _endDate = EndDate.Value;
            _clickCount = 0;
            SetSelectedDates(EndDate);
        }

        private void OnEndCalendar_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            if (_startCalendar != null)
            {
                var selectedDate = _startCalendar.DisplayDate;
                var target = _endCalendar.DisplayDate.AddMonths(-1);
                if (_startCalendar.DisplayDate.Year != target.Year
                    ||
                    _startCalendar.DisplayDate.Month != target.Month)
                {
                    if (IsSyncingDisplayDate)
                        _startCalendar.DisplayDate = target;
                    _startCalendar.DisplayDateEnd = target.AddMonths(1).AddDays(-1);
                }
                else
                {
                    if (!IsSyncingDisplayDate)
                        _startCalendar.DisplayDateEnd = target.AddMonths(1).AddDays(-1);
                }
            }

            if (!_startDate.HasValue || !_endDate.HasValue)
                return;
            SetIsHighlightFalse(_endCalendarDayButtons);
            var isYearMonthBetween = IsYearMonthBetween(e.AddedDate.Value, _startDate.Value, _endDate.Value);
            if (isYearMonthBetween)
                SetIsHighlight(_endDate.Value);
        }

        private void OnStartCalendar_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            if (_endCalendar != null)
            {
                var selectedDate = _endCalendar.DisplayDate;
                var target = _startCalendar.DisplayDate.AddMonths(1);
                if (_endCalendar.DisplayDate.Year != target.Year
                    ||
                    _endCalendar.DisplayDate.Month != target.Month)
                {
                    _endCalendar.DisplayDateStart = target;
                    if (IsSyncingDisplayDate)
                        _endCalendar.DisplayDate = target;
                }
                else
                {
                    if (!IsSyncingDisplayDate)
                        _endCalendar.DisplayDateStart = target;
                }
            }

            if (!_startDate.HasValue || !_endDate.HasValue)
                return;
            SetIsHighlightFalse(_startCalendarDayButtons);
            var isYearMonthBetween = IsYearMonthBetween(e.AddedDate.Value, _startDate.Value, _endDate.Value);
            if (isYearMonthBetween)
                SetIsHighlight(_endDate.Value);
        }

        private void OnCalendar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.Captured is CalendarItem)
            {
                _clickCount++;
                Mouse.Capture(null);
            }
        }

        private void OnWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_popup != null && _popup.IsOpen)
            {
                _popup.IsOpen = false;
                _startDate = null;
                _endDate = null;
                _startCalendar.SelectedDate = null;
                _endCalendar.SelectedDate = null;
            }
        }

        private void SetDate()
        {
            _popup.IsOpen = false;
            StartDate = _startDate;
            EndDate = _endDate;
            if (_startDate.HasValue)
                _textBoxStart.Text = _startDate.Value.ToString(DateFormat);
            if (_endDate.HasValue)
                _textBoxEnd.Text = _endDate.Value.ToString(DateFormat);
        }

        private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Button button)
                return;
            if (_popup != null && !_popup.IsOpen)
                _popup.IsOpen = true;
        }

        private void ResetDate(DateTime? dateTime)
        {
            if (_startDate.HasValue && _endDate.HasValue)
            {
                _startDate = Convert.ToDateTime(dateTime);
                _endDate = null;
                if (_startCalendarDayButtons != null)
                {
                    var startDays = _startCalendarDayButtons.Where(x => DatePickerHelper.GetIsHighlight(x));
                    foreach (var day in startDays) DatePickerHelper.SetIsHighlight(day, false);
                }

                if (_endCalendarDayButtons != null)
                {
                    var endDays = _endCalendarDayButtons.Where(x => DatePickerHelper.GetIsHighlight(x));
                    foreach (var day in endDays) DatePickerHelper.SetIsHighlight(day, false);
                }
            }
            else
            {
                if (!_startDate.HasValue)
                    _startDate = Convert.ToDateTime(dateTime);
                else
                    _endDate = Convert.ToDateTime(dateTime);
            }
        }

        private void SetSelectedDates(DateTime? endDate = null)
        {
            if (_startDate.HasValue && _endDate.HasValue)
            {
                if (DateTime.Compare(_startDate.Value, _endDate.Value) > 0)
                {
                    var temp = _startDate.Value;
                    _startDate = _endDate.Value;
                    _endDate = temp;
                }

                _startCalendar.SelectedDates.Clear();
                _endCalendar.SelectedDates.Clear();
                var eDate = _endDate;
                if (endDate.HasValue)
                    eDate = endDate.Value;
                _startCalendar.SelectedDates.AddRange(_startDate.Value, eDate.Value);
                _endCalendar.SelectedDates.AddRange(_startDate.Value, eDate.Value);
                SetIsHighlight(eDate.Value);
                if (_clickCount == 2) SetDate();
            }
        }

        private void SetIsHighlight(DateTime endDate)
        {
            foreach (var date in _startCalendar.SelectedDates)
            {
                if (date.Date == _startDate.Value.Date || date.Date >= endDate.Date)
                    continue;
                if (_startCalendarDayButtons != null)
                {
                    var day = _startCalendarDayButtons.FirstOrDefault(x =>
                        x.DataContext is DateTime buttonDate && buttonDate.Date == date.Date);
                    if (day != null)
                        if (day.DataContext is DateTime dt)
                            DatePickerHelper.SetIsHighlight(day,
                                dt.Month == _startCalendar.DisplayDate.Month &&
                                dt.Year == _startCalendar.DisplayDate.Year);
                }
            }

            foreach (var date in _endCalendar.SelectedDates)
            {
                if (date.Date == endDate.Date || date.Date <= _startDate.Value.Date)
                    continue;
                if (_endCalendarDayButtons != null)
                {
                    var day = _endCalendarDayButtons.FirstOrDefault(x =>
                        x.DataContext is DateTime buttonDate && buttonDate.Date == date.Date);
                    if (day != null)
                        if (day.DataContext is DateTime dt)
                            DatePickerHelper.SetIsHighlight(day,
                                dt.Month == _endCalendar.DisplayDate.Month && dt.Year == _endCalendar.DisplayDate.Year);
                }
            }
        }

        private void SetIsHighlightFalse(IEnumerable<CalendarDayButton> calendarDayButtons)
        {
            if (calendarDayButtons == null)
                return;
            var days = calendarDayButtons.Where(x => DatePickerHelper.GetIsHighlight(x));
            foreach (var day in days)
                DatePickerHelper.SetIsHighlight(day, false);
        }

        private bool IsYearMonthBetween(DateTime dateToCheck, DateTime startDate, DateTime endDate)
        {
            return dateToCheck.Year == startDate.Year && dateToCheck.Month >= startDate.Month &&
                   dateToCheck.Year == endDate.Year && dateToCheck.Month <= endDate.Month;
        }

        private IEnumerable<CalendarDayButton> GetCalendarDayButtons(DependencyObject parent)
        {
            if (parent == null) yield break;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is CalendarDayButton dayButton)
                    yield return dayButton;
                foreach (var result in GetCalendarDayButtons(child))
                    yield return result;
            }
        }

        private Button GetButtonFromCalendar(Calendar calendar, bool isNext)
        {
            var calendarItemTemplate = calendar.Template.FindName("PART_CalendarItem", calendar) as CalendarItem;

            if (calendarItemTemplate != null)
            {
                var buttonName = isNext ? "PART_NextButton" : "PART_PreviousButton";
                var button = calendarItemTemplate.Template.FindName(buttonName, calendarItemTemplate) as Button;
                if (button != null)
                    return button;
            }

            return null;
        }
    }
}