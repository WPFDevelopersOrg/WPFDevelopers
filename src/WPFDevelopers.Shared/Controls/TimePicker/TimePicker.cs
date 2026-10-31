using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = TimeSelectorTemplateName, Type = typeof(ListBox))]
    [TemplatePart(Name = EditableTextBoxTemplateName, Type = typeof(TextBox))]
    [TemplatePart(Name = PopupTemplateName, Type = typeof(Popup))]
    [TemplatePart(Name = ButtonConfirmTemplateName, Type = typeof(Button))]
    [TemplatePart(Name = ButtonCancelTemplateName, Type = typeof(Button))]
    public class TimePicker : Control
    {
        private const string TimeSelectorTemplateName = "PART_TimeSelector";
        private const string EditableTextBoxTemplateName = "PART_EditableTextBox";
        private const string PopupTemplateName = "PART_Popup";
        private const string ButtonConfirmTemplateName = "PART_ButtonConfirm";
        private const string ButtonCancelTemplateName = "PART_ButtonCancel";

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(TimePicker),
                new PropertyMetadata(false));

        public static readonly DependencyProperty SelectedTimeFormatProperty =
            DependencyProperty.Register("SelectedTimeFormat", typeof(string), typeof(TimePicker),
                new PropertyMetadata("HH:mm:ss", OnSelectedTimeFormatChanged));

        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(TimePicker),
                new UIPropertyMetadata(SystemParameters.PrimaryScreenHeight / 3.0, OnMaxDropDownHeightChanged));

        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register("SelectedTime", typeof(DateTime?), typeof(TimePicker),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, OnSelectedTimeChanged));

        public static readonly DependencyProperty IsCurrentTimeProperty =
            DependencyProperty.Register("IsCurrentTime", typeof(bool), typeof(TimePicker), new PropertyMetadata(false));


        public static readonly RoutedEvent SelectedTimeChangedEvent =
            EventManager.RegisterRoutedEvent("SelectedTimeChanged",
                RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<DateTime?>), typeof(TimePicker));

        private PopupWindowHook _popupWindowHook;
        private Window _window;
        private DateTime _date;
        private Popup _popup;
        private TextBox _textBox;
        private TimeSelector _timeSelector;
        private Button _buttonConfirm;
        private Button _buttonCancel;
        private bool _isSelectingTime;

        static TimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimePicker),
                new FrameworkPropertyMetadata(typeof(TimePicker)));
        }

        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        public string SelectedTimeFormat
        {
            get => (string)GetValue(SelectedTimeFormatProperty);
            set => SetValue(SelectedTimeFormatProperty, value);
        }

        public DateTime? SelectedTime
        {
            get => (DateTime?)GetValue(SelectedTimeProperty);
            set => SetValue(SelectedTimeProperty, value);
        }

        public bool IsCurrentTime
        {
            get => (bool)GetValue(IsCurrentTimeProperty);
            set => SetValue(IsCurrentTimeProperty, value);
        }

        public event RoutedPropertyChangedEventHandler<DateTime?> SelectedTimeChanged
        {
            add { AddHandler(SelectedTimeChangedEvent, value); }
            remove { RemoveHandler(SelectedTimeChangedEvent, value); }
        }

        private static void OnMaxDropDownHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as TimePicker;
            if (ctrl != null)
                ctrl.OnMaxDropDownHeightChanged((double)e.OldValue, (double)e.NewValue);
        }

        private static void OnSelectedTimeFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as TimePicker;
            if (ctrl == null)
                return;

            if (ctrl._timeSelector != null)
            {
                ctrl._timeSelector.IsTwelveHourFormat = UsesTwelveHourFormat((string)e.NewValue);
                ctrl._timeSelector.ShowSeconds = UsesSecondsFormat((string)e.NewValue);
                ctrl._timeSelector.SetTime();
            }
            if (ctrl._textBox != null)
            {
                ctrl._textBox.TextChanged -= ctrl.TextBox_TextChanged;
                ctrl._textBox.Text = ctrl.SelectedTime.HasValue
                    ? ctrl.SelectedTime.Value.ToString(GetDisplayTimeFormat((string)e.NewValue))
                    : string.Empty;
                ctrl._textBox.TextChanged += ctrl.TextBox_TextChanged;
            }
        }

        private static bool UsesTwelveHourFormat(string format)
        {
            return ContainsFormatSpecifier(format, 'h')
                && !ContainsFormatSpecifier(format, 'H');
        }

        private static string GetDisplayTimeFormat(string format)
        {
            if (string.IsNullOrEmpty(format) || UsesTwelveHourFormat(format) || format.Length == 1)
                return format;

            var result = new StringBuilder(format.Length);
            char quote = '\0';
            for (int i = 0; i < format.Length; i++)
            {
                var character = format[i];
                if (character == '\\')
                {
                    result.Append(character);
                    if (i + 1 < format.Length)
                        result.Append(format[++i]);
                    continue;
                }
                if (quote != '\0')
                {
                    result.Append(character);
                    if (character == quote)
                        quote = '\0';
                    continue;
                }
                if (character == '\'' || character == '"')
                {
                    quote = character;
                    result.Append(character);
                    continue;
                }
                if (character == 't')
                {
                    while (result.Length > 0 && char.IsWhiteSpace(result[result.Length - 1]))
                        result.Length--;
                    while (i + 1 < format.Length && format[i + 1] == 't')
                        i++;
                    while (i + 1 < format.Length && char.IsWhiteSpace(format[i + 1]))
                        i++;
                    continue;
                }
                result.Append(character);
            }
            return result.ToString().TrimEnd();
        }

        private static bool UsesSecondsFormat(string format)
        {
            if (format == "T")
                return ContainsFormatSpecifier(CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern, 's');
            if (format == "t")
                return ContainsFormatSpecifier(CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern, 's');
            return ContainsFormatSpecifier(format, 's');
        }

        private static bool ContainsFormatSpecifier(string format, char specifier)
        {
            if (string.IsNullOrEmpty(format))
                return false;

            char quote = '\0';
            bool isEscaped = false;
            foreach (var character in format)
            {
                if (isEscaped)
                {
                    isEscaped = false;
                    continue;
                }
                if (character == '\\')
                {
                    isEscaped = true;
                    continue;
                }
                if (quote != '\0')
                {
                    if (character == quote)
                        quote = '\0';
                    continue;
                }
                if (character == '\'' || character == '"')
                {
                    quote = character;
                    continue;
                }
                if (character == specifier)
                    return true;
            }

            return false;
        }

        protected virtual void OnMaxDropDownHeightChanged(double oldValue, double newValue)
        {
        }

        private static void OnSelectedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as TimePicker;
            if (ctrl != null)
            {
                DateTime? dateTime = DateTime.MinValue;
                if (e.NewValue != null)
                    dateTime = (DateTime)e.NewValue;
                if (ctrl._timeSelector != null && dateTime > DateTime.MinValue)
                    ctrl._timeSelector.SelectedTime = dateTime;
                else
                {
                    if (ctrl._timeSelector != null)
                        ctrl._timeSelector.SelectedTime = null;
                    else
                        ctrl._date = dateTime.Value;
                }
                if (ctrl._buttonConfirm != null && ctrl._textBox != null)
                {
                    ctrl._textBox.TextChanged -= ctrl.TextBox_TextChanged;
                    ctrl._textBox.Text = ctrl.SelectedTime.HasValue
                        ? ctrl.SelectedTime.Value.ToString(GetDisplayTimeFormat(ctrl.SelectedTimeFormat))
                        : string.Empty;
                    ctrl._textBox.TextChanged += ctrl.TextBox_TextChanged;
                }
                ctrl.RaiseSelectedTimeChangedEvent(e.OldValue as DateTime?, e.NewValue as DateTime?);
            }
        }

        protected virtual void RaiseSelectedTimeChangedEvent(DateTime? oldValue, DateTime? newValue)
        {
            var args = new RoutedPropertyChangedEventArgs<DateTime?>(oldValue, newValue);
            args.RoutedEvent = SelectedTimeChangedEvent;
            RaiseEvent(args);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _window = Window.GetWindow(this);
            if (_window != null)
                _popupWindowHook = new PopupWindowHook(_window, () => IsDropDownOpen = false);

            if (_buttonConfirm != null)
                _buttonConfirm.Click -= OnButtonConfirm_Click;
            if (_buttonCancel != null)
                _buttonCancel.Click -= OnButtonCancel_Click;
            _isSelectingTime = false;

            _textBox = GetTemplateChild(EditableTextBoxTemplateName) as TextBox;
            if (_textBox != null)
            {
                _textBox.TextChanged -= TextBox_TextChanged;
                _textBox.PreviewTextInput -= TextBox_PreviewTextInput;
                _textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
                DataObject.RemovePastingHandler(_textBox, TextBox_Pasting);

                _textBox.Text = SelectedTime.HasValue
                    ? SelectedTime.Value.ToString(GetDisplayTimeFormat(SelectedTimeFormat))
                    : string.Empty;

                _textBox.TextChanged += TextBox_TextChanged;
                _textBox.PreviewTextInput += TextBox_PreviewTextInput;
                _textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
                DataObject.AddPastingHandler(_textBox, TextBox_Pasting);
            }
            _buttonConfirm = GetTemplateChild(ButtonConfirmTemplateName) as Button;
            if (_buttonConfirm != null)
                _buttonConfirm.Click += OnButtonConfirm_Click;
            _buttonCancel = GetTemplateChild(ButtonCancelTemplateName) as Button;
            if (_buttonCancel != null)
                _buttonCancel.Click += OnButtonCancel_Click;

            _timeSelector = GetTemplateChild(TimeSelectorTemplateName) as TimeSelector;
            if (_timeSelector != null)
            {
                _timeSelector.IsTwelveHourFormat = UsesTwelveHourFormat(SelectedTimeFormat);
                _timeSelector.ShowSeconds = UsesSecondsFormat(SelectedTimeFormat);
                _timeSelector.SelectedTimeChanged -= OnTimeSelector_SelectedTimeChanged;
                _timeSelector.SelectedTimeChanged += OnTimeSelector_SelectedTimeChanged;
                if (!SelectedTime.HasValue && IsCurrentTime)
                {
                    SelectedTime = DateTime.Now;
                }
                else if (SelectedTime.HasValue)
                {
                    _timeSelector.SelectedTime = SelectedTime;
                }
                else if (_date > DateTime.MinValue)
                {
                    SelectedTime = _date;
                }
            }

            _popup = GetTemplateChild(PopupTemplateName) as Popup;
            if (_popup != null)
            {
                _popup.Opened -= OnPopup_Opened;
                _popup.Opened += OnPopup_Opened;
                _popup.Closed += OnPopup_Closed;
            }
        }

        private static void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var text = e.DataObject.GetData(typeof(string)) as string;
                if (!string.IsNullOrEmpty(text) && !IsAllowedTimeText(text))
                    e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }

        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text) && !IsAllowedTimeText(e.Text))
                e.Handled = true;
        }

        private static void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private static bool IsAllowedTimeText(string text)
        {
            return text.All(c => char.IsDigit(c) || c == ':');
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_textBox != null)
            {
                var text = _textBox.Text ?? string.Empty;
                var sanitizedText = new string(text.Where(c => char.IsDigit(c) || c == ':').ToArray());
                if (sanitizedText != text)
                    _textBox.Text = sanitizedText;

                if (_timeSelector == null)
                    return;

                _timeSelector.SelectedTimeChanged -= OnTimeSelector_SelectedTimeChanged;
                if (DateTime.TryParse(_textBox.Text, out var dateTime))
                {
                    if (SelectedTime.HasValue
                        &&
                        dateTime.ToString(GetDisplayTimeFormat(SelectedTimeFormat)) == SelectedTime.Value.ToString(GetDisplayTimeFormat(SelectedTimeFormat)))
                    {
                        _timeSelector.SelectedTimeChanged += OnTimeSelector_SelectedTimeChanged;
                        return;
                    }
                    SelectedTime = dateTime;
                }
                else
                    SelectedTime = null;
                _timeSelector.SelectedTimeChanged += OnTimeSelector_SelectedTimeChanged;
            }
        }

        private void OnPopup_Opened(object sender, EventArgs e)
        {
            _isSelectingTime = _buttonConfirm != null;
            if (_window != null)
                _window.PreviewMouseDown += OnWindowPreviewMouseDown;
            if (_timeSelector != null)
            {
                if (_isSelectingTime)
                    _timeSelector.SelectedTime = SelectedTime ?? DateTime.Now;
                _timeSelector.SetTime();
            }
        }

        private void OnPopup_Closed(object sender, EventArgs e)
        {
            if (_window != null)
                _window.PreviewMouseDown -= OnWindowPreviewMouseDown;
            if (_isSelectingTime && _timeSelector != null)
                _timeSelector.SelectedTime = SelectedTime;
            _isSelectingTime = false;
        }

        private void OnButtonConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (_timeSelector != null && _timeSelector.SelectedTime.HasValue)
            {
                var selectedTime = _timeSelector.SelectedTime.Value;
                if (SelectedTime.HasValue)
                    selectedTime = SelectedTime.Value.Date + selectedTime.TimeOfDay;
                if (!_timeSelector.ShowSeconds)
                {
                    selectedTime = new DateTime(
                        selectedTime.Year,
                        selectedTime.Month,
                        selectedTime.Day,
                        selectedTime.Hour,
                        selectedTime.Minute,
                        0,
                        selectedTime.Kind);
                }
                SetCurrentValue(SelectedTimeProperty, selectedTime);
                if (_textBox != null)
                {
                    _textBox.TextChanged -= TextBox_TextChanged;
                    _textBox.Text = selectedTime.ToString(GetDisplayTimeFormat(SelectedTimeFormat));
                    _textBox.TextChanged += TextBox_TextChanged;
                }
            }
            SetCurrentValue(IsDropDownOpenProperty, false);
        }

        private void OnButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            SetCurrentValue(IsDropDownOpenProperty, false);
        }
        private void OnWindowPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsMouseOver && (_popup == null || !_popup.IsMouseOver))
                SetCurrentValue(IsDropDownOpenProperty, false);
        }

        private void OnTimeSelector_SelectedTimeChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            if (_buttonConfirm != null || _isSelectingTime)
                return;
            if (_textBox != null && e.NewValue != null)
            {
                _textBox.Text = e.NewValue.Value.ToString(GetDisplayTimeFormat(SelectedTimeFormat));
                SelectedTime = e.NewValue;
            }
        }
    }
}
