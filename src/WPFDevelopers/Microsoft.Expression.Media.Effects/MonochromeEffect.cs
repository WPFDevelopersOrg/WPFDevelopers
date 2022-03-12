using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Expression.Media.Effects
{
    public sealed class MonochromeEffect : ShaderEffect
    {
        public MonochromeEffect()
        {
            base.PixelShader = new PixelShader
            {
                UriSource = Global.MakePackUri("Shaders/Monochrome.ps")
            };
            base.UpdateShaderValue(MonochromeEffect.ColorProperty);
            base.UpdateShaderValue(MonochromeEffect.InputProperty);
        }

        public Color Color
        {
            get
            {
                return (Color)base.GetValue(MonochromeEffect.ColorProperty);
            }
            set
            {
                base.SetValue(MonochromeEffect.ColorProperty, value);
            }
        }

        private Brush Input
        {
            get
            {
                return (Brush)base.GetValue(MonochromeEffect.InputProperty);
            }
            set
            {
                base.SetValue(MonochromeEffect.InputProperty, value);
            }
        }

        private static readonly Color DefaultColor = Colors.White;

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(MonochromeEffect), new PropertyMetadata(MonochromeEffect.DefaultColor, ShaderEffect.PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(MonochromeEffect), 0);
    }
}
