using System;
using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class TimelineItemPanel : Panel
    {
        private const double ContentGap = 12;
        private bool _splitCrossAxis;

        internal bool SplitCrossAxis
        {
            get { return _splitCrossAxis; }
            set
            {
                if (_splitCrossAxis == value) return;
                _splitCrossAxis = value;
                InvalidateMeasure();
            }
        }

        internal double AxisOffset { get; set; }
        internal double Before { get; private set; }
        internal double After { get; private set; }
        internal Point DotCenter { get; private set; }
        internal Size DotSize { get; private set; }

        protected override Size MeasureOverride(Size availableSize)
        {
            var item = TemplatedParent as TimelineItem;
            if (item == null || InternalChildren.Count != 3) return new Size();
            var vertical = item.Orientation == Orientation.Vertical;
            var label = InternalChildren[0];
            var dot = InternalChildren[1];
            var content = InternalChildren[2];
            dot.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            DotSize = dot.DesiredSize;
            var cross = vertical ? availableSize.Width : availableSize.Height;
            var halfDot = (vertical ? DotSize.Width : DotSize.Height) / 2;
            var budget = Math.Max(0, (SplitCrossAxis ? cross / 2 : cross - halfDot) - halfDot - ContentGap);
            var constraint = vertical
                ? new Size(budget, double.PositiveInfinity)
                : new Size(availableSize.Width, budget);
            label.Measure(constraint);
            content.Measure(constraint);
            var labelCross = vertical ? label.DesiredSize.Width : label.DesiredSize.Height;
            var contentCross = vertical ? content.DesiredSize.Width : content.DesiredSize.Height;
            var leading = item.ActualPosition == TimelineItemPosition.Start ? labelCross : contentCross;
            var trailing = item.ActualPosition == TimelineItemPosition.Start ? contentCross : labelCross;
            Before = halfDot + (leading > 0 ? leading + ContentGap : 0);
            After = halfDot + (trailing > 0 ? trailing + ContentGap : 0);
            var length = vertical
                ? Math.Max(DotSize.Height, Math.Max(label.DesiredSize.Height, content.DesiredSize.Height))
                : Math.Max(DotSize.Width, Math.Max(label.DesiredSize.Width, content.DesiredSize.Width));
            return vertical ? new Size(Before + After, length) : new Size(length, Before + After);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var item = TemplatedParent as TimelineItem;
            if (item == null || InternalChildren.Count != 3) return finalSize;
            var vertical = item.Orientation == Orientation.Vertical;
            var halfDot = (vertical ? DotSize.Width : DotSize.Height) / 2;
            DotCenter = vertical ? new Point(AxisOffset, DotSize.Height / 2) : new Point(DotSize.Width / 2, AxisOffset);
            InternalChildren[1].Arrange(new Rect(
                DotCenter.X - DotSize.Width / 2, DotCenter.Y - DotSize.Height / 2, DotSize.Width, DotSize.Height));
            var before = Math.Max(0, AxisOffset - halfDot - ContentGap);
            var afterStart = AxisOffset + halfDot + ContentGap;
            var after = Math.Max(0, (vertical ? finalSize.Width : finalSize.Height) - afterStart);
            var start = item.ActualPosition == TimelineItemPosition.Start;
            ArrangeContent(InternalChildren[0], vertical, !start, before, afterStart, after, finalSize);
            ArrangeContent(InternalChildren[2], vertical, start, before, afterStart, after, finalSize);
            return finalSize;
        }

        private static void ArrangeContent(UIElement child, bool vertical, bool afterAxis,
            double before, double afterStart, double after, Size finalSize)
        {
            if (vertical)
            {
                var width = afterAxis ? after : before;
                child.Arrange(new Rect(afterAxis ? afterStart : Math.Max(0, before - child.DesiredSize.Width),
                    0, Math.Min(width, child.DesiredSize.Width), finalSize.Height));
            }
            else
            {
                child.Arrange(new Rect(0, afterAxis ? afterStart : Math.Max(0, before - child.DesiredSize.Height),
                    finalSize.Width, Math.Min(afterAxis ? after : before, child.DesiredSize.Height)));
            }
        }
    }
}
