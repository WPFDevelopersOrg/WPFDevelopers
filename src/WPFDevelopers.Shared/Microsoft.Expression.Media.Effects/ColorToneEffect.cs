using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Expression.Media.Effects
{
    public sealed class ColorToneEffect : ShaderEffect
    {
        public ColorToneEffect()
        {
            base.PixelShader = new PixelShader
            {
                UriSource = Global.MakePackUri("Shaders/ColorTone.ps")
            };
            base.UpdateShaderValue(ColorToneEffect.InputProperty);
            base.UpdateShaderValue(ColorToneEffect.DesaturationProperty);
            base.UpdateShaderValue(ColorToneEffect.ToneAmountProperty);
            base.UpdateShaderValue(ColorToneEffect.LightColorProperty);
            base.UpdateShaderValue(ColorToneEffect.DarkColorProperty);
        }

        public double Desaturation
        {
            get
            {
                return (double)base.GetValue(ColorToneEffect.DesaturationProperty);
            }
            set
            {
                base.SetValue(ColorToneEffect.DesaturationProperty, value);
            }
        }

        public double ToneAmount
        {
            get
            {
                return (double)base.GetValue(ColorToneEffect.ToneAmountProperty);
            }
            set
            {
                base.SetValue(ColorToneEffect.ToneAmountProperty, value);
            }
        }

        public Color LightColor
        {
            get
            {
                return (Color)base.GetValue(ColorToneEffect.LightColorProperty);
            }
            set
            {
                base.SetValue(ColorToneEffect.LightColorProperty, value);
            }
        }

        public Color DarkColor
        {
            get
            {
                return (Color)base.GetValue(ColorToneEffect.DarkColorProperty);
            }
            set
            {
                base.SetValue(ColorToneEffect.DarkColorProperty, value);
            }
        }

        private Brush Input
        {
            get
            {
                return (Brush)base.GetValue(ColorToneEffect.InputProperty);
            }
            set
            {
                base.SetValue(ColorToneEffect.InputProperty, value);
            }
        }

        private const double DefaultDesaturation = 0.5;

        private const double DefaultToneAmount = 0.5;

        private static readonly Color DefaultLightColor = Color.FromArgb(byte.MaxValue, byte.MaxValue, 229, 128);

        private static readonly Color DefaultDarkColor = Color.FromArgb(byte.MaxValue, 51, 128, 0);

        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(ColorToneEffect), 0);

        public static readonly DependencyProperty DesaturationProperty = DependencyProperty.Register("Desaturation", typeof(double), typeof(ColorToneEffect), new PropertyMetadata(0.5, ShaderEffect.PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty ToneAmountProperty = DependencyProperty.Register("ToneAmount", typeof(double), typeof(ColorToneEffect), new PropertyMetadata(0.5, ShaderEffect.PixelShaderConstantCallback(1)));

        public static readonly DependencyProperty LightColorProperty = DependencyProperty.Register("LightColor", typeof(Color), typeof(ColorToneEffect), new PropertyMetadata(ColorToneEffect.DefaultLightColor, ShaderEffect.PixelShaderConstantCallback(2)));

        public static readonly DependencyProperty DarkColorProperty = DependencyProperty.Register("DarkColor", typeof(Color), typeof(ColorToneEffect), new PropertyMetadata(ColorToneEffect.DefaultDarkColor, ShaderEffect.PixelShaderConstantCallback(3)));
    }
}
