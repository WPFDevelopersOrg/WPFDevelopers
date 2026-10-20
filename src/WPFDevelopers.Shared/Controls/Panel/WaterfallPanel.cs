using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;

namespace WPFDevelopers.Controls
{
    public class WaterfallPanel : VirtualizingPanel
    {
        private double[] _columnHeights = new double[0];
        private bool[] _columnHasChildren = new bool[0];
        private readonly List<Rect> _childLayouts = new List<Rect>();

        protected override Size MeasureOverride(Size availableSize)
        {
            var columns = Math.Max(1, Columns);
            if (_columnHeights.Length != columns)
            {
                _columnHeights = new double[columns];
                _columnHasChildren = new bool[columns];
            }

            Array.Clear(_columnHeights, 0, _columnHeights.Length);
            Array.Clear(_columnHasChildren, 0, _columnHasChildren.Length);
            _childLayouts.Clear();

            var spacing = Math.Max(0, Spacing);
            var width = Math.Max(0, (availableSize.Width - (columns - 1) * spacing) / columns);
            var panelDesiredSize = new Size(0, 0);

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                var child = InternalChildren[i];
                child.Measure(new Size(width, double.PositiveInfinity));

                var columnIndex = 0;
                for (var column = 1; column < columns; column++)
                {
                    if (_columnHeights[column] < _columnHeights[columnIndex])
                    {
                        columnIndex = column;
                    }
                }

                var x = columnIndex * (width + spacing);
                var y = _columnHeights[columnIndex];
                if (_columnHasChildren[columnIndex])
                {
                    y += spacing;
                }

                var childHeight = child.DesiredSize.Height;
                _childLayouts.Add(new Rect(x, y, width, childHeight));
                _columnHeights[columnIndex] = y + childHeight;
                _columnHasChildren[columnIndex] = true;

                panelDesiredSize.Width = Math.Max(panelDesiredSize.Width, x + width);
                panelDesiredSize.Height = Math.Max(panelDesiredSize.Height, _columnHeights[columnIndex]);
            }

            return panelDesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            for (var i = 0; i < InternalChildren.Count; i++)
            {
                InternalChildren[i].Arrange(_childLayouts[i]);
            }

            return finalSize;
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
            DependencyProperty.Register(
                "Columns",
                typeof(int),
                typeof(WaterfallPanel),
                new FrameworkPropertyMetadata(3, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register(
                "Spacing",
                typeof(double),
                typeof(WaterfallPanel),
                new FrameworkPropertyMetadata(5.0, FrameworkPropertyMetadataOptions.AffectsMeasure));
    }
}
