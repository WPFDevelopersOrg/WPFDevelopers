using System;
using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Helpers
{
    public class ElementHelper : DependencyObject
    {
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ElementHelper),
                new PropertyMetadata(new CornerRadius(0)));

        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(ElementHelper),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IsStripeProperty =
            DependencyProperty.RegisterAttached("IsStripe", typeof(bool), typeof(ElementHelper),
                new PropertyMetadata(false));

        public static readonly DependencyProperty IsRoundProperty =
           DependencyProperty.RegisterAttached("IsRound", typeof(bool), typeof(ElementHelper),
               new PropertyMetadata(false));

        public static readonly DependencyProperty IsClearProperty =
          DependencyProperty.RegisterAttached("IsClear", typeof(bool), typeof(ElementHelper),
              new PropertyMetadata(false, OnIsClearChanged));

        public static CornerRadius GetCornerRadius(DependencyObject obj)
        {
            return (CornerRadius) obj.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }

        public static string GetWatermark(DependencyObject obj)
        {
            return (string) obj.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
        }

        public static bool GetIsStripe(DependencyObject obj)
        {
            return (bool) obj.GetValue(IsStripeProperty);
        }

        public static void SetIsStripe(DependencyObject obj, bool value)
        {
            obj.SetValue(IsStripeProperty, value);
        }

        public static bool GetIsRound(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsRoundProperty);
        }

        public static void SetIsRound(DependencyObject obj, bool value)
        {
            obj.SetValue(IsRoundProperty, value);
        }

        public static void SetIsClear(UIElement element, bool value)
        {
            element.SetValue(IsClearProperty, value);
        }

        public static bool GetIsClear(UIElement element)
        {
            return (bool)element.GetValue(IsClearProperty);
        }

        private static void OnIsClearChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as Button;
            if (button != null)
            {
                if ((bool)e.NewValue)
                    button.Click += OnButtonClear_Click;
                else
                    button.Click -= OnButtonClear_Click;
            }
        }

        private static void OnButtonClear_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.TemplatedParent is TextBox textBox)
                    textBox.Clear();
                else if (button.TemplatedParent is PasswordBox passwordBox)
                    passwordBox.Clear();
                else if (button.TemplatedParent is TabItem tabItem)
                {
                    var tabControl = tabItem.Parent as TabControl;
                    if (tabControl != null)
                        tabControl.Items.Remove(tabItem); 
                }
                else if(button.TemplatedParent is DateRangePicker dateRangePicker)
                {
                    dateRangePicker.StartDate = null;
                    dateRangePicker.EndDate = null;
                }
            }
        }
        
    }
}