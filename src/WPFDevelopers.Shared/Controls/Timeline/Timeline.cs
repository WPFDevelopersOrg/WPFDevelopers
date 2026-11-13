using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public enum TimelineMode
    {
        Start,
        Alternate,
        End
    }

    public enum TimelineItemPosition
    {
        Auto,
        Start,
        End
    }

    [TemplatePart(Name = ScrollViewerPartName, Type = typeof(ScrollViewer))]
    public class Timeline : ItemsControl
    {
        private const string ScrollViewerPartName = "PART_ScrollViewer";
        private const int MouseHorizontalWheelMessage = 0x020E;
        private ScrollViewer _scrollViewer;
        private HwndSource _hwndSource;

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Timeline),
                new FrameworkPropertyMetadata(Orientation.Vertical, OnLayoutChanged),
                value => (Orientation)value == Orientation.Vertical || (Orientation)value == Orientation.Horizontal);

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(TimelineMode), typeof(Timeline),
                new FrameworkPropertyMetadata(TimelineMode.Start, OnLayoutChanged),
                value => Enum.IsDefined(typeof(TimelineMode), value));

        public static readonly DependencyProperty ReverseProperty =
            DependencyProperty.Register("Reverse", typeof(bool), typeof(Timeline),
                new FrameworkPropertyMetadata(false, OnLayoutChanged));

        public static readonly DependencyProperty ItemSpacingProperty =
            DependencyProperty.Register("ItemSpacing", typeof(double), typeof(Timeline),
                new FrameworkPropertyMetadata(20.0, OnLayoutChanged), IsValidLength);

        public static readonly DependencyProperty LineBrushProperty =
            DependencyProperty.Register("LineBrush", typeof(Brush), typeof(Timeline),
                new FrameworkPropertyMetadata(null, OnLayoutChanged));

        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register("LineThickness", typeof(double), typeof(Timeline),
                new FrameworkPropertyMetadata(2.0, OnLayoutChanged), IsValidLength);

        static Timeline()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata(typeof(Timeline)));
        }

        public Timeline()
        {
            Loaded += OnTimelineLoaded;
            Unloaded += OnTimelineUnloaded;
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public TimelineMode Mode
        {
            get { return (TimelineMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        public bool Reverse
        {
            get { return (bool)GetValue(ReverseProperty); }
            set { SetValue(ReverseProperty, value); }
        }

        public double ItemSpacing
        {
            get { return (double)GetValue(ItemSpacingProperty); }
            set { SetValue(ItemSpacingProperty, value); }
        }

        public Brush LineBrush
        {
            get { return (Brush)GetValue(LineBrushProperty); }
            set { SetValue(LineBrushProperty, value); }
        }

        public double LineThickness
        {
            get { return (double)GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }

        internal TimelinePanel ItemsHost { get; set; }

        public override void OnApplyTemplate()
        {
            if (_scrollViewer != null)
                _scrollViewer.PreviewMouseWheel -= OnScrollViewerPreviewMouseWheel;
            base.OnApplyTemplate();
            _scrollViewer = GetTemplateChild(ScrollViewerPartName) as ScrollViewer;
            if (_scrollViewer != null)
                _scrollViewer.PreviewMouseWheel += OnScrollViewerPreviewMouseWheel;
        }

        private void OnTimelineLoaded(object sender, RoutedEventArgs e)
        {
            var hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (ReferenceEquals(_hwndSource, hwndSource))
                return;
            if (_hwndSource != null)
                _hwndSource.RemoveHook(OnWindowMessage);
            _hwndSource = hwndSource;
            if (_hwndSource != null)
                _hwndSource.AddHook(OnWindowMessage);
        }

        private void OnTimelineUnloaded(object sender, RoutedEventArgs e)
        {
            if (_hwndSource != null)
            {
                _hwndSource.RemoveHook(OnWindowMessage);
                _hwndSource = null;
            }
        }

        private IntPtr OnWindowMessage(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (message != MouseHorizontalWheelMessage || _scrollViewer == null ||
                Orientation != Orientation.Horizontal || !_scrollViewer.IsVisible)
                return IntPtr.Zero;

            var delta = unchecked((short)((long)wParam >> 16));
            if (delta == 0 || _scrollViewer.ScrollableWidth <= 0)
                return IntPtr.Zero;

            var screenPoint = new Point(
                unchecked((short)(long)lParam),
                unchecked((short)((long)lParam >> 16)));
            var point = _scrollViewer.PointFromScreen(screenPoint);
            if (point.X < 0 || point.Y < 0 ||
                point.X > _scrollViewer.ActualWidth || point.Y > _scrollViewer.ActualHeight)
                return IntPtr.Zero;

            var currentOffset = _scrollViewer.HorizontalOffset;
            var targetOffset = Math.Max(0, Math.Min(currentOffset + delta, _scrollViewer.ScrollableWidth));
            if (targetOffset == currentOffset)
                return IntPtr.Zero;

            _scrollViewer.ScrollToHorizontalOffset(targetOffset);
            handled = true;
            return IntPtr.Zero;
        }

        private void OnScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Handled || e.Delta == 0) return;
            if (Orientation == Orientation.Horizontal &&
                _scrollViewer.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled)
            {
                var canScrollHorizontally = e.Delta > 0
                    ? _scrollViewer.HorizontalOffset > 0
                    : _scrollViewer.HorizontalOffset < _scrollViewer.ScrollableWidth;
                if (canScrollHorizontally)
                {
                    e.Handled = true;
                    _scrollViewer.ScrollToHorizontalOffset(
                        _scrollViewer.HorizontalOffset - e.Delta);
                    return;
                }
            }

            var canScroll = _scrollViewer.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled &&
                (e.Delta > 0 ? _scrollViewer.VerticalOffset > 0 :
                    _scrollViewer.VerticalOffset < _scrollViewer.ScrollableHeight);
            if (canScroll) return;

            e.Handled = true;
            RaiseEvent(new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = MouseWheelEvent,
                Source = this
            });
        }

        internal static bool IsValidLength(object value)
        {
            var length = (double)value;
            return length >= 0 && !double.IsNaN(length) && !double.IsInfinity(length);
        }

        internal void InvalidateTimeline()
        {
            if (ItemsHost == null) return;
            ItemsHost.InvalidateMeasure();
            ItemsHost.InvalidateVisual();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TimelineItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TimelineItem();
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            InvalidateTimeline();
        }

        private static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Timeline)d).InvalidateTimeline();
        }
    }
}
