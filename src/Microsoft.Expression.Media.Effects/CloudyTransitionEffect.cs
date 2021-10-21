using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.Media.Effects
{
    public abstract class CloudyTransitionEffect : RandomizedTransitionEffect
    {
        internal CloudyTransitionEffect()
        {
            this.CloudImage = new ImageBrush(new BitmapImage(Global.MakePackUri("Images/clouds.jpg")));
            base.UpdateShaderValue(CloudyTransitionEffect.CloudImageProperty);
        }

        protected Brush CloudImage
        {
            get
            {
                return (Brush)base.GetValue(CloudyTransitionEffect.CloudImageProperty);
            }
            set
            {
                base.SetValue(CloudyTransitionEffect.CloudImageProperty, value);
            }
        }

        protected static readonly DependencyProperty CloudImageProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("CloudImage", typeof(CloudyTransitionEffect), 2, SamplingMode.Bilinear);
    }
}
