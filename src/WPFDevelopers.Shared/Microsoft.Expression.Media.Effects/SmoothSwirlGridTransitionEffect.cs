using System.Windows;
using System.Windows.Media.Effects;

namespace Microsoft.Expression.Media.Effects
{
    public sealed class SmoothSwirlGridTransitionEffect : TransitionEffect
    {
        public SmoothSwirlGridTransitionEffect(double twist) : this()
        {
            this.TwistAmount = twist;
        }

        public SmoothSwirlGridTransitionEffect()
        {
            base.PixelShader = new PixelShader
            {
                UriSource = Global.MakePackUri("Shaders/SmoothSwirlGridTransitionEffect.ps")
            };
            base.UpdateShaderValue(SmoothSwirlGridTransitionEffect.TwistAmountProperty);
            base.UpdateShaderValue(SmoothSwirlGridTransitionEffect.CellCountProperty);
        }

        public double TwistAmount
        {
            get
            {
                return (double)base.GetValue(SmoothSwirlGridTransitionEffect.TwistAmountProperty);
            }
            set
            {
                base.SetValue(SmoothSwirlGridTransitionEffect.TwistAmountProperty, value);
            }
        }

        public double CellCount
        {
            get
            {
                return (double)base.GetValue(SmoothSwirlGridTransitionEffect.CellCountProperty);
            }
            set
            {
                base.SetValue(SmoothSwirlGridTransitionEffect.CellCountProperty, value);
            }
        }

        protected override TransitionEffect DeepCopy()
        {
            return new SmoothSwirlGridTransitionEffect
            {
                TwistAmount = this.TwistAmount,
                CellCount = this.CellCount
            };
        }

        private const double DefaultCellCount = 10.0;

        private const double DefaultTwistAmount = 10.0;

        public static readonly DependencyProperty TwistAmountProperty = DependencyProperty.Register("TwistAmount", typeof(double), typeof(SmoothSwirlGridTransitionEffect), new PropertyMetadata(10.0, ShaderEffect.PixelShaderConstantCallback(1)));

        public static readonly DependencyProperty CellCountProperty = DependencyProperty.Register("CellCount", typeof(double), typeof(SmoothSwirlGridTransitionEffect), new PropertyMetadata(10.0, ShaderEffect.PixelShaderConstantCallback(2)));
    }
}
