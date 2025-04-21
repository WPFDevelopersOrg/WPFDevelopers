using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Utilities;

namespace WPFDevelopers.Samples.Controls
{
    public class BubblleCanvas : Canvas
    {
        private double _bubbleItemX;
        private double _bubbleItemY;

        private int _number;
        private double _size;
        private const int _maxSize = 120;

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var width = arrangeSize.Width;
            var height = arrangeSize.Height;

            double left = 0d, top = 0d;
            for (var y = 0; y < (int)height / _maxSize; y++)
            {
                double yNum = y + 1;
                yNum = _maxSize * yNum;
                for (var x = 0; x < (int)width / _maxSize; x++)
                {
                    if (_number > InternalChildren.Count - 1)
                        return arrangeSize;

                    var item = InternalChildren[_number] as FrameworkElement;

                    if (DoubleUtil.IsNaN(item.ActualWidth) || DoubleUtil.IsZero(item.ActualWidth) || DoubleUtil.IsNaN(item.ActualHeight) || DoubleUtil.IsZero(item.ActualHeight))
                        ResizeItem(item);

                    _bubbleItemX = Canvas.GetLeft(item);
                    _bubbleItemY = Canvas.GetTop(item);

                    if (double.IsNaN(_bubbleItemX) || double.IsNaN(_bubbleItemY))
                    {
                        double xNum = x + 1;
                        xNum = _maxSize * xNum;
                        _bubbleItemX = Helper.NextDouble(left, xNum - _size * Helper.NextDouble(0.6, 0.9));
                        var _width = _bubbleItemX + _size;
                        _width = _width > width ? width - (width - _bubbleItemX) - _size : _bubbleItemX;
                        _bubbleItemX = _width;
                        _bubbleItemY = Helper.NextDouble(top, yNum - _size * Helper.NextDouble(0.6, 0.9));
                        var _height = _bubbleItemY + _size;
                        _height = _height > height ? height - (height - _bubbleItemY) - _size : _bubbleItemY;
                        _bubbleItemY = _height;

                    }
                    Canvas.SetLeft(item, _bubbleItemX);
                    Canvas.SetTop(item, _bubbleItemY);
                    left = left + _size;

                    _number++;

                    item.Arrange(new Rect(new Point(_bubbleItemX, _bubbleItemY), new Size(_size, _size)));
                }
                left = 0d;
                top = top + _maxSize;
            }

            return arrangeSize;
        }
        private void ResizeItem(FrameworkElement item)
        {
            if (DoubleUtil.GreaterThanOrClose(item.DesiredSize.Width, 55))
                _size = Helper.GetRandom.Next(80, _maxSize);
            else
                _size = Helper.GetRandom.Next(55, _maxSize);
            item.Width = _size;
            item.Height = _size;
        }
    }
}
