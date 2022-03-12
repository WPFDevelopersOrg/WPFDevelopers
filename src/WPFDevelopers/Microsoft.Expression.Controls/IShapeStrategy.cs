using Microsoft.Expression.Drawing.Core;
using Microsoft.Expression.Media;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Expression.Controls
{
    internal class IShapeStrategy : FrameworkElementStrategy
    {
        public IShapeStrategy(LayoutPath layoutPath) : base(layoutPath)
        {
            this.sourceIShape = (IShape)layoutPath.SourceElement;
            this.sourceIShape.RenderedGeometryChanged += this.RenderedGeometryChanged;
            this.sourceShape = (layoutPath.SourceElement as Shape);
        }

        public override void Unhook()
        {
            this.sourceIShape.RenderedGeometryChanged -= this.RenderedGeometryChanged;
            base.Unhook();
        }

        public override IList<GeneralTransform> ComputeTransforms()
        {
            IList<GeneralTransform> list = base.ComputeTransforms() ?? new List<GeneralTransform>();
            if (this.sourceShape != null)
            {
                list.Add(this.sourceShape.GeometryTransform);
            }
            return list;
        }

        protected override PathGeometry UpdateGeometry()
        {
            return this.sourceIShape.RenderedGeometry.AsPathGeometry();
        }

        private void RenderedGeometryChanged(object sender, EventArgs e)
        {
            base.LayoutPath.IsLayoutDirty = true;
        }

        private IShape sourceIShape;

        private Shape sourceShape;
    }
}
