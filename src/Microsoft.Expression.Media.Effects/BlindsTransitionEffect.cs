using System.Windows;
using System.Windows.Media.Effects;

namespace Microsoft.Expression.Media.Effects
{
    public sealed class BlindsTransitionEffect : TransitionEffect
    {
        public BlindsTransitionEffect()
        {
            base.PixelShader = new PixelShader
            {
                UriSource = Global.MakePackUri("Shaders/BlindsTransitionEffect.ps")
            };
            base.UpdateShaderValue(BlindsTransitionEffect.ShaderOrientationProperty);
            base.UpdateShaderValue(BlindsTransitionEffect.CountProperty);
        }

        public BlindOrientation Orientation
        {
            get
            {
                return (BlindOrientation)base.GetValue(BlindsTransitionEffect.OrientationProperty);
            }
            set
            {
                base.SetValue(BlindsTransitionEffect.OrientationProperty, value);
            }
        }

        public double Count
        {
            get
            {
                return (double)base.GetValue(BlindsTransitionEffect.CountProperty);
            }
            set
            {
                base.SetValue(BlindsTransitionEffect.CountProperty, value);
            }
        }

        protected override TransitionEffect DeepCopy()
        {
            return new BlindsTransitionEffect
            {
                Count = this.Count,
                Orientation = this.Orientation,
                ShaderOrientation = this.ShaderOrientation
            };
        }

        private double ShaderOrientation
        {
            get
            {
                return (double)base.GetValue(BlindsTransitionEffect.ShaderOrientationProperty);
            }
            set
            {
                base.SetValue(BlindsTransitionEffect.ShaderOrientationProperty, value);
            }
        }

        private static void OrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BlindsTransitionEffect blindsTransitionEffect = (BlindsTransitionEffect)d;
            BlindOrientation blindOrientation = (BlindOrientation)e.NewValue;
            blindsTransitionEffect.ShaderOrientation = ((blindOrientation == BlindOrientation.Horizontal) ? 0.0 : 1.0);
        }

        private const BlindOrientation DefaultOrientation = BlindOrientation.Horizontal;

        private const double DefaultCount = 5.0;

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(BlindOrientation), typeof(BlindsTransitionEffect), new PropertyMetadata(BlindOrientation.Horizontal, new PropertyChangedCallback(BlindsTransitionEffect.OrientationPropertyChanged)));

        public static readonly DependencyProperty CountProperty = DependencyProperty.Register("Count", typeof(double), typeof(BlindsTransitionEffect), new PropertyMetadata(5.0, ShaderEffect.PixelShaderConstantCallback(2)));

        private static readonly DependencyProperty ShaderOrientationProperty = DependencyProperty.Register("ShaderOrientation", typeof(double), typeof(BlindsTransitionEffect), new PropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(1)));
    }
}
