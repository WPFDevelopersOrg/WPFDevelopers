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
        [DllImport("User32.dll")]
        private static extern IntPtr MonitorFromPoint([In] System.Drawing.Point pt, [In] uint dwFlags);
        [DllImport("Shcore.dll")]
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
        private AdornerLayer adornerLayer;

        /// <summary>
        ///     当前绘制矩形
        /// </summary>
        private Border borderRectangle;

        /// <summary>
        ///     当前箭头
        /// </summary>
        private Control controlArrow;

        private ControlTemplate controlTemplate;

        /// <summary>
        ///     绘制当前椭圆
        /// </summary>
        private Ellipse drawEllipse;

        private FrameworkElement frameworkElement;
        private bool isMouseUp;
        private Point? pointStart, pointEnd;

        /// <summary>
        ///     当前画笔
        /// </summary>
        private Polyline polyLine;

        private Rect rect;
        private ScreenCutAdorner screenCutAdorner;
        private ScreenCutMouseType screenCutMouseType = ScreenCutMouseType.Default;

        /// <summary>
        ///     当前文本
        /// </summary>
        private Border textBorder;

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
        private double y1;
        private int ScreenIndex;
        public static int CaptureScreenID = -1;
        private Bitmap ScreenCapture;
        private ScreenDPI screenDPI;
        public ScreenCut(int index)
        {
            ScreenIndex = index;
            this.Left = Screen.AllScreens[ScreenIndex].WorkingArea.Left;
            ShowInTaskbar = false;
            screenDPI = GetScreenDPI(ScreenIndex);
        }
        static ScreenCut()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScreenCut),
                new FrameworkPropertyMetadata(typeof(ScreenCut)));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
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
            if (_radioButtonArrow != null)
                _radioButtonArrow.Click += _radioButtonArrow_Click;
            _radioButtonInk = GetTemplateChild(RadioButtonInkTemplateName) as RadioButton;
            if (_radioButtonInk != null)
                _radioButtonInk.Click += _radioButtonInk_Click;
            _radioButtonText = GetTemplateChild(RadioButtonTextTemplateName) as RadioButton;
            if (_radioButtonText != null)
                _radioButtonText.Click += _radioButtonText_Click;
            _canvas.Width = Screen.AllScreens[ScreenIndex].Bounds.Width;
            _canvas.Height = Screen.AllScreens[ScreenIndex].Bounds.Height;
            //_canvas.Background = new ImageBrush(ControlsHelper.Capture());
            _canvas.Background = new ImageBrush(ConvertBitmap(CopyScreen()));
            _rectangleLeft.Width = _canvas.Width;
            _rectangleLeft.Height = _canvas.Height;
            _border.Opacity = 0;
            _popup = GetTemplateChild(PopupTemplateName) as Popup;
            _popupBorder = GetTemplateChild(PopupBorderTemplateName) as Border;
            _popupBorder.Loaded += (s, e) => { _popup.HorizontalOffset = -_popupBorder.ActualWidth / 3; };
            _wrapPanel = GetTemplateChild(WrapPanelColorTemplateName) as WrapPanel;
            _wrapPanel.PreviewMouseDown += _wrapPanel_PreviewMouseDown;
            Loaded += ScreenCut_Loaded;
            controlTemplate = (ControlTemplate)FindResource("PART_DrawArrow");
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
        private BitmapSource ConvertBitmap(Bitmap bitmap)
        {
            BitmapSource img;
            IntPtr hBitmap;
            hBitmap = bitmap.GetHbitmap();
            img = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return img;
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
            this.Left = Screen.AllScreens[ScreenIndex].Bounds.Left / screenDPI.scaleX;
            this.Top = Screen.AllScreens[ScreenIndex].Bounds.Top / screenDPI.scaleY;
            this.Width = Screen.AllScreens[ScreenIndex].Bounds.Width / screenDPI.scaleX;
            this.Height = Screen.AllScreens[ScreenIndex].Bounds.Height / screenDPI.scaleY;
            _canvas.Width = Screen.AllScreens[ScreenIndex].Bounds.Width / screenDPI.scaleX;
            _canvas.Height = Screen.AllScreens[ScreenIndex].Bounds.Height / screenDPI.scaleY;
            _canvas.SetValue(LeftProperty, this.Left);
            _canvas.SetValue(TopProperty, this.Top);
            Bitmap ScreenCapture = new Bitmap(Screen.AllScreens[ScreenIndex].Bounds.Width, Screen.AllScreens[ScreenIndex].Bounds.Height);
            using (Graphics g = Graphics.FromImage(ScreenCapture)) 
            {
                g.CopyFromScreen(Screen.AllScreens[ScreenIndex].Bounds.Left, Screen.AllScreens[ScreenIndex].Bounds.Top, 0, 0, Screen.AllScreens[ScreenIndex].Bounds.Size);
            }
            return ScreenCapture;
        }
        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            Close();
        }

        private void _radioButtonInk_Click(object sender, RoutedEventArgs e)
        {
            RadioButtonChecked(_radioButtonInk, ScreenCutMouseType.DrawInk);
        }

        private void _radioButtonText_Click(object sender, RoutedEventArgs e)
        {
            RadioButtonChecked(_radioButtonText, ScreenCutMouseType.DrawText);
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

        private void RadioButtonChecked(RadioButton radioButton, ScreenCutMouseType screenCutMouseTypeRadio)
        {
            if (radioButton.IsChecked == true)
            {
                screenCutMouseType = screenCutMouseTypeRadio;
                _border.Cursor = Cursors.Arrow;
                if (_popup.PlacementTarget != null && _popup.IsOpen)
                    _popup.IsOpen = false;
                _popup.PlacementTarget = radioButton;
                _popup.IsOpen = true;
                DisposeControl();
            }
            else
            {
                if (screenCutMouseType == screenCutMouseTypeRadio)
                    Restore();
            }
        }

        private void Restore()
        {
            _border.Cursor = Cursors.SizeAll;
            if (screenCutMouseType == ScreenCutMouseType.Default) return;
            screenCutMouseType = ScreenCutMouseType.Default;
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
            if (isMouseUp)
            {
                var left = Canvas.GetLeft(_border);
                var top = Canvas.GetTop(_border);
                var beignPoint = new Point(left, top);
                var endPoint = new Point(left + _border.ActualWidth, top + _border.ActualHeight);
                rect = new Rect(beignPoint, endPoint);
                pointStart = beignPoint;
                MoveAllRectangle(endPoint);
            }
            EditBarPosition();
        }
        
        private void _border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (screenCutMouseType == ScreenCutMouseType.Default)
                screenCutMouseType = ScreenCutMouseType.MoveMouse;
        }
        
        private void _buttonSave_Click(object sender, RoutedEventArgs e)
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
                }
            }
        }
        
        private void _buttonComplete_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = CutBitmap();
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
            var renderTargetBitmap = new RenderTargetBitmap((int)_canvas.Width,
                (int)_canvas.Height, 96d, 96d, PixelFormats.Default);
            renderTargetBitmap.Render(_canvas);
            return new CroppedBitmap(renderTargetBitmap,
                new Int32Rect((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height));
        }
        
        private void _buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
            if (CutCanceled != null)
                CutCanceled();
        }
        
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            else if (e.Key == Key.Delete)
            {
                if (_canvas.Children.Count > 0)
                    _canvas.Children.Remove(frameworkElement);
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
                CaptureScreenID = ScreenIndex;
            }
            if (CaptureScreenID != -1 && CaptureScreenID != ScreenIndex)
            {
                e.Handled = true;
                return;
            }

            var vPoint = e.GetPosition(_canvas);
            if (!isMouseUp)
            {
                pointStart = vPoint;
                screenCutMouseType = ScreenCutMouseType.DrawMouse;
                _editBar.Visibility = Visibility.Hidden;
                pointEnd = pointStart;
                rect = new Rect(pointStart.Value, pointEnd.Value);
            }
            else
            {
                if (vPoint.X < rect.Left || vPoint.X > rect.Right)
                    return;

                if (vPoint.Y < rect.Top || vPoint.Y > rect.Bottom)
                    return;
                pointStart = vPoint;
                if (textBorder != null)
                    Focus();

                switch (screenCutMouseType)
                {
                    case ScreenCutMouseType.DrawText:
                        y1 = vPoint.Y;
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
            if (pointStart.Value.X < rect.Right
                &&
                pointStart.Value.X > rect.Left
                &&
                pointStart.Value.Y > rect.Top
                &&
                pointStart.Value.Y < rect.Bottom)
            {
                var currentWAndX = pointStart.Value.X + 40;
                if (textBorder == null)
                {
                    textBorder = new Border
                    {
                        BorderBrush = _currentBrush == null ? Brushes.Red : _currentBrush,
                        BorderThickness = new Thickness(1),
                        Tag = _tag
                    };

                    var textBox = new TextBox();
                    textBox.Style = null;
                    textBox.Background = null;
                    textBox.BorderThickness = new Thickness(0);
                    textBox.Foreground = textBorder.BorderBrush;
                    textBox.FontFamily = DrawingContextHelper.FontFamily;
                    textBox.FontSize = 16;
                    textBox.TextWrapping = TextWrapping.Wrap;
                    textBox.FontWeight = FontWeights.Normal;
                    textBox.MinWidth = _width;
                    textBox.MaxWidth = rect.Right - pointStart.Value.X;
                    textBox.MaxHeight = rect.Height - 4;
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
                    textBorder.SizeChanged += (s, e1) =>
                    {
                        var tb = s as Border;
                        var y = y1;
                        if (y + tb.ActualHeight > rect.Bottom)
                        {
                            var v = Math.Abs(rect.Bottom - (y + tb.ActualHeight));
                            y1 = y - v;
                            Canvas.SetTop(tb, y1 + 2);
                        }
                    };
                    textBorder.PreviewMouseLeftButtonDown += (s, e) =>
                    {
                        _radioButtonText.IsChecked = true;
                        _radioButtonText_Click(null, null);
                        SelectElement();
                        var border = s as Border;
                        frameworkElement = border;
                        frameworkElement.Opacity = .7;
                        border.BorderThickness = new Thickness(1);
                    };
                    textBorder.Child = textBox;
                    _canvas.Children.Add(textBorder);
                    textBox.Focus();
                    var x = pointStart.Value.X;

                    if (currentWAndX > rect.Right)
                        x = x - (currentWAndX - rect.Right);
                    Canvas.SetLeft(textBorder, x - 2);
                    Canvas.SetTop(textBorder, pointStart.Value.Y - 2);
                }
            }
        }
        
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (e.Source is RadioButton)
                return;

            if (pointStart is null)
                return;

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
                    case ScreenCutMouseType.DrawInk:
                        DrwaInkControl(current);
                        break;
                }
            }
        }

        private void CheckPoint(Point current)
        {
            if (current == pointStart) return;

            if (current.X > rect.BottomRight.X
                ||
                current.Y > rect.BottomRight.Y)
                return;
        }
        
        private void DrwaInkControl(Point current)
        {
            CheckPoint(current);
            if (current.X >= rect.Left
                &&
                current.X <= rect.Right
                &&
                current.Y >= rect.Top
                &&
                current.Y <= rect.Bottom)
            {
                if (polyLine == null)
                {
                    polyLine = new Polyline();
                    polyLine.Stroke = _currentBrush == null ? Brushes.Red : _currentBrush;
                    polyLine.Cursor = Cursors.Hand;
                    polyLine.StrokeThickness = 3;
                    polyLine.StrokeLineJoin = PenLineJoin.Round;
                    polyLine.StrokeStartLineCap = PenLineCap.Round;
                    polyLine.StrokeEndLineCap = PenLineCap.Round;
                    polyLine.MouseLeftButtonDown += (s, e) =>
                    {
                        _radioButtonInk.IsChecked = true;
                        _radioButtonInk_Click(null, null);
                        SelectElement();
                        frameworkElement = s as Polyline;
                        frameworkElement.Opacity = .7;
                    };
                    _canvas.Children.Add(polyLine);
                }

                polyLine.Points.Add(current);
            }
        }
        
        private void DrawArrowControl(Point current)
        {
            CheckPoint(current);
            if (screenCutMouseType != ScreenCutMouseType.DrawArrow)
                return;

            if (pointStart is null)
                return;

            var vPoint = pointStart.Value;

            var drawArrow = new Rect(vPoint, current);
            if (controlArrow == null)
            {
                controlArrow = new Control();
                controlArrow.Background = _currentBrush == null ? Brushes.Red : _currentBrush;
                controlArrow.Template = controlTemplate;
                controlArrow.Cursor = Cursors.Hand;
                controlArrow.Tag = _tag;
                controlArrow.MouseLeftButtonDown += (s, e) =>
                {
                    _radioButtonArrow.IsChecked = true;
                    _radioButtonArrow_Click(null, null);
                    SelectElement();
                    frameworkElement = s as Control;
                    frameworkElement.Opacity = .7;
                };
                _canvas.Children.Add(controlArrow);
                Canvas.SetLeft(controlArrow, drawArrow.Left);
                Canvas.SetTop(controlArrow, drawArrow.Top - 7.5);
            }

            var rotate = new RotateTransform();
            var renderOrigin = new Point(0, .5);
            controlArrow.RenderTransformOrigin = renderOrigin;
            controlArrow.RenderTransform = rotate;
            rotate.Angle = ControlsHelper.CalculeAngle(vPoint, current);
            if (current.X < rect.Left
                ||
                current.X > rect.Right
                ||
                current.Y < rect.Top
                ||
                current.Y > rect.Bottom)
            {
                if (current.X >= vPoint.X && current.Y < vPoint.Y)
                {
                    var a1 = (current.Y - vPoint.Y) / (current.X - vPoint.X);
                    var b1 = vPoint.Y - a1 * vPoint.X;
                    var xTop = (rect.Top - b1) / a1;
                    var yRight = a1 * rect.Right + b1;

                    if (xTop <= rect.Right)
                    {
                        current.X = xTop;
                        current.Y = rect.Top;
                    }
                    else
                    {
                        current.X = rect.Right;
                        current.Y = yRight;
                    }
                }
                else if (current.X > vPoint.X && current.Y > vPoint.Y)
                {
                    var a1 = (current.Y - vPoint.Y) / (current.X - vPoint.X);
                    var b1 = vPoint.Y - a1 * vPoint.X;
                    var xBottom = (rect.Bottom - b1) / a1;
                    var yRight = a1 * rect.Right + b1;

                    if (xBottom <= rect.Right)
                    {
                        current.X = xBottom;
                        current.Y = rect.Bottom;
                    }
                    else
                    {
                        current.X = rect.Right;
                        current.Y = yRight;
                    }
                }
                else if (current.X < vPoint.X && current.Y < vPoint.Y)
                {
                    var a1 = (current.Y - vPoint.Y) / (current.X - vPoint.X);
                    var b1 = vPoint.Y - a1 * vPoint.X;
                    var xTop = (rect.Top - b1) / a1;
                    var yLeft = a1 * rect.Left + b1;
                    if (xTop >= rect.Left)
                    {
                        current.X = xTop;
                        current.Y = rect.Top;
                    }
                    else
                    {
                        current.X = rect.Left;
                        current.Y = yLeft;
                    }
                }
                else if (current.X < vPoint.X && current.Y > vPoint.Y)
                {
                    var a1 = (current.Y - vPoint.Y) / (current.X - vPoint.X);
                    var b1 = vPoint.Y - a1 * vPoint.X;
                    var xBottom = (rect.Bottom - b1) / a1;
                    var yLeft = a1 * rect.Left + b1;

                    if (xBottom <= rect.Left)
                    {
                        current.X = rect.Left;
                        current.Y = yLeft;
                    }
                    else
                    {
                        current.X = xBottom;
                        current.Y = rect.Bottom;
                    }
                }
            }

            var x = current.X - vPoint.X;
            var y = current.Y - vPoint.Y;
            var width = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
            width = width < 15 ? 15 : width;
            controlArrow.Width = width;
        }
        
        private void DrawMultipleControl(Point current)
        {
            CheckPoint(current);
            if (pointStart is null)
                return;

            var vPoint = pointStart.Value;

            var drawRect = new Rect(vPoint, current);
            switch (screenCutMouseType)
            {
                case ScreenCutMouseType.DrawRectangle:
                    if (borderRectangle == null)
                    {
                        borderRectangle = new Border
                        {
                            BorderBrush = _currentBrush == null ? Brushes.Red : _currentBrush,
                            BorderThickness = new Thickness(3),
                            CornerRadius = new CornerRadius(3),
                            Tag = _tag,
                            Cursor = Cursors.Hand
                        };
                        borderRectangle.MouseLeftButtonDown += (s, e) =>
                        {
                            _radioButtonRectangle.IsChecked = true;
                            _radioButtonRectangle_Click(null, null);
                            SelectElement();
                            frameworkElement = s as Border;
                            frameworkElement.Opacity = .7;
                        };
                        _canvas.Children.Add(borderRectangle);
                    }

                    break;
                case ScreenCutMouseType.DrawEllipse:
                    if (drawEllipse == null)
                    {
                        drawEllipse = new Ellipse
                        {
                            Stroke = _currentBrush == null ? Brushes.Red : _currentBrush,
                            StrokeThickness = 3,
                            Tag = _tag,
                            Cursor = Cursors.Hand
                        };
                        drawEllipse.MouseLeftButtonDown += (s, e) =>
                        {
                            _radioButtonEllipse.IsChecked = true;
                            _radioButtonEllipse_Click(null, null);
                            SelectElement();
                            frameworkElement = s as Ellipse;
                            frameworkElement.Opacity = .7;
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
                var left = drawRect.Left < Canvas.GetLeft(_border) ? Canvas.GetLeft(_border) :
                    drawRect.Left > wLeft ? wLeft : drawRect.Left;
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
            if (_borderTop < 0)
                _borderTop = Math.Abs(_borderTop);
            if (drawRect.Height + _borderTop < _border.ActualHeight)
            {
                var hTop = Canvas.GetTop(_border) + _border.Height;
                var top = drawRect.Top < Canvas.GetTop(_border) ? Canvas.GetTop(_border) :
                    drawRect.Top > hTop ? hTop : drawRect.Top;
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
            if (pointStart is null)
                return;

            var vPoint = pointStart.Value;

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
            if (pointStart == pointEnd)
            {
                return;
            }
            var fElement = e.Source as FrameworkElement;
            if (fElement != null && fElement.Tag == null)
                SelectElement();
            isMouseUp = true;
            if (screenCutMouseType != ScreenCutMouseType.Default)
            {
                if (screenCutMouseType == ScreenCutMouseType.MoveMouse)
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
                    screenCutMouseType = ScreenCutMouseType.Default;
                else
                    DisposeControl();
            }
        }
        
        private void DisposeControl()
        {
            polyLine = null;
            textBorder = null;
            borderRectangle = null;
            drawEllipse = null;
            controlArrow = null;
            pointStart = null;
            pointEnd = null;
        }
        
        private void EditBarPosition()
        {
            _editBar.Visibility = Visibility.Visible;
            Canvas.SetLeft(_editBar, rect.X + rect.Width - _editBar.ActualWidth);
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
            if (pointStart is null)
                return;

            var vPoint = pointStart.Value;

            pointEnd = current;
            var vEndPoint = current;

            rect = new Rect(vPoint, vEndPoint);
            _rectangleLeft.Width = rect.X < 0 ? 0 : rect.X > _canvas.ActualWidth ? _canvas.ActualWidth : rect.X;
            _rectangleLeft.Height = _canvas.Height;

            Canvas.SetLeft(_rectangleTop, _rectangleLeft.Width);
            _rectangleTop.Width = rect.Width;
            var h = 0.0;
            if (current.Y < vPoint.Y)
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
            _rectangleBottom.Height = rBottomHeight < 0 ? 0 : rBottomHeight;

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
        
    }
}