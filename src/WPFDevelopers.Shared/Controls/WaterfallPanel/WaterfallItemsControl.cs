using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    /// <summary>
    /// An items control with responsive masonry layout and a virtualizing scrolling template.
    /// Give the control a finite height to enable viewport-based container realization.
    /// </summary>
    public class WaterfallItemsControl : ItemsControl
    {
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(int), typeof(WaterfallItemsControl),
                new FrameworkPropertyMetadata(3), value => (int)value > 0);

        public static readonly DependencyProperty MinColumnWidthProperty =
            DependencyProperty.Register("MinColumnWidth", typeof(double), typeof(WaterfallItemsControl),
                new FrameworkPropertyMetadata(0.0), IsFiniteNonNegative);

        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register("Spacing", typeof(double), typeof(WaterfallItemsControl),
                new FrameworkPropertyMetadata(5.0), IsFiniteNonNegative);

        public static readonly DependencyProperty EstimatedItemHeightProperty =
            DependencyProperty.Register("EstimatedItemHeight", typeof(double), typeof(WaterfallItemsControl),
                new FrameworkPropertyMetadata(200.0),
                value => (double)value > 0 && !double.IsInfinity((double)value));

        private static bool IsFiniteNonNegative(object value)
        {
            var number = (double)value;
            return number >= 0 && !double.IsInfinity(number);
        }

        static WaterfallItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WaterfallItemsControl),
                new FrameworkPropertyMetadata(typeof(WaterfallItemsControl)));
        }

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public double MinColumnWidth
        {
            get { return (double)GetValue(MinColumnWidthProperty); }
            set { SetValue(MinColumnWidthProperty, value); }
        }

        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        public double EstimatedItemHeight
        {
            get { return (double)GetValue(EstimatedItemHeightProperty); }
            set { SetValue(EstimatedItemHeightProperty, value); }
        }
    }
}
