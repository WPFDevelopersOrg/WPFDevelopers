using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Expression.Media.Effects
{
    public sealed class SunlightEffect : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(
            "Input", typeof(SunlightEffect), 0);

        public static readonly DependencyProperty ThresholdProperty = DependencyProperty.Register(
            "Threshold", typeof(double), typeof(SunlightEffect),
            new UIPropertyMetadata(1.0d, PixelShaderConstantCallback(0)));

        public SunlightEffect()
        {
            PixelShader = new PixelShader
            {
                UriSource = Global.MakePackUri("Shaders/SunlightEffect.ps"),
            };

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(ThresholdProperty);
        }

        public Brush Input
        {
            get => (Brush)GetValue(InputProperty);
            set => SetValue(InputProperty, value);
        }

        public double Threshold
        {
            get => (double)GetValue(ThresholdProperty);
            set => SetValue(ThresholdProperty, value);
        }
    }
}
