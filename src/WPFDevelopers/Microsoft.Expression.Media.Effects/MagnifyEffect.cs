using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Expression.Media.Effects
{
    public sealed class MagnifyEffect : ShaderEffect
    {
        public MagnifyEffect()
        {
            base.PixelShader = new PixelShader
            {
                UriSource = Global.MakePackUri("Shaders/Magnify.ps")
            };
            base.UpdateShaderValue(MagnifyEffect.InnerRadiusProperty);
            base.UpdateShaderValue(MagnifyEffect.OuterRadiusProperty);
            base.UpdateShaderValue(MagnifyEffect.CenterProperty);
            base.UpdateShaderValue(MagnifyEffect.AmountProperty);
            base.UpdateShaderValue(MagnifyEffect.InputProperty);
            this.generalTransform = new MagnifyEffect.MagnifyGeneralTransform(this);
        }

        public double InnerRadius
        {
            get
            {
                return (double)base.GetValue(MagnifyEffect.InnerRadiusProperty);
            }
            set
            {
                base.SetValue(MagnifyEffect.InnerRadiusProperty, value);
            }
        }

        public double OuterRadius
        {
            get
            {
                return (double)base.GetValue(MagnifyEffect.OuterRadiusProperty);
            }
            set
            {
                base.SetValue(MagnifyEffect.OuterRadiusProperty, value);
            }
        }

        public Point Center
        {
            get
            {
                return (Point)base.GetValue(MagnifyEffect.CenterProperty);
            }
            set
            {
                base.SetValue(MagnifyEffect.CenterProperty, value);
            }
        }

        public double Amount
        {
            get
            {
                return (double)base.GetValue(MagnifyEffect.AmountProperty);
            }
            set
            {
                base.SetValue(MagnifyEffect.AmountProperty, value);
            }
        }

        protected override GeneralTransform EffectMapping
        {
            get
            {
                return this.generalTransform;
            }
        }

        private Brush Input
        {
            get
            {
                return (Brush)base.GetValue(MagnifyEffect.InputProperty);
            }
            set
            {
                base.SetValue(MagnifyEffect.InputProperty, value);
            }
        }

        private const double DefaultOuterRadius = 0.4;

        private const double DefaultInnerRadius = 0.2;

        private const double DefaultAmount = 0.5;

        private static readonly Point DefaultCenter = new Point(0.5, 0.5);

        public static readonly DependencyProperty InnerRadiusProperty = DependencyProperty.Register("InnerRadius", typeof(double), typeof(MagnifyEffect), new PropertyMetadata(0.2, ShaderEffect.PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty OuterRadiusProperty = DependencyProperty.Register("OuterRadius", typeof(double), typeof(MagnifyEffect), new PropertyMetadata(0.4, ShaderEffect.PixelShaderConstantCallback(1)));

        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register("Center", typeof(Point), typeof(MagnifyEffect), new PropertyMetadata(MagnifyEffect.DefaultCenter, ShaderEffect.PixelShaderConstantCallback(2)));

        public static readonly DependencyProperty AmountProperty = DependencyProperty.Register("Amount", typeof(double), typeof(MagnifyEffect), new PropertyMetadata(0.5, ShaderEffect.PixelShaderConstantCallback(3)));

        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(MagnifyEffect), 0);

        private MagnifyEffect.MagnifyGeneralTransform generalTransform;

        private class MagnifyGeneralTransform : GeneralTransform
        {
            public MagnifyGeneralTransform(MagnifyEffect fx)
            {
                this.effect = fx;
            }

            public override GeneralTransform Inverse
            {
                get
                {
                    if (this.inverseTransform == null)
                    {
                        this.inverseTransform = (MagnifyEffect.MagnifyGeneralTransform)base.Clone();
                        this.inverseTransform.IsInverse = !this.IsInverse;
                    }
                    return this.inverseTransform;
                }
            }

            public override Rect TransformBounds(Rect rect)
            {
                Point point;
                bool flag = this.TryTransform(new Point(rect.Left, rect.Top), out point);
                Point point2;
                bool flag2 = this.TryTransform(new Point(rect.Right, rect.Bottom), out point2);
                Rect empty;
                if (flag && flag2)
                {
                    empty = new Rect(point, point2);
                }
                else
                {
                    empty = Rect.Empty;
                }
                return empty;
            }

            public override bool TryTransform(Point targetPoint, out Point result)
            {
                double num = this.effect.InnerRadius;
                double num2 = this.effect.OuterRadius;
                double num3 = 1.0 - this.effect.Amount;
                Point center = this.effect.Center;
                if (num2 < num)
                {
                    num2 = num + 1E-05;
                }
                if (num == num2)
                {
                    num -= 1E-05;
                }
                if (num3 <= 0.0)
                {
                    num3 = 1E-05;
                }
                if (this.IsInverse)
                {
                    if (!MagnifyEffect.MagnifyGeneralTransform.PointIsInCircle(targetPoint, this.effect.Center, num2))
                    {
                        result = targetPoint;
                    }
                    else
                    {
                        Point point = new Point(targetPoint.X - center.X, targetPoint.Y - center.Y);
                        if (MagnifyEffect.MagnifyGeneralTransform.PointIsInCircle(targetPoint, center, num))
                        {
                            result = new Point(center.X + num3 * point.X, center.Y + num3 * point.Y);
                        }
                        else
                        {
                            result = MagnifyEffect.MagnifyGeneralTransform.InToOut(targetPoint, num, num2, center, num3);
                        }
                    }
                }
                else
                {
                    result = MagnifyEffect.MagnifyGeneralTransform.OutToIn(targetPoint, num, num2, center, num3);
                }
                return true;
            }

            private static double InvLerp(double v0, double v, double v1)
            {
                if (v0 != v1)
                {
                    return (v - v0) / (v1 - v0);
                }
                if (v != v0)
                {
                    return 0.0;
                }
                return 1.0;
            }

            private static double SolveForXY(double radius, double innerRadius, double outerRadius, double amount, Point outHat, out Point inHat)
            {
                double num = MagnifyEffect.MagnifyGeneralTransform.InvLerp(innerRadius, radius, outerRadius);
                double num2 = 0.5 * (amount - 1.0) * (Math.Cos(3.141592653589793 * num) + 1.0) + 1.0;
                inHat = new Point(outHat.X / num2, outHat.Y / num2);
                return Math.Sqrt(inHat.X * inHat.X + inHat.Y * inHat.Y);
            }

            private static Point OutToIn(Point outPoint, double innerRadius, double outerRadius, Point center, double amount)
            {
                Point outHat = new Point(outPoint.X - center.X, outPoint.Y - center.Y);
                double num = outerRadius;
                Point point;
                double num2 = MagnifyEffect.MagnifyGeneralTransform.SolveForXY(num, innerRadius, outerRadius, amount, outHat, out point);
                if (num2 >= outerRadius)
                {
                    return outPoint;
                }
                double num3 = innerRadius;
                double num4 = MagnifyEffect.MagnifyGeneralTransform.SolveForXY(num3, innerRadius, outerRadius, amount, outHat, out point);
                if (num4 <= innerRadius)
                {
                    return new Point(outHat.X / amount + center.X, outHat.Y / amount + center.Y);
                }
                int num5 = 100;
                while (num5-- > 0)
                {
                    double num6 = (num * num4 - num3 * num2) / (num4 - num2 + num - num3);
                    double num7 = MagnifyEffect.MagnifyGeneralTransform.SolveForXY(num6, innerRadius, outerRadius, amount, outHat, out point);
                    if (Math.Abs(num6 - num7) <= 0.0010000000474974513)
                    {
                        break;
                    }
                    if (num7 <= num6)
                    {
                        num = num6;
                        num2 = num7;
                    }
                    else
                    {
                        num3 = num6;
                        num4 = num7;
                    }
                }
                return new Point(point.X + center.X, point.Y + center.Y);
            }

            private static Point InToOut(Point inPoint, double innerRadius, double outerRadius, Point center, double amount)
            {
                double v = MagnifyEffect.MagnifyGeneralTransform.Radius(inPoint, center);
                double num = (amount - 1.0) * 0.5 * (Math.Cos(3.141592653589793 * MagnifyEffect.MagnifyGeneralTransform.InvLerp(innerRadius, v, outerRadius)) + 1.0) + 1.0;
                return new Point((inPoint.X - center.X) * num + center.X, (inPoint.Y - center.Y) * num + center.Y);
            }

            private static double Radius(Point point, Point center)
            {
                Point point2 = new Point(point.X - center.X, point.Y - center.Y);
                return Math.Sqrt(point2.X * point2.X + point2.Y * point2.Y);
            }

            protected override Freezable CreateInstanceCore()
            {
                return new MagnifyEffect.MagnifyGeneralTransform(this.effect)
                {
                    IsInverse = this.IsInverse
                };
            }

            private static bool PointIsInCircle(Point pt, Point center, double radius)
            {
                Point point = new Point(pt.X - center.X, pt.Y - center.Y);
                double num = (radius == 0.0) ? ((pt.X == center.X) ? 0.0 : 2.0) : (point.X / radius);
                double num2 = (radius == 0.0) ? ((pt.Y == center.Y) ? 0.0 : 2.0) : (point.Y / radius);
                double num3 = num * num + num2 * num2;
                return num3 <= 1.0;
            }

            private const double biasValue = 1E-05;

            private readonly MagnifyEffect effect;

            private bool IsInverse;

            private MagnifyEffect.MagnifyGeneralTransform inverseTransform;
        }
    }
}
