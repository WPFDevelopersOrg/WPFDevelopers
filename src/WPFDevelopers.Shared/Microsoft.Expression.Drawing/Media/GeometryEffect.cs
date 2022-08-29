using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.Drawing.Media
{
    [TypeConverter(typeof(GeometryEffectConverter))]
    public abstract class GeometryEffect : Freezable
    {
        public static GeometryEffect GetGeometryEffect(DependencyObject obj) => (GeometryEffect)obj.GetValue(GeometryEffectProperty);
        public static void SetGeometryEffect(DependencyObject obj, GeometryEffect value) => obj.SetValue(GeometryEffectProperty, value);

        private static void OnGeometryEffectChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            GeometryEffect geometryEffect = e.OldValue as GeometryEffect;
            GeometryEffect newValue = e.NewValue as GeometryEffect;
            if (geometryEffect != newValue)
            {
                if (geometryEffect != null && obj.Equals(geometryEffect.Parent))
                {
                    geometryEffect.Detach();
                }
                if (newValue != null)
                {
                    if (newValue.Parent != null)
                    {
                        obj.Dispatcher.BeginInvoke(new Action(delegate ()
                        {
                            GeometryEffect value = newValue.CloneCurrentValue();
                            obj.SetValue(GeometryEffectProperty, value);
                        }), DispatcherPriority.Send, null);
                        return;
                    }
                    newValue.Attach(obj);
                }
            }
        }

        public new GeometryEffect CloneCurrentValue() => (GeometryEffect)base.CloneCurrentValue();

        protected abstract GeometryEffect DeepCopy();

        public abstract bool Equals(GeometryEffect geometryEffect);

        public static GeometryEffect DefaultGeometryEffect
        {
            get
            {
                GeometryEffect result;
                if ((result = defaultGeometryEffect) == null)
                    result = defaultGeometryEffect = new NoGeometryEffect();

                return result;
            }
        }

        public Geometry OutputGeometry => cachedGeometry;
         

        static GeometryEffect()
        {
            DrawingPropertyMetadata.DrawingPropertyChanged += delegate (object sender, DrawingPropertyChangedEventArgs args)
            {
                GeometryEffect geometryEffect = sender as GeometryEffect;
                if (geometryEffect != null && args.Metadata.AffectsRender)
                    geometryEffect.InvalidateGeometry(InvalidateGeometryReasons.PropertyChanged);
            };
        }

        public bool InvalidateGeometry(InvalidateGeometryReasons reasons)
        {
            if (!effectInvalidated)
            {
                effectInvalidated = true;
                if (reasons != InvalidateGeometryReasons.ParentInvalidated)
                    InvalidateParent(Parent);

                return true;
            }
            return false;
        }

        public bool ProcessGeometry(Geometry input)
        {
            bool flag = false;
            if (effectInvalidated)
            {
                flag |= UpdateCachedGeometry(input);
                effectInvalidated = false;
            }
            return flag;
        }

        protected abstract bool UpdateCachedGeometry(Geometry input);

        protected internal DependencyObject Parent { get; private set; }

        protected internal virtual void Detach()
        {
            effectInvalidated = true;
            cachedGeometry = null;
            if (Parent != null)
            {
                InvalidateParent(Parent);
                Parent = null;
            }
        }

        protected internal virtual void Attach(DependencyObject obj)
        {
            if (Parent != null)
            {
                Detach();
            }
            effectInvalidated = true;
            cachedGeometry = null;
            if (InvalidateParent(obj))
            {
                Parent = obj;
            }
        }

        private static bool InvalidateParent(DependencyObject parent)
        {
            IShape shape = parent as IShape;
            if (shape != null)
            {
                shape.InvalidateGeometry(InvalidateGeometryReasons.ChildInvalidated);
                return true;
            }
            GeometryEffect geometryEffect = parent as GeometryEffect;
            if (geometryEffect != null)
            {
                geometryEffect.InvalidateGeometry(InvalidateGeometryReasons.ChildInvalidated);
                return true;
            }
            return false;
        }

        protected override Freezable CreateInstanceCore()
        {
            Type type = base.GetType();
            return (Freezable)Activator.CreateInstance(type);
        }

        public static readonly DependencyProperty GeometryEffectProperty = DependencyProperty.RegisterAttached("GeometryEffect", typeof(GeometryEffect), typeof(GeometryEffect), new DrawingPropertyMetadata(DefaultGeometryEffect, DrawingPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnGeometryEffectChanged)));

        private static GeometryEffect defaultGeometryEffect;

        protected Geometry cachedGeometry;

        private bool effectInvalidated;

        private class NoGeometryEffect : GeometryEffect
        {
            protected override bool UpdateCachedGeometry(Geometry input)
            {
                cachedGeometry = input;
                return false;
            }

            protected override GeometryEffect DeepCopy()
            {
                return new NoGeometryEffect();
            }

            public override bool Equals(GeometryEffect geometryEffect)
            {
                return geometryEffect == null || geometryEffect is NoGeometryEffect;
            }
        }
    }
}
