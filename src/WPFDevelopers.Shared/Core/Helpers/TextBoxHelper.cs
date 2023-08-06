using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFDevelopers.Helpers
{
    public static class TextBoxHelper
    {
        public static readonly DependencyProperty SelectAllOnClickProperty =
            DependencyProperty.RegisterAttached("SelectAllOnClick", typeof(bool), typeof(TextBoxHelper), new PropertyMetadata(false, OnSelectAllOnClickChanged));

        private static void OnSelectAllOnClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if ((bool)e.NewValue)
                    textBox.PreviewMouseLeftButtonDown += TextBox_PreviewMouseLeftButtonDown;
                else
                    textBox.PreviewMouseLeftButtonDown -= TextBox_PreviewMouseLeftButtonDown;
            }
        }


        public static readonly DependencyProperty AllowOnlyNumericInputProperty =
       DependencyProperty.RegisterAttached("AllowOnlyNumericInput", typeof(bool), typeof(TextBoxHelper), new PropertyMetadata(false, OnAllowOnlyNumericInputChanged));

        private static void OnAllowOnlyNumericInputChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if ((bool)e.NewValue)
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


        public static bool GetSelectAllOnClick(TextBox textBox)
        {
            return (bool)textBox.GetValue(SelectAllOnClickProperty);
        }

        public static void SetSelectAllOnClick(TextBox textBox, bool value)
        {
            textBox.SetValue(SelectAllOnClickProperty, value);
        }

        public static bool GetAllowOnlyNumericInput(DependencyObject obj)
        {
            return (bool)obj.GetValue(AllowOnlyNumericInputProperty);
        }

        public static void SetAllowOnlyNumericInput(DependencyObject obj, bool value)
        {
            obj.SetValue(AllowOnlyNumericInputProperty, value);
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
                string text = (string)e.DataObject.GetData(typeof(string));

                if (!IsNumeric(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        

        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsNumeric(e.Text) || ContainsChineseCharacters(e.Text))
            {
                e.Handled = true;
            }
        }

        private static bool IsNumeric(string text)
        {
            return !string.IsNullOrEmpty(text) && text.All(char.IsDigit);
        }

        private static bool ContainsChineseCharacters(string text)
        {
            foreach (char c in text)
            {
                if (c >= 0x4E00 && c <= 0x9FFF)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
