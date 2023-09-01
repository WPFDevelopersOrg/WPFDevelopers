using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace WPFDevelopers.Samples.Controls
{
    public class TransformThumb : ContentControl
    {
        static TransformThumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TransformThumb), new FrameworkPropertyMetadata(typeof(TransformThumb)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            IsVisibleChanged += TransformThumb_IsVisibleChanged;
            CreateAdorner();
        }
        void CreateAdorner()
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(this);
            if (adornerLayer != null)
            {
                var adorner = new ElementAdorner(this);
                adornerLayer.Add(adorner);
            }
        }
        private void TransformThumb_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool isVisible)
            {
                if(isVisible)
                {
                    CreateAdorner();
                }
                
            }
        }
    }
}
