using System;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.Controls
{
    public sealed class PathListBoxItem : ListBoxItem, IPathLayoutItem
    {
        public event EventHandler<PathLayoutUpdatedEventArgs> PathLayoutUpdated;

        public int LayoutPathIndex
        {
            get
            {
                return (int)base.GetValue(PathListBoxItem.LayoutPathIndexProperty);
            }
            internal set
            {
                base.SetValue(PathListBoxItem.LayoutPathIndexProperty, value);
            }
        }

        public int GlobalIndex
        {
            get
            {
                return (int)base.GetValue(PathListBoxItem.GlobalIndexProperty);
            }
            internal set
            {
                base.SetValue(PathListBoxItem.GlobalIndexProperty, value);
            }
        }

        public int LocalIndex
        {
            get
            {
                return (int)base.GetValue(PathListBoxItem.LocalIndexProperty);
            }
            internal set
            {
                base.SetValue(PathListBoxItem.LocalIndexProperty, value);
            }
        }

        public double GlobalOffset
        {
            get
            {
                return (double)base.GetValue(PathListBoxItem.GlobalOffsetProperty);
            }
            internal set
            {
                base.SetValue(PathListBoxItem.GlobalOffsetProperty, value);
            }
        }

        public double LocalOffset
        {
            get
            {
                return (double)base.GetValue(PathListBoxItem.LocalOffsetProperty);
            }
            internal set
            {
                base.SetValue(PathListBoxItem.LocalOffsetProperty, value);
            }
        }

        public double NormalAngle
        {
            get
            {
                return (double)base.GetValue(PathListBoxItem.NormalAngleProperty);
            }
            internal set
            {
                base.SetValue(PathListBoxItem.NormalAngleProperty, value);
            }
        }

        public double OrientationAngle
        {
            get
            {
                return (double)base.GetValue(PathListBoxItem.OrientationAngleProperty);
            }
            internal set
            {
                base.SetValue(PathListBoxItem.OrientationAngleProperty, value);
            }
        }

        public bool IsArranged
        {
            get
            {
                return (bool)base.GetValue(PathListBoxItem.IsArrangedProperty);
            }
            internal set
            {
                base.SetValue(PathListBoxItem.IsArrangedProperty, value);
            }
        }

        public PathListBoxItem()
        {
            base.DefaultStyleKey = typeof(PathListBoxItem);
        }

        public void Update(PathLayoutData data)
        {
            ChangedPathLayoutProperties changedPathLayoutProperties = ChangedPathLayoutProperties.None;
            if (this.LayoutPathIndex != data.LayoutPathIndex)
            {
                changedPathLayoutProperties |= ChangedPathLayoutProperties.LayoutPathIndex;
                this.LayoutPathIndex = data.LayoutPathIndex;
            }
            if (this.GlobalIndex != data.GlobalIndex)
            {
                changedPathLayoutProperties |= ChangedPathLayoutProperties.GlobalIndex;
                this.GlobalIndex = data.GlobalIndex;
            }
            if (this.LocalIndex != data.LocalIndex)
            {
                changedPathLayoutProperties |= ChangedPathLayoutProperties.LocalIndex;
                this.LocalIndex = data.LocalIndex;
            }
            if (this.GlobalOffset != data.GlobalOffset)
            {
                changedPathLayoutProperties |= ChangedPathLayoutProperties.GlobalOffset;
                this.GlobalOffset = data.GlobalOffset;
            }
            if (this.LocalOffset != data.LocalOffset)
            {
                changedPathLayoutProperties |= ChangedPathLayoutProperties.LocalOffset;
                this.LocalOffset = data.LocalOffset;
            }
            if (this.NormalAngle != data.NormalAngle)
            {
                changedPathLayoutProperties |= ChangedPathLayoutProperties.NormalAngle;
                this.NormalAngle = data.NormalAngle;
            }
            if (this.OrientationAngle != data.OrientationAngle)
            {
                changedPathLayoutProperties |= ChangedPathLayoutProperties.OrientationAngle;
                this.OrientationAngle = data.OrientationAngle;
            }
            if (this.IsArranged != data.IsArranged)
            {
                changedPathLayoutProperties |= ChangedPathLayoutProperties.IsArranged;
                this.IsArranged = data.IsArranged;
            }
            this.OnPathLayoutUpdated(new PathLayoutUpdatedEventArgs(changedPathLayoutProperties));
        }

        internal void OnPathLayoutUpdated(PathLayoutUpdatedEventArgs pathLayoutArgs)
        {
            if (this.PathLayoutUpdated != null)
            {
                this.PathLayoutUpdated(this, pathLayoutArgs);
            }
        }

        public static readonly DependencyProperty LayoutPathIndexProperty = DependencyProperty.Register("LayoutPathIndex", typeof(int), typeof(PathListBoxItem), new PropertyMetadata(0));

        public static readonly DependencyProperty GlobalIndexProperty = DependencyProperty.Register("GlobalIndex", typeof(int), typeof(PathListBoxItem), new PropertyMetadata(0));

        public static readonly DependencyProperty LocalIndexProperty = DependencyProperty.Register("LocalIndex", typeof(int), typeof(PathListBoxItem), new PropertyMetadata(0));

        public static readonly DependencyProperty GlobalOffsetProperty = DependencyProperty.Register("GlobalOffset", typeof(double), typeof(PathListBoxItem), new PropertyMetadata(0.0));

        public static readonly DependencyProperty LocalOffsetProperty = DependencyProperty.Register("LocalOffset", typeof(double), typeof(PathListBoxItem), new PropertyMetadata(0.0));

        public static readonly DependencyProperty NormalAngleProperty = DependencyProperty.Register("NormalAngle", typeof(double), typeof(PathListBoxItem), new PropertyMetadata(0.0));

        public static readonly DependencyProperty OrientationAngleProperty = DependencyProperty.Register("OrientationAngle", typeof(double), typeof(PathListBoxItem), new PropertyMetadata(0.0));

        public static readonly DependencyProperty IsArrangedProperty = DependencyProperty.Register("IsArranged", typeof(bool), typeof(PathListBoxItem), new PropertyMetadata(false));
    }
}
