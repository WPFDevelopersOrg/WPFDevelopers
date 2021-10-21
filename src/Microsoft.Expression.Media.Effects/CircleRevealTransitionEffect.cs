using System.Windows;
using System.Windows.Media.Effects;

namespace Microsoft.Expression.Media.Effects
{
    public sealed class CircleRevealTransitionEffect : TransitionEffect
    {
        public CircleRevealTransitionEffect()
        {
            base.UpdateShaderValue(CircleRevealTransitionEffect.FeatherAmountProperty);
            base.UpdateShaderValue(CircleRevealTransitionEffect.ReverseShaderProperty);
            base.PixelShader = new PixelShader
            {
                UriSource = Global.MakePackUri("Shaders/CircleRevealTransitionEffect.ps")
            };
        }

        private static void ReversePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircleRevealTransitionEffect circleRevealTransitionEffect = (CircleRevealTransitionEffect)d;
            bool reverse = circleRevealTransitionEffect.Reverse;
            circleRevealTransitionEffect.ReverseShader = (reverse ? 1.0 : 0.0);
        }

        public double FeatherAmount
        {
            get
            {
                return (double)base.GetValue(CircleRevealTransitionEffect.FeatherAmountProperty);
            }
            set
            {
                base.SetValue(CircleRevealTransitionEffect.FeatherAmountProperty, value);
            }
        }

        public bool Reverse
        {
            get
            {
                return (bool)base.GetValue(CircleRevealTransitionEffect.ReverseProperty);
            }
            set
            {
                base.SetValue(CircleRevealTransitionEffect.ReverseProperty, value);
            }
        }

        protected override TransitionEffect DeepCopy()
        {
            return new CircleRevealTransitionEffect
            {
                Progress = base.Progress,
                Reverse = this.Reverse,
                FeatherAmount = this.FeatherAmount
            };
        }

        private double ReverseShader
        {
            get
            {
                return (double)base.GetValue(CircleRevealTransitionEffect.ReverseShaderProperty);
            }
            set
            {
                base.SetValue(CircleRevealTransitionEffect.ReverseShaderProperty, value);
            }
        }

        private const double DefaultFeather = 0.2;

        private const bool DefaultReverse = false;

        private const double DefaultShaderReverse = 0.0;

        public static readonly DependencyProperty FeatherAmountProperty = DependencyProperty.Register("FeatherAmount", typeof(double), typeof(CircleRevealTransitionEffect), new PropertyMetadata(0.2, ShaderEffect.PixelShaderConstantCallback(1)));

        public static readonly DependencyProperty ReverseProperty = DependencyProperty.Register("Reverse", typeof(bool), typeof(CircleRevealTransitionEffect), new PropertyMetadata(false, new PropertyChangedCallback(CircleRevealTransitionEffect.ReversePropertyChanged)));

        private static readonly DependencyProperty ReverseShaderProperty = DependencyProperty.Register("ReverseShader", typeof(double), typeof(CircleRevealTransitionEffect), new PropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(2)));
    }
}
