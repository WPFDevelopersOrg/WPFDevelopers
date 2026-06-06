using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = ItemsTemplateName, Type = typeof(ItemsControl))]
    public class OtpBox : StatefulControlBase
    {
        private const string ItemsTemplateName = "PART_ItemsControl";

        private static readonly Type _typeofSelf = typeof(OtpBox);

        public int Length
        {
            get { return (int)GetValue(LengthProperty); }
            set { SetValue(LengthProperty, value); }
        }

        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.Register("Length", typeof(int), _typeofSelf,
                new FrameworkPropertyMetadata(4, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnLengthChanged));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), _typeofSelf,
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        private ItemsControl _itemsControl;
        private TextBox[] _textBoxes = new TextBox[0];
        private bool _isUpdatingValue = false;
        private bool _hasCompleted = false;
        private readonly ObservableCollection<int> _items = new ObservableCollection<int>();

        static OtpBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(_typeofSelf, new FrameworkPropertyMetadata(_typeofSelf));
            StateProperty.OverrideMetadata(_typeofSelf,
                new PropertyMetadata(ControlState.None, OnStateChanged));
        }

        private DispatcherTimer _resetTimer;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _itemsControl = GetTemplateChild(ItemsTemplateName) as ItemsControl;
            if (_itemsControl != null)
            {
                _itemsControl.Loaded += ItemsControl_Loaded;
                RebuildItems();
            }
        }

        private void ItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            BuildTextBoxes();
        }

        private void RebuildItems()
        {
            _items.Clear();
            for (int i = 0; i < Length; i++)
                _items.Add(i);
            if (_itemsControl != null)
                _itemsControl.ItemsSource = _items;
        }

        private void BuildTextBoxes()
        {
            if (_itemsControl == null) return;

            _textBoxes = new TextBox[Length];
            for (int i = 0; i < Length; i++)
            {
                var container = _itemsControl.ItemContainerGenerator.ContainerFromIndex(i) as ContentPresenter;
                if (container != null)
                {
                    var tb = ControlsHelper.FindVisualChild<TextBox>(container);
                    if (tb != null)
                    {
                        _textBoxes[i] = tb;
                        tb.PreviewTextInput += TextBox_PreviewTextInput;
                        tb.PreviewKeyDown += TextBox_PreviewKeyDown;
                        tb.TextChanged += TextBox_TextChanged;
                    }
                }
            }

            if (!string.IsNullOrEmpty(Value))
                DistributeValue();
        }

        private void DistributeValue()
        {
            _isUpdatingValue = true;
            for (int i = 0; i < _textBoxes.Length; i++)
            {
                if (_textBoxes[i] != null)
                {
                    _textBoxes[i].Text = i < Value.Length ? Value[i].ToString() : string.Empty;
                }
            }
            _isUpdatingValue = false;
        }

        private static void OnLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (OtpBox)d;
            ctrl.RebuildItems();
            ctrl.Dispatcher.BeginInvoke(new Action(ctrl.BuildTextBoxes));
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (OtpBox)d;
            if (ctrl._isUpdatingValue || !ctrl.IsLoaded || string.IsNullOrEmpty(e.NewValue as string))
                return;
            ctrl.DistributeValue();
        }

        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (OtpBox)d;
            switch (ctrl.State)
            {
                case ControlState.Error:
                case ControlState.Success:
                    if (ctrl._resetTimer == null)
                    {
                        ctrl._resetTimer = new DispatcherTimer
                        {
                            Interval = TimeSpan.FromSeconds(1.5)
                        };
                        ctrl._resetTimer.Tick += (sender, args) =>
                        {
                            ctrl._resetTimer.Stop();
                            var wasError = ctrl.State == ControlState.Error;
                            ctrl.State = ControlState.None;
                            if (wasError && ctrl._textBoxes != null && ctrl._textBoxes.Length > 0)
                            {
                                ctrl._isUpdatingValue = true;
                                ctrl._hasCompleted = false;
                                ctrl.Value = string.Empty;
                                ctrl.DistributeValue();
                                ctrl._isUpdatingValue = false;
                                ctrl.Dispatcher.BeginInvoke(new Action(() => ctrl._textBoxes[0]?.Focus()));
                            }
                        };
                    }
                    ctrl._resetTimer.Start();
                    break;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdatingValue) return;
            _resetTimer?.Stop();

            var textBox = sender as TextBox;
            int index = GetTextBoxIndex(textBox);
            if (index >= 0 && !string.IsNullOrEmpty(textBox.Text) && index < _textBoxes.Length - 1)
            {
                _textBoxes[index + 1].Focus();
            }

            UpdateValue();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length != 1 || !char.IsDigit(e.Text[0]))
            {
                e.Handled = true;
                return;
            }

            _resetTimer?.Stop();

            if (State != ControlState.None)
                State = ControlState.None;

            var textBox = sender as TextBox;
            int index = GetTextBoxIndex(textBox);
            if (index < 0) return;

            if (!string.IsNullOrEmpty(textBox.Text) && textBox.SelectionLength == 0)
            {
                e.Handled = true;
                if (index < _textBoxes.Length - 1)
                {
                    _textBoxes[index + 1].Focus();
                    _textBoxes[index + 1].Text = e.Text;
                    _textBoxes[index + 1].CaretIndex = 1;
                    UpdateValue();
                }
                return;
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            int index = GetTextBoxIndex(textBox);
            if (index < 0) return;

            _resetTimer?.Stop();

            if (e.Key == Key.Back)
            {
                if (State != ControlState.None)
                    State = ControlState.None;

                if (string.IsNullOrEmpty(textBox.Text) && index > 0)
                {
                    _textBoxes[index - 1].Focus();
                    if (_textBoxes[index - 1].Text.Length > 0)
                    {
                        _textBoxes[index - 1].Text = _textBoxes[index - 1].Text.Remove(_textBoxes[index - 1].Text.Length - 1);
                    }
                    _textBoxes[index - 1].CaretIndex = _textBoxes[index - 1].Text.Length;
                    UpdateValue();
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Left && textBox.CaretIndex == 0 && index > 0)
            {
                _textBoxes[index - 1].Focus();
                e.Handled = true;
            }
            else if (e.Key == Key.Right && textBox.CaretIndex == textBox.Text.Length && index < _textBoxes.Length - 1)
            {
                _textBoxes[index + 1].Focus();
                e.Handled = true;
            }
            else if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (Clipboard.ContainsText())
                {
                    var text = Clipboard.GetText();
                    PasteValue(text);
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                if (index < _textBoxes.Length - 1)
                {
                    _textBoxes[index + 1].Focus();
                    e.Handled = true;
                }
            }
        }

        private void PasteValue(string text)
        {
            _resetTimer?.Stop();

            if (State != ControlState.None)
                State = ControlState.None;

            _isUpdatingValue = true;
            for (int i = 0; i < Length; i++)
            {
                if (_textBoxes[i] != null)
                {
                    _textBoxes[i].Text = i < text.Length && char.IsDigit(text[i]) ? text[i].ToString() : string.Empty;
                }
            }
            _isUpdatingValue = false;
            UpdateValue();

            for (int i = 0; i < Length; i++)
            {
                if (_textBoxes[i] != null && string.IsNullOrEmpty(_textBoxes[i].Text))
                {
                    _textBoxes[i].Focus();
                    return;
                }
            }
            _textBoxes[Length - 1]?.Focus();
        }

        private int GetTextBoxIndex(TextBox textBox)
        {
            for (int i = 0; i < _textBoxes.Length; i++)
            {
                if (_textBoxes[i] == textBox) return i;
            }
            return -1;
        }

        private void UpdateValue()
        {
            if (_textBoxes.Length == 0) return;

            var segments = new string[Length];
            int filledCount = 0;
            for (int i = 0; i < Length; i++)
            {
                var tb = _textBoxes[i];
                if (tb != null && !string.IsNullOrEmpty(tb.Text) && char.IsDigit(tb.Text[0]))
                {
                    segments[i] = tb.Text.Substring(0, 1);
                    filledCount++;
                }
            }

            if (filledCount == Length)
            {
                var newValue = string.Join("", segments);
                if (newValue != Value)
                {
                    SetValue(ValueProperty, newValue);
                    _hasCompleted = false;
                }
                if (!_hasCompleted)
                {
                    _hasCompleted = true;
                    RaiseCompleted(Value);
                }
            }
            else
            {
                _hasCompleted = false;
                if (!string.IsNullOrEmpty(Value))
                    SetValue(ValueProperty, string.Empty);
            }
        }
    }
}
