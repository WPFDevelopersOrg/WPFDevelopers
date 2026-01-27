using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFDevelopers.Helpers;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Button = System.Windows.Controls.Button;
using Color = System.Windows.Media.Color;
using Control = System.Windows.Controls.Control;
using Cursors = System.Windows.Input.Cursors;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Path = System.Windows.Shapes.Path;
using Point = System.Windows.Point;
using RadioButton = System.Windows.Controls.RadioButton;
using Rectangle = System.Windows.Shapes.Rectangle;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using TextBox = System.Windows.Controls.TextBox;

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

    public struct ScreenDPI
    {
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
        DrawInk,
        DrawMosaic
    }
    [TemplatePart(Name = CanvasTemplateName, Type = typeof(Canvas))]
    [TemplatePart(Name = LeftRectangleTemplateName, Type = typeof(Rectangle))]
    [TemplatePart(Name = TopRectangleTemplateName, Type = typeof(Rectangle))]
    [TemplatePart(Name = RightRectangleTemplateName, Type = typeof(Rectangle))]
    [TemplatePart(Name = BottomRectangleTemplateName, Type = typeof(Rectangle))]
    [TemplatePart(Name = BorderTemplateName, Type = typeof(Border))]
    [TemplatePart(Name = EditBarTemplateName, Type = typeof(Border))]
    [TemplatePart(Name = SaveButtonTemplateName, Type = typeof(Button))]
    [TemplatePart(Name = CancelButtonTemplateName, Type = typeof(Button))]
    [TemplatePart(Name = CompleteButtonTemplateName, Type = typeof(Button))]
    [TemplatePart(Name = UndoButtonTemplateName, Type = typeof(Button))]
    [TemplatePart(Name = RectangleRadioButtonTemplateName, Type = typeof(RadioButton))]
    [TemplatePart(Name = EllipseRadioButtonTemplateName, Type = typeof(RadioButton))]
    [TemplatePart(Name = ArrowRadioButtonTemplateName, Type = typeof(RadioButton))]
    [TemplatePart(Name = InkRadioButtonTemplateName, Type = typeof(RadioButton))]
    [TemplatePart(Name = MosaicRadioButtonTemplateName, Type = typeof(RadioButton))]
    [TemplatePart(Name = TextRadioButtonTemplateName, Type = typeof(RadioButton))]
    [TemplatePart(Name = PopupTemplateName, Type = typeof(Popup))]
    [TemplatePart(Name = BorderPopupTemplateName, Type = typeof(Border))]
    [TemplatePart(Name = ColorWrapPanelTemplateName, Type = typeof(WrapPanel))]

    public class ScreenCut : Window, IDisposable
    {
        private const string CanvasTemplateName = "PART_Canvas";
        private const string LeftRectangleTemplateName = "PART_LeftRectangle";
        private const string TopRectangleTemplateName = "PART_TopRectangle";
        private const string RightRectangleTemplateName = "PART_RightRectangle";
        private const string BottomRectangleTemplateName = "PART_BottomRectangle";
        private const string BorderTemplateName = "PART_Border";
        private const string EditBarTemplateName = "PART_EditBar";
        private const string SaveButtonTemplateName = "PART_SaveButton";
        private const string CancelButtonTemplateName = "PART_CancelButton";
        private const string CompleteButtonTemplateName = "PART_CompleteButton";
        private const string UndoButtonTemplateName = "PART_UndoButton";
        private const string RectangleRadioButtonTemplateName = "PART_RectangleRadioButton";
        private const string EllipseRadioButtonTemplateName = "PART_EllipseRadioButton";
        private const string ArrowRadioButtonTemplateName = "PART_ArrowRadioButton";
        private const string InkRadioButtonTemplateName = "PART_InkRadioButton";
        private const string MosaicRadioButtonTemplateName = "PART_MosaicRadioButton";
        private const string TextRadioButtonTemplateName = "PART_TextRadioButton";
        private const string PopupTemplateName = "PART_Popup";
        private const string BorderPopupTemplateName = "PART_BorderPopup";
        private const string ColorWrapPanelTemplateName = "PART_ColorPanelWrap";

        private const string _tag = "Draw";
        private const int _width = 40;
        private Border _border, _editBar, _popupBorder;
        private Button _saveButton, _cancelButton, _completeButton, _undoButton;
        private Canvas _canvas;

        /// <summary>
        ///     当前选择颜色
        /// </summary>
        private Brush _currentBrush;

        private Popup _popup;

        private RadioButton _rectangleRadioButton,
            _radioButtonEllipse,
            _arrowRadioButton,
            _inkRadioButton,
            _mosaicRadioButton,
            _textRadioButton;

        private Rectangle _leftRectangle, _topRectangle, _rightRectangle, _bottomRectangle;
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
        private RenderTargetBitmap _imageSnapshot;
        private Path _currentStrokeContainer = null;
        private List<Rectangle> _currentStrokeRectangles = new List<Rectangle>();
        private Stack<UIElement> _strokeHistory = new Stack<UIElement>();

        private static readonly HashSet<string> PermanentElementNames = new HashSet<string>
        {
            "PART_LeftRectangle",
            "PART_TopRectangle",
            "PART_RightRectangle",
            "PART_BottomRectangle",
            "PART_Border",
            "PART_EditBar",
            "PART_Popup",
            "PART_ColorPanelWrap",
            "PART_BorderPopup"
        };

        public ScreenCut(int index)
        {
            Width = 0;
            Height = 0;
            _screenIndex = index;
            var screen = Screen.AllScreens[_screenIndex];
            Left = screen.WorkingArea.Left;
            ShowInTaskbar = false;
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Normal;
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
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public static void ClearCaptureScreenID()
        {
            CaptureScreenID = -1;
        }
        
        ~ScreenCut()
        {
            Debug.WriteLine("~ScreenCut");
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _canvas = GetTemplateChild(CanvasTemplateName) as Canvas;
            _leftRectangle = GetTemplateChild(LeftRectangleTemplateName) as Rectangle;
            _topRectangle = GetTemplateChild(TopRectangleTemplateName) as Rectangle;
            _rightRectangle = GetTemplateChild(RightRectangleTemplateName) as Rectangle;
            _bottomRectangle = GetTemplateChild(BottomRectangleTemplateName) as Rectangle;
            _border = GetTemplateChild(BorderTemplateName) as Border;
            _border.MouseLeftButtonDown += Border_MouseLeftButtonDown;

            _editBar = GetTemplateChild(EditBarTemplateName) as Border;
            _saveButton = GetTemplateChild(SaveButtonTemplateName) as Button;
            if (_saveButton != null)
                _saveButton.Click += ButtonSave_Click;
            _cancelButton = GetTemplateChild(CancelButtonTemplateName) as Button;
            if (_cancelButton != null)
                _cancelButton.Click += ButtonCancel_Click;
            _completeButton = GetTemplateChild(CompleteButtonTemplateName) as Button;
            if (_completeButton != null)
                _completeButton.Click += ButtonComplete_Click;
            _undoButton = GetTemplateChild(UndoButtonTemplateName) as Button;
            if (_undoButton != null)
                _undoButton.Click += OnUndoButton_Click;
            _rectangleRadioButton = GetTemplateChild(RectangleRadioButtonTemplateName) as RadioButton;
            if (_rectangleRadioButton != null)
                _rectangleRadioButton.Click += RadioButtonRectangle_Click;
            _radioButtonEllipse = GetTemplateChild(EllipseRadioButtonTemplateName) as RadioButton;
            if (_radioButtonEllipse != null)
                _radioButtonEllipse.Click += RadioButtonEllipse_Click;
            _arrowRadioButton = GetTemplateChild(ArrowRadioButtonTemplateName) as RadioButton;
            if (_arrowRadioButton != null)
                _arrowRadioButton.Click += RadioButtonArrow_Click;
            _inkRadioButton = GetTemplateChild(InkRadioButtonTemplateName) as RadioButton;
            if (_inkRadioButton != null)
                _inkRadioButton.Click += RadioButtonInk_Click;
            _mosaicRadioButton = GetTemplateChild(MosaicRadioButtonTemplateName) as RadioButton;
            if (_mosaicRadioButton != null)
                _mosaicRadioButton.Click += RadioButtonMosaic_Click;
            _textRadioButton = GetTemplateChild(TextRadioButtonTemplateName) as RadioButton;
            if (_textRadioButton != null)
                _textRadioButton.Click += RadioButtonText_Click;
            _leftRectangle.Width = _canvas.Width;
            _leftRectangle.Height = _canvas.Height;
            _border.Opacity = 0;
            _popup = GetTemplateChild(PopupTemplateName) as Popup;
            _popupBorder = GetTemplateChild(BorderPopupTemplateName) as Border;
            if (_popupBorder != null)
                _popupBorder.Loaded += (s, e) => { _popup.HorizontalOffset = -_popupBorder.ActualWidth / 3; };
            _wrapPanel = GetTemplateChild(ColorWrapPanelTemplateName) as WrapPanel;
            if (_wrapPanel != null)
                _wrapPanel.PreviewMouseDown += WrapPanel_PreviewMouseDown;
            Loaded += ScreenCut_Loaded;
            _controlTemplate = (ControlTemplate)FindResource("WD.PART_DrawArrow");
            _screenCapture = CopyScreen();
            using (var tempBitmap = _screenCapture)
            {
                var imageSource = ImagingHelper.CreateBitmapSourceFromBitmap(tempBitmap);
                imageSource.Freeze();
                var writeableBitmap = new WriteableBitmap(imageSource);
                writeableBitmap.Freeze(); 
                _canvas.Background = new ImageBrush(writeableBitmap);
            }
            _screenCapture?.Dispose();
            _screenCapture = null;
            TakeSnapshot();
        }

        private void OnUndoButton_Click(object sender, RoutedEventArgs e)
        {
            Undo();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (_adornerLayer != null && _screenCutAdorner != null)
            {
                _adornerLayer.Remove(_screenCutAdorner);
                _screenCutAdorner = null;
                _adornerLayer = null;
            }
            if (_canvas != null)
            {
                if (_canvas.Background is ImageBrush brush)
                {
                    brush.ImageSource = null;
                }
                _canvas.Background = null;
                _canvas.Children.Clear();
            }
            _imageSnapshot = null;
            Dispose();
        }
       
        private void ScreenCut_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= ScreenCut_Loaded;
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
            Left = Screen.AllScreens[_screenIndex].Bounds.Left;
            Top = Screen.AllScreens[_screenIndex].Bounds.Top;
            Width = Screen.AllScreens[_screenIndex].Bounds.Width / _screenDPI.scaleX;
            Height = Screen.AllScreens[_screenIndex].Bounds.Height / _screenDPI.scaleY;
            _canvas.Width = Width;
            _canvas.Height = Height;
            _canvas.SetValue(LeftProperty, Left);
            _canvas.SetValue(TopProperty, Top);
            var bounds = Screen.AllScreens[_screenIndex].Bounds;
            Bitmap screenCapture = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(screenCapture))
            {
                g.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, screenCapture.Size);
            }
            return screenCapture;
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            OnCanceled();
        }

        private void RadioButtonInk_Click(object sender, RoutedEventArgs e)
        {
            RadioButtonChecked(_inkRadioButton, ScreenCutMouseType.DrawInk);
        }

        private void RadioButtonMosaic_Click(object sender, RoutedEventArgs e)
        {
            RadioButtonChecked(_mosaicRadioButton, ScreenCutMouseType.DrawMosaic);
        }

        private void RadioButtonText_Click(object sender, RoutedEventArgs e)
        {
            RadioButtonChecked(_textRadioButton, ScreenCutMouseType.DrawText);
        }

        private void RadioButtonArrow_Click(object sender, RoutedEventArgs e)
        {
            RadioButtonChecked(_arrowRadioButton, ScreenCutMouseType.DrawArrow);
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
            RadioButtonChecked(_rectangleRadioButton, ScreenCutMouseType.DrawRectangle);
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
                if(screenCutMouseTypeRadio != ScreenCutMouseType.DrawMosaic)
                {
                    _popup.PlacementTarget = radioButton;
                    _popup.IsOpen = true;
                }
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
            _rectangleRadioButton.IsChecked = false;
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
            _leftRectangle.Visibility = Visibility.Collapsed;
            _topRectangle.Visibility = Visibility.Collapsed;
            _rightRectangle.Visibility = Visibility.Collapsed;
            _bottomRectangle.Visibility = Visibility.Collapsed;
            var renderTargetBitmap = new RenderTargetBitmap((int)(_canvas.Width * _screenDPI.scaleX),
                (int)(_canvas.Height * _screenDPI.scaleY), _screenDPI.dpiX, _screenDPI.dpiY, PixelFormats.Default);
            renderTargetBitmap.Render(_canvas);
            var realrect = new Int32Rect(
                (int)(_rect.X * _screenDPI.scaleX),
                (int)(_rect.Y * _screenDPI.scaleY),
                (int)(_rect.Width * _screenDPI.scaleX),
                (int)(_rect.Height * _screenDPI.scaleY));
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
                SetUndoEnabled();
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.Z) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Undo();
            }
        }

        void Undo()
        {
            try
            {
                if (_screenCutMouseType == ScreenCutMouseType.DrawMosaic)
                {
                    UndoLastStroke();
                    return;
                }

                for (int i = _canvas.Children.Count - 1; i >= 0; i--)
                {
                    var element = _canvas.Children[i];
                    if (element is FrameworkElement fe && !string.IsNullOrEmpty(fe.Name))
                    {
                        if (PermanentElementNames.Contains(fe.Name))
                            continue;
                    }
                    if (element is Rectangle rect &&
                        (rect.Name?.StartsWith("PART_") ?? false))
                        continue;
                    _canvas.Children.RemoveAt(i);
                    break;
                }
                SetUndoEnabled();
            }
            catch (Exception)
            {
                throw;
            }
        }

        void SetUndoEnabled()
        {
            if (_undoButton != null)
            {
                bool hasUndoableElements = false;

                foreach (UIElement element in _canvas.Children)
                {
                    if (element is FrameworkElement fe && !string.IsNullOrEmpty(fe.Name))
                    {
                        if (PermanentElementNames.Contains(fe.Name))
                            continue;
                    }
                    hasUndoableElements = true;
                    break;
                }

                _undoButton.IsEnabled = hasUndoableElements;
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
                if (_screenCutMouseType == ScreenCutMouseType.DrawMosaic)
                {
                    _currentStrokeRectangles.Clear();
                }
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
                            SetUndoEnabled();
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
                        _textRadioButton.IsChecked = true;
                        RadioButtonText_Click(null, null);
                        SelectElement();
                        var border = s as Border;
                        _frameworkElement = border;
                        _frameworkElement.Opacity = .7;
                        border.BorderThickness = new Thickness(1);
                    };
                    _textBorder.Child = textBox;
                    _canvas.Children.Add(_textBorder);
                    SetUndoEnabled();
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
                    case ScreenCutMouseType.DrawMosaic:
                        if ((current - _pointStart.Value).Length < 10)
                            return;
                        _pointStart = current;
                        DrawMosaicBlock(current, 10, 20);
                        break;
                }
            }
        }

        private void TakeSnapshot()
        {
            _canvas.Measure(new System.Windows.Size(_canvas.ActualWidth, _canvas.ActualHeight));
            _canvas.Arrange(new Rect(0, 0, _canvas.ActualWidth, _canvas.ActualHeight));

            _imageSnapshot = new RenderTargetBitmap(
                (int)_canvas.ActualWidth,
                (int)_canvas.ActualHeight,
                96, 96, PixelFormats.Pbgra32);

            _imageSnapshot.Render(_canvas);
            _imageSnapshot.Freeze();
        }

        private void DrawMosaicBlock(Point center, int blockSize, int brushSize)
        {
            if (_imageSnapshot == null) return;

            int mosaicSize = blockSize;
            int blocksPerRow = brushSize / mosaicSize;

            for (int i = 0; i < blocksPerRow; i++)
            {
                for (int j = 0; j < blocksPerRow; j++)
                {
                    double x = center.X - brushSize / 2 + i * mosaicSize;
                    double y = center.Y - brushSize / 2 + j * mosaicSize;

                    Point blockCenter = new Point(x + mosaicSize / 2, y + mosaicSize / 2);
                    Color color = GetAreaAverageColor(blockCenter, mosaicSize);

                    var block = new Rectangle
                    {
                        Width = mosaicSize,
                        Height = mosaicSize,
                        Fill = new SolidColorBrush(color),
                        IsHitTestVisible = false
                    };

                    Canvas.SetLeft(block, x);
                    Canvas.SetTop(block, y);

                   _canvas.Children.Add(block);
                    SetUndoEnabled();
                    _currentStrokeRectangles.Add(block);
                }
            }
        }

        private void CompleteCurrentStroke()
        {
            if (_currentStrokeRectangles.Count == 0) return;
            RemoveTemporaryRectangles();
            CreateStrokeContainer();
            _canvas.Children.Add(_currentStrokeContainer);
            SetUndoEnabled();
            _strokeHistory.Push(_currentStrokeContainer);
            _currentStrokeContainer = null;
            _currentStrokeRectangles.Clear();
        }
        
        private void CreateStrokeContainer()
        {
            if (_currentStrokeRectangles.Count == 0) return;

            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (var rect in _currentStrokeRectangles)
            {
                double x = Canvas.GetLeft(rect);
                double y = Canvas.GetTop(rect);

                minX = Math.Min(minX, x);
                minY = Math.Min(minY, y);
                maxX = Math.Max(maxX, x + rect.Width);
                maxY = Math.Max(maxY, y + rect.Height);
            }

            double width = maxX - minX;
            double height = maxY - minY;

            var roundedRect = CreateRoundedRectangleGeometry(width, height);

            var container = new Path
            {
                Data = roundedRect,
                IsHitTestVisible = false,
                Fill = CreateMosaicVisualBrush(minX, minY, width, height)
            };

            Canvas.SetLeft(container, minX);
            Canvas.SetTop(container, minY);

            _currentStrokeContainer = container;
        }

        private Geometry CreateRoundedRectangleGeometry(double width, double height)
        {
            bool isVertical = height > width * 1.5;
            double cornerRadius = isVertical ? Math.Min(width / 2, 30) : Math.Min(height / 2, 30);
            return new RectangleGeometry(new Rect(0, 0, width, height), cornerRadius, cornerRadius);
        }

        private Brush CreateMosaicVisualBrush(double left, double top, double width, double height)
        {
            var drawingVisual = new DrawingVisual();

            using (var context = drawingVisual.RenderOpen())
            {
                foreach (var rect in _currentStrokeRectangles)
                {
                    double relativeX = Canvas.GetLeft(rect) - left;
                    double relativeY = Canvas.GetTop(rect) - top;

                    var rectGeometry = new RectangleGeometry(
                        new Rect(relativeX, relativeY, rect.Width, rect.Height));

                    context.DrawGeometry(rect.Fill, null, rectGeometry);
                }
            }
            return new VisualBrush(drawingVisual)
            {
                Stretch = Stretch.None,
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Top
            };
        }

        private void RemoveTemporaryRectangles()
        {
            foreach (var rect in _currentStrokeRectangles)
            {
                _canvas.Children.Remove(rect);
            }
            SetUndoEnabled();
        }

        private Color GetAreaAverageColor(Point center, int areaSize)
        {
            try
            {
                double scaleX = _imageSnapshot.PixelWidth / _canvas.ActualWidth;
                double scaleY = _imageSnapshot.PixelHeight / _canvas.ActualHeight;
                int pixelX = (int)(center.X * scaleX);
                int pixelY = (int)(center.Y * scaleY);
                int halfSize = areaSize / 2;
                int totalR = 0, totalG = 0, totalB = 0;
                int count = 0;
                for (int dx = -halfSize; dx <= halfSize; dx++)
                {
                    for (int dy = -halfSize; dy <= halfSize; dy++)
                    {
                        int x = pixelX + dx;
                        int y = pixelY + dy;

                        if (x >= 0 && x < _imageSnapshot.PixelWidth &&
                            y >= 0 && y < _imageSnapshot.PixelHeight)
                        {
                            byte[] pixels = new byte[4];
                            _imageSnapshot.CopyPixels(new Int32Rect(x, y, 1, 1), pixels, 4, 0);

                            totalR += pixels[2];
                            totalG += pixels[1];
                            totalB += pixels[0];
                            count++;
                        }
                    }
                }
                if (count == 0) return Colors.Gray;
                return Color.FromRgb(
                    (byte)(totalR / count),
                    (byte)(totalG / count),
                    (byte)(totalB / count));
            }
            catch
            {
                return Colors.Gray;
            }
        }

        private void UndoLastStroke()
        {
            if (_strokeHistory.Count > 0)
            {
                var lastStroke = _strokeHistory.Pop();
                _canvas.Children.Remove(lastStroke);
                SetUndoEnabled();
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
                        _inkRadioButton.IsChecked = true;
                        RadioButtonInk_Click(null, null);
                        SelectElement();
                        _frameworkElement = s as Polyline;
                        _frameworkElement.Opacity = .7;
                    };
                    _canvas.Children.Add(_polyLine);
                    SetUndoEnabled();
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
                    _arrowRadioButton.IsChecked = true;
                    RadioButtonArrow_Click(null, null);
                    SelectElement();
                    _frameworkElement = s as Control;
                    _frameworkElement.Opacity = .7;
                };
                _canvas.Children.Add(_controlArrow);
                SetUndoEnabled();
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
                            _rectangleRadioButton.IsChecked = true;
                            RadioButtonRectangle_Click(null, null);
                            SelectElement();
                            _frameworkElement = s as Border;
                            _frameworkElement.Opacity = .7;
                        };
                        _canvas.Children.Add(_borderRectangle);
                        SetUndoEnabled();
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
                        SetUndoEnabled();
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
                _leftRectangle.Height = _canvas.ActualHeight;
                _leftRectangle.Width = left <= 0 ? 0 : left >= _canvas.ActualWidth ? _canvas.ActualWidth : left;


                Canvas.SetLeft(_topRectangle, _leftRectangle.Width);
                _topRectangle.Height = top <= 0 ? 0 : top >= _canvas.ActualHeight ? _canvas.ActualHeight : top;

                Canvas.SetLeft(_rightRectangle, left + _border.Width);
                var wRight = _canvas.ActualWidth - (_border.Width + _leftRectangle.Width);
                _rightRectangle.Width = wRight <= 0 ? 0 : wRight;
                _rightRectangle.Height = _canvas.ActualHeight;

                Canvas.SetLeft(_bottomRectangle, _leftRectangle.Width);
                Canvas.SetTop(_bottomRectangle, top + _border.Height);
                _bottomRectangle.Width = _border.Width;
                var hBottom = _canvas.ActualHeight - (top + _border.Height);
                _bottomRectangle.Height = hBottom <= 0 ? 0 : hBottom;
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
                else if(_screenCutMouseType == ScreenCutMouseType.DrawMosaic)
                {
                    CompleteCurrentStroke();
                }

                if (_rectangleRadioButton.IsChecked != true
                    &&
                    _radioButtonEllipse.IsChecked != true
                    &&
                    _arrowRadioButton.IsChecked != true
                    &&
                    _textRadioButton.IsChecked != true
                    &&
                    _inkRadioButton.IsChecked != true
                    &&
                    _mosaicRadioButton.IsChecked != true)
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
            _leftRectangle.Width = _rect.X < 0 ? 0 : _rect.X > _canvas.ActualWidth ? _canvas.ActualWidth : _rect.X;
            _leftRectangle.Height = _canvas.Height;

            Canvas.SetLeft(_topRectangle, _leftRectangle.Width);
            _topRectangle.Width = _rect.Width;
            var h = 0.0;
            if (current.Y < vPoint.Y)
                h = current.Y;
            else
                h = current.Y - _rect.Height;

            _topRectangle.Height = h < 0 ? 0 : h > _canvas.ActualHeight ? _canvas.ActualHeight : h;

            Canvas.SetLeft(_rightRectangle, _leftRectangle.Width + _rect.Width);
            var rWidth = _canvas.Width - (_rect.Width + _leftRectangle.Width);
            _rightRectangle.Width = rWidth < 0 ? 0 : rWidth > _canvas.ActualWidth ? _canvas.ActualWidth : rWidth;

            _rightRectangle.Height = _canvas.Height;

            Canvas.SetLeft(_bottomRectangle, _leftRectangle.Width);
            Canvas.SetTop(_bottomRectangle, _rect.Height + _topRectangle.Height);
            _bottomRectangle.Width = _rect.Width;
            var rBottomHeight = _canvas.Height - (_rect.Height + _topRectangle.Height);
            _bottomRectangle.Height = rBottomHeight < 0 ? 0 : rBottomHeight;

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