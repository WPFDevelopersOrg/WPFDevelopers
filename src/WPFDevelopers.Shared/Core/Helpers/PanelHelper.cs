using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

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
            if (panel is StackPanel sp)
            {
                var orientation = GetEffectiveOrientation(sp);
                for (int i = 0; i < panel.Children.Count; i++)
                {
                    if (panel.Children[i] is FrameworkElement fe)
                    {
                        bool isLast = i == panel.Children.Count - 1;
                        if (orientation == Orientation.Horizontal)
                            fe.Margin = new Thickness(spacing, 0, isLast ? 0 : spacing, 0);
                        else
                            fe.Margin = new Thickness(0, spacing, 0, isLast ? 0 : spacing);
                    }
                }
            }
            else if (panel is WrapPanel wp)
            {
                var orientation = GetEffectiveOrientation(wp);
                for (int i = 0; i < panel.Children.Count; i++)
                {
                    if (panel.Children[i] is FrameworkElement fe)
                    {
                        bool isLast = i == panel.Children.Count - 1;
                        if (orientation == Orientation.Horizontal)
                            fe.Margin = new Thickness(spacing, 0, isLast ? 0 : spacing, 0);
                        else
                            fe.Margin = new Thickness(0, spacing, 0, isLast ? 0 : spacing);
                    }
                }
            }
            else if (panel is UniformGrid ug)
            {
                int columns = ug.Columns;
                int rows = ug.Rows;
                if (columns == 0 && rows == 0)
                    columns = rows = (int)Math.Ceiling(Math.Sqrt(panel.Children.Count));
                else if (columns == 0)
                    columns = (panel.Children.Count + rows - 1) / rows;
                else if (rows == 0)
                    rows = (panel.Children.Count + columns - 1) / columns;

                for (int i = 0; i < panel.Children.Count; i++)
                {
                    if (panel.Children[i] is FrameworkElement fe)
                    {
                        int col = i % columns;
                        int row = i / columns;
                        bool isFirstRow = row == 0;
                        bool isFirstCol = col == 0;
                        bool isLastInRow = col == columns - 1;
                        bool isLastInCol = row == rows - 1;
                        fe.Margin = new Thickness(
                            isFirstCol ? spacing : 0,
                            isFirstRow ? spacing : 0,
                            isLastInRow ? 0 : spacing,
                            isLastInCol ? 0 : spacing);
                    }
                }
            }
            else
            {
                for (int i = 0; i < panel.Children.Count; i++)
                {
                    if (panel.Children[i] is FrameworkElement fe)
                    {
                        fe.Margin = new Thickness(spacing);
                    }
                }
            }
        }

        private static Orientation GetEffectiveOrientation(Panel panel)
        {
            if (panel is StackPanel sp)
                return sp.Orientation;
            if (panel is WrapPanel wp)
                return wp.Orientation;
            return Orientation.Horizontal;
        }
    }
}
