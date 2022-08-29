using System.Windows;
using System.Windows.Media.Effects;

namespace Microsoft.Expression.Media.Effects
{
    public sealed class WipeTransitionEffect : TransitionEffect
    {
        public WipeTransitionEffect()
        {
            base.UpdateShaderValue(WipeTransitionEffect.LineOriginProperty);
            base.UpdateShaderValue(WipeTransitionEffect.LineNormalProperty);
            base.UpdateShaderValue(WipeTransitionEffect.LineOffsetProperty);
            base.UpdateShaderValue(WipeTransitionEffect.FeatherAmountProperty);
            base.PixelShader = new PixelShader
            {
                UriSource = Global.MakePackUri("Shaders/WipeTransitionEffect.ps")
            };
        }

        public WipeDirection WipeDirection
        {
            get
            {
                return (WipeDirection)base.GetValue(WipeTransitionEffect.WipeDirectionProperty);
            }
            set
            {
                base.SetValue(WipeTransitionEffect.WipeDirectionProperty, value);
            }
        }

        public double FeatherAmount
        {
            get
            {
                return (double)base.GetValue(WipeTransitionEffect.FeatherAmountProperty);
            }
            set
            {
                base.SetValue(WipeTransitionEffect.FeatherAmountProperty, value);
            }
        }

        protected override TransitionEffect DeepCopy()
        {
            return new WipeTransitionEffect
            {
                FeatherAmount = this.FeatherAmount,
                WipeDirection = this.WipeDirection
            };
        }

        private Point LineOrigin
        {
            get
            {
                return (Point)base.GetValue(WipeTransitionEffect.LineOriginProperty);
            }
            set
            {
                base.SetValue(WipeTransitionEffect.LineOriginProperty, value);
            }
        }

        private Point LineNormal
        {
            get
            {
                return (Point)base.GetValue(WipeTransitionEffect.LineNormalProperty);
            }
            set
            {
                base.SetValue(WipeTransitionEffect.LineNormalProperty, value);
            }
        }

        private Point LineOffset
        {
            get
            {
                return (Point)base.GetValue(WipeTransitionEffect.LineOffsetProperty);
            }
            set
            {
                base.SetValue(WipeTransitionEffect.LineOffsetProperty, value);
            }
        }

        private static void WipeDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WipeTransitionEffect wipeTransitionEffect = (WipeTransitionEffect)d;
            switch ((WipeDirection)e.NewValue)
            {
                case WipeDirection.LeftToRight:
                    wipeTransitionEffect.LineOrigin = new Point(-0.5, 0.5);
                    wipeTransitionEffect.LineOffset = new Point(1.5, 0.5);
                    wipeTransitionEffect.LineNormal = new Point(1.0, 0.0);
                    return;
                case WipeDirection.RightToLeft:
                    wipeTransitionEffect.LineOrigin = new Point(1.5, 0.5);
                    wipeTransitionEffect.LineOffset = new Point(-0.5, 0.5);
                    wipeTransitionEffect.LineNormal = new Point(-1.0, 0.0);
                    return;
                case WipeDirection.TopToBottom:
                    wipeTransitionEffect.LineOrigin = new Point(0.5, -0.5);
                    wipeTransitionEffect.LineOffset = new Point(0.5, 1.5);
                    wipeTransitionEffect.LineNormal = new Point(0.0, 1.0);
                    return;
                case WipeDirection.BottomToTop:
                    wipeTransitionEffect.LineOrigin = new Point(0.5, 1.5);
                    wipeTransitionEffect.LineOffset = new Point(0.5, -0.5);
                    wipeTransitionEffect.LineNormal = new Point(0.0, -1.0);
                    return;
                case WipeDirection.TopLeftToBottomRight:
                    wipeTransitionEffect.LineOrigin = new Point(-0.5, -0.5);
                    wipeTransitionEffect.LineOffset = new Point(1.5, 1.5);
                    wipeTransitionEffect.LineNormal = new Point(1.0, 1.0);
                    return;
                case WipeDirection.BottomRightToTopLeft:
                    wipeTransitionEffect.LineOrigin = new Point(1.5, 1.5);
                    wipeTransitionEffect.LineOffset = new Point(-0.5, -0.5);
                    wipeTransitionEffect.LineNormal = new Point(-1.0, -1.0);
                    return;
                case WipeDirection.BottomLeftToTopRight:
                    wipeTransitionEffect.LineOrigin = new Point(-0.5, 1.5);
                    wipeTransitionEffect.LineOffset = new Point(1.5, -0.5);
                    wipeTransitionEffect.LineNormal = new Point(1.0, -1.0);
                    return;
                case WipeDirection.TopRightToBottomLeft:
                    wipeTransitionEffect.LineOrigin = new Point(1.5, -0.5);
                    wipeTransitionEffect.LineOffset = new Point(-0.5, 1.5);
                    wipeTransitionEffect.LineNormal = new Point(-1.0, 1.0);
                    return;
                default:
                    return;
            }
        }

        private const WipeDirection DefaultWipeDirection = WipeDirection.LeftToRight;

        public static readonly DependencyProperty WipeDirectionProperty = DependencyProperty.Register("WipeDirection", typeof(WipeDirection), typeof(WipeTransitionEffect), new PropertyMetadata(WipeDirection.LeftToRight, new PropertyChangedCallback(WipeTransitionEffect.WipeDirectionPropertyChanged)));

        public static readonly DependencyProperty FeatherAmountProperty = DependencyProperty.Register("FeatherAmount", typeof(double), typeof(WipeTransitionEffect), new PropertyMetadata(0.2, ShaderEffect.PixelShaderConstantCallback(4)));

        private static readonly DependencyProperty LineOriginProperty = DependencyProperty.Register("LineOrigin", typeof(Point), typeof(WipeTransitionEffect), new PropertyMetadata(new Point(-0.5, 0.5), ShaderEffect.PixelShaderConstantCallback(1)));

        private static readonly DependencyProperty LineNormalProperty = DependencyProperty.Register("LineNormal", typeof(Point), typeof(WipeTransitionEffect), new PropertyMetadata(new Point(1.0, 0.0), ShaderEffect.PixelShaderConstantCallback(2)));

        private static readonly DependencyProperty LineOffsetProperty = DependencyProperty.Register("LineOffset", typeof(Point), typeof(WipeTransitionEffect), new PropertyMetadata(new Point(1.5, 0.5), ShaderEffect.PixelShaderConstantCallback(3)));
    }
}
