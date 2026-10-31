using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class TimelinePanel : Panel
    {
        private readonly List<TimelineItem> _items = new List<TimelineItem>();
        private Timeline _owner;
        private double _before;
        private double _after;
        private bool _splitCrossAxis;

        protected override Size MeasureOverride(Size availableSize)
        {
            _owner = ItemsControl.GetItemsOwner(this) as Timeline;
            _items.Clear();
            if (_owner == null) return new Size();
            _owner.ItemsHost = this;
            foreach (UIElement child in InternalChildren)
            {
                var item = child as TimelineItem;
                if (item != null && item.Visibility != Visibility.Collapsed) _items.Add(item);
            }
            if (_owner.Reverse) _items.Reverse();

            var hasStart = false;
            var hasEnd = false;
            for (var i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                var position = item.Position;
                if (position == TimelineItemPosition.Auto)
                    position = _owner.Mode == TimelineMode.End ||
                        (_owner.Mode == TimelineMode.Alternate && i % 2 != 0)
                        ? TimelineItemPosition.End : TimelineItemPosition.Start;
                item.UpdateLayout(_owner.Orientation, position);
                hasStart |= position == TimelineItemPosition.Start || item.Label != null || item.LabelTemplate != null;
                hasEnd |= position == TimelineItemPosition.End || item.Label != null || item.LabelTemplate != null;
            }
            _splitCrossAxis = hasStart && hasEnd;
            _before = 0;
            _after = 0;
            var vertical = _owner.Orientation == Orientation.Vertical;
            var length = 0.0;
            var constraint = vertical ? new Size(availableSize.Width, double.PositiveInfinity) : availableSize;
            if (!vertical && _items.Count > 0 && !double.IsInfinity(availableSize.Width))
                constraint.Width = Math.Max(0, (availableSize.Width - _owner.ItemSpacing * (_items.Count - 1)) / _items.Count);
            foreach (var item in _items)
            {
                item.ApplyTemplate();
                var layout = item.LayoutPanel;
                if (layout != null) layout.SplitCrossAxis = _splitCrossAxis;
                item.Measure(constraint);
                if (layout != null)
                {
                    _before = Math.Max(_before, layout.Before);
                    _after = Math.Max(_after, layout.After);
                }
                length += vertical ? item.DesiredSize.Height : item.DesiredSize.Width;
            }
            length += Math.Max(0, _items.Count - 1) * _owner.ItemSpacing;
            var cross = _splitCrossAxis ? 2 * Math.Max(_before, _after) : _before + _after;
            return vertical ? new Size(cross, length) : new Size(length, cross);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_owner == null) return finalSize;
            var vertical = _owner.Orientation == Orientation.Vertical;
            var cross = vertical ? finalSize.Width : finalSize.Height;
            var axis = _splitCrossAxis ? cross / 2 : (_before > _after ? cross - _after : _before);
            var desiredLength = 0.0;
            foreach (var item in _items)
                desiredLength += vertical ? item.DesiredSize.Height : item.DesiredSize.Width;
            desiredLength += Math.Max(0, _items.Count - 1) * _owner.ItemSpacing;
            var extra = !vertical && _items.Count > 0 ? Math.Max(0, finalSize.Width - desiredLength) / _items.Count : 0;
            var offset = 0.0;
            foreach (var item in _items)
            {
                if (item.LayoutPanel != null)
                {
                    item.LayoutPanel.AxisOffset = axis;
                    item.LayoutPanel.InvalidateArrange();
                }
                var length = (vertical ? item.DesiredSize.Height : item.DesiredSize.Width) + extra;
                item.Arrange(vertical ? new Rect(0, offset, finalSize.Width, length)
                    : new Rect(offset, 0, length, finalSize.Height));
                offset += length + _owner.ItemSpacing;
            }
            InvalidateVisual();
            return finalSize;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (_owner == null || _owner.LineBrush == null || _owner.LineThickness <= 0) return;
            var vertical = _owner.Orientation == Orientation.Vertical;
            for (var i = 1; i < _items.Count; i++)
            {
                var previous = _items[i - 1];
                var current = _items[i];
                if (previous.LayoutPanel == null || current.LayoutPanel == null ||
                    previous.Visibility != Visibility.Visible || current.Visibility != Visibility.Visible) continue;
                var start = previous.LayoutPanel.TranslatePoint(previous.LayoutPanel.DotCenter, this);
                var end = current.LayoutPanel.TranslatePoint(current.LayoutPanel.DotCenter, this);
                var startGap = (vertical ? previous.LayoutPanel.DotSize.Height : previous.LayoutPanel.DotSize.Width) / 2 + 2;
                var endGap = (vertical ? current.LayoutPanel.DotSize.Height : current.LayoutPanel.DotSize.Width) / 2 + 2;
                if (vertical)
                {
                    start.Y += startGap;
                    end.Y -= endGap;
                    if (end.Y <= start.Y) continue;
                }
                else
                {
                    start.X += startGap;
                    end.X -= endGap;
                    if (end.X <= start.X) continue;
                }
                var pen = new Pen(_owner.LineBrush, _owner.LineThickness);
                if (current.IsLoading || previous.IsLoading) pen.DashStyle = DashStyles.Dash;
                drawingContext.DrawLine(pen, start, end);
            }
        }
    }
}
