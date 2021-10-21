using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Expression.Media.Effects
{
    public sealed class SwirlEffect : ShaderEffect
    {
        public SwirlEffect()
        {
            base.PixelShader = new PixelShader
            {
                UriSource = Global.MakePackUri("Shaders/Swirl.ps")
            };
            base.UpdateShaderValue(SwirlEffect.CenterProperty);
            base.UpdateShaderValue(SwirlEffect.TwistAmountProperty);
            base.UpdateShaderValue(SwirlEffect.AngleFrequencyProperty);
            base.UpdateShaderValue(SwirlEffect.InputProperty);
            this.generalTransform = new SwirlEffect.SwirlGeneralTransform(this);
        }

        public Point Center
        {
            get
            {
                return (Point)base.GetValue(SwirlEffect.CenterProperty);
            }
            set
            {
                base.SetValue(SwirlEffect.CenterProperty, value);
            }
        }

        public double TwistAmount
        {
            get
            {
                return (double)base.GetValue(SwirlEffect.TwistAmountProperty);
            }
            set
            {
                base.SetValue(SwirlEffect.TwistAmountProperty, value);
            }
        }

        private Vector AngleFrequency
        {
            get
            {
                return (Vector)base.GetValue(SwirlEffect.AngleFrequencyProperty);
            }
            set
            {
                base.SetValue(SwirlEffect.AngleFrequencyProperty, value);
            }
        }

        private Brush Input
        {
            get
            {
                return (Brush)base.GetValue(SwirlEffect.InputProperty);
            }
            set
            {
                base.SetValue(SwirlEffect.InputProperty, value);
            }
        }

        protected override GeneralTransform EffectMapping
        {
            get
            {
                return this.generalTransform;
            }
        }

        private const double DefaultCenter = 0.5;

        private const double DefaultTwistAmount = 10.0;

        private const double DefaultAngleFrequency = 45.0;

        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(SwirlEffect), 0);

        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register("Center", typeof(Point), typeof(SwirlEffect), new PropertyMetadata(new Point(0.5, 0.5), ShaderEffect.PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty TwistAmountProperty = DependencyProperty.Register("TwistAmount", typeof(double), typeof(SwirlEffect), new PropertyMetadata(10.0, ShaderEffect.PixelShaderConstantCallback(1)));

        public static readonly DependencyProperty AngleFrequencyProperty = DependencyProperty.Register("AngleFrequency", typeof(Vector), typeof(SwirlEffect), new PropertyMetadata(new Vector(1.0, 1.0), ShaderEffect.PixelShaderConstantCallback(2)));

        private SwirlEffect.SwirlGeneralTransform generalTransform;

        private class SwirlGeneralTransform : GeneralTransform
        {
            public SwirlGeneralTransform(SwirlEffect eff)
            {
                this.theEffect = eff;
            }

            public override GeneralTransform Inverse
            {
                get
                {
                    if (this.inverseTransform == null)
                    {
                        this.inverseTransform = (SwirlEffect.SwirlGeneralTransform)base.Clone();
                        this.inverseTransform.thisIsInverse = !this.thisIsInverse;
                    }
                    return this.inverseTransform;
                }
            }

            public override Rect TransformBounds(Rect rect)
            {
                Point point;
                Point point2;
                Point point3;
                Point point4;
                if (this.TryTransform(new Point(rect.Left, rect.Top), out point) && this.TryTransform(new Point(rect.Right, rect.Top), out point2) && this.TryTransform(new Point(rect.Left, rect.Bottom), out point3) && this.TryTransform(new Point(rect.Right, rect.Bottom), out point4))
                {
                    double num = Math.Max(point.X, Math.Max(point2.X, Math.Max(point3.X, point4.X)));
                    double num2 = Math.Min(point.X, Math.Min(point2.X, Math.Min(point3.X, point4.X)));
                    double num3 = Math.Max(point.Y, Math.Max(point2.Y, Math.Max(point3.Y, point4.Y)));
                    double num4 = Math.Min(point.Y, Math.Min(point2.Y, Math.Min(point3.Y, point4.Y)));
                    return new Rect(num2, num4, num - num2, num3 - num4);
                }
                return Rect.Empty;
            }

            public override bool TryTransform(Point targetPoint, out Point result)
            {
                Point point = new Point(targetPoint.X - this.theEffect.Center.X, targetPoint.Y - this.theEffect.Center.Y);
                double num = Math.Sqrt(point.X * point.X + point.Y * point.Y);
                if (num == 0.0)
                {
                    result = targetPoint;
                    return true;
                }
                point.X /= num;
                point.Y /= num;
                double num2 = Math.Atan2(point.Y, point.X);
                double num3 = (double)(this.thisIsInverse ? 1 : -1);
                double num4 = num2 + num3 * this.theEffect.TwistAmount * num;
                Point point2 = new Point(this.theEffect.AngleFrequency.X, this.theEffect.AngleFrequency.Y);
                double num5 = Math.Cos(point2.X * num4) * num;
                double num6 = Math.Sin(point2.Y * num4) * num;
                result = new Point(this.theEffect.Center.X + num5, this.theEffect.Center.Y + num6);
                return true;
            }

            protected override Freezable CreateInstanceCore()
            {
                return new SwirlEffect.SwirlGeneralTransform(this.theEffect)
                {
                    thisIsInverse = this.thisIsInverse
                };
            }

            private readonly SwirlEffect theEffect;

            private bool thisIsInverse;

            private SwirlEffect.SwirlGeneralTransform inverseTransform;
        }
    }
}
