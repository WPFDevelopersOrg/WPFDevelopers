using Microsoft.Expression.Drawing.Core;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Expression.Controls
{
    internal class PathStrategy : ShapeStrategy
    {
        private Geometry DataListener
        {
            get
            {
                return (Geometry)base.GetValue(PathStrategy.DataListenerProperty);
            }
            set
            {
                base.SetValue(PathStrategy.DataListenerProperty, value);
            }
        }

        public PathStrategy(LayoutPath layoutPath) : base(layoutPath)
        {
            this.sourcePath = (Path)layoutPath.SourceElement;
            base.SetListenerBinding(PathStrategy.DataListenerProperty, "Data");
        }

        public override bool HasGeometryChanged()
        {
            Geometry currentGeometry = this.GetCurrentGeometry();
            return !GeometryHelper.GeometryEquals(this.oldGeometry, currentGeometry);
        }

        protected override PathGeometry UpdateGeometry()
        {
            Geometry currentGeometry = this.GetCurrentGeometry();
            if (currentGeometry == null)
            {
                this.oldGeometry = null;
                return null;
            }
            this.oldGeometry = currentGeometry.CloneCurrentValue();
            return this.oldGeometry.AsPathGeometry();
        }

        public override IList<GeneralTransform> ComputeTransforms()
        {
            IList<GeneralTransform> list = base.ComputeTransforms() ?? new List<GeneralTransform>();
            Geometry currentGeometry = this.GetCurrentGeometry();
            if (currentGeometry != null)
            {
                list.Add(currentGeometry.Transform);
            }
            return list;
        }

        private Geometry GetCurrentGeometry()
        {
            if (this.sourcePath == null || this.sourcePath.Data == null)
            {
                return null;
            }
            return this.sourcePath.Data;
        }

        private Path sourcePath;

        private static readonly DependencyProperty DataListenerProperty = DependencyProperty.Register("DataListener", typeof(Geometry), typeof(PathStrategy), new PropertyMetadata(new PropertyChangedCallback(GeometryStrategy.LayoutPropertyChanged)));

        private Geometry oldGeometry;
    }
}
