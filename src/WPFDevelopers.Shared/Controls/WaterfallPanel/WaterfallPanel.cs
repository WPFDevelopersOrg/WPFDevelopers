using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;

namespace WPFDevelopers.Controls
{
    /// <summary>
    /// Arranges items in equal-width columns, placing each item in the shortest column.
    /// With an unbounded width, the widest item's natural desired width determines the column width.
    /// Collapsed items do not occupy a column or contribute spacing.
    /// As an ItemsControl panel, a finite ScrollViewer viewport with CanContentScroll enabled
    /// supports UI virtualization. Other hosts generate and lay out all items.
    /// </summary>
    public partial class WaterfallPanel : VirtualizingPanel
    {
        private double[] _nonVirtualColumnHeights = new double[0];
        private bool[] _columnHasChildren = new bool[0];
        private readonly List<Rect> _childLayouts = new List<Rect>();
        private double _columnWidth;
        private int _actualColumns = 3;
        private bool _usingVirtualization;

        /// <summary>
        /// Minimum column width for responsive layout; zero keeps the fixed Columns setting.
        /// A container narrower than this value still displays one column.
        /// </summary>
        public double MinColumnWidth
        {
            get { return (double)GetValue(MinColumnWidthProperty); }
            set { SetValue(MinColumnWidthProperty, value); }
        }

        public static readonly DependencyProperty MinColumnWidthProperty =
            DependencyProperty.Register(
                "MinColumnWidth", typeof(double), typeof(WaterfallPanel),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure, OnLayoutPropertyChanged),
                value => (double)value >= 0 && !double.IsInfinity((double)value));

        internal static int ResolveColumns(double availableWidth, int columns, double minColumnWidth, double spacing)
        {
            if (minColumnWidth == 0 || double.IsInfinity(availableWidth))
                return columns;

            return (int)Math.Max(1, Math.Min(int.MaxValue,
                Math.Floor((availableWidth + spacing) / (minColumnWidth + spacing))));
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var children = InternalChildren;
            _itemsControl = ItemsControl.GetItemsOwner(this);
            var virtualize = _itemsControl != null && ScrollOwner != null &&
                !double.IsInfinity(availableSize.Width) && !double.IsInfinity(availableSize.Height);
            if (_usingVirtualization != virtualize)
            {
                RemoveAllRealizedChildren();
                ResetItemMeasurements();
                _usingVirtualization = virtualize;
                _scrollOffset = new Point();
            }
            if (virtualize)
                return MeasureVirtualized(availableSize);

            if (_itemsControl != null)
                GenerateAllContainers(children);

            _extentSize = new Size();
            _viewportSize = new Size();
            if (ScrollOwner != null)
                ScrollOwner.InvalidateScrollInfo();
            return MeasureNonVirtualized(availableSize);
        }

        private void GenerateAllContainers(UIElementCollection children)
        {
            var generator = ItemContainerGenerator;
            using (generator.StartAt(new System.Windows.Controls.Primitives.GeneratorPosition(-1, 0),
                System.Windows.Controls.Primitives.GeneratorDirection.Forward, true))
            {
                for (var index = 0; index < _itemsControl.Items.Count; index++)
                {
                    bool newlyRealized;
                    var child = (UIElement)generator.GenerateNext(out newlyRealized);
                    if (newlyRealized || !children.Contains(child))
                    {
                        InsertInternalChild(index, child);
                        generator.PrepareItemContainer(child);
                    }
                    SetVirtualItemIndex(child, index);
                }
            }
        }

        private Size MeasureNonVirtualized(Size availableSize)
        {
            _actualColumns = ResolveColumns(availableSize.Width, Columns, MinColumnWidth, Spacing);
            SetValue(ActualColumnsPropertyKey, _actualColumns);
            double width;
            if (double.IsPositiveInfinity(availableSize.Width))
            {
                width = 0;
                foreach (UIElement child in InternalChildren)
                {
                    if (child.Visibility == Visibility.Collapsed)
                        continue;

                    child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    width = Math.Max(width, child.DesiredSize.Width);
                }
                width = Math.Max(width, MinColumnWidth);
            }
            else
            {
                width = GetColumnWidth(availableSize.Width);
            }

            return MeasureChildren(width);
        }

        private double GetColumnWidth(double availableWidth)
        {
            return Math.Max(0, (availableWidth - (_actualColumns - 1) * Spacing) / _actualColumns);
        }

        private Size MeasureChildren(double width)
        {
            var columns = _actualColumns;
            if (_nonVirtualColumnHeights.Length != columns)
            {
                _nonVirtualColumnHeights = new double[columns];
                _columnHasChildren = new bool[columns];
            }

            Array.Clear(_nonVirtualColumnHeights, 0, _nonVirtualColumnHeights.Length);
            Array.Clear(_columnHasChildren, 0, _columnHasChildren.Length);
            _childLayouts.Clear();

            _columnWidth = width;
            var spacing = Spacing;
            var panelDesiredSize = new Size(0, 0);

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                var child = InternalChildren[i];
                if (child.Visibility == Visibility.Collapsed)
                {
                    _childLayouts.Add(new Rect());
                    continue;
                }

                child.Measure(new Size(width, double.PositiveInfinity));

                var columnIndex = 0;
                for (var column = 1; column < columns; column++)
                {
                    if (!_columnHasChildren[columnIndex])
                        break;

                    if (!_columnHasChildren[column] ||
                        _nonVirtualColumnHeights[column] < _nonVirtualColumnHeights[columnIndex])
                    {
                        columnIndex = column;
                    }
                }

                var x = columnIndex * (width + spacing);
                var y = _nonVirtualColumnHeights[columnIndex];
                if (_columnHasChildren[columnIndex])
                {
                    y += spacing;
                }

                var childHeight = child.DesiredSize.Height;
                _childLayouts.Add(new Rect(x, y, width, childHeight));
                _nonVirtualColumnHeights[columnIndex] = y + childHeight;
                _columnHasChildren[columnIndex] = true;

                panelDesiredSize.Width = columns * width + (columns - 1) * spacing;
                panelDesiredSize.Height = Math.Max(panelDesiredSize.Height, _nonVirtualColumnHeights[columnIndex]);
            }

            return panelDesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_usingVirtualization)
            {
                ArrangeRealizedChildren();
                return finalSize;
            }
            var columns = ResolveColumns(finalSize.Width, Columns, MinColumnWidth, Spacing);
            var width = GetColumnWidth(finalSize.Width);
            if (columns != _actualColumns)
            {
                _actualColumns = columns;
                width = GetColumnWidth(finalSize.Width);
            }
            SetValue(ActualColumnsPropertyKey, _actualColumns);
            if (width != _columnWidth || _nonVirtualColumnHeights.Length != columns ||
                _childLayouts.Count != InternalChildren.Count)
            {
                MeasureChildren(width);
            }

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

        /// <summary>
        /// Gets or sets the positive number of columns.
        /// </summary>
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
                new FrameworkPropertyMetadata(3, FrameworkPropertyMetadataOptions.AffectsMeasure, OnLayoutPropertyChanged),
                value => (int)value > 0);

        /// <summary>
        /// Gets or sets the finite, nonnegative spacing between items and columns.
        /// </summary>
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
                new FrameworkPropertyMetadata(5.0, FrameworkPropertyMetadataOptions.AffectsMeasure, OnLayoutPropertyChanged),
                value => (double)value >= 0 && !double.IsInfinity((double)value));
    }
}
