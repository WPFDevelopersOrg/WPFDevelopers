using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows;

namespace WPFDevelopers.Controls
{
    public class WaterfallPanel : VirtualizingPanel
    {
        private List<double> columnHeights = new List<double>();
        protected override Size MeasureOverride(Size availableSize)
        {
            columnHeights.Clear();
            var panelDesiredSize = new Size(0, 0);
            columnHeights = new double[Columns].ToList();
            double currentX = 0;
            var width = availableSize.Width / Columns - (Columns * Spacing);
            for (int i = 0; i < InternalChildren.Count; i++)
            {
                var child = InternalChildren[i] as FrameworkElement;
                if (child == null)
                    continue;
                child.Measure(availableSize);
                child.Width = width;
                int columnIndex = i % Columns;
                double x = columnIndex != 0 ? currentX + Spacing : 0;
                double y = columnHeights[columnIndex];
                if (i >= Columns)
                    y = y + Spacing;
                var size = new Size(width, child.DesiredSize.Height);
                child.Arrange(new Rect(new Point(x, y), size));
                panelDesiredSize.Width = Math.Max(panelDesiredSize.Width, x + child.DesiredSize.Width);
                panelDesiredSize.Height = Math.Max(panelDesiredSize.Height, y + child.DesiredSize.Height);
                currentX = x + size.Width;
                if (currentX >= Width)
                    currentX = 0;
                columnHeights[columnIndex] += child.DesiredSize.Height + (i >= Columns ? Spacing : 0);
            }
            return panelDesiredSize;
        }

        public void AddChild(UIElement element)
        {
            Children.Add(element);
        }

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(int), typeof(WaterfallPanel), new PropertyMetadata(3));

        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register("Spacing", typeof(double), typeof(WaterfallPanel), new PropertyMetadata(5.0));
    }
}
