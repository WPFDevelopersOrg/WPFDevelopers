using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using WPFDevelopers.Helpers;
using RadioButton = System.Windows.Controls.RadioButton;
using Button = System.Windows.Controls.Button;
using Cursors = System.Windows.Input.Cursors;
using Control = System.Windows.Controls.Control;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using TextBox = System.Windows.Controls.TextBox;
using System.Collections.Generic;
using System.Drawing;
using Point = System.Windows.Point;
using Brush = System.Windows.Media.Brush;
using Rectangle = System.Windows.Shapes.Rectangle;
using Brushes = System.Windows.Media.Brushes;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Drawing.Imaging;
using System.Windows.Threading;

namespace WPFDevelopers.Controls
{
    public static class ScreenExtensions
    {
        public static void GetDpi(this Screen screen, DpiType dpiType, out uint dpiX, out uint dpiY)
        {
            var pnt = new System.Drawing.Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
            var mon = MonitorFromPoint(pnt, 2);
            GetDpiForMonitor(mon, dpiType, out dpiX, out dpiY);
        }
        [DllImport(Win32.User32)]
        private static extern IntPtr MonitorFromPoint([In] System.Drawing.Point pt, [In] uint dwFlags);
        [DllImport(Win32.Shcore)]
        private static extern IntPtr GetDpiForMonitor([In] IntPtr hmonitor, [In] DpiType dpiType, [Out] out uint dpiX, [Out] out uint dpiY);
    }
    public struct ScreenDPI {
        public uint dpiX;
        public uint dpiY;
        public float scaleX;
        public float scaleY;
    }
    public enum DpiType
    {
        Effective = 0,
        Angular = 1,
        Raw = 2,
    }
    public enum ScreenCutMouseType
    {
        Default,
        DrawMouse,
        MoveMouse,
        DrawRectangle,
        DrawEllipse,
        DrawArrow,
        DrawText,
        DrawInk
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
    [TemplatePart(Name = RadioButtonInkTemplateName, Type = typeof(RadioButton))]
    [TemplatePart(Name = RadioButtonTextTemplateName, Type = typeof(RadioButton))]
    [TemplatePart(Name = PopupTemplateName, Type = typeof(Popup))]
    [TemplatePart(Name = PopupBorderTemplateName, Type = typeof(Border))]
    [TemplatePart(Name = WrapPanelColorTemplateName, Type = typeof(WrapPanel))]

    public class ScreenCut : Window, IDisposable
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
        private const string RadioButtonInkTemplateName = "PART_RadioButtonInk";
        private const string RadioButtonTextTemplateName = "PART_RadioButtonText";
        private const string PopupTemplateName = "PART_Popup";
        private const string PopupBorderTemplateName = "PART_PopupBorder";
        private const string WrapPanelColorTemplateName = "PART_WrapPanelColor";

        private const string _tag = "Draw";
        private const int _width = 40;
        private Border _border, _editBar, _popupBorder;
        private Button _buttonSave, _buttonCancel, _buttonComplete;
        private Canvas _canvas;

        /// <summary>
        ///     当前选择颜色
        /// </summary>
        private Brush _currentBrush;

        private Popup _popup;

        private RadioButton _radioButtonRectangle,
            _radioButtonEllipse,
            _radioButtonArrow,
            _radioButtonInk,
            _radioButtonText;

        private Rectangle _rectangleLeft, _rectangleTop, _rectangleRight, _rectangleBottom;
        private WrapPanel _wrapPanel;
        private AdornerLayer _adornerLayer;

        /// <summary>
        ///     当前绘制矩形
        /// </summary>
        private Border _borderRectangle;

        /// <summary>
        ///     当前箭头
        /// </summary>
        private Control _controlArrow;

        private ControlTemplate _controlTemplate;

        /// <summary>
        ///     绘制当前椭圆
        /// </summary>
        private Ellipse _drawEllipse;

        private FrameworkElement _frameworkElement;
        private bool _isMouseUp;
        private Point? _pointStart, _pointEnd;

        /// <summary>
        ///     当前画笔
        /// </summary>
        private Polyline _polyLine;

        private Rect _rect;
        private ScreenCutAdorner _screenCutAdorner;
        private ScreenCutMouseType _screenCutMouseType = ScreenCutMouseType.Default;

        /// <summary>
        ///     当前文本
        /// </summary>
        private Border _textBorder;

        /// <summary>
        /// 截图完成委托
        /// </summary>
        public delegate void ScreenShotDone(CroppedBitmap bitmap);
        /// <summary>
        /// 截图完成事件
        /// </summary>
        public event ScreenShotDone CutCompleted;
        /// <summary>
        /// 截图取消委托
        /// </summary>
        public delegate void ScreenShotCanceled();
        /// <summary>
        /// 截图取消事件
        /// </summary>
        public event ScreenShotCanceled CutCanceled;
        /// <summary>
        /// 获取保存的图片全路径
        /// </summary>
        public event Action<string> CutFullPath;
        private double _y1;
        private int _screenIndex;
        public static int CaptureScreenID = -1;
        private Bitmap _screenCapture;
        private ScreenDPI _screenDPI;
        public ScreenCut(int index)
        {
            _screenIndex = index;
            this.Left = Screen.AllScreens[_screenIndex].WorkingArea.Left;
            ShowInTaskbar = false;
            _screenDPI = GetScreenDPI(_screenIndex);
        }
        static ScreenCut()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScreenCut),
                new FrameworkPropertyMetadata(typeof(ScreenCut)));
        }

        public void Dispose()
        {
            _canvas.Background = null;
            GC.SuppressFinalize(this);
            GC.Collect();
        }
        public static void ClearCaptureScreenID()
        {
            CaptureScreenID = -1;
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
            _border.MouseLeftButtonDown += Border_MouseLeftButtonDown;

            _editBar = GetTemplateChild(EditBarTemplateName) as Border;
            _buttonSave = GetTemplateChild(ButtonSaveTemplateName) as Button;
            if (_buttonSave != null)
                _buttonSave.Click += ButtonSave_Click;
            _buttonCancel = GetTemplateChild(ButtonCancelTemplateName) as Button;
            if (_buttonCancel != null)
                _buttonCancel.Click += ButtonCancel_Click;
            _buttonComplete = GetTemplateChild(ButtonCompleteTemplateName) as Button;
            if (_buttonComplete != null)
                _buttonComplete.Click += ButtonComplete_Click;
            _radioButtonRectangle = GetTemplateChild(RadioButtonRectangleTemplateName) as RadioButton;
            if (_radioButtonRectangle != null)
                _radioButtonRectangle.Click += RadioButtonRectangle_Click;
            _radioButtonEllipse = GetTemplateChild(RadioButtonEllipseTemplateName) as RadioButton;
            if (_radioButtonEllipse != null)
                _radioButtonEllipse.Click += RadioButtonEllipse_Click;
            _radioButtonArrow = GetTemplateChild(RadioButtonArrowTemplateName) as RadioButton;
            if (_radioButtonArrow != null)
                _radioButtonArrow.Click += RadioButtonArrow_Click;
            _radioButtonInk = GetTemplateChild(RadioButtonInkTemplateName) as RadioButton;
            if (_radioButtonInk != null)
                _radioButtonInk.Click += RadioButtonInk_Click;
            _radioButtonText = GetTemplateChild(RadioButtonTextTemplateName) as RadioButton;
            if (_radioButtonText != null)
                _radioButtonText.Click += RadioButtonText_Click;
            _canvas.Width = Screen.AllScreens[_screenIndex].Bounds.Width;
            _canvas.Height = Screen.AllScreens[_screenIndex].Bounds.Height;
            //_canvas.Background = new ImageBrush(ControlsHelper.Capture());
            _canvas.Background = new ImageBrush(ImagingHelper.CreateBitmapSourceFromBitmap(CopyScreen()));
            _rectangleLeft.Width = _canvas.Width;
            _rectangleLeft.Height = _canvas.Height;
            _border.Opacity = 0;
            _popup = GetTemplateChild(PopupTemplateName) as Popup;
            _popupBorder = GetTemplateChild(PopupBorderTemplateName) as Border;
            _popupBorder.Loaded += (s, e) => { _popup.HorizontalOffset = -_popupBorder.ActualWidth / 3; };
            _wrapPanel = GetTemplateChild(WrapPanelColorTemplateName) as WrapPanel;
            _wrapPanel.PreviewMouseDown += WrapPanel_PreviewMouseDown;
            Loaded += ScreenCut_Loaded;
            _controlTemplate = (ControlTemplate)FindResource("WD.PART_DrawArrow");
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Dispose();
        }

        public static BitmapSource BitmapSourceFromBrush(Brush drawingBrush, int x = 32, int y = 32, int dpi = 96)
        {
            // RenderTargetBitmap = builds a bitmap rendering of a visual
            var pixelFormat = PixelFormats.Pbgra32;
            RenderTargetBitmap rtb = new RenderTargetBitmap(x, y, dpi, dpi, pixelFormat);

            // Drawing visual allows us to compose graphic drawing parts into a visual to render
            var drawingVisual = new DrawingVisual();
            using (DrawingContext context = drawingVisual.RenderOpen())
            {
                // Declaring drawing a rectangle using the input brush to fill up the visual
                context.DrawRectangle(drawingBrush, null, new Rect(0, 0, x, y));
            }

            // Actually rendering the bitmap
            rtb.Render(drawingVisual);
            return rtb;
        }
        private void ScreenCut_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        
        private ScreenDPI GetScreenDPI(int screenIndex)
        {
            ScreenDPI dpi = new ScreenDPI();
            Screen.AllScreens[screenIndex].GetDpi(DpiType.Effective, out dpi.dpiX, out dpi.dpiY);
            dpi.scaleX = (dpi.dpiX / 0.96f) / 100;
            dpi.scaleY = (dpi.dpiY / 0.96f) / 100;
            return dpi;
        }
        private Bitmap CopyScreen()
        {
            Left = Screen.AllScreens[_screenIndex].Bounds.Left / _screenDPI.scaleX;
            Top = Screen.AllScreens[_screenIndex].Bounds.Top / _screenDPI.scaleY;
            Width = Screen.AllScreens[_screenIndex].Bounds.Width / _screenDPI.scaleX;
            Height = Screen.AllScreens[_screenIndex].Bounds.Height / _screenDPI.scaleY;
            _canvas.Width = Screen.AllScreens[_screenIndex].Bounds.Width / _screenDPI.scaleX;
            _canvas.Height = Screen.AllScreens[_screenIndex].Bounds.Height / _screenDPI.scaleY;
            _canvas.SetValue(LeftProperty, Left);
            _canvas.SetValue(TopProperty, Top);
            Bitmap ScreenCapture = new Bitmap(Screen.AllScreens[_screenIndex].Bounds.Width, Screen.AllScreens[_screenIndex].Bounds.Height);
            using (Graphics g = Graphics.FromImage(ScreenCapture)) 
            {
                g.CopyFromScreen(Screen.AllScreens[_screenIndex].Bounds.Left, Screen.AllScreens[_screenIndex].Bounds.Top, 0, 0, Screen.AllScreens[_screenIndex].Bounds.Size);
            }
            return ScreenCapture;
        }
        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            OnCanceled();
        }

        private void RadioButtonInk_Click(object sender, RoutedEventArgs e)
        {
            RadioButtonChecked(_radioButtonInk, ScreenCutMouseType.DrawInk);
        }

        private void RadioButtonText_Click(object sender, RoutedEventArgs e)
        {
            RadioButtonChecked(_radioButtonText, ScreenCutMouseType.DrawText);
        }

        private void RadioButtonArrow_Click(object sender, RoutedEventArgs e)
        {
            RadioButtonChecked(_radioButtonArrow, ScreenCutMouseType.DrawArrow);
        }

        private void WrapPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is RadioButton)
            {
                var radioButton = (RadioButton)e.Source;
                _currentBrush = radioButton.Background;
            }
        }

        private void RadioButtonRectangle_Click(object sender, RoutedEventArgs e)
        {
            RadioButtonChecked(_radioButtonRectangle, ScreenCutMouseType.DrawRectangle);
        }

        private void RadioButtonEllipse_Click(object sender, RoutedEventArgs e)
        {
            RadioButtonChecked(_radioButtonEllipse, ScreenCutMouseType.DrawEllipse);
        }

        private void RadioButtonChecked(RadioButton radioButton, ScreenCutMouseType screenCutMouseTypeRadio)
        {
            if (radioButton.IsChecked == true)
            {
                _screenCutMouseType = screenCutMouseTypeRadio;
                _border.Cursor = Cursors.Arrow;
                if (_popup.PlacementTarget != null && _popup.IsOpen)
                    _popup.IsOpen = false;
                _popup.PlacementTarget = radioButton;
                _popup.IsOpen = true;
                DisposeControl();
            }
            else
            {
                if (_screenCutMouseType == screenCutMouseTypeRadio)
                    Restore();
            }
        }

        private void Restore()
        {
            _border.Cursor = Cursors.SizeAll;
            if (_screenCutMouseType == ScreenCutMouseType.Default) return;
            _screenCutMouseType = ScreenCutMouseType.Default;
            if (_popup.PlacementTarget != null && _popup.IsOpen)
                _popup.IsOpen = false;
        }

        private void ResoreRadioButton()
        {
            _radioButtonRectangle.IsChecked = false;
            _radioButtonEllipse.IsChecked = false;
        }

        private void _border_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_isMouseUp)
            {
                var left = Canvas.GetLeft(_border);
                var top = Canvas.GetTop(_border);
                var beignPoint = new Point(left, top);
                var endPoint = new Point(left + _border.ActualWidth, top + _border.ActualHeight);
                _rect = new Rect(beignPoint, endPoint);
                _pointStart = beignPoint;
                MoveAllRectangle(endPoint);
            }
            EditBarPosition();
        }
        
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_screenCutMouseType == ScreenCutMouseType.Default)
                _screenCutMouseType = ScreenCutMouseType.MoveMouse;
        }
        
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.FileName = $"WPFDevelopers{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg";
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "image file|*.jpg";
            
            if (dlg.ShowDialog() == true)
            {
                BitmapEncoder pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(CutBitmap()));
                using (var fs = File.OpenWrite(dlg.FileName))
                {
                    pngEncoder.Save(fs);
                    fs.Dispose();
                    fs.Close();
                    Close();
                    if (CutFullPath != null)
                        CutFullPath(dlg.FileName);
                }
            }
        }
        
        private void ButtonComplete_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = CutBitmap();
            bitmap.Freeze();
            if (CutCompleted != null)
                CutCompleted(bitmap);
            Close();
        }
        
        private CroppedBitmap CutBitmap()
        {
            _border.Visibility = Visibility.Collapsed;
            _editBar.Visibility = Visibility.Collapsed;
            _rectangleLeft.Visibility = Visibility.Collapsed;
            _rectangleTop.Visibility = Visibility.Collapsed;
            _rectangleRight.Visibility = Visibility.Collapsed;
            _rectangleBottom.Visibility = Visibility.Collapsed;
            // 屏幕等比截图需要考虑缩放率进行等比放大canvas
            var renderTargetBitmap = new RenderTargetBitmap((int)(_canvas.Width * _screenDPI.scaleX),
                (int)(_canvas.Height * _screenDPI.scaleY), _screenDPI.dpiX, _screenDPI.dpiY, PixelFormats.Default);
            renderTargetBitmap.Render(_canvas);
            // 屏幕等比截图需要考虑缩放率进行等比放大截图区域
            var realrect = new Int32Rect(
                (int)(_rect.X * _screenDPI.scaleX),
                (int)(_rect.Y * _screenDPI.scaleY),
                (int)(_rect.Width * _screenDPI.scaleX),
                (int)(_rect.Height * _screenDPI.scaleY));
            //return new CroppedBitmap(renderTargetBitmap,
            //    new Int32Rect((int)_rect.X, (int)_rect.Y, (int)_rect.Width, (int)_rect.Height));
            return new CroppedBitmap(renderTargetBitmap, realrect);
        }
        
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            OnCanceled();
        }
        void OnCanceled()
        {
            Close();
            if (CutCanceled != null)
                CutCanceled();
        }
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                OnCanceled();
            }
            else if (e.Key == Key.Delete)
            {
                if (_canvas.Children.Count > 0)
                    _canvas.Children.Remove(_frameworkElement);
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.Z) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (_canvas.Children.Count > 0)
                    _canvas.Children.Remove(_canvas.Children[_canvas.Children.Count - 1]);
            }
        }
        
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.Source is RadioButton)
                return;

            // if is multi-screen, only one screen is allowed.
            if (CaptureScreenID == -1)
            {
                CaptureScreenID = _screenIndex;
            }
            if (CaptureScreenID != -1 && CaptureScreenID != _screenIndex)
            {
                e.Handled = true;
                return;
            }

            var vPoint = e.GetPosition(_canvas);
            if (!_isMouseUp)
            {
                _pointStart = vPoint;
                _screenCutMouseType = ScreenCutMouseType.DrawMouse;
                _editBar.Visibility = Visibility.Hidden;
                _pointEnd = _pointStart;
                _rect = new Rect(_pointStart.Value, _pointEnd.Value);
            }
            else
            {
                if (vPoint.X < _rect.Left || vPoint.X > _rect.Right)
                    return;

                if (vPoint.Y < _rect.Top || vPoint.Y > _rect.Bottom)
                    return;
                _pointStart = vPoint;
                if (_textBorder != null)
                    Focus();

                switch (_screenCutMouseType)
                {
                    case ScreenCutMouseType.DrawText:
                        _y1 = vPoint.Y;
                        DrawText();
                        break;
                    default:
                        Focus();
                        break;
                }
            }
        }
        
        private void DrawText()
        {
            if (_pointStart.Value.X < _rect.Right
                &&
                _pointStart.Value.X > _rect.Left
                &&
                _pointStart.Value.Y > _rect.Top
                &&
                _pointStart.Value.Y < _rect.Bottom)
            {
                var currentWAndX = _pointStart.Value.X + 40;
                if (_textBorder == null)
                {
                    _textBorder = new Border
                    {
                        BorderBrush = _currentBrush == null ? Brushes.Red : _currentBrush,
                        BorderThickness = new Thickness(1),
                        Tag = _tag
                    };

                    var textBox = new TextBox();
                    textBox.Style = null;
                    textBox.Background = null;
                    textBox.BorderThickness = new Thickness(0);
                    textBox.Foreground = _textBorder.BorderBrush;
                    textBox.FontFamily = DrawingContextHelper.FontFamily;
                    textBox.FontSize = 16;
                    textBox.TextWrapping = TextWrapping.Wrap;
                    textBox.FontWeight = FontWeights.Normal;
                    textBox.MinWidth = _width;
                    textBox.MaxWidth = _rect.Right - _pointStart.Value.X;
                    textBox.MaxHeight = _rect.Height - 4;
                    textBox.Cursor = Cursors.Hand;

                    textBox.Padding = new Thickness(4);
                    textBox.LostKeyboardFocus += (s, e1) =>
                    {
                        var tb = s as TextBox;

                        var parent = VisualTreeHelper.GetParent(tb);
                        if (parent != null && parent is Border border)
                        {
                            border.BorderThickness = new Thickness(0);
                            if (string.IsNullOrWhiteSpace(tb.Text))
                                _canvas.Children.Remove(border);
                        }
                    };
                    _textBorder.SizeChanged += (s, e1) =>
                    {
                        var tb = s as Border;
                        var y = _y1;
                        if (y + tb.ActualHeight > _rect.Bottom)
                        {
                            var v = Math.Abs(_rect.Bottom - (y + tb.ActualHeight));
                            _y1 = y - v;
                            Canvas.SetTop(tb, _y1 + 2);
                        }
                    };
                    _textBorder.PreviewMouseLeftButtonDown += (s, e) =>
                    {
                        _radioButtonText.IsChecked = true;
                        RadioButtonText_Click(null, null);
                        SelectElement();
                        var border = s as Border;
                        _frameworkElement = border;
                        _frameworkElement.Opacity = .7;
                        border.BorderThickness = new Thickness(1);
                    };
                    _textBorder.Child = textBox;
                    _canvas.Children.Add(_textBorder);
                    textBox.Focus();
                    var x = _pointStart.Value.X;

                    if (currentWAndX > _rect.Right)
                        x = x - (currentWAndX - _rect.Right);
                    Canvas.SetLeft(_textBorder, x - 2);
                    Canvas.SetTop(_textBorder, _pointStart.Value.Y - 2);
                }
            }
        }
        
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (e.Source is RadioButton)
                return;

            if (_pointStart is null)
                return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var current = e.GetPosition(_canvas);
                switch (_screenCutMouseType)
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
                    case ScreenCutMouseType.DrawInk:
                        DrwaInkControl(current);
                        break;
                }
            }
        }

        private void CheckPoint(Point current)
        {
            if (current == _pointStart) return;

            if (current.X > _rect.BottomRight.X
                ||
                current.Y > _rect.BottomRight.Y)
                return;
        }
        
        private void DrwaInkControl(Point current)
        {
            CheckPoint(current);
            if (current.X >= _rect.Left
                &&
                current.X <= _rect.Right
                &&
                current.Y >= _rect.Top
                &&
                current.Y <= _rect.Bottom)
            {
                if (_polyLine == null)
                {
                    _polyLine = new Polyline();
                    _polyLine.Stroke = _currentBrush == null ? Brushes.Red : _currentBrush;
                    _polyLine.Cursor = Cursors.Hand;
                    _polyLine.StrokeThickness = 3;
                    _polyLine.StrokeLineJoin = PenLineJoin.Round;
                    _polyLine.StrokeStartLineCap = PenLineCap.Round;
                    _polyLine.StrokeEndLineCap = PenLineCap.Round;
                    _polyLine.MouseLeftButtonDown += (s, e) =>
                    {
                        _radioButtonInk.IsChecked = true;
                        RadioButtonInk_Click(null, null);
                        SelectElement();
                        _frameworkElement = s as Polyline;
                        _frameworkElement.Opacity = .7;
                    };
                    _canvas.Children.Add(_polyLine);
                }

                _polyLine.Points.Add(current);
            }
        }
        
        private void DrawArrowControl(Point current)
        {
            CheckPoint(current);
            if (_screenCutMouseType != ScreenCutMouseType.DrawArrow)
                return;

            if (_pointStart is null)
                return;

            var vPoint = _pointStart.Value;

            var drawArrow = new Rect(vPoint, current);
            if (_controlArrow == null)
            {
                _controlArrow = new Control();
                _controlArrow.Background = _currentBrush == null ? Brushes.Red : _currentBrush;
                _controlArrow.Template = _controlTemplate;
                _controlArrow.Cursor = Cursors.Hand;
                _controlArrow.Tag = _tag;
                _controlArrow.MouseLeftButtonDown += (s, e) =>
                {
                    _radioButtonArrow.IsChecked = true;
                    RadioButtonArrow_Click(null, null);
                    SelectElement();
                    _frameworkElement = s as Control;
                    _frameworkElement.Opacity = .7;
                };
                _canvas.Children.Add(_controlArrow);
                Canvas.SetLeft(_controlArrow, drawArrow.Left);
                Canvas.SetTop(_controlArrow, drawArrow.Top - 7.5);
            }

            var rotate = new RotateTransform();
            var renderOrigin = new Point(0, .5);
            _controlArrow.RenderTransformOrigin = renderOrigin;
            _controlArrow.RenderTransform = rotate;
            rotate.Angle = Helper.CalculeAngle(vPoint, current);
            if (current.X < _rect.Left
                ||
                current.X > _rect.Right
                ||
                current.Y < _rect.Top
                ||
                current.Y > _rect.Bottom)
            {
                if (current.X >= vPoint.X && current.Y < vPoint.Y)
                {
                    var a1 = (current.Y - vPoint.Y) / (current.X - vPoint.X);
                    var b1 = vPoint.Y - a1 * vPoint.X;
                    var xTop = (_rect.Top - b1) / a1;
                    var yRight = a1 * _rect.Right + b1;

                    if (xTop <= _rect.Right)
                    {
                        current.X = xTop;
                        current.Y = _rect.Top;
                    }
                    else
                    {
                        current.X = _rect.Right;
                        current.Y = yRight;
                    }
                }
                else if (current.X > vPoint.X && current.Y > vPoint.Y)
                {
                    var a1 = (current.Y - vPoint.Y) / (current.X - vPoint.X);
                    var b1 = vPoint.Y - a1 * vPoint.X;
                    var xBottom = (_rect.Bottom - b1) / a1;
                    var yRight = a1 * _rect.Right + b1;

                    if (xBottom <= _rect.Right)
                    {
                        current.X = xBottom;
                        current.Y = _rect.Bottom;
                    }
                    else
                    {
                        current.X = _rect.Right;
                        current.Y = yRight;
                    }
                }
                else if (current.X < vPoint.X && current.Y < vPoint.Y)
                {
                    var a1 = (current.Y - vPoint.Y) / (current.X - vPoint.X);
                    var b1 = vPoint.Y - a1 * vPoint.X;
                    var xTop = (_rect.Top - b1) / a1;
                    var yLeft = a1 * _rect.Left + b1;
                    if (xTop >= _rect.Left)
                    {
                        current.X = xTop;
                        current.Y = _rect.Top;
                    }
                    else
                    {
                        current.X = _rect.Left;
                        current.Y = yLeft;
                    }
                }
                else if (current.X < vPoint.X && current.Y > vPoint.Y)
                {
                    var a1 = (current.Y - vPoint.Y) / (current.X - vPoint.X);
                    var b1 = vPoint.Y - a1 * vPoint.X;
                    var xBottom = (_rect.Bottom - b1) / a1;
                    var yLeft = a1 * _rect.Left + b1;

                    if (xBottom <= _rect.Left)
                    {
                        current.X = _rect.Left;
                        current.Y = yLeft;
                    }
                    else
                    {
                        current.X = xBottom;
                        current.Y = _rect.Bottom;
                    }
                }
            }

            var x = current.X - vPoint.X;
            var y = current.Y - vPoint.Y;
            var width = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
            width = width < 15 ? 15 : width;
            _controlArrow.Width = width;
        }
        
        private void DrawMultipleControl(Point current)
        {
            CheckPoint(current);
            if (_pointStart is null)
                return;

            var vPoint = _pointStart.Value;

            var drawRect = new Rect(vPoint, current);
            switch (_screenCutMouseType)
            {
                case ScreenCutMouseType.DrawRectangle:
                    if (_borderRectangle == null)
                    {
                        _borderRectangle = new Border
                        {
                            BorderBrush = _currentBrush == null ? Brushes.Red : _currentBrush,
                            BorderThickness = new Thickness(3),
                            CornerRadius = new CornerRadius(3),
                            Tag = _tag,
                            Cursor = Cursors.Hand
                        };
                        _borderRectangle.MouseLeftButtonDown += (s, e) =>
                        {
                            _radioButtonRectangle.IsChecked = true;
                            RadioButtonRectangle_Click(null, null);
                            SelectElement();
                            _frameworkElement = s as Border;
                            _frameworkElement.Opacity = .7;
                        };
                        _canvas.Children.Add(_borderRectangle);
                    }

                    break;
                case ScreenCutMouseType.DrawEllipse:
                    if (_drawEllipse == null)
                    {
                        _drawEllipse = new Ellipse
                        {
                            Stroke = _currentBrush == null ? Brushes.Red : _currentBrush,
                            StrokeThickness = 3,
                            Tag = _tag,
                            Cursor = Cursors.Hand
                        };
                        _drawEllipse.MouseLeftButtonDown += (s, e) =>
                        {
                            _radioButtonEllipse.IsChecked = true;
                            RadioButtonEllipse_Click(null, null);
                            SelectElement();
                            _frameworkElement = s as Ellipse;
                            _frameworkElement.Opacity = .7;
                        };
                        _canvas.Children.Add(_drawEllipse);
                    }

                    break;
            }

            var _borderLeft = drawRect.Left - Canvas.GetLeft(_border);

            if (_borderLeft < 0)
                _borderLeft = Math.Abs(_borderLeft);
            if (drawRect.Width + _borderLeft < _border.ActualWidth)
            {
                var wLeft = Canvas.GetLeft(_border) + _border.ActualWidth;
                var left = drawRect.Left < Canvas.GetLeft(_border) ? Canvas.GetLeft(_border) :
                    drawRect.Left > wLeft ? wLeft : drawRect.Left;
                if (_borderRectangle != null)
                {
                    _borderRectangle.Width = drawRect.Width;
                    Canvas.SetLeft(_borderRectangle, left);
                }

                if (_drawEllipse != null)
                {
                    _drawEllipse.Width = drawRect.Width;
                    Canvas.SetLeft(_drawEllipse, left);
                }
            }

            var _borderTop = drawRect.Top - Canvas.GetTop(_border);
            if (_borderTop < 0)
                _borderTop = Math.Abs(_borderTop);
            if (drawRect.Height + _borderTop < _border.ActualHeight)
            {
                var hTop = Canvas.GetTop(_border) + _border.Height;
                var top = drawRect.Top < Canvas.GetTop(_border) ? Canvas.GetTop(_border) :
                    drawRect.Top > hTop ? hTop : drawRect.Top;
                if (_borderRectangle != null)
                {
                    _borderRectangle.Height = drawRect.Height;
                    Canvas.SetTop(_borderRectangle, top);
                }

                if (_drawEllipse != null)
                {
                    _drawEllipse.Height = drawRect.Height;
                    Canvas.SetTop(_drawEllipse, top);
                }
            }
        }
        
        private void SelectElement()
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(_canvas); i++)
            {
                var child = VisualTreeHelper.GetChild(_canvas, i);
                if (child is FrameworkElement frameworkElement && frameworkElement.Tag != null)
                    if (frameworkElement.Tag.ToString() == _tag)
                        frameworkElement.Opacity = 1;
            }
        }
        
        private void MoveRect(Point current)
        {
            if (_pointStart is null)
                return;

            var vPoint = _pointStart.Value;

            if (current != vPoint)
            {
                var vector = Point.Subtract(current, vPoint);
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
                _pointStart = current;

                Canvas.SetLeft(_border, left);
                Canvas.SetTop(_border, top);
                _rect = new Rect(new Point(left, top), new Point(left + _border.Width, top + _border.Height));
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
            if (_pointStart == _pointEnd)
            {
                return;
            }
            var fElement = e.Source as FrameworkElement;
            if (fElement != null && fElement.Tag == null)
                SelectElement();
            _isMouseUp = true;
            if (_screenCutMouseType != ScreenCutMouseType.Default)
            {
                if (_screenCutMouseType == ScreenCutMouseType.MoveMouse)
                    EditBarPosition();

                if (_radioButtonRectangle.IsChecked != true
                    &&
                    _radioButtonEllipse.IsChecked != true
                    &&
                    _radioButtonArrow.IsChecked != true
                    &&
                    _radioButtonText.IsChecked != true
                    &&
                    _radioButtonInk.IsChecked != true)
                    _screenCutMouseType = ScreenCutMouseType.Default;
                else
                    DisposeControl();
            }
        }
        
        private void DisposeControl()
        {
            _polyLine = null;
            _textBorder = null;
            _borderRectangle = null;
            _drawEllipse = null;
            _controlArrow = null;
            _pointStart = null;
            _pointEnd = null;
        }
        
        private void EditBarPosition()
        {
            _editBar.Visibility = Visibility.Visible;
            Canvas.SetLeft(_editBar, _rect.X + _rect.Width - _editBar.ActualWidth);
            var y = Canvas.GetTop(_border) + _border.ActualHeight + _editBar.ActualHeight + _popupBorder.ActualHeight +
                    24;
            if (y > _canvas.ActualHeight && Canvas.GetTop(_border) > _editBar.ActualHeight)
                y = Canvas.GetTop(_border) - _editBar.ActualHeight - 8;
            else if (y > _canvas.ActualHeight && Canvas.GetTop(_border) < _editBar.ActualHeight)
                y = _border.ActualHeight - _editBar.ActualHeight - 8;
            else
                y = Canvas.GetTop(_border) + _border.ActualHeight + 8;
            Canvas.SetTop(_editBar, y);
            if (_popup != null && _popup.IsOpen)
            {
                _popup.IsOpen = false;
                _popup.IsOpen = true;
            }
        }
        
        private void MoveAllRectangle(Point current)
        {
            if (_pointStart is null)
                return;

            var vPoint = _pointStart.Value;

            _pointEnd = current;
            var vEndPoint = current;

            _rect = new Rect(vPoint, vEndPoint);
            _rectangleLeft.Width = _rect.X < 0 ? 0 : _rect.X > _canvas.ActualWidth ? _canvas.ActualWidth : _rect.X;
            _rectangleLeft.Height = _canvas.Height;

            Canvas.SetLeft(_rectangleTop, _rectangleLeft.Width);
            _rectangleTop.Width = _rect.Width;
            var h = 0.0;
            if (current.Y < vPoint.Y)
                h = current.Y;
            else
                h = current.Y - _rect.Height;

            _rectangleTop.Height = h < 0 ? 0 : h > _canvas.ActualHeight ? _canvas.ActualHeight : h;

            Canvas.SetLeft(_rectangleRight, _rectangleLeft.Width + _rect.Width);
            var rWidth = _canvas.Width - (_rect.Width + _rectangleLeft.Width);
            _rectangleRight.Width = rWidth < 0 ? 0 : rWidth > _canvas.ActualWidth ? _canvas.ActualWidth : rWidth;

            _rectangleRight.Height = _canvas.Height;

            Canvas.SetLeft(_rectangleBottom, _rectangleLeft.Width);
            Canvas.SetTop(_rectangleBottom, _rect.Height + _rectangleTop.Height);
            _rectangleBottom.Width = _rect.Width;
            var rBottomHeight = _canvas.Height - (_rect.Height + _rectangleTop.Height);
            _rectangleBottom.Height = rBottomHeight < 0 ? 0 : rBottomHeight;

            _border.Height = _rect.Height;
            _border.Width = _rect.Width;
            Canvas.SetLeft(_border, _rect.X);
            Canvas.SetTop(_border, _rect.Y);

            if (_adornerLayer != null) return;
            _border.Opacity = 1;
            _adornerLayer = AdornerLayer.GetAdornerLayer(_border);
            _screenCutAdorner = new ScreenCutAdorner(_border);
            _screenCutAdorner.PreviewMouseDown += (s, e) =>
            {
                Restore();
                ResoreRadioButton();
            };
            _adornerLayer.Add(_screenCutAdorner);
            _border.SizeChanged += _border_SizeChanged;
        }
        
    }
}