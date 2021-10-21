using System.Windows;
using System.Windows.Media.Effects;

namespace Microsoft.Expression.Media.Effects
{
    public abstract class RandomizedTransitionEffect : TransitionEffect
    {
        internal RandomizedTransitionEffect()
        {
            base.UpdateShaderValue(RandomizedTransitionEffect.RandomSeedProperty);
        }

        public double RandomSeed
        {
            get
            {
                return (double)base.GetValue(RandomizedTransitionEffect.RandomSeedProperty);
            }
            set
            {
                base.SetValue(RandomizedTransitionEffect.RandomSeedProperty, value);
            }
        }

        private const double DefaultRandomSeed = 0.0;

        public static readonly DependencyProperty RandomSeedProperty = DependencyProperty.Register("RandomSeed", typeof(double), typeof(RandomizedTransitionEffect), new PropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(1)));
    }
}
