using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

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

        public static readonly DependencyProperty SelectedTimeFormatProperty =
            DependencyProperty.Register("SelectedTimeFormat", typeof(string), typeof(TimePicker),
                new PropertyMetadata("HH:mm:ss"));

        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(TimePicker),
                new UIPropertyMetadata(SystemParameters.PrimaryScreenHeight / 3.0, OnMaxDropDownHeightChanged));

        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register("SelectedTime", typeof(DateTime?), typeof(TimePicker),
                new PropertyMetadata(null, OnSelectedTimeChanged));

        public static readonly DependencyProperty IsCurrentTimeProperty =
            DependencyProperty.Register("IsCurrentTime", typeof(bool), typeof(TimePicker), new PropertyMetadata(false));

        private DateTime _date;
        private Popup _popup;
        private TextBox _textBox;
        private TimeSelector _timeSelector;

        static TimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimePicker),
                new FrameworkPropertyMetadata(typeof(TimePicker)));
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
            if (ctrl != null && e.NewValue != null)
            {
                var dateTime = (DateTime) e.NewValue;
                if (ctrl._timeSelector != null && dateTime > DateTime.MinValue)
                    ctrl._timeSelector.SelectedTime = dateTime;
                else
                    ctrl._date = dateTime;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
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
                _popup.Opened -= Popup_Opened;
                _popup.Opened += Popup_Opened;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_textBox != null)
            {
                _timeSelector.SelectedTimeChanged -= TimeSelector_SelectedTimeChanged;
                if (DateTime.TryParse(_textBox.Text, out var dateTime))
                {
                    if (SelectedTime.HasValue && dateTime.ToString(SelectedTimeFormat) == SelectedTime.Value.ToString(SelectedTimeFormat)) return;
                    SelectedTime = dateTime;
                }
                else
                    SelectedTime = null;
                _timeSelector.SelectedTimeChanged += TimeSelector_SelectedTimeChanged;
            }
        }
        private void Popup_Opened(object sender, EventArgs e)
        {
            if (_timeSelector != null)
                _timeSelector.SetTime();
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