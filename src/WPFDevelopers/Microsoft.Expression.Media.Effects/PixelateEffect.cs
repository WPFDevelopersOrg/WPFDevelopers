using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Expression.Media.Effects
{
    public sealed class PixelateEffect : ShaderEffect
    {
        public PixelateEffect()
        {
            base.PixelShader = new PixelShader
            {
                UriSource = Global.MakePackUri("Shaders/Pixelate.ps")
            };
            base.UpdateShaderValue(PixelateEffect.InputProperty);
            base.UpdateShaderValue(PixelateEffect.PixelationProperty);
            base.DdxUvDdyUvRegisterIndex = 1;
        }

        public double Pixelation
        {
            get
            {
                return (double)base.GetValue(PixelateEffect.PixelationProperty);
            }
            set
            {
                base.SetValue(PixelateEffect.PixelationProperty, value);
            }
        }

        private Brush Input
        {
            get
            {
                return (Brush)base.GetValue(PixelateEffect.InputProperty);
            }
            set
            {
                base.SetValue(PixelateEffect.InputProperty, value);
            }
        }

        private const double DefaultPixelAmount = 0.75;

        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(PixelateEffect), 0);

        public static readonly DependencyProperty PixelationProperty = DependencyProperty.Register("Pixelation", typeof(double), typeof(PixelateEffect), new PropertyMetadata(0.75, ShaderEffect.PixelShaderConstantCallback(0)));
    }
}
