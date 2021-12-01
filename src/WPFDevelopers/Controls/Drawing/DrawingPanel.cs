using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFDevelopers.Assists;
using WPFDevelopers.Utilities;

namespace WPFDevelopers.Controls
{
    public enum DrawingMode
    {
        Left,
        Right,
        Top,
        Bottom,
    }

    public class DrawingPanel : Panel
    {
        public static readonly DependencyProperty BorderBrushProperty =
                               DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(DrawingPanel), new PropertyMetadata(default, OnRenderPropertyChangedCallBack));

        public static readonly DependencyProperty BorderThicknessProperty =
                               DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(DrawingPanel), new PropertyMetadata(new Thickness(), OnRenderPropertyChangedCallBack));

        public static readonly DependencyProperty CornerRadiusProperty =
                               DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(DrawingPanel), new PropertyMetadata(new CornerRadius(), OnRenderPropertyChangedCallBack));

        public static readonly DependencyProperty OffsetProperty =
                               DependencyProperty.Register("Offset", typeof(double), typeof(DrawingPanel), new PropertyMetadata(0d, OnRenderPropertyChangedCallBack));

        public static readonly DependencyProperty ModeProperty =
                               DependencyProperty.Register("Mode", typeof(DrawingMode), typeof(DrawingPanel), new PropertyMetadata(default(DrawingMode), OnRenderPropertyChangedCallBack));

        public static readonly DependencyProperty StartOffsetAngleProperty =
                               DependencyProperty.Register("StartOffsetAngle", typeof(double), typeof(DrawingPanel), new PropertyMetadata(0d, OnRenderPropertyChangedCallBack));


        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        public Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        public DrawingMode Mode
        {
            get { return (DrawingMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        public double StartOffsetAngle
        {
            get { return (double)GetValue(StartOffsetAngleProperty); }
            set { SetValue(StartOffsetAngleProperty, value); }
        }

        public string MultipleArrayDescription
        {
            get { return (string)GetValue(MultipleArrayDescriptionProperty); }
            set { SetValue(MultipleArrayDescriptionProperty, value); }
        }

        public static readonly DependencyProperty MultipleArrayDescriptionProperty =
            DependencyProperty.Register("MultipleArrayDescription", typeof(string), typeof(DrawingPanel), new PropertyMetadata(default, OnRenderPropertyChangedCallBack));

        public double LayerSpace
        {
            get { return (double)GetValue(LayerSpaceProperty); }
            set { SetValue(LayerSpaceProperty, value); }
        }

        public static readonly DependencyProperty LayerSpaceProperty =
            DependencyProperty.Register("LayerSpace", typeof(double), typeof(DrawingPanel), new PropertyMetadata(5d, OnRenderPropertyChangedCallBack));


        private List<int> _MultipleArray = default;

        private static void OnRenderPropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DrawingPanel drawing))
                return;

            drawing.InvalidateVisual();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double width = 0;
            double height = 0;

            foreach (var child in InternalChildren)
            {
                if (!(child is UIElement uiElement))
                    continue;

                uiElement.Measure(availableSize);
                width = Math.Max(width, uiElement.DesiredSize.Width);
                height = Math.Max(height, uiElement.DesiredSize.Height);

                uiElement.RenderSize = new Size(width, height);
            }

            if (double.IsPositiveInfinity(availableSize.Width))
                availableSize.Width = width * 4 + Offset * 2;

            if (double.IsPositiveInfinity(availableSize.Height))
                availableSize.Height = height * 4 + Offset * 2;

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (double.IsPositiveInfinity(finalSize.Width)
                || double.IsPositiveInfinity(finalSize.Height)
                || double.IsNaN(finalSize.Width)
                || double.IsNaN(finalSize.Height)
                || double.IsNegativeInfinity(finalSize.Width)
                || double.IsNegativeInfinity(finalSize.Height))
                return finalSize;
            double realValue = Math.Min(finalSize.Width, finalSize.Height);
            Size desiredSzie = new Size(realValue, realValue);

            MultipleArrangeOverride();

            if (IsMuliple())
                ArrangeOverrideMultiple(realValue);
            else
                ArrangeOverrideSingle(realValue);

            return desiredSzie;
        }

        protected override void OnRender(DrawingContext dc)
        {
            OnRenderBackground(dc);
        }

        private bool OnRenderBackground(DrawingContext dc)
        {
            Thickness border = BorderThickness;
            Brush borderBrush = BorderBrush;
            Brush backGround = Background;

            CornerRadius cornerRadius = CornerRadius;
            double outerCornerRadius = cornerRadius.TopLeft; // Already validated that all corners have the same radius
            bool roundedCorners = !DoubleUtil.IsZero(outerCornerRadius);

            if (!border.IsZero() && borderBrush != null)
            {
                Pen pen = new Pen();
                pen.Brush = borderBrush;
                pen.Thickness = border.Left;

                if (borderBrush.IsFrozen)
                    pen.Freeze();

                double halfThickness;
                if (border.IsUniform())
                {
                    halfThickness = pen.Thickness * 0.5;
                    Rect rect = new Rect(new Point(halfThickness, halfThickness), new Point(RenderSize.Width - halfThickness, RenderSize.Height - halfThickness));

                    if (roundedCorners)
                    {
                        dc.DrawRoundedRectangle(
                            backGround,
                            pen,
                            rect,
                            outerCornerRadius,
                            outerCornerRadius);
                    }
                    else
                    {
                        dc.DrawRectangle(
                            backGround,
                            pen,
                            rect);
                    }
                }
                else
                {
                    if (DoubleUtil.GreaterThan(border.Left, 0))
                    {
                        halfThickness = pen.Thickness * 0.5;
                        dc.DrawLine(
                            pen,
                            new Point(halfThickness, 0),
                            new Point(halfThickness, RenderSize.Height));
                    }

                    if (DoubleUtil.GreaterThan(border.Right, 0))
                    {
                        halfThickness = pen.Thickness * 0.5;
                        dc.DrawLine(
                            pen,
                            new Point(RenderSize.Width - halfThickness, 0),
                            new Point(RenderSize.Width - halfThickness, RenderSize.Height));
                    }

                    if (DoubleUtil.GreaterThan(border.Top, 0))
                    {
                        halfThickness = pen.Thickness * 0.5;
                        dc.DrawLine(
                            pen,
                            new Point(0, halfThickness),
                            new Point(RenderSize.Width, halfThickness));
                    }

                    if (DoubleUtil.GreaterThan(border.Bottom, 0))
                    {
                        halfThickness = pen.Thickness * 0.5;
                        dc.DrawLine(
                            pen,
                            new Point(0, RenderSize.Height - halfThickness),
                            new Point(RenderSize.Width, RenderSize.Height - halfThickness));
                    }
                }
            }
            else
            {
                Rect rect = new Rect(new Point(0, 0), new Point(RenderSize.Width, RenderSize.Height));

                if (roundedCorners)
                {
                    dc.DrawRoundedRectangle(
                        backGround,
                        null,
                        rect,
                        outerCornerRadius,
                        outerCornerRadius);
                }
                else
                {
                    dc.DrawRectangle(
                        backGround,
                        null,
                        rect);
                }
            }

            return true;
        }

        private bool MultipleArrangeOverride()
        {
            _MultipleArray?.Clear();
            _MultipleArray = default;

            if (string.IsNullOrWhiteSpace(MultipleArrayDescription))
                return true;

            _MultipleArray = new List<int>();
            var vArray = MultipleArrayDescription.Split(',');
            foreach (var item in vArray)
            {
                if (string.IsNullOrWhiteSpace(item))
                    continue;

                if (!int.TryParse(item, out var vResult))
                    continue;

                if (vResult <= 0)
                    continue;

                //var v = _MultipleArray.Where(ix => ix == vResult).ToList();
                //if (v.Count == 0)
                //_MultipleArray.Add(vResult);

                _MultipleArray.Add(vResult);
            }

            int nCount = 0;
            foreach (var item in _MultipleArray)
                nCount += item;

            if (nCount < InternalChildren.Count)
                _MultipleArray.Add(InternalChildren.Count - nCount);

            return true;
        }

        private bool ArrangeOverrideSingle(double realValue)
        {
            var vInternalChildren = InternalChildren;
            double angle = 360d / vInternalChildren.Count;

            switch (Mode)
            {
                case DrawingMode.Left:
                    ArrangeOverride_Left(vInternalChildren, realValue, angle);
                    break;
                case DrawingMode.Right:
                    ArrangeOverride_Right(vInternalChildren, realValue, angle);
                    break;
                case DrawingMode.Top:
                    ArrangeOverride_Top(vInternalChildren, realValue, angle);
                    break;
                case DrawingMode.Bottom:
                    ArrangeOverride_Bottom(vInternalChildren, realValue, angle);
                    break;
                default:
                    ArrangeOverride_Left(vInternalChildren, realValue, angle);
                    break;
            }

            return true;
        }

        private bool ArrangeOverride_Left(UIElementCollection internalChildren, double realValue, double angle)
        {
            if (internalChildren == null || internalChildren.Count <= 0)
                return false;

            if (realValue <= 0 || angle <= 0)
                return false;

            int index = 0;
            foreach (var child in internalChildren)
            {
                index++;
                if (!(child is UIElement uiElement))
                    continue;

                if (uiElement.DesiredSize.IsEmpty)
                    continue;

                if (DoubleUtil.IsZero(uiElement.DesiredSize.Width) || DoubleUtil.IsZero(uiElement.DesiredSize.Height))
                    continue;

                Point size = new Point(Offset, (realValue - uiElement.DesiredSize.Height) / 2d);

                uiElement.Arrange(new Rect(size, uiElement.DesiredSize));

                var vRotateTransform = new RotateTransform()
                {
                    Angle = angle * (index - 1) + StartOffsetAngle,
                };

                var vRealValue = realValue - 2 * Offset;
                if (vRealValue <= 0)
                    return false;

                var vRet = vRealValue / 2 / uiElement.DesiredSize.Width;

                uiElement.RenderTransformOrigin = new Point(vRet, 0.5);
                uiElement.RenderTransform = vRotateTransform;
            }

            return true;
        }

        private bool ArrangeOverride_Right(UIElementCollection internalChildren, double realValue, double angle)
        {
            if (internalChildren == null || internalChildren.Count <= 0)
                return false;

            if (realValue <= 0 || angle <= 0)
                return false;

            int index = 0;
            foreach (var child in internalChildren)
            {
                index++;
                if (!(child is UIElement uiElement))
                    continue;

                if (uiElement.DesiredSize.IsEmpty)
                    continue;

                if (DoubleUtil.IsZero(uiElement.DesiredSize.Width) || DoubleUtil.IsZero(uiElement.DesiredSize.Height))
                    continue;

                Point size = new Point(realValue - Offset - uiElement.DesiredSize.Width, (realValue - uiElement.DesiredSize.Height) / 2d);

                uiElement.Arrange(new Rect(size, uiElement.DesiredSize));

                var vRotateTransform = new RotateTransform()
                {
                    Angle = angle * (index - 1) + StartOffsetAngle,
                };

                var vRealValue = realValue - 2 * Offset;
                if (vRealValue <= 0)
                    return false;

                var vRet = vRealValue / 2 / uiElement.DesiredSize.Width - 1;

                uiElement.RenderTransformOrigin = new Point(-vRet, 0.5);
                uiElement.RenderTransform = vRotateTransform;
            }

            return true;
        }

        private bool ArrangeOverride_Top(UIElementCollection internalChildren, double realValue, double angle)
        {
            if (internalChildren == null || internalChildren.Count <= 0)
                return false;

            if (realValue <= 0 || angle <= 0)
                return false;

            int index = 0;
            foreach (var child in internalChildren)
            {
                index++;
                if (!(child is UIElement uiElement))
                    continue;

                if (uiElement.DesiredSize.IsEmpty)
                    continue;

                if (DoubleUtil.IsZero(uiElement.DesiredSize.Width) || DoubleUtil.IsZero(uiElement.DesiredSize.Height))
                    continue;

                Point size = new Point((realValue - uiElement.DesiredSize.Width) / 2d, Offset);

                uiElement.Arrange(new Rect(size, uiElement.DesiredSize));

                var vRotateTransform = new RotateTransform()
                {
                    Angle = angle * (index - 1) + StartOffsetAngle,
                };

                var vRealValue = realValue - 2 * Offset;
                if (vRealValue <= 0)
                    return false;

                var vRet = vRealValue / 2 / uiElement.DesiredSize.Height;

                uiElement.RenderTransformOrigin = new Point(0.5, vRet);
                uiElement.RenderTransform = vRotateTransform;
            }

            return true;
        }

        private bool ArrangeOverride_Bottom(UIElementCollection internalChildren, double realValue, double angle)
        {
            if (internalChildren == null || internalChildren.Count <= 0)
                return false;

            if (realValue <= 0 || angle <= 0)
                return false;

            int index = 0;
            foreach (var child in internalChildren)
            {
                index++;
                if (!(child is UIElement uiElement))
                    continue;

                if (uiElement.DesiredSize.IsEmpty)
                    continue;

                if (DoubleUtil.IsZero(uiElement.DesiredSize.Width) || DoubleUtil.IsZero(uiElement.DesiredSize.Height))
                    continue;

                Point size = new Point((realValue - uiElement.DesiredSize.Width) / 2d, realValue - Offset - uiElement.DesiredSize.Height);

                uiElement.Arrange(new Rect(size, uiElement.DesiredSize));

                var vRotateTransform = new RotateTransform()
                {
                    Angle = angle * (index - 1) + StartOffsetAngle,
                };

                var vRealValue = realValue - 2 * Offset;
                if (vRealValue <= 0)
                    return false;

                var vRet = vRealValue / 2 / uiElement.DesiredSize.Height - 1;

                uiElement.RenderTransformOrigin = new Point(0.5, -vRet);
                uiElement.RenderTransform = vRotateTransform;
            }

            return true;
        }

        private bool IsMuliple()
        {
            if (_MultipleArray?.Count > 0)
            {
                int nCount = 0;
                foreach (var item in _MultipleArray)
                    nCount += item;

                if (nCount > InternalChildren.Count)
                    return false;

                return true;
            }

            return false;
        }

        private bool ArrangeOverrideMultiple(double realValue)
        {
            double offset = Offset;
            var vInternalChildren = InternalChildren;

            int nStartIndex = 0;

            double lastMaxWidth = 0;
            double lastMaxHeight = 0;

            int nLoopIndex = 0;
            foreach (var item in _MultipleArray)
            {
                double angle = 360d / item;

                int nIndex = 0;
                double maxWidth = 0;
                double maxHeight = 0;
                for (int i = nStartIndex; i < nStartIndex + item; ++i)
                {
                    var child = vInternalChildren[i];

                    Size vSize = Size.Empty;
                    switch (Mode)
                    {
                        case DrawingMode.Left:
                            {
                                var vOffset = offset + lastMaxWidth + LayerSpace * nLoopIndex;
                                ArrangeOverride_Left(child, nIndex, vOffset, realValue, angle, out vSize);
                            }
                            break;
                        case DrawingMode.Right:
                            {
                                var vOffset = offset + lastMaxWidth + LayerSpace * nLoopIndex;
                                ArrangeOverride_Right(child, nIndex, vOffset, realValue, angle, out vSize);
                            }
                            break;
                        case DrawingMode.Top:
                            {
                                var vOffset = offset + lastMaxHeight + LayerSpace * nLoopIndex;
                                ArrangeOverride_Top(child, nIndex, vOffset, realValue, angle, out vSize);
                            }
                            break;
                        case DrawingMode.Bottom:
                            {
                                var vOffset = offset + lastMaxHeight + LayerSpace * nLoopIndex;
                                ArrangeOverride_Bottom(child, nIndex, vOffset, realValue, angle, out vSize);
                            }
                            break;
                        default:
                            {
                                var vOffset = offset + lastMaxWidth + LayerSpace * nLoopIndex;
                                ArrangeOverride_Left(child, nIndex, vOffset, realValue, angle, out vSize);
                            }
                            break;
                    }

                    maxWidth = Math.Max(maxWidth, vSize.Width);
                    maxHeight = Math.Max(maxHeight, vSize.Height);

                    nIndex++;
                }

                lastMaxWidth += maxWidth;
                lastMaxHeight += maxHeight;

                nStartIndex += item;
                nLoopIndex++;
            }

            return true;
        }

        private bool ArrangeOverride_Left(UIElement uiElement, int index, double offset, double realValue, double angle, out Size desiredSize)
        {
            desiredSize = Size.Empty;
            if (realValue <= 0 || angle <= 0)
                return false;

            if (uiElement.DesiredSize.IsEmpty)
                return false;

            if (DoubleUtil.IsZero(uiElement.DesiredSize.Width) || DoubleUtil.IsZero(uiElement.DesiredSize.Height))
                return false;

            Point size = new Point(offset, (realValue - uiElement.DesiredSize.Height) / 2d);

            uiElement.Arrange(new Rect(size, uiElement.DesiredSize));
            desiredSize = uiElement.DesiredSize;

            var vRotateTransform = new RotateTransform()
            {
                Angle = angle * index + StartOffsetAngle,
            };

            var vRealValue = realValue - 2 * offset;
            if (vRealValue <= 0)
                return false;

            var vRet = vRealValue / 2 / uiElement.DesiredSize.Width;

            uiElement.RenderTransformOrigin = new Point(vRet, 0.5);
            uiElement.RenderTransform = vRotateTransform;

            return true;
        }

        private bool ArrangeOverride_Right(UIElement uiElement, int index, double offset, double realValue, double angle, out Size desiredSize)
        {
            desiredSize = Size.Empty;
            if (realValue <= 0 || angle <= 0)
                return false;

            if (uiElement.DesiredSize.IsEmpty)
                return false;

            if (DoubleUtil.IsZero(uiElement.DesiredSize.Width) || DoubleUtil.IsZero(uiElement.DesiredSize.Height))
                return false;

            Point size = new Point(realValue - offset - uiElement.DesiredSize.Width, (realValue - uiElement.DesiredSize.Height) / 2d);

            uiElement.Arrange(new Rect(size, uiElement.DesiredSize));
            desiredSize = uiElement.DesiredSize;

            var vRotateTransform = new RotateTransform()
            {
                Angle = angle * index + StartOffsetAngle,
            };

            var vRealValue = realValue - 2 * offset;
            if (vRealValue <= 0)
                return false;

            var vRet = vRealValue / 2 / uiElement.DesiredSize.Width - 1;

            uiElement.RenderTransformOrigin = new Point(-vRet, 0.5);
            uiElement.RenderTransform = vRotateTransform;

            return true;
        }

        private bool ArrangeOverride_Top(UIElement uiElement, int index, double offset, double realValue, double angle, out Size desiredSize)
        {
            desiredSize = Size.Empty;
            if (realValue <= 0 || angle <= 0)
                return false;

            if (uiElement.DesiredSize.IsEmpty)
                return false;

            if (DoubleUtil.IsZero(uiElement.DesiredSize.Width) || DoubleUtil.IsZero(uiElement.DesiredSize.Height))
                return false;

            Point size = new Point((realValue - uiElement.DesiredSize.Width) / 2d, offset);

            uiElement.Arrange(new Rect(size, uiElement.DesiredSize));
            desiredSize = uiElement.DesiredSize;

            var vRotateTransform = new RotateTransform()
            {
                Angle = angle * index + StartOffsetAngle,
            };

            var vRealValue = realValue - 2 * offset;
            if (vRealValue <= 0)
                return false;

            var vRet = vRealValue / 2 / uiElement.DesiredSize.Height;

            uiElement.RenderTransformOrigin = new Point(0.5, vRet);
            uiElement.RenderTransform = vRotateTransform;

            return true;
        }

        private bool ArrangeOverride_Bottom(UIElement uiElement, int index, double offset, double realValue, double angle, out Size desiredSize)
        {
            desiredSize = Size.Empty;
            if (realValue <= 0 || angle <= 0)
                return false;

            if (uiElement.DesiredSize.IsEmpty)
                return false;

            if (DoubleUtil.IsZero(uiElement.DesiredSize.Width) || DoubleUtil.IsZero(uiElement.DesiredSize.Height))
                return false;

            Point size = new Point((realValue - uiElement.DesiredSize.Width) / 2d, realValue - offset - uiElement.DesiredSize.Height);

            uiElement.Arrange(new Rect(size, uiElement.DesiredSize));
            desiredSize = uiElement.DesiredSize;

            var vRotateTransform = new RotateTransform()
            {
                Angle = angle * index + StartOffsetAngle,
            };

            var vRealValue = realValue - 2 * offset;
            if (vRealValue <= 0)
                return false;

            var vRet = vRealValue / 2 / uiElement.DesiredSize.Height - 1;

            uiElement.RenderTransformOrigin = new Point(0.5, -vRet);
            uiElement.RenderTransform = vRotateTransform;
            return true;
        }

    }
}
