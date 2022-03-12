using System.Windows;
using System.Windows.Media.Effects;

namespace Microsoft.Expression.Media.Effects
{
    public sealed class SlideInTransitionEffect : TransitionEffect
    {
        public SlideInTransitionEffect()
        {
            base.PixelShader = new PixelShader
            {
                UriSource = Global.MakePackUri("Shaders/SlideInTransitionEffect.ps")
            };
            base.UpdateShaderValue(SlideInTransitionEffect.SlideNormalProperty);
        }

        public SlideDirection SlideDirection
        {
            get
            {
                return (SlideDirection)base.GetValue(SlideInTransitionEffect.SlideDirectionProperty);
            }
            set
            {
                base.SetValue(SlideInTransitionEffect.SlideDirectionProperty, value);
            }
        }

        protected override TransitionEffect DeepCopy()
        {
            return new SlideInTransitionEffect
            {
                SlideDirection = this.SlideDirection
            };
        }

        private Point SlideNormal
        {
            get
            {
                return (Point)base.GetValue(SlideInTransitionEffect.SlideNormalProperty);
            }
            set
            {
                base.SetValue(SlideInTransitionEffect.SlideNormalProperty, value);
            }
        }

        private static void SlideDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SlideInTransitionEffect slideInTransitionEffect = (SlideInTransitionEffect)d;
            switch ((SlideDirection)e.NewValue)
            {
                case SlideDirection.LeftToRight:
                    slideInTransitionEffect.SlideNormal = new Point(-1.0, 0.0);
                    return;
                case SlideDirection.RightToLeft:
                    slideInTransitionEffect.SlideNormal = new Point(1.0, 0.0);
                    return;
                case SlideDirection.TopToBottom:
                    slideInTransitionEffect.SlideNormal = new Point(0.0, -1.0);
                    return;
                case SlideDirection.BottomToTop:
                    slideInTransitionEffect.SlideNormal = new Point(0.0, 1.0);
                    return;
                default:
                    return;
            }
        }

        private const SlideDirection DefaultSlideDirection = SlideDirection.LeftToRight;

        public static readonly DependencyProperty SlideDirectionProperty = DependencyProperty.Register("SlideDirection", typeof(SlideDirection), typeof(SlideInTransitionEffect), new PropertyMetadata(SlideDirection.LeftToRight, new PropertyChangedCallback(SlideInTransitionEffect.SlideDirectionPropertyChanged)));

        public static readonly DependencyProperty SlideNormalProperty = DependencyProperty.Register("SlideNormal", typeof(Point), typeof(SlideInTransitionEffect), new PropertyMetadata(new Point(-1.0, 0.0), ShaderEffect.PixelShaderConstantCallback(1)));
    }
}
