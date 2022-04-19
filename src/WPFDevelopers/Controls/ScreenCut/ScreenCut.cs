using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public enum ScreenCutMouseType
    {
        Default,
        DrawMouse,
        MoveMouse,
        DrawRectangle,
        DrawEllipse,
        DrawArrow
    }
    [TemplatePart(Name = CanvasTemplateName, Type = typeof(Canvas))]
    [TemplatePart(Name = RectangleLeftTemplateName, Type = typeof(Rectangle))]
    [TemplatePart(Name = RectangleTopTemplateName, Type = typeof(Rectangle))]
    [TemplatePart(Name = RectangleRightTemplateName, Type = typeof(Rectangle))]
    [TemplatePart(Name = RectangleBottomTemplateName, Type = typeof(Rectangle))]
    [TemplatePart(Name = BorderTemplateName, Type = typeof(Border))]
    [TemplatePart(Name = EditBarTemplateName, Type = typeof(Border))]
    [TemplatePart(Name = ButtonSaveTemplateName, Type = typeof(Button))]
    [TemplatePart(Name = ButtonCancelTemplateName, Type = typeof(Button))]
    [TemplatePart(Name = ButtonCompleteTemplateName, Type = typeof(Button))]
    [TemplatePart(Name = RadioButtonRectangleTemplateName, Type = typeof(RadioButton))]
    [TemplatePart(Name = RadioButtonEllipseTemplateName, Type = typeof(RadioButton))]
    [TemplatePart(Name = RadioButtonArrowTemplateName, Type = typeof(RadioButton))]
    [TemplatePart(Name = PopupTemplateName, Type = typeof(Popup))]
    [TemplatePart(Name = PopupBorderTemplateName, Type = typeof(Border))]
    [TemplatePart(Name = WrapPanelColorTemplateName, Type = typeof(WrapPanel))]
    public class ScreenCut : Window
    {
        private const string CanvasTemplateName = "PART_Canvas";
        private const string RectangleLeftTemplateName = "PART_RectangleLeft";
        private const string RectangleTopTemplateName = "PART_RectangleTop";
        private const string RectangleRightTemplateName = "PART_RectangleRight";
        private const string RectangleBottomTemplateName = "PART_RectangleBottom";
        private const string BorderTemplateName = "PART_Border";
        private const string EditBarTemplateName = "PART_EditBar";
        private const string ButtonSaveTemplateName = "PART_ButtonSave";
        private const string ButtonCancelTemplateName = "PART_ButtonCancel";
        private const string ButtonCompleteTemplateName = "PART_ButtonComplete";
        private const string RadioButtonRectangleTemplateName = "PART_RadioButtonRectangle";
        private const string RadioButtonEllipseTemplateName = "PART_RadioButtonEllipse";
        private const string RadioButtonArrowTemplateName = "PART_RadioButtonArrow";
        private const string PopupTemplateName = "PART_Popup";
        private const string PopupBorderTemplateName = "PART_PopupBorder";
        private const string WrapPanelColorTemplateName = "PART_WrapPanelColor";

        private Canvas _canvas;
        private Rectangle _rectangleLeft, _rectangleTop, _rectangleRight, _rectangleBottom;
        private Border _border, _editBar, _popupBorder;
        private Button _buttonSave, _buttonCancel, _buttonComplete;
        private RadioButton _radioButtonRectangle, _radioButtonEllipse, _radioButtonArrow;
        private Popup _popup;
        private WrapPanel _wrapPanel;
        private Rect rect;
        private Point pointStart, pointEnd;
        private bool isMouseUp = false;
        private Win32ApiHelper.DeskTopSize size;
        private ScreenCutMouseType screenCutMouseType = ScreenCutMouseType.Default;
        private AdornerLayer adornerLayer;
        private ScreenCutAdorner screenCutAdorner;
        /// <summary>
        /// 当前绘制矩形
        /// </summary>
        private Border borderRectangle;
        /// <summary>
        /// 绘制当前椭圆
        /// </summary>
        private Ellipse drawEllipse;
        /// <summary>
        /// 当前选择颜色
        /// </summary>
        private Brush _currentBrush;
        /// <summary>
        /// 箭头
        /// </summary>
        private Control controlArrow;
        private ControlTemplate controlTemplate;

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
            _border.MouseLeftButtonDown += _border_MouseLeftButtonDown;

            _editBar = GetTemplateChild(EditBarTemplateName) as Border;
            _buttonSave = GetTemplateChild(ButtonSaveTemplateName) as Button;
            if (_buttonSave != null)
                _buttonSave.Click += _buttonSave_Click;
            _buttonCancel = GetTemplateChild(ButtonCancelTemplateName) as Button;
            if (_buttonCancel != null)
                _buttonCancel.Click += _buttonCancel_Click;
            _buttonComplete = GetTemplateChild(ButtonCompleteTemplateName) as Button;
            if (_buttonComplete != null)
                _buttonComplete.Click += _buttonComplete_Click;
            _radioButtonRectangle = GetTemplateChild(RadioButtonRectangleTemplateName) as RadioButton;
            if (_radioButtonRectangle != null)
                _radioButtonRectangle.Click += _radioButtonRectangle_Click;
            _radioButtonEllipse = GetTemplateChild(RadioButtonEllipseTemplateName) as RadioButton;
            if (_radioButtonEllipse != null)
                _radioButtonEllipse.Click += _radioButtonEllipse_Click;
            _radioButtonArrow = GetTemplateChild(RadioButtonArrowTemplateName) as RadioButton;
            if(_radioButtonArrow != null)
                _radioButtonArrow.Click += _radioButtonArrow_Click;
            _canvas.Background = new ImageBrush(Capture());
            _rectangleLeft.Width = _canvas.Width;
            _rectangleLeft.Height = _canvas.Height;
            _border.Opacity = 0;
            _popup = GetTemplateChild(PopupTemplateName) as Popup;
            _popupBorder = GetTemplateChild(PopupBorderTemplateName) as Border;
            _popupBorder.Loaded += (s, e) => 
            {
                _popup.HorizontalOffset = -_popupBorder.ActualWidth / 3 + 6;
            };
            _wrapPanel = GetTemplateChild(WrapPanelColorTemplateName) as WrapPanel;
            _wrapPanel.PreviewMouseDown += _wrapPanel_PreviewMouseDown;

            controlTemplate = (ControlTemplate)FindResource("PART_DrawArrow");
        }

        private void _radioButtonArrow_Click(object sender, RoutedEventArgs e)
        {
            RadioButtonChecked(_radioButtonArrow, ScreenCutMouseType.DrawArrow);
        }

        private void _wrapPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is RadioButton)
            {
                var radioButton = (RadioButton)e.Source;
                _currentBrush = radioButton.Background;
            }
        }

        private void _radioButtonRectangle_Click(object sender, RoutedEventArgs e)
        {
            RadioButtonChecked(_radioButtonRectangle, ScreenCutMouseType.DrawRectangle);
        }
        private void _radioButtonEllipse_Click(object sender, RoutedEventArgs e)
        {
            RadioButtonChecked(_radioButtonEllipse, ScreenCutMouseType.DrawEllipse);
        }
        void RadioButtonChecked(RadioButton radioButton, ScreenCutMouseType screenCutMouseTypeRadio)
        {
            if (radioButton.IsChecked == true)
            {
                screenCutMouseType = screenCutMouseTypeRadio;
                _border.Cursor = Cursors.Arrow;
                if (_popup.PlacementTarget != null && _popup.IsOpen)
                    _popup.IsOpen = false;
                _popup.PlacementTarget = radioButton;
                _popup.IsOpen = true;
            }
            else
            {
                if (screenCutMouseType == screenCutMouseTypeRadio)
                    Restore();

            }
        }
        void Restore()
        {
            _border.Cursor = Cursors.SizeAll;
            if (screenCutMouseType == ScreenCutMouseType.Default) return;
            screenCutMouseType = ScreenCutMouseType.Default;
            if (_popup.PlacementTarget != null && _popup.IsOpen)
                _popup.IsOpen = false;
        }
        void ResoreRadioButton()
        {
            _radioButtonRectangle.IsChecked = false;
            _radioButtonEllipse.IsChecked = false;
        }
        private void _border_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var left = Canvas.GetLeft(_border);
            var top = Canvas.GetTop(_border);
            var beignPoint = new Point(left, top);
            var endPoint = new Point(left + _border.ActualWidth, top + _border.ActualHeight);
            rect = new Rect(beignPoint, endPoint);
            pointStart = beignPoint;
            MoveAllRectangle(endPoint);
            EditBarPosition(); 
        }

        private void _border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (screenCutMouseType == ScreenCutMouseType.Default)
                screenCutMouseType = ScreenCutMouseType.MoveMouse;
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
                screenCutMouseType = ScreenCutMouseType.DrawMouse;
                _editBar.Visibility = Visibility.Hidden;
                pointEnd = pointStart;
                rect = new Rect(pointStart, pointEnd);
            }
        }
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var current = e.GetPosition(_canvas);
                switch (screenCutMouseType)
                {
                    case ScreenCutMouseType.DrawMouse:
                        MoveAllRectangle(current);
                        break;
                    case ScreenCutMouseType.MoveMouse:
                        MoveRect(current);
                        break;
                    case ScreenCutMouseType.DrawRectangle:
                    case ScreenCutMouseType.DrawEllipse:
                        DrawMultipleControl(current);
                        break;
                    case ScreenCutMouseType.DrawArrow:
                        DrawArrowControl(current);
                        break;
                    default:
                        break;
                }

            }
        }
        void CheckPoint(Point current)
        {
            if (current == pointStart) return;

            if (current.X > rect.BottomRight.X
                ||
                current.Y > rect.BottomRight.Y)
                return;
        }
        void DrawArrowControl(Point current)
        {
            CheckPoint(current);
            if (screenCutMouseType != ScreenCutMouseType.DrawArrow)
                return;

            var drawArrow = new Rect(pointStart, current);

            if (controlArrow == null)
            {
                controlArrow = new Control();
                controlArrow.Background = _currentBrush == null ? Brushes.Red : _currentBrush;
                controlArrow.Template = controlTemplate;
                _canvas.Children.Add(controlArrow);
                Canvas.SetLeft(controlArrow, drawArrow.Left);
                Canvas.SetTop(controlArrow, drawArrow.Top - 7.5);
            }
            var rotate = new RotateTransform();
            var renderOrigin = new Point(0, .5);
            controlArrow.RenderTransformOrigin = renderOrigin;
            controlArrow.RenderTransform = rotate;
            rotate.Angle = ControlsHelper.CalculeAngle(pointStart, current);
           
            if (drawArrow.Left <= rect.Left)
            {
                var a = rect.Left - pointStart.X;
                var d = current.X - rect.Left;
                var bc = current.Y - pointStart.Y;
                var c = bc / (a / d + 1);
                var b = a / d * c;

                if (current.Y > pointStart.Y)
                    current.Y = pointStart.Y + b;
                else
                    current.Y = pointStart.Y - b;
                current.X = pointStart.X + a;
            }
            else if (drawArrow.Right >= rect.Right)
            {
                var a = rect.Right - pointStart.X;
                var d = current.X - rect.Right;
                var bc = current.Y - pointStart.Y;
                var c = bc / (a / d + 1);
                var b = a / d * c;
                if (current.Y > pointStart.Y)
                    current.Y = pointStart.Y + b;
                else
                    current.Y = pointStart.Y - b;
                current.X = pointStart.X + a;
            }
            else if (drawArrow.Top <= rect.Top)
            {
                var a = rect.Top - pointStart.Y;
                var d = current.Y - rect.Top;
                var bc = current.X - pointStart.X;
                var c = bc / (a / d + 1);
                var b = a / d * c;

                if (current.X > pointStart.X)
                    current.X = pointStart.X + b;
                else
                    current.X = pointStart.X - b;
                current.Y = pointStart.Y + a;
            }
            else if(drawArrow.Bottom >= rect.Bottom)
            {
                var a = rect.Bottom - pointStart.Y;
                var d = current.Y - rect.Bottom;
                var bc = current.X - pointStart.X;
                var c = bc / (a / d + 1);
                var b = a / d * c;
                if (current.X > pointStart.X)
                    current.X = pointStart.X + b;
                else
                    current.X = pointStart.X - b;
                current.Y = pointStart.Y + a;
            }
            var x = current.X - pointStart.X;
            var y = current.Y - pointStart.Y;



            var width = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));

            

            controlArrow.Width = width;
        }
        
        void DrawMultipleControl(Point current)
        {
            CheckPoint(current);
            var drawRect = new Rect(pointStart, current);
            switch (screenCutMouseType)
            {
                case ScreenCutMouseType.DrawRectangle:
                    if (borderRectangle == null)
                    {
                        borderRectangle = new Border()
                        {
                            BorderBrush = _currentBrush == null ? Brushes.Red : _currentBrush,
                            BorderThickness = new Thickness(3),
                            CornerRadius = new CornerRadius(3),
                        };
                        _canvas.Children.Add(borderRectangle);
                    }
                    break;
                case ScreenCutMouseType.DrawEllipse:
                    if (drawEllipse == null)
                    {
                        drawEllipse = new Ellipse()
                        {
                            Stroke = _currentBrush == null ? Brushes.Red : _currentBrush,
                            StrokeThickness = 3,
                        };
                        _canvas.Children.Add(drawEllipse);
                    }
                    break;
               
            }
           
            var _borderLeft = drawRect.Left - Canvas.GetLeft(_border);
           
            if (_borderLeft < 0)
                _borderLeft = Math.Abs(_borderLeft);
            if (drawRect.Width + _borderLeft < _border.ActualWidth)
            {
                var wLeft = Canvas.GetLeft(_border) + _border.ActualWidth;
                var left = drawRect.Left < Canvas.GetLeft(_border) ? Canvas.GetLeft(_border) : drawRect.Left > wLeft ? wLeft : drawRect.Left;
                if (borderRectangle != null)
                {
                    borderRectangle.Width = drawRect.Width;
                    Canvas.SetLeft(borderRectangle, left);
                }
                if (drawEllipse != null)
                {
                    drawEllipse.Width = drawRect.Width;
                    Canvas.SetLeft(drawEllipse, left);
                }
             
               
            }
           
            var _borderTop = drawRect.Top - Canvas.GetTop(_border);
            if(_borderTop < 0)
                _borderTop = Math.Abs(_borderTop);
            if (drawRect.Height + _borderTop < _border.ActualHeight)
            {
                var hTop = Canvas.GetTop(_border) + _border.Height;
                var top = drawRect.Top < Canvas.GetTop(_border) ? Canvas.GetTop(_border) : drawRect.Top > hTop ? hTop : drawRect.Top;
                if (borderRectangle != null)
                {
                    borderRectangle.Height = drawRect.Height;
                    Canvas.SetTop(borderRectangle, top);
                }

                if (drawEllipse != null)
                {
                    drawEllipse.Height = drawRect.Height;
                    Canvas.SetTop(drawEllipse, top);
                }

            }
        }
        void MoveRect(Point current)
        {
            if (current != pointStart)
            {
                Console.WriteLine($"current:{current}");
                Console.WriteLine($"pointStart:{pointStart}");
                var vector = Point.Subtract(current, pointStart);
                var left = Canvas.GetLeft(_border) + vector.X;
                var top = Canvas.GetTop(_border) + vector.Y;
                Console.WriteLine($"left:{left}");
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
                _rectangleLeft.Height = _canvas.ActualHeight;
                _rectangleLeft.Width = left <= 0 ? 0 : left >= _canvas.ActualWidth ? _canvas.ActualWidth : left;


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

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (e.Source is ToggleButton)
                return;
           
            isMouseUp = true;
            if (screenCutMouseType != ScreenCutMouseType.Default)
            {
                if (screenCutMouseType == ScreenCutMouseType.MoveMouse)
                    EditBarPosition();

                if (_radioButtonRectangle.IsChecked != true
                    &&
                    _radioButtonEllipse.IsChecked != true
                    &&
                    _radioButtonArrow.IsChecked != true)
                    screenCutMouseType = ScreenCutMouseType.Default;
                else
                {
                    borderRectangle = null;
                    drawEllipse = null;
                    controlArrow = null;
                }
                   
            }
               
        }
        void EditBarPosition()
        {
            _editBar.Visibility = Visibility.Visible;
            Canvas.SetLeft(_editBar, rect.X + rect.Width - _editBar.ActualWidth);
            var y = Canvas.GetTop(_border) + _border.ActualHeight + _editBar.ActualHeight + _popupBorder.ActualHeight + 24;
            if (y > _canvas.ActualHeight)
                y = Canvas.GetTop(_border) - _editBar.ActualHeight - 8;
            else
                y = Canvas.GetTop(_border) + _border.ActualHeight + 8;
            Canvas.SetTop(_editBar, y);
            if (_popup != null && _popup.IsOpen)
            {
                _popup.IsOpen = false;
                _popup.IsOpen = true;
            }
        }
        void MoveAllRectangle(Point current)
        {
            pointEnd = current;
            rect = new Rect(pointStart, pointEnd);
            _rectangleLeft.Width = rect.X < 0 ? 0 : rect.X > _canvas.ActualWidth ? _canvas.ActualWidth : rect.X;
            _rectangleLeft.Height = _canvas.Height;

            Canvas.SetLeft(_rectangleTop, _rectangleLeft.Width);
            _rectangleTop.Width = rect.Width;
            double h = 0.0;
            if (current.Y < pointStart.Y)
                h = current.Y;
            else
                h = current.Y - rect.Height;

            _rectangleTop.Height = h < 0 ? 0 : h > _canvas.ActualHeight ? _canvas.ActualHeight : h;

            Canvas.SetLeft(_rectangleRight, _rectangleLeft.Width + rect.Width);
            var rWidth = _canvas.Width - (rect.Width + _rectangleLeft.Width);
            _rectangleRight.Width = rWidth < 0 ? 0 : rWidth > _canvas.ActualWidth ? _canvas.ActualWidth : rWidth;

            _rectangleRight.Height = _canvas.Height;

            Canvas.SetLeft(_rectangleBottom, _rectangleLeft.Width);
            Canvas.SetTop(_rectangleBottom, rect.Height + _rectangleTop.Height);
            _rectangleBottom.Width = rect.Width;
            var rBottomHeight = _canvas.Height - (rect.Height + _rectangleTop.Height);
            _rectangleBottom.Height = rBottomHeight < 0 ?0: rBottomHeight;

            _border.Height = rect.Height;
            _border.Width = rect.Width;
            Canvas.SetLeft(_border, rect.X);
            Canvas.SetTop(_border, rect.Y);

            if (adornerLayer != null) return;
            _border.Opacity = 1;
            adornerLayer = AdornerLayer.GetAdornerLayer(_border);
            screenCutAdorner = new ScreenCutAdorner(_border);
            screenCutAdorner.PreviewMouseDown += (s, e) => 
            {
                Restore();
                ResoreRadioButton();
            };
            adornerLayer.Add(screenCutAdorner);
            _border.SizeChanged += _border_SizeChanged;
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
