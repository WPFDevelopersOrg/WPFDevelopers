using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Expression.Media.Effects
{
    public sealed class SharpenEffect : ShaderEffect
    {
        public SharpenEffect()
        {
            base.PixelShader = new PixelShader
            {
                UriSource = Global.MakePackUri("Shaders/Sharpen.ps")
            };
            base.UpdateShaderValue(SharpenEffect.InputProperty);
            base.UpdateShaderValue(SharpenEffect.AmountProperty);
            base.UpdateShaderValue(SharpenEffect.HeightProperty);
        }

        public double Amount
        {
            get
            {
                return (double)base.GetValue(SharpenEffect.AmountProperty);
            }
            set
            {
                base.SetValue(SharpenEffect.AmountProperty, value);
            }
        }

        public double Height
        {
            get
            {
                return (double)base.GetValue(SharpenEffect.HeightProperty);
            }
            set
            {
                base.SetValue(SharpenEffect.HeightProperty, value);
            }
        }

        private Brush Input
        {
            get
            {
                return (Brush)base.GetValue(SharpenEffect.InputProperty);
            }
            set
            {
                base.SetValue(SharpenEffect.InputProperty, value);
            }
        }

        private const double DefaultAmount = 2.0;

        private const double DefaultHeight = 0.0005;

        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(SharpenEffect), 0);

        public static readonly DependencyProperty AmountProperty = DependencyProperty.Register("Amount", typeof(double), typeof(SharpenEffect), new PropertyMetadata(2.0, ShaderEffect.PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(SharpenEffect), new PropertyMetadata(0.0005, ShaderEffect.PixelShaderConstantCallback(1)));
    }
}
