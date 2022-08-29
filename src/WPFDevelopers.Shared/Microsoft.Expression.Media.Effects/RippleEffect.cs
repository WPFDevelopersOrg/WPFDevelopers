using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Expression.Media.Effects
{
    public sealed class RippleEffect : ShaderEffect
    {
        public RippleEffect()
        {
            base.PixelShader = new PixelShader
            {
                UriSource = Global.MakePackUri("Shaders/Ripple.ps")
            };
            base.UpdateShaderValue(RippleEffect.CenterProperty);
            base.UpdateShaderValue(RippleEffect.MagnitudeProperty);
            base.UpdateShaderValue(RippleEffect.PhaseProperty);
            base.UpdateShaderValue(RippleEffect.FrequencyProperty);
            base.UpdateShaderValue(RippleEffect.InputProperty);
        }

        public Point Center
        {
            get
            {
                return (Point)base.GetValue(RippleEffect.CenterProperty);
            }
            set
            {
                base.SetValue(RippleEffect.CenterProperty, value);
            }
        }

        public double Magnitude
        {
            get
            {
                return (double)base.GetValue(RippleEffect.MagnitudeProperty);
            }
            set
            {
                base.SetValue(RippleEffect.MagnitudeProperty, value);
            }
        }

        public double Frequency
        {
            get
            {
                return (double)base.GetValue(RippleEffect.FrequencyProperty);
            }
            set
            {
                base.SetValue(RippleEffect.FrequencyProperty, value);
            }
        }

        public double Phase
        {
            get
            {
                return (double)base.GetValue(RippleEffect.PhaseProperty);
            }
            set
            {
                base.SetValue(RippleEffect.PhaseProperty, value);
            }
        }

        private Brush Input
        {
            get
            {
                return (Brush)base.GetValue(RippleEffect.InputProperty);
            }
            set
            {
                base.SetValue(RippleEffect.InputProperty, value);
            }
        }

        private const double DefaultMagnitude = 0.1;

        private const double DefaultPhase = 10.0;

        private const double DefaultFrequency = 40.0;

        private static readonly Point DefaultCenter = new Point(0.5, 0.5);

        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register("Center", typeof(Point), typeof(RippleEffect), new PropertyMetadata(RippleEffect.DefaultCenter, ShaderEffect.PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty MagnitudeProperty = DependencyProperty.Register("Magnitude", typeof(double), typeof(RippleEffect), new PropertyMetadata(0.1, ShaderEffect.PixelShaderConstantCallback(1)));

        public static readonly DependencyProperty FrequencyProperty = DependencyProperty.Register("Frequency", typeof(double), typeof(RippleEffect), new PropertyMetadata(40.0, ShaderEffect.PixelShaderConstantCallback(2)));

        public static readonly DependencyProperty PhaseProperty = DependencyProperty.Register("Phase", typeof(double), typeof(RippleEffect), new PropertyMetadata(10.0, ShaderEffect.PixelShaderConstantCallback(3)));

        private static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(RippleEffect), 0);
    }
}
