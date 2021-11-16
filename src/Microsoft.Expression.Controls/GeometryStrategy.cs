using Microsoft.Expression.Drawing.Core;
using Microsoft.Expression.Media;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Expression.Controls
{
    internal abstract class GeometryStrategy : DependencyObject
    {
        private protected LayoutPath LayoutPath { get; private set; }

        protected abstract PathGeometry UpdateGeometry();

        public abstract bool HasGeometryChanged();

        public IList<PolylineData> Polylines
        {
            get
            {
                if (this.polylineCache == null)
                {
                    this.UpdatePolyline(this.UpdateGeometry());
                }
                return this.polylineCache;
            }
        }

        public abstract IList<GeneralTransform> ComputeTransforms();

        protected GeometryStrategy(LayoutPath layoutPath)
        {
            this.LayoutPath = layoutPath;
        }

        public static GeometryStrategy Create(LayoutPath layoutPath)
        {
            if (layoutPath == null)
            {
                throw new ArgumentNullException("layoutPath");
            }
            if (layoutPath.SourceElement == null)
            {
                throw new InvalidOperationException();
            }
            if (layoutPath.SourceElement is IShape)
            {
                return new IShapeStrategy(layoutPath);
            }
            if (layoutPath.SourceElement is Path)
            {
                return new PathStrategy(layoutPath);
            }
            if (layoutPath.SourceElement is Rectangle)
            {
                return new RectangleStrategy(layoutPath);
            }
            if (layoutPath.SourceElement is Ellipse)
            {
                return new EllipseStrategy(layoutPath);
            }
            if (layoutPath.SourceElement is Line)
            {
                return new LineStrategy(layoutPath);
            }
            if (layoutPath.SourceElement is Polygon)
            {
                return new PolygonStrategy(layoutPath);
            }
            if (layoutPath.SourceElement is Polyline)
            {
                return new PolylineStrategy(layoutPath);
            }
            if (layoutPath.SourceElement is Shape)
            {
                return new ShapeStrategy(layoutPath);
            }
            return new FrameworkElementStrategy(layoutPath);
        }

        public void InvalidatePolylineCache()
        {
            this.polylineCache = null;
        }

        public virtual void Unhook()
        {
            this.LayoutPath = null;
        }

        protected static void LayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GeometryStrategy geometryStrategy = d as GeometryStrategy;
            if (geometryStrategy == null || geometryStrategy.LayoutPath == null || !geometryStrategy.LayoutPath.IsAttached)
            {
                return;
            }
            geometryStrategy.LayoutPath.IsLayoutDirty = true;
        }

        protected void SetListenerBinding(DependencyProperty targetProperty, string sourceProperty)
        {
            BindingOperations.SetBinding(this, targetProperty, new Binding(sourceProperty)
            {
                Source = this.LayoutPath.SourceElement,
                Mode = BindingMode.OneWay
            });
        }

        private void UpdatePolyline(PathGeometry pathGeometry)
        {
            if (pathGeometry == null)
            {
                this.polylineCache = new List<PolylineData>();
                return;
            }
            List<PolylineData> list = new List<PolylineData>();
            foreach (PathFigure figure in pathGeometry.Figures)
            {
                List<Point> list2 = new List<Point>();
                bool removeRepeat = true;
                PathFigureHelper.FlattenFigure(figure, list2, 0.1, removeRepeat);
                if (list2.Count > 0)
                {
                    if (list2.Count == 1)
                    {
                        list2.Add(list2[0]);
                    }
                    list.Add(new PolylineData(list2));
                }
            }
            this.polylineCache = list;
        }

        private const double FlatteningTolerance = 0.1;

        private List<PolylineData> polylineCache;
    }
}
