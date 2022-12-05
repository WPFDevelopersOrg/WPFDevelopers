using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace WPFDevelopers
{
    public class GrayscaleEffect : ShaderEffect
    {
        /// <summary>
        /// Identifies the Input property.
        /// </summary>
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(GrayscaleEffect), 0);

        /// <summary>
        /// Identifies the Factor property.
        /// </summary>
        public static readonly DependencyProperty FactorProperty = DependencyProperty.Register("Factor", typeof(double), typeof(GrayscaleEffect), new UIPropertyMetadata(0D, PixelShaderConstantCallback(0)));

        /// <summary>
        /// Identifies the Brightness property.
        /// </summary>
        public static readonly DependencyProperty BrightnessProperty = DependencyProperty.Register("Brightness", typeof(double), typeof(GrayscaleEffect), new UIPropertyMetadata(0D, PixelShaderConstantCallback(1)));

        /// <summary>
        /// Creates a new instance of the <see cref="GrayscaleEffect"/> class.
        /// </summary>
        public GrayscaleEffect()
        {
            var pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri("WPFDevelopers;component/Effects/GrayscaleEffect.ps", UriKind.Relative);

            PixelShader = pixelShader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(FactorProperty);
            UpdateShaderValue(BrightnessProperty);
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> used as input for the shader.
        /// </summary>
        public Brush Input
        {
            get => ((Brush)(GetValue(InputProperty)));
            set => SetValue(InputProperty, value);
        }

        /// <summary>
        /// Gets or sets the factor used in the shader.
        /// </summary>
        public double Factor
        {
            get => ((double)(GetValue(FactorProperty)));
            set => SetValue(FactorProperty, value);
        }

        /// <summary>
        /// Gets or sets the brightness of the effect.
        /// </summary>
        public double Brightness
        {
            get => ((double)(GetValue(BrightnessProperty)));
            set => SetValue(BrightnessProperty, value);
        }
    }
}
