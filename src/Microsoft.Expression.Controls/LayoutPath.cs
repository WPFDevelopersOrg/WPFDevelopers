using Microsoft.Expression.Drawing.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Microsoft.Expression.Controls
{
    public sealed class LayoutPath : Animatable
    {
        public FrameworkElement SourceElement
        {
            get
            {
                return (FrameworkElement)base.GetValue(LayoutPath.SourceElementProperty);
            }
            set
            {
                base.SetValue(LayoutPath.SourceElementProperty, value);
            }
        }

        public Distribution Distribution
        {
            get
            {
                return (Distribution)base.GetValue(LayoutPath.DistributionProperty);
            }
            set
            {
                base.SetValue(LayoutPath.DistributionProperty, value);
            }
        }

        [TypeConverter(typeof(LayoutPathCapacityConverter))]
        public double Capacity
        {
            get
            {
                return (double)base.GetValue(LayoutPath.CapacityProperty);
            }
            set
            {
                base.SetValue(LayoutPath.CapacityProperty, value);
            }
        }

        public double Padding
        {
            get
            {
                return (double)base.GetValue(LayoutPath.PaddingProperty);
            }
            set
            {
                base.SetValue(LayoutPath.PaddingProperty, value);
            }
        }

        public Orientation Orientation
        {
            get
            {
                return (Orientation)base.GetValue(LayoutPath.OrientationProperty);
            }
            set
            {
                base.SetValue(LayoutPath.OrientationProperty, value);
            }
        }

        public double Start
        {
            get
            {
                return (double)base.GetValue(LayoutPath.StartProperty);
            }
            set
            {
                base.SetValue(LayoutPath.StartProperty, value);
            }
        }

        public double Span
        {
            get
            {
                return (double)base.GetValue(LayoutPath.SpanProperty);
            }
            set
            {
                base.SetValue(LayoutPath.SpanProperty, value);
            }
        }

        public FillBehavior FillBehavior
        {
            get
            {
                return (FillBehavior)base.GetValue(LayoutPath.FillBehaviorProperty);
            }
            set
            {
                base.SetValue(LayoutPath.FillBehaviorProperty, value);
            }
        }

        public LayoutPath()
        {
            this.oldTransformedTestPoints = new Point[LayoutPath.testPoints.Length];
        }

        public double ActualCapacity { get; internal set; }

        public bool IsValid
        {
            get
            {
                if (this.SourceElement == null || this.pathPanel == null)
                {
                    this.isValid = null;
                    return false;
                }
                if (this.isValid != null)
                {
                    return this.isValid.Value;
                }
                this.isValid = new bool?(true);
                if (this.SourceElement is PathPanel || this.SourceElement is PathListBox)
                {
                    this.isValid = new bool?(false);
                }
                else
                {
                    for (DependencyObject parent = VisualTreeHelper.GetParent(this.SourceElement); parent != null; parent = VisualTreeHelper.GetParent(parent))
                    {
                        PathPanel pathPanel = parent as PathPanel;
                        if (pathPanel != null && pathPanel == this.pathPanel)
                        {
                            this.isValid = new bool?(false);
                            break;
                        }
                    }
                }
                return this.isValid.Value;
            }
        }

        internal bool IsLayoutDirty
        {
            get
            {
                return this.isLayoutDirty;
            }
            set
            {
                if (value && this.IsAttached)
                {
                    this.transformedPolylines = null;
                    this.strategy.InvalidatePolylineCache();
                    this.pathPanel.InvalidateArrange();
                }
                this.isLayoutDirty = value;
            }
        }

        internal bool IsRenderDirty
        {
            get
            {
                return this.isRenderDirty;
            }
            set
            {
                if (value && this.IsAttached)
                {
                    this.transformedPolylines = null;
                    this.pathPanel.InvalidateArrange();
                }
                this.isRenderDirty = value;
            }
        }

        internal IList<PolylineData> Polylines
        {
            get
            {
                if (!this.IsAttached)
                {
                    return new List<PolylineData>();
                }
                return this.transformedPolylines;
            }
        }

        internal double TotalLength
        {
            get
            {
                double num = 0.0;
                if (this.Polylines != null)
                {
                    foreach (PolylineData polylineData in this.Polylines)
                    {
                        num += polylineData.TotalLength;
                    }
                }
                return num;
            }
        }

        internal void CheckLayoutState()
        {
            if (this.IsLayoutDirty)
            {
                return;
            }
            if (!this.IsAttached)
            {
                return;
            }
            if (this.strategy.HasGeometryChanged())
            {
                this.IsLayoutDirty = true;
            }
        }

        internal void CheckRenderState()
        {
            if (this.IsRenderDirty)
            {
                return;
            }
            if (!this.IsAttached)
            {
                return;
            }
            if (this.HaveTestPointsChanged())
            {
                this.IsRenderDirty = true;
            }
        }

        internal void Attach(PathPanel pathPanel)
        {
            this.Detach();
            this.pathPanel = pathPanel;
            if (this.IsValid)
            {
                this.strategy = GeometryStrategy.Create(this);
            }
        }

        internal bool IsAttached
        {
            get
            {
                return this.pathPanel != null && this.SourceElement != null && this.strategy != null;
            }
        }

        internal void Detach()
        {
            if (this.strategy != null)
            {
                this.strategy.Unhook();
                this.strategy = null;
            }
            this.pathPanel = null;
        }

        internal void UpdateCache()
        {
            if (this.IsRenderDirty || this.IsLayoutDirty)
            {
                IEnumerable<GeneralTransform> transforms = this.ComputeTransforms();
                LayoutPath.testPoints.ForEach(delegate (Point p, int i)
                {
                    this.oldTransformedTestPoints[i] = transforms.TransformPoint(p);
                });
                this.transformedPolylines = new List<PolylineData>(this.strategy.Polylines.Count);
                this.transformedPolylines.EnsureListCount(this.strategy.Polylines.Count, null);
                for (int j = 0; j < this.strategy.Polylines.Count; j++)
                {
                    PolylineData polylineData = this.strategy.Polylines[j];
                    Point[] points = new Point[polylineData.Count];
                    polylineData.Points.ForEach(delegate (Point p, int i)
                    {
                        points[i] = transforms.TransformPoint(p);
                    });
                    this.transformedPolylines[j] = new PolylineData(points);
                }
                this.IsLayoutDirty = false;
                this.IsRenderDirty = false;
            }
        }

        internal int Distribute(int pathIndex, int childIndex)
        {
            if (!this.IsAttached)
            {
                throw new InvalidOperationException();
            }
            this.UpdateCache();
            return DistributionStrategy.Distribute(this.pathPanel, pathIndex, childIndex);
        }

        internal double GetLengthTo(PolylineData line, MarchLocation location)
        {
            double num = 0.0;
            foreach (PolylineData polylineData in this.Polylines)
            {
                if (polylineData == line)
                {
                    break;
                }
                num += polylineData.TotalLength;
            }
            num += location.GetArcLength(line.AccumulatedLength);
            return num;
        }

        protected override Freezable CreateInstanceCore()
        {
            return new LayoutPath();
        }

        private static void LayoutPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LayoutPath layoutPath = d as LayoutPath;
            if (layoutPath == null || layoutPath.pathPanel == null)
            {
                return;
            }
            if (e.Property == LayoutPath.SourceElementProperty && e.NewValue != e.OldValue)
            {
                layoutPath.isValid = null;
                layoutPath.Attach(layoutPath.pathPanel);
            }
            layoutPath.pathPanel.InvalidateArrange();
        }

        private IEnumerable<GeneralTransform> ComputeTransforms()
        {
            IList<GeneralTransform> list = this.strategy.ComputeTransforms() ?? new List<GeneralTransform>();
            list.Add(GeometryHelper.RelativeTransform(this.SourceElement, this.pathPanel));
            return list;
        }

        private bool HaveTestPointsChanged()
        {
            IEnumerable<GeneralTransform> transforms = this.ComputeTransforms();
            for (int i = 0; i < LayoutPath.testPoints.Length; i++)
            {
                if (this.oldTransformedTestPoints[i] != transforms.TransformPoint(LayoutPath.testPoints[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private PathPanel pathPanel;

        private GeometryStrategy strategy;

        private bool isLayoutDirty = true;

        private bool isRenderDirty = true;

        private bool? isValid;

        private static readonly Point[] testPoints = new Point[]
{
            new Point(0.0, 0.0),
            new Point(1.0, 0.0),
            new Point(0.0, 1.0),
            new Point(1.0, 1.0)
};

        private Point[] oldTransformedTestPoints;

        private List<PolylineData> transformedPolylines;

        public static readonly DependencyProperty SourceElementProperty = DependencyProperty.Register("SourceElement", typeof(FrameworkElement), typeof(LayoutPath), new PropertyMetadata(new PropertyChangedCallback(LayoutPath.LayoutPathChanged)));

        public static readonly DependencyProperty DistributionProperty = DependencyProperty.Register("Distribution", typeof(Distribution), typeof(LayoutPath), new PropertyMetadata(Distribution.Padded, new PropertyChangedCallback(LayoutPath.LayoutPathChanged)));

        public static readonly DependencyProperty CapacityProperty = DependencyProperty.Register("Capacity", typeof(double), typeof(LayoutPath), new PropertyMetadata(double.NaN, new PropertyChangedCallback(LayoutPath.LayoutPathChanged)));

        public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register("Padding", typeof(double), typeof(LayoutPath), new PropertyMetadata(10.0, new PropertyChangedCallback(LayoutPath.LayoutPathChanged)));

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(LayoutPath), new PropertyMetadata(Orientation.None, new PropertyChangedCallback(LayoutPath.LayoutPathChanged)));

        public static readonly DependencyProperty StartProperty = DependencyProperty.Register("Start", typeof(double), typeof(LayoutPath), new PropertyMetadata(0.0, new PropertyChangedCallback(LayoutPath.LayoutPathChanged)));

        public static readonly DependencyProperty SpanProperty = DependencyProperty.Register("Span", typeof(double), typeof(LayoutPath), new PropertyMetadata(1.0, new PropertyChangedCallback(LayoutPath.LayoutPathChanged)));

        public static readonly DependencyProperty FillBehaviorProperty = DependencyProperty.Register("FillBehavior", typeof(FillBehavior), typeof(LayoutPath), new PropertyMetadata(FillBehavior.FullSpan, new PropertyChangedCallback(LayoutPath.LayoutPathChanged)));
    }
}
