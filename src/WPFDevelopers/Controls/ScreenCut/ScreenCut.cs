using Microsoft.Win32;
using System;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        private Button _buttonSave,_buttonCancel, _buttonComplete;
        private Rect rect;
        private Point pointStart, pointEnd;
        private bool isMouseUp = false;

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
            this._canvas.Background = new ImageBrush(ChangeBitmapToImageSource(CaptureScreen()));
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
            var renderTargetBitmap = new RenderTargetBitmap((int)_canvas.Width,
  (int)_canvas.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            renderTargetBitmap.Render(_canvas);
            return  new CroppedBitmap(renderTargetBitmap, new Int32Rect((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height));
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
            if (!isMouseUp)
            {
                _wrapPanel.Visibility = Visibility.Hidden;
                pointStart = e.GetPosition(_canvas);
                pointEnd = pointStart;
                rect = new Rect(pointStart, pointEnd);
            }

        }
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !isMouseUp)
            {
                var current = e.GetPosition(_canvas);
                MoveAllRectangle(current);
            }
        }
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (!isMouseUp)
            {
                _wrapPanel.Visibility = Visibility.Visible;
                Canvas.SetLeft(this._wrapPanel, rect.X + rect.Width - this._wrapPanel.ActualWidth);
                Canvas.SetTop(this._wrapPanel, rect.Y + rect.Height + 4);
                isMouseUp = true;
            }
        }

        void MoveAllRectangle(Point current)
        {
            pointEnd = current;
            rect = new Rect(pointStart, pointEnd);
            this._rectangleLeft.Width = rect.X;
            this._rectangleLeft.Height = _canvas.Height;

            Canvas.SetLeft(this._rectangleTop, this._rectangleLeft.Width);
            this._rectangleTop.Width = rect.Width;
            double h = 0.0;
            if (current.Y < pointStart.Y)
                h = current.Y;
            else
                h = current.Y - rect.Height;
            this._rectangleTop.Height = h;

            Canvas.SetLeft(this._rectangleRight, this._rectangleLeft.Width + rect.Width);
            this._rectangleRight.Width = _canvas.Width - (rect.Width + this._rectangleLeft.Width);
            this._rectangleRight.Height = _canvas.Height;

            Canvas.SetLeft(this._rectangleBottom, this._rectangleLeft.Width);
            Canvas.SetTop(this._rectangleBottom, rect.Height + this._rectangleTop.Height);
            this._rectangleBottom.Width = rect.Width;
            this._rectangleBottom.Height = _canvas.Height - (rect.Height + this._rectangleTop.Height);

            this._border.Height = rect.Height;
            this._border.Width = rect.Width;
            Canvas.SetLeft(this._border, rect.X);
            Canvas.SetTop(this._border, rect.Y);
        }

        System.Drawing.Bitmap CaptureScreen()
        {
            //修复缩放比例不等于100%（DPI不等于96）时，不能显示全屏的问题
            var source = PresentationSource.FromVisual(_canvas);
            double dpiX = source.CompositionTarget.TransformToDevice.M11; //96.0 * 
            double dpiY = source.CompositionTarget.TransformToDevice.M22; //96.0 * 

            var bmpCaptured = new System.Drawing.Bitmap((int)(SystemParameters.PrimaryScreenWidth * dpiX), (int)(SystemParameters.PrimaryScreenHeight * dpiY), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmpCaptured))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.CopyFromScreen(0, 0, 0, 0, bmpCaptured.Size, System.Drawing.CopyPixelOperation.SourceCopy);
            }
            return bmpCaptured;
        }
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        ImageSource ChangeBitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!DeleteObject(hBitmap))
            {
                throw new System.ComponentModel.Win32Exception();
            }
            return wpfBitmap;
        }

    }
}
