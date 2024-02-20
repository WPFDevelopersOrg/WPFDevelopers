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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _textBox1 = GetTemplateChild(TextBox1TemplateName) as TextBox;
            if (_textBox1 != null)
            {
                _textBox1.TextChanged -= TextBox1_TextChanged;
                _textBox1.TextChanged += TextBox1_TextChanged;
                _textBox1.PreviewKeyDown -= TextBox_PreviewKeyDown;
                _textBox1.PreviewKeyDown += TextBox_PreviewKeyDown;
                _textBox1.Loaded -= TextBox_Loaded;
                _textBox1.Loaded += TextBox_Loaded;
            }
            _textBox2 = GetTemplateChild(TextBox2TemplateName) as TextBox;
            if (_textBox2 != null)
            {
                _textBox2.TextChanged -= TextBox2_TextChanged;
                _textBox2.TextChanged += TextBox2_TextChanged;
                _textBox2.PreviewKeyDown -= TextBox_PreviewKeyDown;
                _textBox2.PreviewKeyDown += TextBox_PreviewKeyDown;
                _textBox2.Loaded -= TextBox_Loaded; ;
                _textBox2.Loaded += TextBox_Loaded;
            }
            _textBox3 = GetTemplateChild(TextBox3TemplateName) as TextBox;
            if (_textBox3 != null)
            {
                _textBox3.TextChanged -= TextBox3_TextChanged;
                _textBox3.TextChanged += TextBox3_TextChanged;
                _textBox3.PreviewKeyDown -= TextBox_PreviewKeyDown;
                _textBox3.PreviewKeyDown += TextBox_PreviewKeyDown;
                _textBox3.Loaded -= TextBox_Loaded;
                _textBox3.Loaded += TextBox_Loaded;
            }
            _textBox4 = GetTemplateChild(TextBox4TemplateName) as TextBox;
            _textBox4.TextChanged -= TextBox4_TextChanged;
            _textBox4.TextChanged += TextBox4_TextChanged;
            _textBox4.PreviewKeyDown -= TextBox_PreviewKeyDown;
            _textBox4.PreviewKeyDown += TextBox_PreviewKeyDown;
            _textBox4.Loaded -= TextBox_Loaded;
            _textBox4.Loaded += TextBox_Loaded;
            if (!string.IsNullOrWhiteSpace(Text))
                PasteTextIPTextBox(Text);
        }

        private void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_textBox1.Text.ToString().Length >= 3) _textBox2.Focus();
            UpdateText();
        }
        private void TextBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_textBox2.Text.ToString().Length >= 3) _textBox3.Focus();
            UpdateText();
        }

        private void TextBox3_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_textBox3.Text.ToString().Length >= 3) _textBox4.Focus();
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
                UpdateText();
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

        void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control) && e.Key == Key.V)
            {
                ClipboardHandle();
                _isChangingText = false;
                e.Handled = true;
            }
            else if(e.Key == Key.Left)
            {
                if (textBox.CaretIndex == 0 && !IsFirstTextBox(textBox))
                {
                    GetPreviousTextBox(textBox).Focus();
                }
            }
            else if (e.Key == Key.Right)
            {
                if (textBox.CaretIndex == textBox.Text.Length && !IsLastTextBox(textBox))
                {
                    GetNextTextBox(textBox).Focus();
                }
            }
            else if (e.Key == Key.Back)
            {
                if (textBox.CaretIndex == 0 && !IsFirstTextBox(textBox))
                {
                    var previousTextBox = GetPreviousTextBox(textBox);
                    if (previousTextBox.Text.Length > 0)
                        previousTextBox.Text = previousTextBox.Text.Remove(previousTextBox.Text.Length - 1);
                    previousTextBox.Focus();
                    previousTextBox.CaretIndex = previousTextBox.Text.Length;
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                _isChangingText = true;
            }
            else
                _isChangingText = false;
        }

        bool IsFirstTextBox(TextBox textBox)
        {
            return textBox == _textBox1;
        }

        bool IsLastTextBox(TextBox textBox)
        {
            return textBox == _textBox4;
        }
        TextBox GetPreviousTextBox(TextBox textBox)
        {
            if (textBox == _textBox2)
                return _textBox1;
            else if (textBox == _textBox3)
                return _textBox2;
            else if (textBox == _textBox4)
                return _textBox3;
            return textBox;
        }

        TextBox GetNextTextBox(TextBox textBox)
        {
            if (textBox == _textBox1)
                return _textBox2;
            else if (textBox == _textBox2)
                return _textBox3;
            else if (textBox == _textBox3)
                return _textBox4;
            return textBox;
        }

        void PasteTextIPTextBox(string text)
        {
            _textBox1.TextChanged -= TextBox1_TextChanged;
            _textBox2.TextChanged -= TextBox2_TextChanged;
            _textBox3.TextChanged -= TextBox3_TextChanged;
            _textBox4.TextChanged -= TextBox4_TextChanged;
            if (string.IsNullOrWhiteSpace(text))
            {
                _textBox1.Text = string.Empty;
                _textBox2.Text = string.Empty;
                _textBox3.Text = string.Empty;
                _textBox4.Text = string.Empty;
            }
            else
            {
                var strs = text.Split('.');
                var _textboxBoxes = new TextBox[] { _textBox1, _textBox2, _textBox3, _textBox4 };
                for (short i = 0; i < _textboxBoxes.Length; i++)
                {
                    var str = i < strs.Length ? strs[i] : string.Empty;
                    _textboxBoxes[i].Text = str;
                }
            }
            _textBox1.TextChanged += TextBox1_TextChanged;
            _textBox2.TextChanged += TextBox2_TextChanged;
            _textBox3.TextChanged += TextBox3_TextChanged;
            _textBox4.TextChanged += TextBox4_TextChanged;
        }
        void UpdateText()
        {
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
                SetValue(TextProperty, string.Empty);
                return;
            }
            var noEmpty = segments.Where(s => !string.IsNullOrWhiteSpace(s));
            if (noEmpty.Count() != 4) return;
            var ip = string.Join(".", noEmpty);
            if (ip != Text)
                SetValue(TextProperty, ip);
        }
    }
}
