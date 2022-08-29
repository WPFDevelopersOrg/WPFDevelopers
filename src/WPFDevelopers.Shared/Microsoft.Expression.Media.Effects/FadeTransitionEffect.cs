using System.Windows.Media.Effects;

namespace Microsoft.Expression.Media.Effects
{
    public class FadeTransitionEffect : TransitionEffect
    {
        public FadeTransitionEffect()
        {
            base.PixelShader = new PixelShader
            {
                UriSource = Global.MakePackUri("Shaders/FadeTransitionEffect.ps")
            };
        }

        protected override TransitionEffect DeepCopy()
        {
            return new FadeTransitionEffect();
        }
    }
}
