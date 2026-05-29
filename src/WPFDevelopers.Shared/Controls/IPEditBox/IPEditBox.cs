using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = TextBox1TemplateName, Type = typeof(TextBox))]
    [TemplatePart(Name = TextBox2TemplateName, Type = typeof(TextBox))]
    [TemplatePart(Name = TextBox3TemplateName, Type = typeof(TextBox))]
    [TemplatePart(Name = TextBox4TemplateName, Type = typeof(TextBox))]
    public class IPEditBox : Control
    {
        private const string TextBox1TemplateName = "PART_TextBox1";
        private const string TextBox2TemplateName = "PART_TextBox2";
        private const string TextBox3TemplateName = "PART_TextBox3";
        private const string TextBox4TemplateName = "PART_TextBox4";

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(IPEditBox), new PropertyMetadata(string.Empty, OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as IPEditBox;
            if (e.NewValue is string text && !ctrl._isChangingText && ctrl.IsLoaded)
                ctrl.PasteTextIPTextBox(text);
        }

        private TextBox _textBox1, _textBox2, _textBox3, _textBox4;
        private bool _isChangingText = false;
        private readonly TextBox[] _textBoxes = new TextBox[4];

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _textBox1 = GetTemplateChild(TextBox1TemplateName) as TextBox;
            _textBox2 = GetTemplateChild(TextBox2TemplateName) as TextBox;
            _textBox3 = GetTemplateChild(TextBox3TemplateName) as TextBox;
            _textBox4 = GetTemplateChild(TextBox4TemplateName) as TextBox;

            _textBoxes[0] = _textBox1;
            _textBoxes[1] = _textBox2;
            _textBoxes[2] = _textBox3;
            _textBoxes[3] = _textBox4;

            UnsubscribeTextBoxEvents();

            if (_textBox1 != null)
            {
                _textBox1.TextChanged += TextBox1_TextChanged;
                _textBox1.PreviewKeyDown += TextBox_PreviewKeyDown;
                _textBox1.PreviewTextInput += TextBox_PreviewTextInput;
                _textBox1.Loaded += TextBox_Loaded;
            }
            if (_textBox2 != null)
            {
                _textBox2.TextChanged += TextBox2_TextChanged;
                _textBox2.PreviewKeyDown += TextBox_PreviewKeyDown;
                _textBox2.PreviewTextInput += TextBox_PreviewTextInput;
                _textBox2.Loaded += TextBox_Loaded;
            }
            if (_textBox3 != null)
            {
                _textBox3.TextChanged += TextBox3_TextChanged;
                _textBox3.PreviewKeyDown += TextBox_PreviewKeyDown;
                _textBox3.PreviewTextInput += TextBox_PreviewTextInput;
                _textBox3.Loaded += TextBox_Loaded;
            }
            if (_textBox4 != null)
            {
                _textBox4.TextChanged += TextBox4_TextChanged;
                _textBox4.PreviewKeyDown += TextBox_PreviewKeyDown;
                _textBox4.PreviewTextInput += TextBox_PreviewTextInput;
                _textBox4.Loaded += TextBox_Loaded;
            }

            if (!string.IsNullOrWhiteSpace(Text))
                PasteTextIPTextBox(Text);
        }

        private void UnsubscribeTextBoxEvents()
        {
            if (_textBox1 != null)
            {
                _textBox1.TextChanged -= TextBox1_TextChanged;
                _textBox1.PreviewKeyDown -= TextBox_PreviewKeyDown;
                _textBox1.PreviewTextInput -= TextBox_PreviewTextInput;
                _textBox1.Loaded -= TextBox_Loaded;
            }
            if (_textBox2 != null)
            {
                _textBox2.TextChanged -= TextBox2_TextChanged;
                _textBox2.PreviewKeyDown -= TextBox_PreviewKeyDown;
                _textBox2.PreviewTextInput -= TextBox_PreviewTextInput;
                _textBox2.Loaded -= TextBox_Loaded;
            }
            if (_textBox3 != null)
            {
                _textBox3.TextChanged -= TextBox3_TextChanged;
                _textBox3.PreviewKeyDown -= TextBox_PreviewKeyDown;
                _textBox3.PreviewTextInput -= TextBox_PreviewTextInput;
                _textBox3.Loaded -= TextBox_Loaded;
            }
            if (_textBox4 != null)
            {
                _textBox4.TextChanged -= TextBox4_TextChanged;
                _textBox4.PreviewKeyDown -= TextBox_PreviewKeyDown;
                _textBox4.PreviewTextInput -= TextBox_PreviewTextInput;
                _textBox4.Loaded -= TextBox_Loaded;
            }
        }

        private void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_textBox1.Text.Length >= 3) _textBox2?.Focus();
            UpdateText();
        }

        private void TextBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_textBox2.Text.Length >= 3) _textBox3?.Focus();
            UpdateText();
        }

        private void TextBox3_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_textBox3.Text.Length >= 3) _textBox4?.Focus();
            UpdateText();
        }

        private void TextBox4_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateText();
        }

        void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            CommandManager.AddPreviewExecutedHandler((sender as TextBox), TextBox_PreviewExecuted);
        }

        void TextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
            {
                ClipboardHandle();
                e.Handled = true;
            }
            else if (e.Command == ApplicationCommands.Copy)
            {
                var ip = $"{_textBox1.Text}.{_textBox2.Text}.{_textBox3.Text}.{_textBox4.Text}";
                Clipboard.SetText(ip);
                e.Handled = true;
            }
        }

        void ClipboardHandle()
        {
            var data = Clipboard.GetDataObject();
            if (data.GetDataPresent(DataFormats.Text))
            {
                var text = (string)data.GetData(DataFormats.UnicodeText);
                PasteTextIPTextBox(text);
            }
        }

        void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length != 1 || !char.IsDigit(e.Text[0]))
                return;

            var textBox = sender as TextBox;

            var currentText = textBox.Text;
            var insertPos = Math.Max(0, Math.Min(textBox.SelectionStart, currentText.Length));
            var newText = currentText.Insert(insertPos, e.Text);

            if (int.TryParse(newText, out int value) && value > 255)
            {
                if (textBox != _textBox4)
                {
                    var nextTextBox = GetNextTextBox(textBox);
                    nextTextBox.Focus();
                    nextTextBox.Text = e.Text;
                    nextTextBox.CaretIndex = 1;
                    e.Handled = true;
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;

            if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control) && e.Key == Key.V)
            {
                return;
            }

            if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control) && e.Key == Key.A)
            {
                textBox.SelectAll();
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Left)
            {
                if (textBox.CaretIndex == 0 && textBox != _textBox1)
                {
                    GetPreviousTextBox(textBox).Focus();
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Right)
            {
                if (textBox.CaretIndex == textBox.Text.Length && textBox != _textBox4)
                {
                    GetNextTextBox(textBox).Focus();
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Back)
            {
                if (textBox.CaretIndex == 0 && textBox != _textBox1)
                {
                    var previousTextBox = GetPreviousTextBox(textBox);
                    if (previousTextBox.Text.Length > 0)
                        previousTextBox.Text = previousTextBox.Text.Remove(previousTextBox.Text.Length - 1);
                    previousTextBox.Focus();
                    previousTextBox.CaretIndex = previousTextBox.Text.Length;
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Delete)
            {
                if (textBox.CaretIndex == textBox.Text.Length && textBox != _textBox4)
                {
                    var nextTextBox = GetNextTextBox(textBox);
                    if (nextTextBox.Text.Length > 0)
                        nextTextBox.Text = nextTextBox.Text.Remove(0, 1);
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                if (textBox != _textBox4)
                {
                    GetNextTextBox(textBox).Focus();
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Decimal || e.Key == Key.OemPeriod)
            {
                if (textBox != _textBox4)
                {
                    GetNextTextBox(textBox).Focus();
                    e.Handled = true;
                }
            }
        }

        TextBox GetPreviousTextBox(TextBox textBox)
        {
            if (textBox == _textBox2) return _textBox1;
            if (textBox == _textBox3) return _textBox2;
            if (textBox == _textBox4) return _textBox3;
            return _textBox1;
        }

        TextBox GetNextTextBox(TextBox textBox)
        {
            if (textBox == _textBox1) return _textBox2;
            if (textBox == _textBox2) return _textBox3;
            if (textBox == _textBox3) return _textBox4;
            return _textBox4;
        }

        void PasteTextIPTextBox(string text)
        {
            _isChangingText = true;
            if (_textBox1 != null) _textBox1.TextChanged -= TextBox1_TextChanged;
            if (_textBox2 != null) _textBox2.TextChanged -= TextBox2_TextChanged;
            if (_textBox3 != null) _textBox3.TextChanged -= TextBox3_TextChanged;
            if (_textBox4 != null) _textBox4.TextChanged -= TextBox4_TextChanged;
            if (string.IsNullOrWhiteSpace(text))
            {
                _textBox1?.Clear();
                _textBox2?.Clear();
                _textBox3?.Clear();
                _textBox4?.Clear();
            }
            else
            {
                var strs = text.Split('.');
                for (short i = 0; i < 4; i++)
                {
                    var str = i < strs.Length ? strs[i] : string.Empty;
                    var tb = _textBoxes[i];
                    if (tb == null) continue;
                    if (int.TryParse(str, out int value))
                    {
                        if (value < 0) value = 0;
                        if (value > 255) value = 255;
                        tb.Text = value.ToString();
                    }
                    else
                    {
                        tb.Text = string.Empty;
                    }
                }
            }

            if (_textBox1 != null) _textBox1.TextChanged += TextBox1_TextChanged;
            if (_textBox2 != null) _textBox2.TextChanged += TextBox2_TextChanged;
            if (_textBox3 != null) _textBox3.TextChanged += TextBox3_TextChanged;
            if (_textBox4 != null) _textBox4.TextChanged += TextBox4_TextChanged;
            _isChangingText = false;
            UpdateText();
        }


        void UpdateText()
        {
            if (_textBox1 == null || _textBox2 == null || _textBox3 == null || _textBox4 == null)
                return;
            var segments = new string[4]
            {
                _textBox1.Text.Trim(),
                _textBox2.Text.Trim(),
                _textBox3.Text.Trim(),
                _textBox4.Text.Trim()
            };
            var allEmpty = segments.All(string.IsNullOrEmpty);
            if (allEmpty)
            {
                if (!string.IsNullOrEmpty(Text))
                    SetValue(TextProperty, string.Empty);
                return;
            }
            var noEmpty = segments.Where(s => !string.IsNullOrWhiteSpace(s));
            if (noEmpty.Count() != 4) return;
            foreach (var seg in segments)
            {
                if (!int.TryParse(seg, out int v) || v < 0 || v > 255)
                    return;
            }
            var ip = string.Join(".", segments);
            if (ip != Text)
                SetValue(TextProperty, ip);
        }
    }
}
