using System.Windows.Media.Effects;

namespace Microsoft.Expression.Media.Effects
{
    public sealed class PixelateTransitionEffect : TransitionEffect
    {
        public PixelateTransitionEffect()
        {
            base.PixelShader = new PixelShader
            {
                UriSource = Global.MakePackUri("Shaders/PixelateTransitionEffect.ps")
            };
        }

        protected override TransitionEffect DeepCopy()
        {
            return new PixelateTransitionEffect();
        }
    }
}
