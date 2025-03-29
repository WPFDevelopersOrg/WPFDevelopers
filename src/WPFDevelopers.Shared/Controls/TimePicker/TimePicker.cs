using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = TimeSelectorTemplateName, Type = typeof(ListBox))]
    [TemplatePart(Name = EditableTextBoxTemplateName, Type = typeof(TextBox))]
    [TemplatePart(Name = PopupTemplateName, Type = typeof(Popup))]
    public class TimePicker : Control
    {
        private const string TimeSelectorTemplateName = "PART_TimeSelector";
        private const string EditableTextBoxTemplateName = "PART_EditableTextBox";
        private const string PopupTemplateName = "PART_Popup";

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(TimePicker),
                new PropertyMetadata(false));

        public static readonly DependencyProperty SelectedTimeFormatProperty =
            DependencyProperty.Register("SelectedTimeFormat", typeof(string), typeof(TimePicker),
                new PropertyMetadata("HH:mm:ss"));

        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(TimePicker),
                new UIPropertyMetadata(SystemParameters.PrimaryScreenHeight / 3.0, OnMaxDropDownHeightChanged));

        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register("SelectedTime", typeof(DateTime?), typeof(TimePicker),
                new FrameworkPropertyMetadata(null, 
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                    OnSelectedTimeChanged));

        public static readonly DependencyProperty IsCurrentTimeProperty =
            DependencyProperty.Register("IsCurrentTime", typeof(bool), typeof(TimePicker), new PropertyMetadata(false));

        private HwndSource _hwndSource;
        private Window _window;
        private DateTime _date;
        private Popup _popup;
        private TextBox _textBox;
        private TimeSelector _timeSelector;

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
            get => (string) GetValue(SelectedTimeFormatProperty);
            set => SetValue(SelectedTimeFormatProperty, value);
        }

        public DateTime? SelectedTime
        {
            get => (DateTime?) GetValue(SelectedTimeProperty);
            set => SetValue(SelectedTimeProperty, value);
        }

        public bool IsCurrentTime
        {
            get => (bool) GetValue(IsCurrentTimeProperty);
            set => SetValue(IsCurrentTimeProperty, value);
        }

        private static void OnMaxDropDownHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as TimePicker;
            if (ctrl != null)
                ctrl.OnMaxDropDownHeightChanged((double) e.OldValue, (double) e.NewValue);
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
                    if(ctrl._timeSelector != null)
                        ctrl._timeSelector.SelectedTime = null;
                    else
                        ctrl._date = dateTime.Value;
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _window = Window.GetWindow(this);
            if (_window != null)
            {
                if (_window.IsInitialized)
                    WindowSourceInitialized(_window, EventArgs.Empty);
                else
                {
                    _window.SourceInitialized -= WindowSourceInitialized;
                    _window.SourceInitialized += WindowSourceInitialized;
                }
            }
            _textBox = GetTemplateChild(EditableTextBoxTemplateName) as TextBox;
            if (_textBox != null)
                _textBox.TextChanged += TextBox_TextChanged;
            _timeSelector = GetTemplateChild(TimeSelectorTemplateName) as TimeSelector;
            if (_timeSelector != null)
            {
                _timeSelector.SelectedTimeChanged -= TimeSelector_SelectedTimeChanged;
                _timeSelector.SelectedTimeChanged += TimeSelector_SelectedTimeChanged;
                if (!SelectedTime.HasValue && IsCurrentTime)
                {
                    SelectedTime = DateTime.Now;
                }
                else
                {
                    SelectedTime = null;
                    SelectedTime = _date;
                }
            }

            _popup = GetTemplateChild(PopupTemplateName) as Popup;
            if (_popup != null)
            {
                _popup.Closed += OnPopup_Closed;
                _popup.Closed -= OnPopup_Closed;
                _popup.Opened -= OnPopup_Opened;
                _popup.Opened += OnPopup_Opened;
            }
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
                if (_hwndSource != null)
                {
                    _hwndSource.AddHook(WndProc);
                }
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_NCLBUTTONDOWN = 0x00A1;
            if (msg == WM_NCLBUTTONDOWN)
            {
                IsDropDownOpen = false;
            }
            return IntPtr.Zero;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_textBox != null)
            {
                _timeSelector.SelectedTimeChanged -= TimeSelector_SelectedTimeChanged;
                if (DateTime.TryParse(_textBox.Text, out var dateTime))
                {
                    if (SelectedTime.HasValue
                        && 
                        dateTime.ToString(SelectedTimeFormat) == SelectedTime.Value.ToString(SelectedTimeFormat))
                    {
                        _timeSelector.SelectedTimeChanged += TimeSelector_SelectedTimeChanged;
                        return;
                    } 
                    SelectedTime = dateTime;
                }
                else
                    SelectedTime = null;
                _timeSelector.SelectedTimeChanged += TimeSelector_SelectedTimeChanged;
            }
        }

        private void OnPopup_Opened(object sender, EventArgs e)
        {
            _window.PreviewMouseDown += OnWindowPreviewMouseDown;
            if (_timeSelector != null)
                _timeSelector.SetTime();
        }
        private void OnWindowPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsMouseOver)
                IsDropDownOpen = false;
        }

        private void TimeSelector_SelectedTimeChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            if (_textBox != null && e.NewValue != null)
            {
                _textBox.Text = e.NewValue.Value.ToString(SelectedTimeFormat);
                SelectedTime = e.NewValue;
            }
        }
    }
}