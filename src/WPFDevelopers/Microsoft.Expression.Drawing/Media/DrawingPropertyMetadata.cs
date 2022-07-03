using System;
using System.Windows;

namespace Microsoft.Expression.Drawing.Media
{
    internal class DrawingPropertyMetadata : FrameworkPropertyMetadata
    {
        public DrawingPropertyMetadata(object defaultValue) : this(defaultValue, DrawingPropertyMetadataOptions.None, null)
        {
        }

        public DrawingPropertyMetadata(PropertyChangedCallback propertyChangedCallback) : this(DependencyProperty.UnsetValue, DrawingPropertyMetadataOptions.None, propertyChangedCallback)
        {
        }

        public DrawingPropertyMetadata(object defaultValue, DrawingPropertyMetadataOptions options) : this(defaultValue, options, null)
        {
        }

        public DrawingPropertyMetadata(object defaultValue, DrawingPropertyMetadataOptions options, PropertyChangedCallback propertyChangedCallback) : base(defaultValue, (FrameworkPropertyMetadataOptions)options, AttachCallback(defaultValue, options, propertyChangedCallback))
        {
            this.options = options;
        }

        public static event EventHandler<DrawingPropertyChangedEventArgs> DrawingPropertyChanged;

        private DrawingPropertyMetadata(DrawingPropertyMetadataOptions options, object defaultValue) : base(defaultValue, (FrameworkPropertyMetadataOptions)options)
        {
            this.options = options;
        }

        private static PropertyChangedCallback AttachCallback(object defaultValue, DrawingPropertyMetadataOptions options, PropertyChangedCallback propertyChangedCallback)
        {
            return new PropertyChangedCallback(new DrawingPropertyMetadata(options, defaultValue)
            {
                propertyChangedCallback = propertyChangedCallback
            }.InternalCallback);
        }

        private void InternalCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (DrawingPropertyChanged != null)
            {
                DrawingPropertyChanged(sender, new DrawingPropertyChangedEventArgs
                {
                    Metadata = this,
                    IsAnimated = DependencyPropertyHelper.GetValueSource(sender, e.Property).IsAnimated
                });
            }

            propertyChangedCallback?.Invoke(sender, e);
        }

        static DrawingPropertyMetadata()
        {
            DrawingPropertyChanged += delegate (object sender, DrawingPropertyChangedEventArgs args)
            {
                IShape shape = sender as IShape;
                if (shape != null && args.Metadata.AffectsRender)
                {
                    InvalidateGeometryReasons invalidateGeometryReasons = InvalidateGeometryReasons.PropertyChanged;
                    if (args.IsAnimated)
                    {
                        invalidateGeometryReasons |= InvalidateGeometryReasons.IsAnimated;
                    }
                    shape.InvalidateGeometry(invalidateGeometryReasons);
                }
            };
        }

        private DrawingPropertyMetadataOptions options;

        private PropertyChangedCallback propertyChangedCallback;
    }
}
