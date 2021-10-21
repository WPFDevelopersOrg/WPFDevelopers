using System.Windows.Media.Effects;

namespace Microsoft.Expression.Media.Effects
{
    public sealed class CloudRevealTransitionEffect : CloudyTransitionEffect
    {
        public CloudRevealTransitionEffect()
        {
            base.PixelShader = new PixelShader
            {
                UriSource = Global.MakePackUri("Shaders/CloudRevealTransitionEffect.ps")
            };
        }

        protected override TransitionEffect DeepCopy()
        {
            return new CloudRevealTransitionEffect
            {
                RandomSeed = base.RandomSeed
            };
        }
    }
}
