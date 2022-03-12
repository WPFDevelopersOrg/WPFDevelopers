using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Drawing.Core
{
    public static class TransformExtensions
    {
        public static Point TransformPoint(this IEnumerable<GeneralTransform> transforms, Point point)
        {
            if (transforms == null)
            {
                return point;
            }
            foreach (GeneralTransform transform in transforms)
            {
                point = GeometryHelper.SafeTransform(transform, point);
            }
            return point;
        }

        public static Transform CloneTransform(this Transform transform)
        {
            if (transform == null)
            {
                return null;
            }
            TranslateTransform translateTransform = transform as TranslateTransform;
            if (translateTransform != null)
            {
                return new TranslateTransform
                {
                    X = translateTransform.X,
                    Y = translateTransform.Y
                };
            }
            RotateTransform rotateTransform = transform as RotateTransform;
            if (rotateTransform != null)
            {
                return new RotateTransform
                {
                    Angle = rotateTransform.Angle,
                    CenterX = rotateTransform.CenterX,
                    CenterY = rotateTransform.CenterY
                };
            }
            ScaleTransform scaleTransform = transform as ScaleTransform;
            if (scaleTransform != null)
            {
                return new ScaleTransform
                {
                    ScaleX = scaleTransform.ScaleX,
                    ScaleY = scaleTransform.ScaleY,
                    CenterX = scaleTransform.CenterX,
                    CenterY = scaleTransform.CenterY
                };
            }
            SkewTransform skewTransform = transform as SkewTransform;
            if (skewTransform != null)
            {
                return new SkewTransform
                {
                    AngleX = skewTransform.AngleX,
                    AngleY = skewTransform.AngleY,
                    CenterX = skewTransform.CenterX,
                    CenterY = skewTransform.CenterY
                };
            }
            MatrixTransform matrixTransform = transform as MatrixTransform;
            if (matrixTransform != null)
            {
                return new MatrixTransform
                {
                    Matrix = matrixTransform.Matrix
                };
            }
            TransformGroup transformGroup = transform as TransformGroup;
            if (transformGroup != null)
            {
                TransformGroup transformGroup2 = new TransformGroup();
                foreach (Transform transform2 in transformGroup.Children)
                {
                    transformGroup2.Children.Add(transform2.CloneTransform());
                }
                return transformGroup2;
            }
            return transform.DeepCopy<Transform>();
        }

        public static bool TransformEquals(this Transform firstTransform, Transform secondTransform)
        {
            if (firstTransform == null && secondTransform == null)
            {
                return true;
            }
            if (firstTransform == null || secondTransform == null)
            {
                return false;
            }
            if (firstTransform == secondTransform)
            {
                return true;
            }
            TranslateTransform translateTransform = firstTransform as TranslateTransform;
            TranslateTransform translateTransform2 = secondTransform as TranslateTransform;
            if (translateTransform != null && translateTransform2 != null)
            {
                return TranslateTransformEquals(translateTransform, translateTransform2);
            }
            RotateTransform rotateTransform = firstTransform as RotateTransform;
            RotateTransform rotateTransform2 = secondTransform as RotateTransform;
            if (rotateTransform != null && rotateTransform2 != null)
            {
                return RotateTransformEquals(rotateTransform, rotateTransform2);
            }
            ScaleTransform scaleTransform = firstTransform as ScaleTransform;
            ScaleTransform scaleTransform2 = secondTransform as ScaleTransform;
            if (scaleTransform != null && scaleTransform2 != null)
            {
                return ScaleTransformEquals(scaleTransform, scaleTransform2);
            }
            SkewTransform skewTransform = firstTransform as SkewTransform;
            SkewTransform skewTransform2 = secondTransform as SkewTransform;
            if (skewTransform != null && skewTransform2 != null)
            {
                return SkewTransformEquals(skewTransform, skewTransform2);
            }
            MatrixTransform matrixTransform = firstTransform as MatrixTransform;
            MatrixTransform matrixTransform2 = secondTransform as MatrixTransform;
            if (matrixTransform != null && matrixTransform2 != null)
            {
                return MatrixTransformEquals(matrixTransform, matrixTransform2);
            }
            TransformGroup transformGroup = firstTransform as TransformGroup;
            TransformGroup transformGroup2 = secondTransform as TransformGroup;
            if (transformGroup != null && transformGroup2 != null)
            {
                return TransformGroupEquals(transformGroup, transformGroup2);
            }
            TransformGroup transformGroup3 = new TransformGroup();
            transformGroup3.Children.Add(firstTransform);
            TransformGroup transformGroup4 = new TransformGroup();
            transformGroup4.Children.Add(secondTransform);
            return transformGroup3.Value == transformGroup4.Value;
        }

        private static bool TranslateTransformEquals(TranslateTransform firstTransform, TranslateTransform secondTransform)
        {
            return firstTransform.X == secondTransform.X && firstTransform.Y == secondTransform.Y;
        }

        private static bool RotateTransformEquals(RotateTransform firstTransform, RotateTransform secondTransform)
        {
            return firstTransform.Angle == secondTransform.Angle && firstTransform.CenterX == secondTransform.CenterX && firstTransform.CenterY == secondTransform.CenterY;
        }

        private static bool ScaleTransformEquals(ScaleTransform firstTransform, ScaleTransform secondTransform)
        {
            return firstTransform.ScaleX == secondTransform.ScaleX && firstTransform.ScaleY == secondTransform.ScaleY && firstTransform.CenterX == secondTransform.CenterX && firstTransform.CenterY == secondTransform.CenterY;
        }

        private static bool SkewTransformEquals(SkewTransform firstTransform, SkewTransform secondTransform)
        {
            return firstTransform.AngleX == secondTransform.AngleX && firstTransform.AngleY == secondTransform.AngleY && firstTransform.CenterX == secondTransform.CenterX && firstTransform.CenterY == secondTransform.CenterY;
        }

        private static bool TransformGroupEquals(TransformGroup firstTransform, TransformGroup secondTransform)
        {
            if (firstTransform.Children.Count != secondTransform.Children.Count)
            {
                return false;
            }
            for (int i = 0; i < firstTransform.Children.Count; i++)
            {
                if (!firstTransform.Children[i].TransformEquals(secondTransform.Children[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool MatrixTransformEquals(MatrixTransform firstTransform, MatrixTransform secondTransform)
        {
            return firstTransform.Matrix == secondTransform.Matrix;
        }
    }
}
