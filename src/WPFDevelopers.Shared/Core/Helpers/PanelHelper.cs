using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WPFDevelopers.Helpers
{
    public class PanelHelper : DependencyObject
    {
        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.RegisterAttached("Spacing", typeof(double), typeof(PanelHelper),
                new PropertyMetadata(0d, OnSpacingChanged),
                value => (double)value >= 0 && !double.IsNaN((double)value) && !double.IsInfinity((double)value));

        private static readonly DependencyProperty SpacingStateProperty =
            DependencyProperty.RegisterAttached("SpacingState", typeof(SpacingState), typeof(PanelHelper));

        public static double GetSpacing(DependencyObject obj)
        {
            return (double)obj.GetValue(SpacingProperty);
        }

        public static void SetSpacing(DependencyObject obj, double value)
        {
            obj.SetValue(SpacingProperty, value);
        }

        private static void OnSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = d as Panel;
            if (panel == null) return;
            var state = (SpacingState)panel.GetValue(SpacingStateProperty);
            if ((double)e.NewValue == 0)
            {
                if (state == null) return;
                state.Detach();
                panel.ClearValue(SpacingStateProperty);
                return;
            }
            if (state == null)
            {
                state = new SpacingState(panel);
                panel.SetValue(SpacingStateProperty, state);
            }
            state.Update();
        }

        private sealed class ChildMargin
        {
            internal Thickness Original;
            internal Thickness Applied;
            internal bool HadLocalValue;
        }

        private sealed class SpacingState
        {
            private readonly Panel _panel;
            private readonly Dictionary<FrameworkElement, ChildMargin> _margins =
                new Dictionary<FrameworkElement, ChildMargin>();
            private bool _updating;

            internal SpacingState(Panel panel)
            {
                _panel = panel;
                panel.Loaded += OnLoaded;
                panel.Unloaded += OnUnloaded;
                if (panel.IsLoaded) panel.LayoutUpdated += OnLayoutUpdated;
            }

            private void OnLoaded(object sender, RoutedEventArgs e)
            {
                _panel.LayoutUpdated -= OnLayoutUpdated;
                _panel.LayoutUpdated += OnLayoutUpdated;
                Update();
            }

            private void OnUnloaded(object sender, RoutedEventArgs e)
            {
                _panel.LayoutUpdated -= OnLayoutUpdated;
                RestoreMargins();
            }

            private void OnLayoutUpdated(object sender, EventArgs e)
            {
                Update();
            }

            internal void Detach()
            {
                _panel.Loaded -= OnLoaded;
                _panel.Unloaded -= OnUnloaded;
                _panel.LayoutUpdated -= OnLayoutUpdated;
                RestoreMargins();
            }

            private void RestoreMargins()
            {
                foreach (var entry in _margins) Restore(entry.Key, entry.Value);
                _margins.Clear();
            }

            internal void Update()
            {
                if (_updating) return;
                _updating = true;
                try
                {
                    var children = new List<FrameworkElement>();
                    foreach (UIElement child in _panel.Children)
                    {
                        var element = child as FrameworkElement;
                        if (element != null) children.Add(element);
                    }
                    var removed = new List<FrameworkElement>();
                    foreach (var entry in _margins)
                    {
                        if (!children.Contains(entry.Key)) removed.Add(entry.Key);
                    }
                    foreach (var child in removed)
                    {
                        Restore(child, _margins[child]);
                        _margins.Remove(child);
                    }

                    var last = children.FindLast(element => element.Visibility != Visibility.Collapsed);
                    var spacing = GetSpacing(_panel);
                    foreach (var child in children)
                    {
                        ChildMargin margin;
                        if (!_margins.TryGetValue(child, out margin))
                        {
                            margin = new ChildMargin
                            {
                                Original = child.Margin,
                                Applied = child.Margin,
                                HadLocalValue = child.ReadLocalValue(FrameworkElement.MarginProperty) != DependencyProperty.UnsetValue
                            };
                            _margins.Add(child, margin);
                        }
                        // Bindings and application code can change Margin after spacing has been applied.
                        if (child.Margin != margin.Applied)
                        {
                            margin.Original = child.Margin;
                            margin.HadLocalValue = child.ReadLocalValue(FrameworkElement.MarginProperty) != DependencyProperty.UnsetValue;
                        }
                        var gap = new Thickness();
                        if (child.Visibility != Visibility.Collapsed)
                        {
                            var stack = _panel as StackPanel;
                            if (stack != null)
                            {
                                if (child != last)
                                    gap = stack.Orientation == Orientation.Horizontal
                                        ? new Thickness(0, 0, spacing, 0) : new Thickness(0, 0, 0, spacing);
                            }
                            else if (_panel is WrapPanel || _panel is UniformGrid)
                            {
                                // Half-spacing on each side gives an exact gap on both axes, including wrapped rows.
                                gap = new Thickness(spacing / 2);
                            }
                            else
                            {
                                gap = new Thickness(spacing);
                            }
                        }
                        margin.Applied = new Thickness(
                            margin.Original.Left + gap.Left, margin.Original.Top + gap.Top,
                            margin.Original.Right + gap.Right, margin.Original.Bottom + gap.Bottom);
                        if (child.Margin != margin.Applied)
                            child.SetCurrentValue(FrameworkElement.MarginProperty, margin.Applied);
                    }
                }
                finally
                {
                    _updating = false;
                }
            }

            private static void Restore(FrameworkElement child, ChildMargin margin)
            {
                if (child.Margin != margin.Applied) return;
                if (margin.HadLocalValue)
                    child.SetCurrentValue(FrameworkElement.MarginProperty, margin.Original);
                else
                    child.ClearValue(FrameworkElement.MarginProperty);
            }
        }
    }
}
