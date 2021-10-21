using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Expression.Media.Effects
{
    public sealed class BloomEffect : ShaderEffect
    {
        public BloomEffect()
        {
            base.PixelShader = new PixelShader
            {
                UriSource = Global.MakePackUri("Shaders/Bloom.ps")
            };
            base.UpdateShaderValue(BloomEffect.InputProperty);
            base.UpdateShaderValue(BloomEffect.BaseBloomIntensityProperty);
            base.UpdateShaderValue(BloomEffect.BaseBloomSaturationProperty);
            base.UpdateShaderValue(BloomEffect.ThresholdProperty);
        }

        public double Threshold
        {
            get
            {
                return (double)base.GetValue(BloomEffect.ThresholdProperty);
            }
            set
            {
                base.SetValue(BloomEffect.ThresholdProperty, value);
            }
        }

        public double BaseIntensity
        {
            get
            {
                return (double)base.GetValue(BloomEffect.BaseIntensityProperty);
            }
            set
            {
                base.SetValue(BloomEffect.BaseIntensityProperty, value);
            }
        }

        public double BloomIntensity
        {
            get
            {
                return (double)base.GetValue(BloomEffect.BloomIntensityProperty);
            }
            set
            {
                base.SetValue(BloomEffect.BloomIntensityProperty, value);
            }
        }

        public double BaseSaturation
        {
            get
            {
                return (double)base.GetValue(BloomEffect.BaseSaturationProperty);
            }
            set
            {
                base.SetValue(BloomEffect.BaseSaturationProperty, value);
            }
        }

        public double BloomSaturation
        {
            get
            {
                return (double)base.GetValue(BloomEffect.BloomSaturationProperty);
            }
            set
            {
                base.SetValue(BloomEffect.BloomSaturationProperty, value);
            }
        }

        private Brush Input
        {
            get
            {
                return (Brush)base.GetValue(BloomEffect.InputProperty);
            }
            set
            {
                base.SetValue(BloomEffect.InputProperty, value);
            }
        }

        private Point BaseBloomIntensity
        {
            get
            {
                return (Point)base.GetValue(BloomEffect.BaseBloomIntensityProperty);
            }
            set
            {
                base.SetValue(BloomEffect.BaseBloomIntensityProperty, value);
            }
        }

        private Point BaseBloomSaturation
        {
            get
            {
                return (Point)base.GetValue(BloomEffect.BaseBloomSaturationProperty);
            }
            set
            {
                base.SetValue(BloomEffect.BaseBloomSaturationProperty, value);
            }
        }

        private static void BaseIntensityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BloomEffect bloomEffect = (BloomEffect)d;
            Point baseBloomIntensity = bloomEffect.BaseBloomIntensity;
            baseBloomIntensity.X = (double)e.NewValue;
            bloomEffect.BaseBloomIntensity = baseBloomIntensity;
        }

        private static void BloomIntensityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BloomEffect bloomEffect = (BloomEffect)d;
            Point baseBloomIntensity = bloomEffect.BaseBloomIntensity;
            baseBloomIntensity.Y = (double)e.NewValue;
            bloomEffect.BaseBloomIntensity = baseBloomIntensity;
        }

        private static void BaseSaturationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BloomEffect bloomEffect = (BloomEffect)d;
            Point baseBloomSaturation = bloomEffect.BaseBloomSaturation;
            baseBloomSaturation.X = (double)e.NewValue;
            bloomEffect.BaseBloomSaturation = baseBloomSaturation;
        }

        private static void BloomSaturationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BloomEffect bloomEffect = (BloomEffect)d;
            Point baseBloomSaturation = bloomEffect.BaseBloomSaturation;
            baseBloomSaturation.Y = (double)e.NewValue;
            bloomEffect.BaseBloomSaturation = baseBloomSaturation;
        }

        private const double DefaultThreshold = 0.25;

        private const double DefaultBaseIntensity = 1.0;

        private const double DefaultBloomIntensity = 1.25;

        private const double DefaultBaseSaturation = 1.0;

        private const double DefaultBloomSaturation = 1.0;

        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(BloomEffect), 0);

        public static readonly DependencyProperty BloomIntensityProperty = DependencyProperty.Register("BloomIntensity", typeof(double), typeof(BloomEffect), new PropertyMetadata(1.25, new PropertyChangedCallback(BloomEffect.BloomIntensityPropertyChanged)));

        public static readonly DependencyProperty BaseIntensityProperty = DependencyProperty.Register("BaseIntensity", typeof(double), typeof(BloomEffect), new PropertyMetadata(1.0, new PropertyChangedCallback(BloomEffect.BaseIntensityPropertyChanged)));

        public static readonly DependencyProperty BloomSaturationProperty = DependencyProperty.Register("BloomSaturation", typeof(double), typeof(BloomEffect), new PropertyMetadata(1.0, new PropertyChangedCallback(BloomEffect.BloomSaturationPropertyChanged)));

        public static readonly DependencyProperty BaseSaturationProperty = DependencyProperty.Register("BaseSaturation", typeof(double), typeof(BloomEffect), new PropertyMetadata(1.0, new PropertyChangedCallback(BloomEffect.BaseSaturationPropertyChanged)));

        public static readonly DependencyProperty BaseBloomIntensityProperty = DependencyProperty.Register("BaseBloomIntensityProperty", typeof(Point), typeof(BloomEffect), new PropertyMetadata(new Point(1.0, 1.25), ShaderEffect.PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty BaseBloomSaturationProperty = DependencyProperty.Register("BaseBloomSaturationProperty", typeof(Point), typeof(BloomEffect), new PropertyMetadata(new Point(1.0, 1.0), ShaderEffect.PixelShaderConstantCallback(1)));

        public static readonly DependencyProperty ThresholdProperty = DependencyProperty.Register("Threshold", typeof(double), typeof(BloomEffect), new PropertyMetadata(0.25, ShaderEffect.PixelShaderConstantCallback(2)));
    }
}
