using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace WPFDevelopers.Controls
{
    public partial class WaterfallPanel : IScrollInfo
    {
        private const double ScrollLineAmount = 16.0;
        private const int MaximumCorrectionPasses = 3;

        private readonly List<double> _itemHeights = new List<double>();
        private readonly List<bool> _collapsedItems = new List<bool>();
        private readonly List<Rect> _itemLayouts = new List<Rect>();
        private readonly List<int> _realizationIndices = new List<int>();
        private ItemsControl _itemsControl;
        private double _layoutWidth = double.NaN;
        private int _layoutColumns;
        private bool _hasLayout;
        private bool _hasExtent;
        private bool _correctionQueued;
        private int _bringIntoViewIndex = -1;
        private double[] _columnHeights = new double[0];
        private bool[] _columnHasItems = new bool[0];

        private static readonly DependencyPropertyKey ActualColumnsPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "ActualColumns",
                typeof(int),
                typeof(WaterfallPanel),
                new FrameworkPropertyMetadata(3));

        /// <summary>Read-only number of columns selected for the current panel width.</summary>
        public static readonly DependencyProperty ActualColumnsProperty = ActualColumnsPropertyKey.DependencyProperty;

        public int ActualColumns
        {
            get { return (int)GetValue(ActualColumnsProperty); }
        }

        public static readonly DependencyProperty EstimatedItemHeightProperty =
            DependencyProperty.Register(
                "EstimatedItemHeight",
                typeof(double),
                typeof(WaterfallPanel),
                new FrameworkPropertyMetadata(200.0, OnEstimateChanged),
                IsFinitePositive);

        /// <summary>
        /// Gets or sets the positive finite height used to position items before they are measured.
        /// Better estimates reduce position corrections when scrolling to distant items.
        /// </summary>
        public double EstimatedItemHeight
        {
            get { return (double)GetValue(EstimatedItemHeightProperty); }
            set { SetValue(EstimatedItemHeightProperty, value); }
        }

        private Size _extentSize;
        private Size _viewportSize;
        private Point _scrollOffset;
        private ScrollViewer _scrollOwner;

        private static bool IsFinitePositive(object value)
        {
            double number = (double)value;
            return !double.IsNaN(number) && !double.IsInfinity(number) && number > 0;
        }

        private static void OnLayoutPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var panel = (WaterfallPanel)dependencyObject;
            panel._hasLayout = false;
            panel.InvalidateMeasure();
        }

        private static void OnEstimateChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            WaterfallPanel panel = (WaterfallPanel)dependencyObject;
            panel.ResetItemMeasurements();
            panel.InvalidateMeasure();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            _itemsControl = ItemsControl.GetItemsOwner(this);
        }

        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            base.OnItemsChanged(sender, args);

            RemoveAllRealizedChildren();
            ResetItemMeasurements();
            _bringIntoViewIndex = -1;
            InvalidateMeasure();
        }

        private void RemoveAllRealizedChildren()
        {
            IItemContainerGenerator generator = ItemContainerGenerator;
            for (int visualIndex = Children.Count - 1; visualIndex >= 0; visualIndex--)
            {
                UIElement child = Children[visualIndex];
                int itemIndex = _itemsControl == null ? -1 : _itemsControl.ItemContainerGenerator.IndexFromContainer(child);
                if (itemIndex >= 0)
                {
                    GeneratorPosition position = generator.GeneratorPositionFromIndex(itemIndex);
                    if (position.Index >= 0)
                    {
                        if (ShouldRecycle() && !(_itemsControl.Items[itemIndex] is UIElement) &&
                            generator is IRecyclingItemContainerGenerator)
                            ((IRecyclingItemContainerGenerator)generator).Recycle(position, 1);
                        else
                            generator.Remove(position, 1);
                    }
                }

                RemoveInternalChildRange(visualIndex, 1);
            }
        }

        private bool ShouldRecycle()
        {
            return _itemsControl != null &&
                VirtualizingStackPanel.GetVirtualizationMode(_itemsControl) == VirtualizationMode.Recycling;
        }

        private void ResetItemMeasurements()
        {
            _itemHeights.Clear();
            _collapsedItems.Clear();
            _itemLayouts.Clear();
            _hasLayout = false;
        }

        private Size MeasureVirtualized(Size availableSize)
        {
            if (double.IsInfinity(availableSize.Width) || double.IsInfinity(availableSize.Height))
                throw new InvalidOperationException("Virtualized waterfall layout requires a finite viewport width and height.");

            if (_itemsControl == null)
                _itemsControl = ItemsControl.GetItemsOwner(this);
            if (_itemsControl == null)
                return availableSize;

            int itemCount = _itemsControl.Items.Count;
            EnsureItemState(itemCount);

            double width = Math.Max(0, availableSize.Width);
            double viewportHeight = Math.Max(0, availableSize.Height);
            int columns = WaterfallPanel.ResolveColumns(width, Columns, MinColumnWidth, Spacing);
            if (columns < 1)
                columns = 1;

            bool widthChanged = !_hasLayout || width != _layoutWidth || columns != _layoutColumns;
            if (widthChanged)
                ResetItemMeasurementsForCount(itemCount);

            _layoutWidth = width;
            _layoutColumns = columns;
            _hasLayout = true;
            SetValue(ActualColumnsPropertyKey, columns);

            double columnWidth = Math.Max(0, (width - (columns - 1) * Spacing) / columns);
            _viewportSize = new Size(width, viewportHeight);
            bool needsCorrection = false;

            for (int pass = 0; pass < MaximumCorrectionPasses; pass++)
            {
                BuildItemLayouts(itemCount, columns, columnWidth);
                UpdateExtent(width, viewportHeight);
                ClampVerticalOffset();
                EnsurePendingItemVisible(viewportHeight);

                SelectRealizationItems(itemCount, viewportHeight);
                ReconcileRealizedChildren();
                bool passHeightChanged = MeasureRealizedChildren(columnWidth);

                if (!passHeightChanged)
                    break;
                if (pass == MaximumCorrectionPasses - 1)
                    needsCorrection = true;
            }

            if (needsCorrection)
                QueueCorrectionMeasure();

            // Rebuild once with the latest measurements for the arrange pass.
            BuildItemLayouts(itemCount, columns, columnWidth);
            UpdateExtent(width, viewportHeight);
            ClampVerticalOffset();
            EnsurePendingItemVisible(viewportHeight);
            for (int i = 0; i < itemCount; i++)
            {
                var layout = _itemLayouts[i];
                if (!layout.IsEmpty && layout.Bottom >= _scrollOffset.Y &&
                    layout.Top <= _scrollOffset.Y + viewportHeight &&
                    _itemsControl.ItemContainerGenerator.ContainerFromIndex(i) == null)
                {
                    QueueCorrectionMeasure();
                    break;
                }
            }
            _bringIntoViewIndex = -1;
            return availableSize;
        }

        private void EnsureItemState(int itemCount)
        {
            while (_itemHeights.Count < itemCount)
            {
                _itemHeights.Add(EstimatedItemHeight);
                _collapsedItems.Add(false);
            }

            while (_itemHeights.Count > itemCount)
            {
                _itemHeights.RemoveAt(_itemHeights.Count - 1);
                _collapsedItems.RemoveAt(_collapsedItems.Count - 1);
            }
        }

        private void ResetItemMeasurementsForCount(int itemCount)
        {
            EnsureItemState(itemCount);
            for (int i = 0; i < itemCount; i++)
            {
                _itemHeights[i] = EstimatedItemHeight;
                _collapsedItems[i] = false;
            }
        }

        private void BuildItemLayouts(int itemCount, int columns, double columnWidth)
        {
            EnsureItemState(itemCount);
            _itemLayouts.Clear();
            if (_columnHeights.Length != columns)
            {
                _columnHeights = new double[columns];
                _columnHasItems = new bool[columns];
            }
            Array.Clear(_columnHeights, 0, columns);
            Array.Clear(_columnHasItems, 0, columns);

            for (int i = 0; i < itemCount; i++)
            {
                if (_collapsedItems[i])
                {
                    _itemLayouts.Add(new Rect(0, _columnHeights[0], columnWidth, 0));
                    continue;
                }

                int shortestColumn = 0;
                for (int column = 1; column < columns; column++)
                {
                    if (!_columnHasItems[shortestColumn])
                        break;
                    if (!_columnHasItems[column] || _columnHeights[column] < _columnHeights[shortestColumn])
                        shortestColumn = column;
                }

                double y = _columnHeights[shortestColumn];
                if (_columnHasItems[shortestColumn])
                    y += Spacing;

                Rect layout = new Rect(shortestColumn * (columnWidth + Spacing), y, columnWidth, _itemHeights[i]);
                _itemLayouts.Add(layout);
                _columnHeights[shortestColumn] = y + _itemHeights[i];
                _columnHasItems[shortestColumn] = true;
            }
        }

        private void UpdateExtent(double width, double viewportHeight)
        {
            double oldMaxOffset = Math.Max(0, _extentSize.Height - _viewportSize.Height);
            bool wasAtBottom = _hasExtent && oldMaxOffset > 0 &&
                Math.Abs(_scrollOffset.Y - oldMaxOffset) < 0.5;
            double height = 0;
            for (int i = 0; i < _itemLayouts.Count; i++)
            {
                Rect layout = _itemLayouts[i];
                if (!layout.IsEmpty)
                    height = Math.Max(height, layout.Bottom);
            }

            _extentSize = new Size(width, Math.Max(viewportHeight, height));
            if (wasAtBottom)
                _scrollOffset.Y = Math.Max(0, _extentSize.Height - viewportHeight);
            _hasExtent = true;
            if (_scrollOwner != null)
                _scrollOwner.InvalidateScrollInfo();
        }

        private void ClampVerticalOffset()
        {
            double maxOffset = Math.Max(0, _extentSize.Height - _viewportSize.Height);
            _scrollOffset.Y = Math.Max(0, Math.Min(_scrollOffset.Y, maxOffset));
            _scrollOffset.X = 0;
        }

        private void SelectRealizationItems(int itemCount, double viewportHeight)
        {
            _realizationIndices.Clear();
            if (!VirtualizingStackPanel.GetIsVirtualizing(_itemsControl))
            {
                for (int i = 0; i < itemCount; i++)
                    _realizationIndices.Add(i);
                return;
            }

            double cacheTop = Math.Max(0, _scrollOffset.Y - viewportHeight);
            double cacheBottom = _scrollOffset.Y + viewportHeight * 2;

            for (int i = 0; i < itemCount; i++)
            {
                Rect layout = _itemLayouts[i];
                if (!layout.IsEmpty && layout.Bottom >= cacheTop && layout.Top <= cacheBottom)
                    _realizationIndices.Add(i);
            }
        }

        private bool IsSelected(int itemIndex)
        {
            int low = 0;
            int high = _realizationIndices.Count - 1;
            while (low <= high)
            {
                int middle = low + (high - low) / 2;
                int candidate = _realizationIndices[middle];
                if (candidate == itemIndex)
                    return true;
                if (candidate < itemIndex)
                    low = middle + 1;
                else
                    high = middle - 1;
            }
            return false;
        }

        private void ReconcileRealizedChildren()
        {
            IItemContainerGenerator generator = ItemContainerGenerator;
            for (int visualIndex = Children.Count - 1; visualIndex >= 0; visualIndex--)
            {
                UIElement child = Children[visualIndex];
                int itemIndex = _itemsControl.ItemContainerGenerator.IndexFromContainer(child);
                if (itemIndex < 0)
                    itemIndex = GetVirtualItemIndex(child);

                if (itemIndex < 0 || !IsSelected(itemIndex))
                {
                    int currentIndex = _itemsControl.ItemContainerGenerator.IndexFromContainer(child);
                    if (currentIndex >= 0)
                    {
                        GeneratorPosition position = generator.GeneratorPositionFromIndex(currentIndex);
                        if (position.Index >= 0)
                        {
                            if (ShouldRecycle() && !(_itemsControl.Items[currentIndex] is UIElement) &&
                                generator is IRecyclingItemContainerGenerator)
                                ((IRecyclingItemContainerGenerator)generator).Recycle(position, 1);
                            else
                                generator.Remove(position, 1);
                        }
                    }
                    RemoveInternalChildRange(visualIndex, 1);
                }
            }

            int visualTargetIndex = 0;
            for (int targetIndex = 0; targetIndex < _realizationIndices.Count; targetIndex++)
            {
                int itemIndex = _realizationIndices[targetIndex];
                GeneratorPosition position = generator.GeneratorPositionFromIndex(itemIndex);
                UIElement child;
                bool newlyRealized;

                using (generator.StartAt(position, GeneratorDirection.Forward, true))
                    child = (UIElement)generator.GenerateNext(out newlyRealized);

                SetVirtualItemIndex(child, itemIndex);
                if (newlyRealized)
                {
                    InsertInternalChild(visualTargetIndex, child);
                }
                else
                {
                    int currentVisualIndex = Children.IndexOf(child);
                    if (currentVisualIndex < 0)
                        InsertInternalChild(visualTargetIndex, child);
                    else if (currentVisualIndex != visualTargetIndex)
                    {
                        RemoveInternalChildRange(currentVisualIndex, 1);
                        InsertInternalChild(visualTargetIndex, child);
                    }
                }

                if (newlyRealized)
                    generator.PrepareItemContainer(child);
                visualTargetIndex++;
            }
        }

        private bool MeasureRealizedChildren(double columnWidth)
        {
            bool changed = false;
            for (int i = 0; i < Children.Count; i++)
            {
                UIElement child = Children[i];
                int itemIndex = GetVirtualItemIndex(child);
                if (itemIndex < 0 || itemIndex >= _itemHeights.Count)
                    continue;

                child.Measure(new Size(columnWidth, double.PositiveInfinity));
                bool collapsed = child.Visibility == Visibility.Collapsed;
                double measuredHeight = collapsed ? 0 : child.DesiredSize.Height;
                if (double.IsNaN(measuredHeight) || double.IsInfinity(measuredHeight) || measuredHeight < 0)
                    throw new InvalidOperationException("A waterfall item returned an invalid desired height.");

                if (_collapsedItems[itemIndex] != collapsed || _itemHeights[itemIndex] != measuredHeight)
                {
                    _collapsedItems[itemIndex] = collapsed;
                    _itemHeights[itemIndex] = measuredHeight;
                    changed = true;
                }
            }
            return changed;
        }

        private void ArrangeRealizedChildren()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                UIElement child = Children[i];
                int itemIndex = GetVirtualItemIndex(child);
                Rect layout = itemIndex >= 0 && itemIndex < _itemLayouts.Count
                    ? _itemLayouts[itemIndex]
                    : Rect.Empty;

                if (layout.IsEmpty)
                    child.Arrange(new Rect());
                else
                    child.Arrange(new Rect(layout.X, layout.Y - _scrollOffset.Y, layout.Width, layout.Height));
            }
        }

        private void QueueCorrectionMeasure()
        {
            if (_correctionQueued)
                return;

            _correctionQueued = true;
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
            {
                _correctionQueued = false;
                InvalidateMeasure();
            }));
        }

        private static readonly DependencyProperty VirtualItemIndexProperty =
            DependencyProperty.RegisterAttached(
                "VirtualItemIndex",
                typeof(int),
                typeof(WaterfallPanel),
                new FrameworkPropertyMetadata(-1));

        private static int GetVirtualItemIndex(DependencyObject element)
        {
            return (int)element.GetValue(VirtualItemIndexProperty);
        }

        private static void SetVirtualItemIndex(DependencyObject element, int index)
        {
            element.SetValue(VirtualItemIndexProperty, index);
        }

        protected override void BringIndexIntoView(int index)
        {
            var owner = _itemsControl ?? ItemsControl.GetItemsOwner(this);
            if (owner == null || index < 0 || index >= owner.Items.Count)
                throw new ArgumentOutOfRangeException("index");

            _bringIntoViewIndex = index;
            InvalidateMeasure();
            if (index >= _itemLayouts.Count)
                return;

            Rect item = _itemLayouts[index];
            if (item.IsEmpty)
                return;

            double offset = _scrollOffset.Y;
            if (item.Top < offset)
                offset = item.Top;
            else if (item.Bottom > offset + _viewportSize.Height)
                offset = item.Bottom - _viewportSize.Height;
            SetVerticalOffset(offset);
        }

        private void EnsurePendingItemVisible(double viewportHeight)
        {
            if (_bringIntoViewIndex < 0 || _bringIntoViewIndex >= _itemLayouts.Count)
                return;

            Rect item = _itemLayouts[_bringIntoViewIndex];
            if (item.IsEmpty)
                return;

            if (item.Top < _scrollOffset.Y)
                _scrollOffset.Y = item.Top;
            else if (item.Bottom > _scrollOffset.Y + viewportHeight)
                _scrollOffset.Y = Math.Max(item.Top, item.Bottom - viewportHeight);
            ClampVerticalOffset();
        }

        #region IScrollInfo

        public bool CanHorizontallyScroll { get; set; }

        private bool _canVerticallyScroll;
        public bool CanVerticallyScroll
        {
            get { return _canVerticallyScroll; }
            set
            {
                if (_canVerticallyScroll == value)
                    return;
                _canVerticallyScroll = value;
                if (!value)
                    _scrollOffset.Y = 0;
                InvalidateMeasure();
            }
        }

        public double ExtentWidth { get { return _extentSize.Width; } }
        public double ExtentHeight { get { return _extentSize.Height; } }
        public double ViewportWidth { get { return _viewportSize.Width; } }
        public double ViewportHeight { get { return _viewportSize.Height; } }
        public double HorizontalOffset { get { return 0; } }
        public double VerticalOffset { get { return _scrollOffset.Y; } }
        public ScrollViewer ScrollOwner
        {
            get { return _scrollOwner; }
            set
            {
                if (_scrollOwner == value)
                    return;
                _scrollOwner = value;
                InvalidateMeasure();
            }
        }

        public void LineUp() { SetVerticalOffset(VerticalOffset - ScrollLineAmount); }
        public void LineDown() { SetVerticalOffset(VerticalOffset + ScrollLineAmount); }
        public void LineLeft() { }
        public void LineRight() { }
        public void MouseWheelUp() { SetVerticalOffset(VerticalOffset - ScrollLineAmount * 3); }
        public void MouseWheelDown() { SetVerticalOffset(VerticalOffset + ScrollLineAmount * 3); }
        public void MouseWheelLeft() { }
        public void MouseWheelRight() { }
        public void PageUp() { SetVerticalOffset(VerticalOffset - ViewportHeight); }
        public void PageDown() { SetVerticalOffset(VerticalOffset + ViewportHeight); }
        public void PageLeft() { }
        public void PageRight() { }

        public void SetHorizontalOffset(double offset)
        {
            if (double.IsNaN(offset))
                throw new ArgumentOutOfRangeException("offset");
            _scrollOffset.X = 0;
        }

        public void SetVerticalOffset(double offset)
        {
            if (double.IsNaN(offset))
                throw new ArgumentOutOfRangeException("offset");
            if (!CanVerticallyScroll)
                offset = 0;
            if (double.IsInfinity(offset))
                offset = offset > 0 ? double.MaxValue : 0;

            double maxOffset = Math.Max(0, ExtentHeight - ViewportHeight);
            offset = Math.Max(0, Math.Min(offset, maxOffset));
            if (_scrollOffset.Y != offset)
            {
                _scrollOffset.Y = offset;
                InvalidateMeasure();
                InvalidateArrange();
                if (_scrollOwner != null)
                    _scrollOwner.InvalidateScrollInfo();
            }
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            if (visual == null || rectangle.IsEmpty || Children.Count == 0)
                return Rect.Empty;

            DependencyObject current = visual;
            while (current != null && current != this && !(current is UIElement && Children.Contains((UIElement)current)))
                current = VisualTreeHelper.GetParent(current);
            if (current == null || current == this)
                return Rect.Empty;

            int itemIndex = GetVirtualItemIndex(current);
            if (itemIndex < 0 || itemIndex >= _itemLayouts.Count)
                return Rect.Empty;

            Rect visible = visual.TransformToAncestor(this).TransformBounds(rectangle);

            double previousOffset = _scrollOffset.Y;
            double desiredOffset = previousOffset;
            if (visible.Top < 0)
                desiredOffset += visible.Top;
            else if (visible.Bottom > _viewportSize.Height)
                desiredOffset += visible.Bottom - _viewportSize.Height;
            SetVerticalOffset(desiredOffset);

            double appliedDelta = _scrollOffset.Y - previousOffset;
            double top = Math.Max(0, visible.Top - appliedDelta);
            double bottom = Math.Min(_viewportSize.Height, visible.Bottom - appliedDelta);
            if (bottom <= top)
                return Rect.Empty;
            return new Rect(visible.X, top, visible.Width, bottom - top);
        }

        #endregion
    }
}
