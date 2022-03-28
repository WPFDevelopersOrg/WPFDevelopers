using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = CanvasTemplateName, Type = typeof(Canvas))]
    [TemplatePart(Name = RectangleLeftTemplateName, Type = typeof(Rectangle))]
    [TemplatePart(Name = RectangleTopTemplateName, Type = typeof(Rectangle))]
    [TemplatePart(Name = RectangleRightTemplateName, Type = typeof(Rectangle))]
    [TemplatePart(Name = RectangleBottomTemplateName, Type = typeof(Rectangle))]
    [TemplatePart(Name = BorderTemplateName, Type = typeof(Border))]
    [TemplatePart(Name = WrapPanelTemplateName, Type = typeof(WrapPanel))]
    [TemplatePart(Name = ButtonSaveTemplateName, Type = typeof(Button))]
    [TemplatePart(Name = ButtonCancelTemplateName, Type = typeof(Button))]
    [TemplatePart(Name = ButtonCompleteTemplateName, Type = typeof(Button))]

    public class ScreenCut : Window
    {
        private const string CanvasTemplateName = "PART_Canvas";
        private const string RectangleLeftTemplateName = "PART_RectangleLeft";
        private const string RectangleTopTemplateName = "PART_RectangleTop";
        private const string RectangleRightTemplateName = "PART_RectangleRight";
        private const string RectangleBottomTemplateName = "PART_RectangleBottom";
        private const string BorderTemplateName = "PART_Border";
        private const string WrapPanelTemplateName = "PART_WrapPanel";
        private const string ButtonSaveTemplateName = "PART_ButtonSave";
        private const string ButtonCancelTemplateName = "PART_ButtonCancel";
        private const string ButtonCompleteTemplateName = "PART_ButtonComplete";

        private Canvas _canvas;
        private Rectangle _rectangleLeft, _rectangleTop, _rectangleRight, _rectangleBottom;
        private Border _border;
        private WrapPanel _wrapPanel;
        private Button _buttonSave, _buttonCancel, _buttonComplete;
        private Rect rect;
        private Point pointStart, pointEnd;
        private bool isMouseUp = false;
        private Win32ApiHelper.DeskTopSize size;

        static ScreenCut()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScreenCut), new FrameworkPropertyMetadata(typeof(ScreenCut)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _canvas = GetTemplateChild(CanvasTemplateName) as Canvas;
            _rectangleLeft = GetTemplateChild(RectangleLeftTemplateName) as Rectangle;
            _rectangleTop = GetTemplateChild(RectangleTopTemplateName) as Rectangle;
            _rectangleRight = GetTemplateChild(RectangleRightTemplateName) as Rectangle;
            _rectangleBottom = GetTemplateChild(RectangleBottomTemplateName) as Rectangle;
            _border = GetTemplateChild(BorderTemplateName) as Border;
          
            _wrapPanel = GetTemplateChild(WrapPanelTemplateName) as WrapPanel;
            _buttonSave = GetTemplateChild(ButtonSaveTemplateName) as Button;
            if (_buttonSave != null)
                _buttonSave.Click += _buttonSave_Click;
            _buttonCancel = GetTemplateChild(ButtonCancelTemplateName) as Button;
            if (_buttonCancel != null)
                _buttonCancel.Click += _buttonCancel_Click;
            _buttonComplete = GetTemplateChild(ButtonCompleteTemplateName) as Button;
            if (_buttonComplete != null)
                _buttonComplete.Click += _buttonComplete_Click;
            _canvas.Background = new ImageBrush(Capture());
            _rectangleLeft.Width = _canvas.Width;
            _rectangleLeft.Height = _canvas.Height;
        }


        private void _buttonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = $"WPFDevelopers{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg";
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "image file|*.jpg";

            if (dlg.ShowDialog() == true)
            {
                BitmapEncoder pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(CutBitmap()));
                using (var fs = System.IO.File.OpenWrite(dlg.FileName))
                {
                    pngEncoder.Save(fs);
                    fs.Dispose();
                    fs.Close();
                }
            }
            Close();
        }

        private void _buttonComplete_Click(object sender, RoutedEventArgs e)
        {

            Clipboard.SetImage(CutBitmap());
            Close();
        }
        CroppedBitmap CutBitmap()
        {
            _border.Visibility = Visibility.Collapsed;
            _rectangleLeft.Visibility = Visibility.Collapsed;
            _rectangleTop.Visibility = Visibility.Collapsed;
            _rectangleRight.Visibility = Visibility.Collapsed;
            _rectangleBottom.Visibility = Visibility.Collapsed;
            var renderTargetBitmap = new RenderTargetBitmap((int)_canvas.Width,
  (int)_canvas.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            renderTargetBitmap.Render(_canvas);
            return new CroppedBitmap(renderTargetBitmap, new Int32Rect((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height));
        }
        private void _buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            pointStart = e.GetPosition(_canvas);
            if (!isMouseUp)
            {
                _wrapPanel.Visibility = Visibility.Hidden;
                pointEnd = pointStart;
                rect = new Rect(pointStart, pointEnd);
            }
        }
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var current = e.GetPosition(_canvas);
                if (!isMouseUp)
                {
                    MoveAllRectangle(current);
                }

                else
                {
                    if (current != pointStart)
                    {
                        var vector = Point.Subtract(current, pointStart);
                        var left = Canvas.GetLeft(_border) + vector.X;
                        var top = Canvas.GetTop(_border) + vector.Y;
                        if (left <= 0)
                            left = 0;
                        if (top <= 0)
                            top = 0;
                        if (left + _border.Width >= _canvas.ActualWidth)
                            left = _canvas.ActualWidth - _border.ActualWidth;
                        if (top + _border.Height >= _canvas.ActualHeight)
                            top = _canvas.ActualHeight - _border.ActualHeight;
                        pointStart = current;

                        Canvas.SetLeft(_border, left);
                        Canvas.SetTop(_border, top);
                        rect = new Rect(new Point(left, top), new Point(left + _border.Width, top + _border.Height));
                        _rectangleLeft.Width = left <= 0 ? 0 : left >= _canvas.ActualWidth ? _canvas.ActualWidth : left;
                        _rectangleLeft.Height = _canvas.ActualHeight;

                        Canvas.SetLeft(_rectangleTop, _rectangleLeft.Width);
                        _rectangleTop.Height = top <= 0 ? 0 : top >= _canvas.ActualHeight ? _canvas.ActualHeight : top;

                        Canvas.SetLeft(_rectangleRight, left + _border.Width);
                        var wRight = _canvas.ActualWidth - (_border.Width + _rectangleLeft.Width);
                        _rectangleRight.Width = wRight <= 0 ? 0 : wRight;
                        _rectangleRight.Height = _canvas.ActualHeight;

                        Canvas.SetLeft(_rectangleBottom, _rectangleLeft.Width);
                        Canvas.SetTop(_rectangleBottom, top + _border.Height);
                        _rectangleBottom.Width = _border.Width;
                        var hBottom = _canvas.ActualHeight - (top + _border.Height);
                        _rectangleBottom.Height = hBottom <= 0 ? 0 : hBottom;
                    }

                }

            }
        }


        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _wrapPanel.Visibility = Visibility.Visible;
            Canvas.SetLeft(_wrapPanel, rect.X + rect.Width - _wrapPanel.ActualWidth);
            var y = Canvas.GetTop(_border) + _border.ActualHeight + _wrapPanel.ActualHeight;
            if (y > _canvas.ActualHeight)
                y = Canvas.GetTop(_border) - _wrapPanel.ActualHeight - 4;
            else
                y = Canvas.GetTop(_border) + _border.ActualHeight + 4;
            Canvas.SetTop(_wrapPanel, y);
            isMouseUp = true;
        }

        void MoveAllRectangle(Point current)
        {
            pointEnd = current;
            rect = new Rect(pointStart, pointEnd);
            _rectangleLeft.Width = rect.X;
            _rectangleLeft.Height = _canvas.Height;

            Canvas.SetLeft(_rectangleTop, _rectangleLeft.Width);
            _rectangleTop.Width = rect.Width;
            double h = 0.0;
            if (current.Y < pointStart.Y)
                h = current.Y;
            else
                h = current.Y - rect.Height;

            _rectangleTop.Height = h;

            Canvas.SetLeft(_rectangleRight, _rectangleLeft.Width + rect.Width);
            _rectangleRight.Width = _canvas.Width - (rect.Width + _rectangleLeft.Width);
            _rectangleRight.Height = _canvas.Height;

            Canvas.SetLeft(_rectangleBottom, _rectangleLeft.Width);
            Canvas.SetTop(_rectangleBottom, rect.Height + _rectangleTop.Height);
            _rectangleBottom.Width = rect.Width;
            _rectangleBottom.Height = _canvas.Height - (rect.Height + _rectangleTop.Height);

            _border.Height = rect.Height;
            _border.Width = rect.Width;
            Canvas.SetLeft(_border, rect.X);
            Canvas.SetTop(_border, rect.Y);
        }
        BitmapSource Capture()
        {

            IntPtr hBitmap;
            IntPtr hDC = Win32ApiHelper.GetDC(Win32ApiHelper.GetDesktopWindow());
            IntPtr hMemDC = Win32ApiHelper.CreateCompatibleDC(hDC);
            size.cx = Win32ApiHelper.GetSystemMetrics(0);
            size.cy = Win32ApiHelper.GetSystemMetrics(1);
            hBitmap = Win32ApiHelper.CreateCompatibleBitmap(hDC, size.cx, size.cy);
            if (hBitmap != IntPtr.Zero)
            {
                IntPtr hOld = (IntPtr)Win32ApiHelper.SelectObject(hMemDC, hBitmap);
                Win32ApiHelper.BitBlt(hMemDC, 0, 0, size.cx, size.cy, hDC, 0, 0, Win32ApiHelper.TernaryRasterOperations.SRCCOPY);
                Win32ApiHelper.SelectObject(hMemDC, hOld);
                Win32ApiHelper.DeleteDC(hMemDC);
                Win32ApiHelper.ReleaseDC(Win32ApiHelper.GetDesktopWindow(), hDC);
                var bsource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                Win32ApiHelper.DeleteObject(hBitmap);
                GC.Collect();
                return bsource;
            }
            return null;
        }

    }
}
