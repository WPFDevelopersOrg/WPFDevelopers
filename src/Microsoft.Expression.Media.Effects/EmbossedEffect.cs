using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Expression.Media.Effects
{
    public sealed class EmbossedEffect : ShaderEffect
    {
        public EmbossedEffect()
        {
            base.PixelShader = new PixelShader
            {
                UriSource = Global.MakePackUri("Shaders/Embossed.ps")
            };
            base.UpdateShaderValue(EmbossedEffect.InputProperty);
            base.UpdateShaderValue(EmbossedEffect.AmountProperty);
            base.UpdateShaderValue(EmbossedEffect.HeightProperty);
            base.UpdateShaderValue(EmbossedEffect.ColorProperty);
        }

        public double Amount
        {
            get
            {
                return (double)base.GetValue(EmbossedEffect.AmountProperty);
            }
            set
            {
                base.SetValue(EmbossedEffect.AmountProperty, value);
            }
        }

        public double Height
        {
            get
            {
                return (double)base.GetValue(EmbossedEffect.HeightProperty);
            }
            set
            {
                base.SetValue(EmbossedEffect.HeightProperty, value);
            }
        }

        public Color Color
        {
            get
            {
                return (Color)base.GetValue(EmbossedEffect.ColorProperty);
            }
            set
            {
                base.SetValue(EmbossedEffect.ColorProperty, value);
            }
        }

        private Brush Input
        {
            get
            {
                return (Brush)base.GetValue(EmbossedEffect.InputProperty);
            }
            set
            {
                base.SetValue(EmbossedEffect.InputProperty, value);
            }
        }

        private const double DefaultAmount = 3.0;

        private const double DefaultHeight = 0.001;

        private static readonly Color DefaultColor = Colors.Gray;

        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(EmbossedEffect), 0, SamplingMode.Bilinear);

        public static readonly DependencyProperty AmountProperty = DependencyProperty.Register("Amount", typeof(double), typeof(EmbossedEffect), new PropertyMetadata(3.0, ShaderEffect.PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(EmbossedEffect), new PropertyMetadata(0.001, ShaderEffect.PixelShaderConstantCallback(1)));

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(EmbossedEffect), new PropertyMetadata(EmbossedEffect.DefaultColor, ShaderEffect.PixelShaderConstantCallback(2)));
    }
}
