using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = LayoutPartName, Type = typeof(TimelineItemPanel))]
    public class TimelineItem : ContentControl
    {
        private const string LayoutPartName = "PART_Layout";

        private static readonly DependencyPropertyKey OrientationPropertyKey =
            DependencyProperty.RegisterReadOnly("Orientation", typeof(Orientation), typeof(TimelineItem),
                new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure));

        private static readonly DependencyPropertyKey ActualPositionPropertyKey =
            DependencyProperty.RegisterReadOnly("ActualPosition", typeof(TimelineItemPosition), typeof(TimelineItem),
                new FrameworkPropertyMetadata(TimelineItemPosition.Start, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty OrientationProperty = OrientationPropertyKey.DependencyProperty;
        public static readonly DependencyProperty ActualPositionProperty = ActualPositionPropertyKey.DependencyProperty;

        public static readonly DependencyProperty LabelProperty = Register("Label", typeof(object), null);
        public static readonly DependencyProperty LabelTemplateProperty = Register("LabelTemplate", typeof(DataTemplate), null);
        public static readonly DependencyProperty DotProperty = Register("Dot", typeof(object), null);
        public static readonly DependencyProperty DotTemplateProperty = Register("DotTemplate", typeof(DataTemplate), null);
        public static readonly DependencyProperty DotBrushProperty = Register("DotBrush", typeof(Brush), null);
        public static readonly DependencyProperty IsLoadingProperty = Register("IsLoading", typeof(bool), false);

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(TimelineItemPosition), typeof(TimelineItem),
                new FrameworkPropertyMetadata(TimelineItemPosition.Auto,
                    FrameworkPropertyMetadataOptions.AffectsMeasure, OnLayoutChanged),
                value => Enum.IsDefined(typeof(TimelineItemPosition), value));

        static TimelineItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimelineItem), new FrameworkPropertyMetadata(typeof(TimelineItem)));
            HorizontalContentAlignmentProperty.OverrideMetadata(typeof(TimelineItem),
                new FrameworkPropertyMetadata(HorizontalAlignment.Stretch));
        }

        public object Label
        {
            get { return GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public DataTemplate LabelTemplate
        {
            get { return (DataTemplate)GetValue(LabelTemplateProperty); }
            set { SetValue(LabelTemplateProperty, value); }
        }

        public object Dot
        {
            get { return GetValue(DotProperty); }
            set { SetValue(DotProperty, value); }
        }

        public DataTemplate DotTemplate
        {
            get { return (DataTemplate)GetValue(DotTemplateProperty); }
            set { SetValue(DotTemplateProperty, value); }
        }

        public Brush DotBrush
        {
            get { return (Brush)GetValue(DotBrushProperty); }
            set { SetValue(DotBrushProperty, value); }
        }

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        public TimelineItemPosition Position
        {
            get { return (TimelineItemPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
        }

        public TimelineItemPosition ActualPosition
        {
            get { return (TimelineItemPosition)GetValue(ActualPositionProperty); }
        }

        internal TimelineItemPanel LayoutPanel { get; private set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            LayoutPanel = GetTemplateChild(LayoutPartName) as TimelineItemPanel;
        }

        internal void UpdateLayout(Orientation orientation, TimelineItemPosition position)
        {
            if (Orientation == orientation && ActualPosition == position) return;
            SetValue(OrientationPropertyKey, orientation);
            SetValue(ActualPositionPropertyKey, position);
            if (LayoutPanel != null) LayoutPanel.InvalidateMeasure();
        }

        private static DependencyProperty Register(string name, Type type, object defaultValue)
        {
            return DependencyProperty.Register(name, type, typeof(TimelineItem),
                new FrameworkPropertyMetadata(defaultValue, FrameworkPropertyMetadataOptions.AffectsMeasure, OnLayoutChanged));
        }

        private static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = ItemsControl.ItemsControlFromItemContainer((TimelineItem)d) as Timeline;
            if (owner != null) owner.InvalidateTimeline();
        }
    }
}
