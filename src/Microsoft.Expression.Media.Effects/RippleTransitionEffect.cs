using System.Windows.Media.Effects;

namespace Microsoft.Expression.Media.Effects
{
    public sealed class RippleTransitionEffect : TransitionEffect
    {
        public RippleTransitionEffect()
        {
            base.PixelShader = new PixelShader
            {
                UriSource = Global.MakePackUri("Shaders/RippleTransitionEffect.ps")
            };
        }

        protected override TransitionEffect DeepCopy()
        {
            return new RippleTransitionEffect();
        }
    }
}
