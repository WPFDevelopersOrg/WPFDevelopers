using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFDevelopers.Helpers
{
    public static class TextBoxHelper
    {
        public static readonly DependencyProperty SelectAllOnClickProperty =
            DependencyProperty.RegisterAttached("SelectAllOnClick", typeof(bool), typeof(TextBoxHelper),
                new PropertyMetadata(false, OnSelectAllOnClickChanged));


        public static readonly DependencyProperty AllowOnlyNumericInputProperty =
            DependencyProperty.RegisterAttached("AllowOnlyNumericInput", typeof(bool), typeof(TextBoxHelper),
                new PropertyMetadata(false, OnAllowOnlyNumericInputChanged));

        public static readonly DependencyProperty IsEnterUpdateEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnterUpdateEnabled", typeof(bool), typeof(TextBoxHelper),
                new PropertyMetadata(false, OnIsEnterUpdateEnabledChanged));

        public static readonly DependencyProperty MinValueProperty =
       DependencyProperty.RegisterAttached("MinValue", typeof(int), typeof(TextBoxHelper), 
           new PropertyMetadata(int.MinValue, OnMinMaxValueChanged));

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.RegisterAttached("MaxValue", typeof(int), typeof(TextBoxHelper), 
                new PropertyMetadata(int.MaxValue, OnMinMaxValueChanged));

        private static void OnSelectAllOnClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if ((bool) e.NewValue)
                    textBox.PreviewMouseLeftButtonDown += TextBox_PreviewMouseLeftButtonDown;
                else
                    textBox.PreviewMouseLeftButtonDown -= TextBox_PreviewMouseLeftButtonDown;
            }
        }

        private static void OnAllowOnlyNumericInputChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if ((bool) e.NewValue)
                {
                    DataObject.AddPastingHandler(textBox, TextBox_Pasting);
                    textBox.PreviewTextInput += TextBox_PreviewTextInput;
                    InputMethod.SetIsInputMethodEnabled(textBox, false);
                }
                else
                {
                    DataObject.RemovePastingHandler(textBox, TextBox_Pasting);
                    textBox.PreviewTextInput -= TextBox_PreviewTextInput;
                    InputMethod.SetIsInputMethodEnabled(textBox, true);
                }
            }
        }

        private static void OnIsEnterUpdateEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if ((bool) e.NewValue)
                    textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
                else
                    textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
            }
        }

        public static bool GetSelectAllOnClick(TextBox textBox)
        {
            return (bool) textBox.GetValue(SelectAllOnClickProperty);
        }

        public static void SetSelectAllOnClick(TextBox textBox, bool value)
        {
            textBox.SetValue(SelectAllOnClickProperty, value);
        }

        public static bool GetAllowOnlyNumericInput(DependencyObject obj)
        {
            return (bool) obj.GetValue(AllowOnlyNumericInputProperty);
        }

        public static void SetAllowOnlyNumericInput(DependencyObject obj, bool value)
        {
            obj.SetValue(AllowOnlyNumericInputProperty, value);
        }

        public static bool GetIsEnterUpdateEnabled(DependencyObject obj)
        {
            return (bool) obj.GetValue(IsEnterUpdateEnabledProperty);
        }

        public static void SetIsEnterUpdateEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnterUpdateEnabledProperty, value);
        }

        public static int GetMinValue(DependencyObject obj)
        {
            return (int)obj.GetValue(MinValueProperty);
        }

        public static void SetMinValue(DependencyObject obj, int value)
        {
            obj.SetValue(MinValueProperty, value);
        }

        public static int GetMaxValue(DependencyObject obj)
        {
            return (int)obj.GetValue(MaxValueProperty);
        }

        public static void SetMaxValue(DependencyObject obj, int value)
        {
            obj.SetValue(MaxValueProperty, value);
        }

        private static void OnMinMaxValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.TextChanged -= TextBox_TextChanged;
                textBox.TextChanged += TextBox_TextChanged;
                textBox.PreviewTextInput -= TextBoxMinMaxValue_PreviewTextInput;
                textBox.PreviewTextInput += TextBoxMinMaxValue_PreviewTextInput;
            }
        }

        private static void TextBoxMinMaxValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                int minValue = GetMinValue(textBox);
                int maxValue = GetMaxValue(textBox);

                if (!int.TryParse(textBox.Text + e.Text, out int value))
                {
                    e.Handled = true;
                }
                else
                {
                    if (value < minValue || value > maxValue)
                    {
                        e.Handled = true;
                    }
                }
            }
        }
        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                var minValue = GetMinValue(textBox);
                var maxValue = GetMaxValue(textBox);
                if (!int.TryParse(textBox.Text, out int value))
                    textBox.Text = "";
                else
                {
                    if (value < minValue)
                        textBox.Text = minValue.ToString();
                    else if (value > maxValue)
                        textBox.Text = maxValue.ToString();
                }
            }
        }

        private static void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox && !textBox.IsKeyboardFocusWithin)
            {
                textBox.Focus();
                textBox.SelectAll();
                e.Handled = true;
            }
        }


        private static void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var text = (string) e.DataObject.GetData(typeof(string));

                if (!IsNumeric(text)) e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }


        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsNumeric(e.Text) || ContainsChineseCharacters(e.Text)) e.Handled = true;
        }

        private static bool IsNumeric(string text)
        {
            return !string.IsNullOrEmpty(text) && text.All(char.IsDigit);
        }

        private static bool ContainsChineseCharacters(string text)
        {
            foreach (var c in text)
                if (c >= 0x4E00 && c <= 0x9FFF)
                    return true;
            return false;
        }

        private static void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !e.IsRepeat)
            {
                var textBox = (TextBox) sender;
                var expression = textBox.GetBindingExpression(TextBox.TextProperty);
                expression?.UpdateSource();
            }
        }
    }
}