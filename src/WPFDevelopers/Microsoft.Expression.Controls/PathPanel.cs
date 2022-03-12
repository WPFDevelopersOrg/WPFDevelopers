using Microsoft.Expression.Drawing.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Expression.Controls
{
    public sealed class PathPanel : Panel
    {
        public LayoutPathCollection LayoutPaths
        {
            get
            {
                return (LayoutPathCollection)base.GetValue(PathPanel.LayoutPathsProperty);
            }
            set
            {
                base.SetValue(PathPanel.LayoutPathsProperty, value);
            }
        }

        public double StartItemIndex
        {
            get
            {
                return (double)base.GetValue(PathPanel.StartItemIndexProperty);
            }
            set
            {
                base.SetValue(PathPanel.StartItemIndexProperty, value);
            }
        }

        public bool WrapItems
        {
            get
            {
                return (bool)base.GetValue(PathPanel.WrapItemsProperty);
            }
            set
            {
                base.SetValue(PathPanel.WrapItemsProperty, value);
            }
        }

        public PathPanel()
        {
            LayoutPathCollection value = new LayoutPathCollection();
            base.SetValue(PathPanel.LayoutPathsProperty, value);
            base.Loaded += this.PathPanel_Loaded;
            base.Unloaded += this.PathPanel_Unloaded;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (object obj in base.Children)
            {
                UIElement uielement = (UIElement)obj;
                if (uielement != null)
                {
                    uielement.Measure(PathPanel.InfinteSize);
                }
            }
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size result = base.ArrangeOverride(finalSize);
            if (this.LayoutPaths != null && this.LayoutPaths.Count > 0)
            {
                foreach (LayoutPath layoutPath in this.LayoutPaths)
                {
                    if (!layoutPath.IsAttached)
                    {
                        layoutPath.Attach(this);
                    }
                }
                this.ValidPaths = new List<LayoutPath>(from path in this.LayoutPaths
                                                       where path.IsAttached && path.SourceElement.Visibility != Visibility.Collapsed
                                                       select path);
            }
            else
            {
                this.ValidPaths = null;
            }
            if (base.Children.Count == 0)
            {
                return result;
            }
            if (this.ValidPaths == null || this.ValidPaths.Count == 0)
            {
                this.ArrangeFirstChild();
                return result;
            }
            if (!this.UpdateIndirection())
            {
                return result;
            }
            this.lastPoint = default(Point);
            this.previousLength = 0.0;
            foreach (LayoutPath layoutPath2 in this.ValidPaths)
            {
                layoutPath2.UpdateCache();
            }
            this.totalLength = 0.0;
            foreach (LayoutPath layoutPath3 in this.ValidPaths)
            {
                this.totalLength += layoutPath3.TotalLength;
            }
            int i = 0;
            for (int j = 0; j < this.LayoutPaths.Count; j++)
            {
                if (i >= this.Count)
                {
                    break;
                }
                LayoutPath layoutPath4 = this.LayoutPaths[j];
                if (layoutPath4.IsAttached && layoutPath4.SourceElement.Visibility != Visibility.Collapsed)
                {
                    i = layoutPath4.Distribute(j, i);
                    this.previousLength += layoutPath4.TotalLength;
                }
            }
            while (i < this.Count)
            {
                UIElement hiddenChild = base.Children[this.indices[i]];
                PathPanel.HideAtPoint(hiddenChild, this.lastPoint);
                i++;
            }
            return result;
        }

        internal IList<LayoutPath> ValidPaths { get; private set; }

        internal int Count
        {
            get
            {
                if (this.indices == null)
                {
                    return 0;
                }
                return this.indices.Length;
            }
        }

        internal void ArrangeChild(int indirectIndex, int pathIndex, PolylineData polyline, MarchLocation location, int localIndex)
        {
            LayoutPath layoutPath = this.LayoutPaths[pathIndex];
            int num = this.indices[indirectIndex];
            UIElement uielement = base.Children[num];
            this.lastPoint = location.GetPoint(polyline.Points);
            if (this.shouldLayoutHiddenChildren)
            {
                int num2 = Math.Min(base.Children.Count, Math.Max(0, (int)Math.Round(this.StartItemIndex)));
                for (int i = 0; i < num2; i++)
                {
                    PathPanel.HideAtPoint(base.Children[i], this.lastPoint);
                }
                this.shouldLayoutHiddenChildren = false;
            }
            IPathLayoutItem pathLayoutItem = uielement as IPathLayoutItem;
            if (pathLayoutItem != null)
            {
                Vector vector = -location.GetNormal(polyline, 10.0);
                double num3 = Vector.AngleBetween(PathPanel.Up, vector);
                double lengthTo = layoutPath.GetLengthTo(polyline, location);
                double rhs = layoutPath.TotalLength;
                pathLayoutItem.Update(new PathLayoutData
                {
                    LayoutPathIndex = pathIndex,
                    GlobalIndex = num,
                    LocalIndex = localIndex,
                    NormalAngle = num3,
                    OrientationAngle = ((layoutPath.Orientation == Orientation.OrientToPath) ? num3 : 0.0),
                    LocalOffset = MathHelper.SafeDivide(lengthTo, rhs, 0.0),
                    GlobalOffset = MathHelper.SafeDivide(this.previousLength + lengthTo, this.totalLength, 0.0),
                    IsArranged = true
                });
            }
            Rect rect = new Rect(this.lastPoint, default(Size));
            double num4 = uielement.DesiredSize.Width / 2.0;
            double num5 = uielement.DesiredSize.Height / 2.0;
            rect = GeometryHelper.Inflate(rect, new Thickness(num4, num5, num4, num5));
            uielement.Arrange(rect);
        }

        internal double GetChildRadius(int indirectIndex)
        {
            UIElement uielement = base.Children[this.indices[indirectIndex]];
            return Math.Max(uielement.DesiredSize.Width, uielement.DesiredSize.Height) / 2.0;
        }

        private bool UpdateIndirection()
        {
            int num = base.Children.Count;
            int num2 = Math.Max(0, (int)Math.Round(this.StartItemIndex));
            if (!this.WrapItems)
            {
                num2 = Math.Min(num, num2);
                num -= num2;
                if (num <= 0)
                {
                    foreach (object obj in base.Children)
                    {
                        UIElement uielement = (UIElement)obj;
                        uielement.Arrange(PathPanel.ZeroRect);
                        PathPanel.RemovePathLayoutProperties(uielement as PathListBoxItem, false);
                    }
                    return false;
                }
                this.shouldLayoutHiddenChildren = true;
            }
            else
            {
                num2 %= num;
            }
            if (this.indices == null || this.indices.Length != num)
            {
                this.indices = new int[num];
                int num3 = num2;
                for (int i = 0; i < num; i++)
                {
                    this.indices[i] = num3;
                    num3 = (num3 + 1) % base.Children.Count;
                }
            }
            return true;
        }

        private static void HideAtPoint(UIElement hiddenChild, Point point)
        {
            Point location = point.Minus(new Point(hiddenChild.DesiredSize.Width / 2.0, hiddenChild.DesiredSize.Height / 2.0));
            PathPanel.RemovePathLayoutProperties(hiddenChild as PathListBoxItem, false);
            hiddenChild.Arrange(new Rect(location, new Size(0.0, 0.0)));
        }

        private void ArrangeFirstChild()
        {
            UIElement uielement = base.Children[0];
            uielement.Arrange(new Rect(default(Point), base.Children[0].DesiredSize));
            bool isArranged = true;
            PathPanel.RemovePathLayoutProperties(uielement as PathListBoxItem, isArranged);
            for (int i = 1; i < base.Children.Count; i++)
            {
                UIElement uielement2 = base.Children[i];
                uielement2.Arrange(PathPanel.ZeroRect);
                PathPanel.RemovePathLayoutProperties(uielement2 as PathListBoxItem, false);
            }
        }

        private void PathPanel_Loaded(object sender, RoutedEventArgs e)
        {
            base.InvalidateArrange();
        }

        private void PathPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering -= this.CheckOnRenderHandler;
            base.LayoutUpdated -= this.CheckOnLayoutHandler;
            this.isListening = false;
            this.isUnloaded = true;
        }

        private static void LayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PathPanel pathPanel = d as PathPanel;
            if (pathPanel == null)
            {
                return;
            }
            if (e.Property == PathPanel.StartItemIndexProperty || e.Property == PathPanel.WrapItemsProperty)
            {
                pathPanel.indices = null;
            }
            else if (e.Property == PathPanel.LayoutPathsProperty)
            {
                if (e.NewValue == e.OldValue)
                {
                    return;
                }
                LayoutPathCollection layoutPathCollection = e.OldValue as LayoutPathCollection;
                if (layoutPathCollection != null)
                {
                    foreach (LayoutPath layoutPath in layoutPathCollection)
                    {
                        layoutPath.Detach();
                    }
                    ((INotifyCollectionChanged)layoutPathCollection).CollectionChanged -= pathPanel.LayoutPaths_CollectionChanged;
                }
                LayoutPathCollection layoutPathCollection2 = e.NewValue as LayoutPathCollection;
                if (layoutPathCollection2 != null)
                {
                    foreach (LayoutPath layoutPath2 in layoutPathCollection2)
                    {
                        layoutPath2.Attach(pathPanel);
                    }
                    ((INotifyCollectionChanged)layoutPathCollection2).CollectionChanged += pathPanel.LayoutPaths_CollectionChanged;
                }
                PathPanel.UpdateListeners(pathPanel);
            }
            pathPanel.InvalidateArrange();
        }

        private void LayoutPaths_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            bool flag = false;
            if (e.OldItems != null)
            {
                foreach (object obj in e.OldItems)
                {
                    LayoutPath layoutPath = (LayoutPath)obj;
                    layoutPath.Detach();
                    flag = true;
                }
            }
            if (e.NewItems != null)
            {
                foreach (object obj2 in e.NewItems)
                {
                    LayoutPath layoutPath2 = (LayoutPath)obj2;
                    layoutPath2.Attach(this);
                    flag = true;
                }
            }
            if (flag)
            {
                PathPanel.UpdateListeners(this);
                base.InvalidateArrange();
            }
        }

        private static void UpdateListeners(PathPanel pathPanel)
        {
            if (pathPanel.isUnloaded)
            {
                return;
            }
            bool flag = pathPanel.LayoutPaths != null && pathPanel.LayoutPaths.Count > 0;
            if (!pathPanel.isListening && flag)
            {
                CompositionTarget.Rendering += pathPanel.CheckOnRenderHandler;
                pathPanel.LayoutUpdated += pathPanel.CheckOnLayoutHandler;
                pathPanel.isListening = true;
                return;
            }
            if (pathPanel.isListening && !flag)
            {
                CompositionTarget.Rendering -= pathPanel.CheckOnRenderHandler;
                pathPanel.LayoutUpdated -= pathPanel.CheckOnLayoutHandler;
                pathPanel.isListening = false;
            }
        }

        private void CheckOnRenderHandler(object sender, EventArgs e)
        {
            if (this.LayoutPaths == null || this.LayoutPaths.Count == 0)
            {
                return;
            }
            foreach (LayoutPath layoutPath in this.LayoutPaths)
            {
                if (layoutPath.IsAttached)
                {
                    layoutPath.CheckRenderState();
                }
            }
        }

        private void CheckOnLayoutHandler(object sender, EventArgs e)
        {
            if (this.LayoutPaths == null || this.LayoutPaths.Count == 0)
            {
                return;
            }
            foreach (LayoutPath layoutPath in this.LayoutPaths)
            {
                if (layoutPath.IsAttached)
                {
                    layoutPath.CheckLayoutState();
                }
            }
        }

        private static void RemovePathLayoutProperties(IPathLayoutItem pathLayoutItem, bool isArranged = false)
        {
            if (pathLayoutItem == null)
            {
                return;
            }
            pathLayoutItem.Update(new PathLayoutData
            {
                LayoutPathIndex = 0,
                GlobalIndex = 0,
                LocalIndex = 0,
                NormalAngle = 0.0,
                OrientationAngle = 0.0,
                LocalOffset = 0.0,
                GlobalOffset = 0.0,
                IsArranged = isArranged
            });
        }

        private const double SmoothNormalRange = 10.0;

        private static readonly Rect ZeroRect = new Rect(0.0, 0.0, 0.0, 0.0);

        private static readonly Size InfinteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);

        private static readonly Vector Up = new Vector(0.0, -1.0);

        private Point lastPoint;

        private double totalLength;

        private double previousLength;

        private bool shouldLayoutHiddenChildren;

        private bool isListening;

        private bool isUnloaded;

        private int[] indices;

        public static readonly DependencyProperty LayoutPathsProperty = DependencyProperty.Register("LayoutPaths", typeof(LayoutPathCollection), typeof(PathPanel), new PropertyMetadata(null, new PropertyChangedCallback(PathPanel.LayoutPropertyChanged)));

        public static readonly DependencyProperty StartItemIndexProperty = DependencyProperty.Register("StartItemIndex", typeof(double), typeof(PathPanel), new PropertyMetadata(0.0, new PropertyChangedCallback(PathPanel.LayoutPropertyChanged)));

        public static readonly DependencyProperty WrapItemsProperty = DependencyProperty.Register("WrapItems", typeof(bool), typeof(PathPanel), new PropertyMetadata(false, new PropertyChangedCallback(PathPanel.LayoutPropertyChanged)));
    }
}
