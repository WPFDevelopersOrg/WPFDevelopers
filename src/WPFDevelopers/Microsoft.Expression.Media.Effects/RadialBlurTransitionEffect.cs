using System.Windows.Media.Effects;

namespace Microsoft.Expression.Media.Effects
{
    public class RadialBlurTransitionEffect : TransitionEffect
    {
        public RadialBlurTransitionEffect()
        {
            base.PixelShader = new PixelShader
            {
                UriSource = Global.MakePackUri("Shaders/RadialBlurTransitionEffect.ps")
            };
        }

        protected override TransitionEffect DeepCopy()
        {
            return new RadialBlurTransitionEffect();
        }
    }
}
