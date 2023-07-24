using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows;
using System.Xml.Linq;
using WPFDevelopers.Utilities;
using System.Windows.Controls;
using WPFDevelopers.Datas;

namespace WPFDevelopers.Controls
{
    [DefaultEvent("ValueChanged"), DefaultProperty("Value")]
    [TemplatePart(Name = TextBoxTemplateName, Type = typeof(TextBox))]
    [TemplatePart(Name = NumericUpTemplateName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = NumericDownTemplateName, Type = typeof(RepeatButton))]
    public class NumericBox : Control
    {
        private static readonly Type _typeofSelf = typeof(NumericBox);

        private const double DefaultInterval = 1d;
        private const int DefaultDelay = 500;

        private const string TextBoxTemplateName = "PART_TextBox";
        private const string NumericUpTemplateName = "PART_NumericUp";
        private const string NumericDownTemplateName = "PART_NumericDown";

        private static RoutedCommand _increaseCommand = null;
        private static RoutedCommand _decreaseCommand = null;

        private TextBox _valueTextBox;
        private RepeatButton _repeatUp;
        private RepeatButton _repeatDown;

        private double _internalLargeChange = DefaultInterval;
        private double _intervalValueSinceReset = 0;
        private double? _lastOldValue = null;

        private bool _isManual;
        private bool _isBusy;

        static NumericBox()
        {
            InitializeCommands();

            DefaultStyleKeyProperty.OverrideMetadata(_typeofSelf, new FrameworkPropertyMetadata(_typeofSelf));
        }

        #region Command

        private static void InitializeCommands()
        {
            _increaseCommand = new RoutedCommand("Increase", _typeofSelf);
            _decreaseCommand = new RoutedCommand("Decrease", _typeofSelf);

            CommandManager.RegisterClassCommandBinding(_typeofSelf, new CommandBinding(_increaseCommand, OnIncreaseCommand, OnCanIncreaseCommand));
            CommandManager.RegisterClassCommandBinding(_typeofSelf, new CommandBinding(_decreaseCommand, OnDecreaseCommand, OnCanDecreaseCommand));
        }

        public static RoutedCommand IncreaseCommand
        {
            get { return _increaseCommand; }
        }

        public static RoutedCommand DecreaseCommand
        {
            get { return _decreaseCommand; }
        }

        private static void OnIncreaseCommand(object sender, RoutedEventArgs e)
        {
            var numericBox = sender as NumericBox;
            numericBox.ContinueChangeValue(true);
        }

        private static void OnCanIncreaseCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            var numericBox = sender as NumericBox;
            e.CanExecute = (!numericBox.IsReadOnly && numericBox.IsEnabled && DoubleUtil.LessThan(numericBox.Value, numericBox.Maximum));
        }

        private static void OnDecreaseCommand(object sender, RoutedEventArgs e)
        {
            var numericBox = sender as NumericBox;
            numericBox.ContinueChangeValue(false);
        }

        private static void OnCanDecreaseCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            var numericBox = sender as NumericBox;
            e.CanExecute = (!numericBox.IsReadOnly && numericBox.IsEnabled && DoubleUtil.GreaterThan(numericBox.Value, numericBox.Minimum));
        }

        #endregion

        #region RouteEvent

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), _typeofSelf);
        public event RoutedPropertyChangedEventHandler<double> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        #endregion

        #region Properties

        public static readonly DependencyProperty DisabledValueChangedWhileBusyProperty = DependencyProperty.Register("DisabledValueChangedWhileBusy", typeof(bool), _typeofSelf,
           new PropertyMetadata(false));
        [Category("Common")]
        [DefaultValue(true)]
        public bool DisabledValueChangedWhileBusy
        {
            get { return (bool)GetValue(DisabledValueChangedWhileBusyProperty); }
            set { SetValue(DisabledValueChangedWhileBusyProperty, value); }
        }

        public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register("Interval", typeof(double), _typeofSelf,
           new FrameworkPropertyMetadata(DefaultInterval, IntervalChanged, CoerceInterval));
        [Category("Behavior")]
        [DefaultValue(DefaultInterval)]
        public double Interval
        {
            get { return (double)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        private static object CoerceInterval(DependencyObject d, object value)
        {
            var interval = (double)value;
            return DoubleUtil.IsNaN(interval) ? 0 : Math.Max(interval, 0);
        }

        private static void IntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericBox = (NumericBox)d;
            numericBox.ResetInternal();
        }

        public static readonly DependencyProperty SpeedupProperty = DependencyProperty.Register("Speedup", typeof(bool), _typeofSelf,
           new PropertyMetadata(true));
        [Category("Common")]
        [DefaultValue(true)]
        public bool Speedup
        {
            get { return (bool)GetValue(SpeedupProperty); }
            set { SetValue(SpeedupProperty, value); }
        }

        public static readonly DependencyProperty DelayProperty = DependencyProperty.Register("Delay", typeof(int), _typeofSelf,
            new PropertyMetadata(DefaultDelay, null, CoerceDelay));
        [DefaultValue(DefaultDelay)]
        [Category("Behavior")]
        public int Delay
        {
            get { return (int)GetValue(DelayProperty); }
            set { SetValue(DelayProperty, value); }
        }

        private static object CoerceDelay(DependencyObject d, object value)
        {
            var delay = (int)value;
            return Math.Max(delay, 0);
        }

        public static readonly DependencyProperty UpDownButtonsWidthProperty = DependencyProperty.Register("UpDownButtonsWidth", typeof(double), _typeofSelf,
           new PropertyMetadata(20d));
        [Category("Appearance")]
        [DefaultValue(20d)]
        public double UpDownButtonsWidth
        {
            get { return (double)GetValue(UpDownButtonsWidthProperty); }
            set { SetValue(UpDownButtonsWidthProperty, value); }
        }

        public static readonly DependencyProperty TextAlignmentProperty = TextBox.TextAlignmentProperty.AddOwner(_typeofSelf);
        [Category("Common")]
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        public static readonly DependencyProperty IsReadOnlyProperty = TextBoxBase.IsReadOnlyProperty.AddOwner(_typeofSelf,
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, IsReadOnlyPropertyChangedCallback));
        [Category("Appearance")]
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        private static void IsReadOnlyPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue == e.NewValue || e.NewValue == null)
                return;

            ((NumericBox)d).ToggleReadOnlyMode((bool)e.NewValue);
        }

        public static readonly DependencyProperty PrecisionProperty = DependencyProperty.Register("Precision", typeof(int?), _typeofSelf,
            new PropertyMetadata(null, OnPrecisionChanged, CoercePrecision));
        [Category("Common")]
        public int? Precision
        {
            get { return (int?)GetValue(PrecisionProperty); }
            set { SetValue(PrecisionProperty, value); }
        }

        private static object CoercePrecision(DependencyObject d, object value)
        {
            var precision = (int?)value;
            return (precision.HasValue && precision.Value < 0) ? 0 : precision;
        }

        private static void OnPrecisionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericBox = (NumericBox)d;
            var newPrecision = (int?)e.NewValue;

            var roundValue = numericBox.CorrectPrecision(newPrecision, numericBox.Value);

            if (DoubleUtil.AreClose(numericBox.Value, roundValue))
                numericBox.InternalSetText(roundValue);
            else
                numericBox.Value = roundValue;
        }

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), _typeofSelf,
            new PropertyMetadata(double.MinValue, OnMinimumChanged));
        [Category("Common")]
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericBox = (NumericBox)d;

            numericBox.CoerceValue(MaximumProperty, numericBox.Maximum);
            numericBox.CoerceValue(ValueProperty, numericBox.Value);
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), _typeofSelf,
            new PropertyMetadata(double.MaxValue, OnMaximumChanged, CoerceMaximum));
        [Category("Common")]
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        private static object CoerceMaximum(DependencyObject d, object value)
        {
            var minimum = ((NumericBox)d).Minimum;
            var val = (double)value;
            return DoubleUtil.LessThan(val, minimum) ? minimum : val;
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericBox = (NumericBox)d;
            numericBox.CoerceValue(ValueProperty, numericBox.Value);
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), _typeofSelf,
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged, CoerceValue));
        [Category("Common")]
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static object CoerceValue(DependencyObject d, object value)
        {
            var numericBox = (NumericBox)d;
            var val = (double)value;

            if (DoubleUtil.LessThan(val, numericBox.Minimum))
                return numericBox.Minimum;

            if (DoubleUtil.GreaterThan(val, numericBox.Maximum))
                return numericBox.Maximum;

            return numericBox.CorrectPrecision(numericBox.Precision, val);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericBox = (NumericBox)d;
            numericBox.OnValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        #endregion

        #region Virtual

        protected virtual void OnValueChanged(double oldValue, double newValue)
        {
            InternalSetText(newValue);
            InvalidateRequerySuggested(newValue);

            if ((!_isBusy || !DisabledValueChangedWhileBusy) && !DoubleUtil.AreClose(oldValue, newValue))
            {
                RaiseEvent(new TextBoxValueChangedEventArgs<double>(oldValue, newValue, _isManual, _isBusy, ValueChangedEvent));
            }

            _isManual = false;
        }

        #endregion

        #region Override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            UnsubscribeEvents();

            _valueTextBox = GetTemplateChild(TextBoxTemplateName) as TextBox;
            _repeatUp = GetTemplateChild(NumericUpTemplateName) as RepeatButton;
            _repeatDown = GetTemplateChild(NumericDownTemplateName) as RepeatButton;

            if (_valueTextBox == null || _repeatUp == null || _repeatDown == null)
            {
                throw new NullReferenceException(string.Format("You have missed to specify {0}, {1} or {2} in your template", NumericUpTemplateName, NumericDownTemplateName, TextBoxTemplateName));
            }

            _repeatUp.PreviewMouseUp += OnRepeatButtonPreviewMouseUp;
            _repeatDown.PreviewMouseUp += OnRepeatButtonPreviewMouseUp;

            ToggleReadOnlyMode(IsReadOnly);
            OnValueChanged(Value, Value);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            if (Focusable && !IsReadOnly)
            {
                Focused();
                SelectAll();
            }
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);

            if (e.Delta != 0 && (IsFocused || _valueTextBox.IsFocused))
                ContinueChangeValue(e.Delta >= 0, false);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            switch (e.Key)
            {
                case Key.Enter:
                    DealInputText(_valueTextBox.Text);
                    SelectAll();
                    e.Handled = true;
                    break;

                case Key.Up:
                    ContinueChangeValue(true);
                    e.Handled = true;
                    break;

                case Key.Down:
                    ContinueChangeValue(false);
                    e.Handled = true;
                    break;
            }
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);

            switch (e.Key)
            {
                case Key.Down:
                case Key.Up:
                    ResetInternal();
                    break;
            }
        }

        #endregion

        #region Event 

        private void OnRepeatButtonPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ResetInternal();
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Focusable && !IsReadOnly && !_valueTextBox.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                Focused();
                SelectAll();
            }
        }

        private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox)sender;
            DealInputText(tb.Text);
        }

        private void OnValueTextBoxPaste(object sender, DataObjectPastingEventArgs e)
        {
            var textBox = (TextBox)sender;
            var textPresent = textBox.Text;

            if (!e.SourceDataObject.GetDataPresent(DataFormats.Text, true))
                return;

            var text = e.SourceDataObject.GetData(DataFormats.Text) as string;
            var newText = string.Concat(textPresent.Substring(0, textBox.SelectionStart), text, textPresent.Substring(textBox.SelectionStart + textBox.SelectionLength));

            double number;
            if (!double.TryParse(newText, out number))
                e.CancelCommand();
        }

        #endregion

        #region Private

        private void UnsubscribeEvents()
        {
            if (_valueTextBox != null)
            {
                _valueTextBox.LostFocus -= OnTextBoxLostFocus;
                _valueTextBox.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
                DataObject.RemovePastingHandler(_valueTextBox, OnValueTextBoxPaste);
            }

            if (_repeatUp != null)
                _repeatUp.PreviewMouseUp -= OnRepeatButtonPreviewMouseUp;

            if (_repeatDown != null)
                _repeatDown.PreviewMouseUp -= OnRepeatButtonPreviewMouseUp;
        }

        private void Focused()
        {
            _valueTextBox?.Focus();
        }

        private void SelectAll()
        {
            _valueTextBox?.SelectAll();
        }

        private void DealInputText(string inputText)
        {
            double convertedValue;
            if (double.TryParse(inputText, out convertedValue))
            {
                if (DoubleUtil.AreClose(Value, convertedValue))
                {
                    InternalSetText(Value);
                    return;
                }

                _isManual = true;

                if (convertedValue > Maximum)
                {
                    if (DoubleUtil.AreClose(Value, Maximum))
                        OnValueChanged(Value, Value);
                    else
                        Value = Maximum;
                }
                else if (convertedValue < Minimum)
                {
                    if (DoubleUtil.AreClose(Value, Minimum))
                        OnValueChanged(Value, Value);
                    else
                        Value = Minimum;
                }
                else
                    Value = convertedValue;
            }
            else
                InternalSetText(Value);
        }

        private void MoveFocus()
        {
            var request = new TraversalRequest((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next);
            var elementWithFocus = Keyboard.FocusedElement as UIElement;

            elementWithFocus?.MoveFocus(request);
        }

        private void ToggleReadOnlyMode(bool isReadOnly)
        {
            if (_valueTextBox == null)
                return;

            if (isReadOnly)
            {
                _valueTextBox.LostFocus -= OnTextBoxLostFocus;
                _valueTextBox.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
                DataObject.RemovePastingHandler(_valueTextBox, OnValueTextBoxPaste);
            }
            else
            {
                _valueTextBox.LostFocus += OnTextBoxLostFocus;
                _valueTextBox.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
                DataObject.AddPastingHandler(_valueTextBox, OnValueTextBoxPaste);
            }
        }

        private void InternalSetText(double newValue)
        {
            var text = newValue.ToString(GetPrecisionFormat());
            if (_valueTextBox != null && !Equals(text, _valueTextBox.Text))
                _valueTextBox.Text = text;
        }

        private string GetPrecisionFormat()
        {
            return Precision.HasValue == false
                ? "g"
                : (Precision.Value == 0
                    ? "#0"
                    : ("#0.0" + string.Join("", Enumerable.Repeat("#", Precision.Value - 1))));
        }

        private void CoerceValue(DependencyProperty dp, object localValue)
        {
            SetCurrentValue(dp, localValue);
            CoerceValue(dp);
        }

        private double CorrectPrecision(int? precision, double originValue)
        {
            return Math.Round(originValue, precision ?? 0, MidpointRounding.AwayFromZero);
        }

        private void ContinueChangeValue(bool isIncrease, bool isContinue = true)
        {
            if (IsReadOnly || !IsEnabled)
                return;

            if (isIncrease && DoubleUtil.LessThan(Value, Maximum))
            {
                if (!_isBusy && isContinue)
                {
                    _isBusy = true;

                    if (DisabledValueChangedWhileBusy)
                        _lastOldValue = Value;
                }

                _isManual = true;
                Value = (double)CoerceValue(this, Value + CalculateInterval(isContinue));
            }

            if (!isIncrease && DoubleUtil.GreaterThan(Value, Minimum))
            {
                if (!_isBusy && isContinue)
                {
                    _isBusy = true;

                    if (DisabledValueChangedWhileBusy)
                        _lastOldValue = Value;
                }

                _isManual = true;
                Value = (double)CoerceValue(this, Value - CalculateInterval(isContinue));
            }
        }

        private double CalculateInterval(bool isContinue = true)
        {
            if (!Speedup || !isContinue)
                return Interval;

            if (DoubleUtil.GreaterThan((_intervalValueSinceReset += _internalLargeChange), _internalLargeChange * 100))
                _internalLargeChange *= 10;

            return _internalLargeChange;
        }

        private void ResetInternal()
        {
            _internalLargeChange = Interval;
            _intervalValueSinceReset = 0;

            _isBusy = false;

            if (_lastOldValue.HasValue)
            {
                _isManual = true;
                OnValueChanged(_lastOldValue.Value, Value);
                _lastOldValue = null;
            }
        }

        private void InvalidateRequerySuggested(double value)
        {
            if (_repeatUp == null || _repeatDown == null)
                return;

            if (DoubleUtil.AreClose(value, Maximum) && _repeatUp.IsEnabled
               || DoubleUtil.AreClose(value, Minimum) && _repeatDown.IsEnabled)
                CommandManager.InvalidateRequerySuggested();
            else
            {
                if (!_repeatUp.IsEnabled || !_repeatDown.IsEnabled)
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        #endregion
    }
}
