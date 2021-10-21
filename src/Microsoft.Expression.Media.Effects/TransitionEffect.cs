using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Expression.Media.Effects
{
    public abstract class TransitionEffect : ShaderEffect
    {
        public new TransitionEffect CloneCurrentValue()
        {
            return (TransitionEffect)base.CloneCurrentValue();
        }

        protected abstract TransitionEffect DeepCopy();

        protected TransitionEffect()
        {
            base.UpdateShaderValue(TransitionEffect.InputProperty);
            base.UpdateShaderValue(TransitionEffect.OldImageProperty);
            base.UpdateShaderValue(TransitionEffect.ProgressProperty);
        }

        public Brush Input
        {
            get
            {
                return (Brush)base.GetValue(TransitionEffect.InputProperty);
            }
            set
            {
                base.SetValue(TransitionEffect.InputProperty, value);
            }
        }

        public Brush OldImage
        {
            get
            {
                return (Brush)base.GetValue(TransitionEffect.OldImageProperty);
            }
            set
            {
                base.SetValue(TransitionEffect.OldImageProperty, value);
            }
        }

        public double Progress
        {
            get
            {
                return (double)base.GetValue(TransitionEffect.ProgressProperty);
            }
            set
            {
                base.SetValue(TransitionEffect.ProgressProperty, value);
            }
        }

        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(TransitionEffect), 0, SamplingMode.NearestNeighbor);

        public static readonly DependencyProperty OldImageProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("OldImage", typeof(TransitionEffect), 1, SamplingMode.NearestNeighbor);

        public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register("Progress", typeof(double), typeof(TransitionEffect), new PropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(0)));
    }
}
