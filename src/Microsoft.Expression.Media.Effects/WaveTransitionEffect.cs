using System.Windows;
using System.Windows.Media.Effects;

namespace Microsoft.Expression.Media.Effects
{
    public sealed class WaveTransitionEffect : TransitionEffect
    {
        public double Magnitude
        {
            get
            {
                return (double)base.GetValue(WaveTransitionEffect.MagnitudeProperty);
            }
            set
            {
                base.SetValue(WaveTransitionEffect.MagnitudeProperty, value);
            }
        }

        public double Phase
        {
            get
            {
                return (double)base.GetValue(WaveTransitionEffect.PhaseProperty);
            }
            set
            {
                base.SetValue(WaveTransitionEffect.PhaseProperty, value);
            }
        }

        public double Frequency
        {
            get
            {
                return (double)base.GetValue(WaveTransitionEffect.FrequencyProperty);
            }
            set
            {
                base.SetValue(WaveTransitionEffect.FrequencyProperty, value);
            }
        }

        public WaveTransitionEffect()
        {
            base.PixelShader = new PixelShader
            {
                UriSource = Global.MakePackUri("Shaders/WaveTransitionEffect.ps")
            };
            base.UpdateShaderValue(WaveTransitionEffect.MagnitudeProperty);
            base.UpdateShaderValue(WaveTransitionEffect.PhaseProperty);
            base.UpdateShaderValue(WaveTransitionEffect.FrequencyProperty);
        }

        protected override TransitionEffect DeepCopy()
        {
            return new WaveTransitionEffect
            {
                Magnitude = this.Magnitude,
                Phase = this.Phase,
                Frequency = this.Frequency
            };
        }

        private const double DefaultMagnitude = 0.1;

        private const double DefaultPhase = 14.0;

        private const double DefaultFrequency = 20.0;

        public static readonly DependencyProperty MagnitudeProperty = DependencyProperty.Register("Magnitude", typeof(double), typeof(WaveTransitionEffect), new PropertyMetadata(0.1, ShaderEffect.PixelShaderConstantCallback(1)));

        public static readonly DependencyProperty PhaseProperty = DependencyProperty.Register("Phase", typeof(double), typeof(WaveTransitionEffect), new PropertyMetadata(14.0, ShaderEffect.PixelShaderConstantCallback(2)));

        public static readonly DependencyProperty FrequencyProperty = DependencyProperty.Register("Frequency", typeof(double), typeof(WaveTransitionEffect), new PropertyMetadata(20.0, ShaderEffect.PixelShaderConstantCallback(3)));
    }
}
