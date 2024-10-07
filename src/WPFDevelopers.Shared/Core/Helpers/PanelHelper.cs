using System;
using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Helpers
{
    public class PanelHelper : DependencyObject
    {
        public static double GetSpacing(DependencyObject obj)
        {
            return (double)obj.GetValue(SpacingProperty);
        }

        public static void SetSpacing(DependencyObject obj, double value)
        {
            obj.SetValue(SpacingProperty, value);
        }

        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.RegisterAttached("Spacing", typeof(double), typeof(PanelHelper), new UIPropertyMetadata(0d, OnSpacingChanged));

        private static void OnSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Panel panel)
            {
                double newValue = (double)e.NewValue;
                double oldValue = (double)e.OldValue;
                if (panel.IsLoaded)
                {
                    UpdateChildrenSpacing(panel, newValue);
                }
                else
                {
                    panel.Loaded -= OnPanelLoaded;
                    panel.Loaded += OnPanelLoaded;
                    panel.SetValue(SpacingProperty, newValue);
                }
            }
        }

        private static void OnPanelLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is Panel panel)
            {
                double spacing = (double)panel.GetValue(SpacingProperty);
                UpdateChildrenSpacing(panel, spacing);
                panel.Loaded -= OnPanelLoaded;
            }
        }

        private static void UpdateChildrenSpacing(Panel panel, double spacing)
        {
            foreach (UIElement child in panel.Children)
            {
                if (child is FrameworkElement frameworkElement)
                {
                    frameworkElement.Margin = new Thickness(spacing);
                }
            }
        }
    }
}
