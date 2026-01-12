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
    [TemplatePart(Name = StartTextBoxTemplateName, Type = typeof(DatePickerTextBox))]
    [TemplatePart(Name = EndTextBoxTemplateName, Type = typeof(DatePickerTextBox))]
    [TemplatePart(Name = StartTimePickerTemplateName, Type = typeof(TimePicker))]
    [TemplatePart(Name = EndTimePickerTemplateName, Type = typeof(TimePicker))]
    public class DateRangePicker : Control
    {
        private const string PopupTemplateName = "PART_Popup";
        private const string StartCalendarTemplateName = "PART_StartCalendar";
        private const string EndCalendarTemplateName = "PART_EndCalendar";
        private const string StartTextBoxTemplateName = "PART_StartTextBox";
        private const string EndTextBoxTemplateName = "PART_EndTextBox";
        private const string StartTimePickerTemplateName = "PART_StartTimePicker";
        private const string EndTimePickerTemplateName = "PART_EndTimePicker";

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

        public static readonly DependencyProperty DateTimeFormatProperty =
            DependencyProperty.Register("DateTimeFormat", typeof(string), typeof(DateRangePicker),
                new FrameworkPropertyMetadata("yyyy-MM-dd HH:mm:ss",
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDateFormatChanged));

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

        private static readonly DependencyPropertyKey HasTimeFormatPropertyKey =
    DependencyProperty.RegisterReadOnly("HasTimeFormat", typeof(bool), typeof(DateRangePicker),
        new PropertyMetadata(false));

        public static readonly DependencyProperty HasTimeFormatProperty = HasTimeFormatPropertyKey.DependencyProperty;

        private int _clickCount;
        private HwndSource _hwndSource;
        private bool _isHandlingSelectionChange;
        private Popup _popup;
        private Calendar _startCalendar, _endCalendar;
        private IEnumerable<CalendarDayButton> _startCalendarDayButtons, _endCalendarDayButtons;
        private DateTime? _startDate, _endDate;
        private DatePickerTextBox _startTextBox, _endTextBox;
        private Window _window;
        private TimePicker _startTimePicker, _endTimePicker;

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

        public string DateTimeFormat
        {
            get => (string)GetValue(DateTimeFormatProperty);
            set => SetValue(DateTimeFormatProperty, value);
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
        public bool HasTimeFormat
        {
            get => (bool)GetValue(HasTimeFormatProperty);
            private set => SetValue(HasTimeFormatPropertyKey, value);
        }

        private static void OnStartDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as DateRangePicker;
            if (ctrl != null && ctrl._startTextBox != null)
            {
                ctrl._startTextBox.Text = e.NewValue?.ToString();
                ctrl.UpdateStartDateFromTextBox();
            }
        }

        private static void OnEndDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as DateRangePicker;
            if (ctrl != null && ctrl._endTextBox != null)
            {
                ctrl._endTextBox.Text = e.NewValue?.ToString();
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

        private static void OnDateFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (DateRangePicker)d;
            if (ctrl == null) return;
            ctrl.UpdateTimeFormatDetection();
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
            _startTextBox = (DatePickerTextBox)GetTemplateChild(StartTextBoxTemplateName);
            if (_startTextBox != null)
                _startTextBox.TextChanged += TextBoxStart_TextChanged;
            _endTextBox = (DatePickerTextBox)GetTemplateChild(EndTextBoxTemplateName);
            if (_endTextBox != null)
                _endTextBox.TextChanged += TextBoxEnd_TextChanged;
            UpdateTimeFormatDetection();
            _startTimePicker = (TimePicker)GetTemplateChild(StartTimePickerTemplateName);
            if (_startTimePicker != null)
                _startTimePicker.SelectedTimeChanged += OnStartTimePicker_SelectedTimeChanged;
            _endTimePicker = (TimePicker)GetTemplateChild(EndTimePickerTemplateName);
            if (_endTimePicker != null)
                _endTimePicker.SelectedTimeChanged += OnEndTimePicker_SelectedTimeChanged;
            Loaded += DateRangePicker_Loaded;
        }

        private void UpdateTimeFormatDetection()
        {
            HasTimeFormat = ContainsTimeFormat(DateTimeFormat);
            UpdateTimePickerVisibility();
        }
        private bool ContainsTimeFormat(string format)
        {
            if (string.IsNullOrEmpty(format))
                return false;

            char[] timeFormatChars = { 'H', 'h', 'm', 's', 't', 'K', 'z', 'f', 'F' };
            return timeFormatChars.Any(c => format.Contains(c));
        }

        private void UpdateTimePickerVisibility()
        {
            if (_startTimePicker != null)
            {
                _startTimePicker.Visibility = HasTimeFormat ? Visibility.Visible : Visibility.Collapsed;
            }

            if (_endTimePicker != null)
            {
                _endTimePicker.Visibility = HasTimeFormat ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void OnEndTimePicker_SelectedTimeChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            if (_isHandlingSelectionChange || !HasTimeFormat || !_endDate.HasValue) return;
            var date = _endDate.Value.Date + _endTimePicker.SelectedTime.Value.TimeOfDay;
            _endDate = date;
            SetSelectedDates();
            SetDate();
        }

        private void OnStartTimePicker_SelectedTimeChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            if (_isHandlingSelectionChange || !HasTimeFormat || !_startDate.HasValue) return;
            var date = _startDate.Value.Date + _startTimePicker.SelectedTime.Value.TimeOfDay;
            _startDate = date;
            SetSelectedDates();
            SetDate();
        }

        private void OnEndCalendar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            OnCalendar_PreviewMouseUp(sender, e, false);
        }

        private void OnStartCalendar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            OnCalendar_PreviewMouseUp(sender, e, true);
        }

        private void OnCalendar_PreviewMouseUp(object sender, MouseButtonEventArgs e, bool isStartCalendar)
        {
            if (_isHandlingSelectionChange)
                return;
            _isHandlingSelectionChange = true;
            try
            {
                if (e.OriginalSource is FrameworkElement fe)
                {
                    var dayButton = ControlsHelper.FindParent<CalendarDayButton>(fe);
                    if (dayButton != null && dayButton.DataContext is DateTime clickedDate)
                    {
                        HandleDateSelection(clickedDate, isStartCalendar);
                    }
                }
                SetSelectedDates();
                SetDate();
            }
            finally
            {
                _isHandlingSelectionChange = false;
            }
        }

        private void HandleDateSelection(DateTime clickedDate, bool isStartCalendar)
        {
            TimeSpan startTime = _startTimePicker?.SelectedTime?.TimeOfDay ?? TimeSpan.Zero;
            TimeSpan endTime = _endTimePicker?.SelectedTime?.TimeOfDay ?? TimeSpan.Zero;

            if (!_startDate.HasValue)
            {
                _startDate = clickedDate + startTime;

                if (isStartCalendar)
                    _endCalendar.SelectedDates.Clear();
                else
                    _startCalendar.SelectedDate = clickedDate;
            }
            else if (!_endDate.HasValue)
            {
                DateTime clickedDateTime;
                DateTime otherDateTime;

                if (clickedDate < _startDate.Value.Date)
                {
                    clickedDateTime = clickedDate + startTime; 
                    otherDateTime = _startDate.Value.Date + endTime; 

                    _startDate = clickedDateTime;
                    _endDate = otherDateTime;

                    if (!isStartCalendar)
                        _startCalendar.SelectedDate = clickedDate;
                    else if (_startDate.Value.Date != clickedDate)
                        _endCalendar.SelectedDate = _startDate.Value.Date;
                }
                else
                {
                    clickedDateTime = clickedDate + endTime; 
                    _endDate = clickedDateTime;

                    if (isStartCalendar)
                        _endCalendar.SelectedDate = clickedDate;
                }
            }
            else
            {
                if (isStartCalendar)
                {
                    _startDate = clickedDate + startTime;
                    _endDate = null;
                    _endCalendar.SelectedDates.Clear();
                }
                else
                {
                    _endDate = clickedDate + endTime;
                    _startDate = null;
                    _startCalendar.SelectedDates.Clear();
                }
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
            if (_endTextBox != null)
            {
                if (string.IsNullOrWhiteSpace(_endTextBox.Text))
                {
                    _endDate = null;
                    SetIsHighlightFalse(_startCalendarDayButtons);
                    return;
                }

                if (DateTime.TryParse(_endTextBox.Text, out var dateTime))
                {
                    if (EndDate.HasValue && dateTime.ToString(DateTimeFormat) == EndDate.Value.ToString(DateTimeFormat))
                        return;
                    if (StartDate.HasValue && dateTime < StartDate.Value.Date)
                    {
                        EndDate = _endDate;
                        _endTextBox.Text = _endDate.Value.ToString(DateTimeFormat);
                        return;
                    }

                    SetIsHighlightFalse(_endCalendarDayButtons);
                    EndDate = dateTime;
                    PopupOpened();
                }
                else
                {
                    EndDate = _endDate;
                    _endTextBox.Text = _endDate.Value.ToString(DateTimeFormat);
                }
            }
        }

        private void TextBoxStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateStartDateFromTextBox();
        }

        private void UpdateStartDateFromTextBox()
        {
            if (_startTextBox != null)
            {
                if (string.IsNullOrWhiteSpace(_startTextBox.Text))
                {
                    _startDate = null;
                    SetIsHighlightFalse(_startCalendarDayButtons);
                    return;
                }

                if (DateTime.TryParse(_startTextBox.Text, out var dateTime))
                {
                    if (StartDate.HasValue && dateTime.ToString(DateTimeFormat) == StartDate.Value.ToString(DateTimeFormat))
                        return;
                    if (EndDate.HasValue && dateTime < EndDate.Value.Date)
                    {
                        StartDate = _startDate;
                        _startTextBox.Text = _startDate.Value.ToString(DateTimeFormat);
                        return;
                    }

                    SetIsHighlightFalse(_startCalendarDayButtons);
                    StartDate = dateTime;
                    PopupOpened();
                }
                else
                {
                    StartDate = _startDate;
                    _startTextBox.Text = _startDate.Value.ToString(DateTimeFormat);
                }
            }
        }

        private void DateRangePicker_Loaded(object sender, RoutedEventArgs e)
        {
            if (_startTextBox != null && StartDate.HasValue)
                _startTextBox.Text = StartDate.Value.ToString(DateTimeFormat);
            if (_endTextBox != null && EndDate.HasValue)
                _endTextBox.Text = EndDate.Value.ToString(DateTimeFormat);
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
            if (_clickCount == 2)
                _popup.IsOpen = false;
            StartDate = _startDate;
            EndDate = _endDate;
            if (_startDate.HasValue)
                _startTextBox.Text = _startDate.Value.ToString(DateTimeFormat);
            if (_endDate.HasValue)
                _endTextBox.Text = _endDate.Value.ToString(DateTimeFormat);
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
                var actualStartDate = GetBoundedDate(_startDate.Value);
                var actualEndDate = GetBoundedDate(eDate.Value);
                if (actualStartDate > actualEndDate)
                {
                    actualStartDate = actualEndDate;
                }

                try
                {
                    _startCalendar.SelectedDates.AddRange(actualStartDate, actualEndDate);
                    _endCalendar.SelectedDates.AddRange(actualStartDate, actualEndDate);
                    SetIsHighlight(actualEndDate);
                    SetDate();
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw;
                }
                SetIsHighlight(eDate.Value);
                SetDate();
            }
        }

        private DateTime GetBoundedDate(DateTime date)
        {
            if (MinDate.HasValue && date < MinDate.Value)
                return MinDate.Value;

            if (MaxDate.HasValue && date > MaxDate.Value)
                return MaxDate.Value;

            return date;
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