using Microsoft.Expression.Drawing.Core;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Controls
{
    internal class FrameworkElementStrategy : GeometryStrategy
    {
        private Visibility VisibilityListener
        {
            get
            {
                return (Visibility)base.GetValue(FrameworkElementStrategy.VisibilityListenerProperty);
            }
            set
            {
                base.SetValue(FrameworkElementStrategy.VisibilityListenerProperty, value);
            }
        }

        public FrameworkElementStrategy(LayoutPath layoutPath) : base(layoutPath)
        {
            this.sourceElement = layoutPath.SourceElement;
            this.sourceElement.SizeChanged += this.SourceElement_SizeChanged;
            base.SetListenerBinding(FrameworkElementStrategy.VisibilityListenerProperty, "Visibility");
        }

        public override bool HasGeometryChanged()
        {
            return false;
        }

        protected Size Size
        {
            get
            {
                if (!this.size.HasValidArea())
                {
                    this.size = new Size(this.sourceElement.ActualWidth, this.sourceElement.ActualHeight);
                }
                return this.size;
            }
        }

        protected override PathGeometry UpdateGeometry()
        {
            return new RectangleGeometry
            {
                Rect = new Rect(default(Point), this.Size)
            }.AsPathGeometry();
        }

        public override void Unhook()
        {
            this.sourceElement.SizeChanged -= this.SourceElement_SizeChanged;
            base.Unhook();
        }

        public override IList<GeneralTransform> ComputeTransforms()
        {
            return null;
        }

        private void SourceElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.size = e.NewSize;
            base.LayoutPath.IsLayoutDirty = true;
        }

        private FrameworkElement sourceElement;

        private Size size;

        private static readonly DependencyProperty VisibilityListenerProperty = DependencyProperty.Register("Visibility", typeof(Visibility), typeof(FrameworkElementStrategy), new PropertyMetadata(new PropertyChangedCallback(GeometryStrategy.LayoutPropertyChanged)));
    }
}
