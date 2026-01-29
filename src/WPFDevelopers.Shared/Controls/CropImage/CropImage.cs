using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
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
    public class CropImage : Control
    {
        private const string CanvasTemplateName = "PART_Canvas";
        private const string RectangleLeftTemplateName = "PART_RectangleLeft";
        private const string RectangleTopTemplateName = "PART_RectangleTop";
        private const string RectangleRightTemplateName = "PART_RectangleRight";
        private const string RectangleBottomTemplateName = "PART_RectangleBottom";
        private const string BorderTemplateName = "PART_Border";

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(CropImage),
                new PropertyMetadata(null, OnSourceChanged));

        public static readonly DependencyProperty CurrentRectProperty =
            DependencyProperty.Register("CurrentRect", typeof(Rect), typeof(CropImage), new PropertyMetadata(null));

        public static readonly DependencyProperty CurrentAreaBitmapProperty =
            DependencyProperty.Register("CurrentAreaBitmap", typeof(ImageSource), typeof(CropImage),
                new PropertyMetadata(null));

        public static readonly DependencyProperty IsRatioScaleProperty =
            DependencyProperty.Register("IsRatioScale", typeof(bool), typeof(CropImage),
                new PropertyMetadata(false, OnIsRatioScalePropertyChanged));

        private static void OnIsRatioScalePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cropImage = d as CropImage;
            if (cropImage != null)
                cropImage.DrawImage();
        }

        public static readonly DependencyProperty ScaleSizeProperty =
            DependencyProperty.Register("ScaleSize", typeof(Size), typeof(CropImage),
                new PropertyMetadata(new Size(2, 1)));

        public static readonly DependencyProperty RectScaleProperty =
            DependencyProperty.Register("RectScale", typeof(double), typeof(CropImage),
                new PropertyMetadata(.5d, OnRectScalePropertyChanged));

        private static void OnRectScalePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (double)e.NewValue;
            if (!IsValueValid(newValue))
                throw new ArgumentException("Value must be between 0 and 1.");
            var cropImage = d as CropImage;
            if (cropImage != null)
                cropImage.DrawImage();
        }

        private static bool IsValueValid(double value)
        {
            return (value >= 0.0 && value <= 1.0);
        }

        private Border _border;
        private Canvas _canvas;
        private Rectangle _rectangleLeft, _rectangleTop, _rectangleRight, _rectangleBottom;

        private AdornerLayer _adornerLayer;
        private BitmapFrame _bitmapFrame;
        private bool _isDragging;
        private double _offsetX, _offsetY;
        private ScreenCutAdorner _screenCutAdorner;
        private bool _isInitialized = false;
        private bool _isUnloaded = false;

        static CropImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CropImage),
                new FrameworkPropertyMetadata(typeof(CropImage)));
        }

        public CropImage()
        {
            Loaded -= OnCropImage_Loaded;
            Loaded += OnCropImage_Loaded;
            Unloaded -= OnCropImage_Unloaded;
            Unloaded += OnCropImage_Unloaded;
        }

        public ImageSource Source
        {
            get => (ImageSource)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public Rect CurrentRect
        {
            get => (Rect)GetValue(CurrentRectProperty);
            private set => SetValue(CurrentRectProperty, value);
        }

        public ImageSource CurrentAreaBitmap
        {
            get => (ImageSource)GetValue(CurrentAreaBitmapProperty);
            private set => SetValue(CurrentAreaBitmapProperty, value);
        }

        public bool IsRatioScale
        {
            get => (bool)GetValue(IsRatioScaleProperty);
            set => SetValue(IsRatioScaleProperty, value);
        }

        public Size ScaleSize
        {
            get => (Size)GetValue(ScaleSizeProperty);
            set => SetValue(ScaleSizeProperty, value);
        }

        public double RectScale
        {
            get => (double)GetValue(RectScaleProperty);
            set => SetValue(RectScaleProperty, value);
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var crop = (CropImage)d;
            if (crop != null)
            {
                crop.CleanupResources();
                crop.DrawImage();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (_isUnloaded) return;
            _canvas = GetTemplateChild(CanvasTemplateName) as Canvas;
            _rectangleLeft = GetTemplateChild(RectangleLeftTemplateName) as Rectangle;
            _rectangleTop = GetTemplateChild(RectangleTopTemplateName) as Rectangle;
            _rectangleRight = GetTemplateChild(RectangleRightTemplateName) as Rectangle;
            _rectangleBottom = GetTemplateChild(RectangleBottomTemplateName) as Rectangle;
            _border = GetTemplateChild(BorderTemplateName) as Border;

            if (!_isInitialized)
            {
                _isInitialized = true;
                InitializeEvents();
            }

            DrawImage();
        }

        private void OnCropImage_Unloaded(object sender, RoutedEventArgs e)
        {
            _isUnloaded = true;
            CleanupResources(true);
        }

        private void OnCropImage_Loaded(object sender, RoutedEventArgs e)
        {
            _isUnloaded = false;
            if (!_isInitialized && _border != null)
            {
                InitializeEvents();
            }
            DrawImage();
        }

        private void CleanupResources(bool fullCleanup = false)
        {
            try
            {
                if (_screenCutAdorner != null && _adornerLayer != null)
                {
                    _adornerLayer.Remove(_screenCutAdorner);
                    _screenCutAdorner = null;
                }

                if (fullCleanup)
                {
                    UninitializeEvents();
                    _adornerLayer = null;

                    if (_bitmapFrame != null)
                    {
                        _bitmapFrame = null;
                    }

                    if (_canvas != null)
                    {
                        var brush = _canvas.Background as ImageBrush;
                        if (brush != null)
                        {
                            brush.ImageSource = null;
                            _canvas.Background = null;
                        }
                    }

                    if (CurrentAreaBitmap != null)
                    {
                        CurrentAreaBitmap = null;
                    }

                    _border = null;
                    _canvas = null;
                    _isInitialized = false;
                    _isDragging = false;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CleanupResources error: {ex.Message}");
            }
        }


        private void InitializeEvents()
        {
            if (_border == null) return;

            _border.SizeChanged -= Border_SizeChanged;
            _border.SizeChanged += Border_SizeChanged;
            _border.MouseDown -= Border_MouseDown;
            _border.MouseDown += Border_MouseDown;
            _border.MouseMove -= Border_MouseMove;
            _border.MouseMove += Border_MouseMove;
            _border.MouseUp -= Border_MouseUp;
            _border.MouseUp += Border_MouseUp;
        }


        private void UninitializeEvents()
        {
            if (_border == null) return;

            _border.SizeChanged -= Border_SizeChanged;
            _border.MouseDown -= Border_MouseDown;
            _border.MouseMove -= Border_MouseMove;
            _border.MouseUp -= Border_MouseUp;
        }

        private void DrawImage()
        {
            if (Source == null) return;

            CleanupResources();

            if (_border.Visibility == Visibility.Collapsed)
                _border.Visibility = Visibility.Visible;

            var bitmap = Source as BitmapImage;
            if (bitmap == null) return;

            _bitmapFrame = ControlsHelper.CreateResizedImage(bitmap, (int)bitmap.Width, (int)bitmap.Height, 0);

            _canvas.Width = bitmap.Width;
            _canvas.Height = bitmap.Height;

            var imageBrush = new ImageBrush(bitmap)
            {
                Stretch = Stretch.Uniform
            };
            _canvas.Background = imageBrush;

            if (IsRatioScale && !ScaleSize.IsEmpty &&
                ScaleSize.Width > 0 && ScaleSize.Height > 0)
            {
                double baseScale = RectScale;
                double imageAspect = bitmap.Width / bitmap.Height;
                double targetAspect = ScaleSize.Width / ScaleSize.Height;

                if (imageAspect >= targetAspect)
                {
                    _border.Height = bitmap.Height * baseScale;
                    _border.Width = _border.Height * targetAspect;

                    if (_border.Width > bitmap.Width)
                    {
                        _border.Width = bitmap.Width * baseScale;
                        _border.Height = _border.Width / targetAspect;
                    }
                }
                else
                {
                    _border.Width = bitmap.Width * baseScale;
                    _border.Height = _border.Width / targetAspect;

                    if (_border.Height > bitmap.Height)
                    {
                        _border.Height = bitmap.Height * baseScale;
                        _border.Width = _border.Height * targetAspect;
                    }
                }

                const double MIN_SIZE = 10.0;
                _border.Width = Math.Max(_border.Width, MIN_SIZE);
                _border.Height = Math.Max(_border.Height, MIN_SIZE);
            }
            else
            {
                _border.Width = bitmap.Width * RectScale;
                _border.Height = bitmap.Height * RectScale;

                const double MIN_SIZE = 10.0;
                _border.Width = Math.Max(_border.Width, MIN_SIZE);
                _border.Height = Math.Max(_border.Height, MIN_SIZE);
            }

            var cx = Math.Max(0, Math.Min(_canvas.Width / 2 - _border.Width / 2, _canvas.Width - _border.Width));
            var cy = Math.Max(0, Math.Min(_canvas.Height / 2 - _border.Height / 2, _canvas.Height - _border.Height));

            Canvas.SetLeft(_border, cx);
            Canvas.SetTop(_border, cy);

            if (_adornerLayer == null)
            {
                _adornerLayer = AdornerLayer.GetAdornerLayer(_border);
            }
            if (_screenCutAdorner == null && _adornerLayer != null)
            {
                _screenCutAdorner = new ScreenCutAdorner(_border, IsRatioScale, ScaleSize);
                _adornerLayer.Add(_screenCutAdorner);
            }
            Render();
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            var draggableControl = sender as UIElement;
            draggableControl?.ReleaseMouseCapture();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isDragging)
            {
                _isDragging = true;
                var draggableControl = sender as UIElement;
                var position = e.GetPosition(this);
                _offsetX = position.X - Canvas.GetLeft(draggableControl);
                _offsetY = position.Y - Canvas.GetTop(draggableControl);
                draggableControl?.CaptureMouse();
            }
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                var draggableControl = sender as UIElement;
                var position = e.GetPosition(this);

                var x = position.X - _offsetX;
                x = Math.Max(0, Math.Min(x, _canvas.Width - _border.ActualWidth));

                var y = position.Y - _offsetY;
                y = Math.Max(0, Math.Min(y, _canvas.Height - _border.ActualHeight));

                Canvas.SetLeft(draggableControl, x);
                Canvas.SetTop(draggableControl, y);
                Render();
            }
        }

        private void Render()
        {
            if (_border == null || _canvas == null) return;

            var cy = Math.Max(0, Canvas.GetTop(_border));
            var borderLeft = Math.Max(0, Canvas.GetLeft(_border));
            UpdateMaskRectangles(cy, borderLeft);
            var bitmap = CutBitmap();
            if (bitmap == null) return;
            var frame = BitmapFrame.Create(bitmap);
            CurrentAreaBitmap = frame;
            CurrentRect = new Rect(borderLeft, cy, _border.ActualWidth, _border.ActualHeight);
        }

        private void UpdateMaskRectangles(double cy, double borderLeft)
        {
            _rectangleLeft.Width = borderLeft;
            _rectangleLeft.Height = _border.ActualHeight;
            Canvas.SetTop(_rectangleLeft, cy);

            _rectangleTop.Width = _canvas.Width;
            _rectangleTop.Height = cy;

            var rx = Math.Min(borderLeft + _border.ActualWidth, _canvas.Width);
            _rectangleRight.Width = Math.Max(0, _canvas.Width - rx);
            _rectangleRight.Height = _border.ActualHeight;
            Canvas.SetLeft(_rectangleRight, rx);
            Canvas.SetTop(_rectangleRight, cy);

            var by = Math.Min(cy + _border.ActualHeight, _canvas.Height);
            _rectangleBottom.Width = _canvas.Width;
            _rectangleBottom.Height = Math.Max(0, _canvas.Height - by);
            Canvas.SetTop(_rectangleBottom, by);
        }

        private void Border_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Render();
        }

        private WriteableBitmap CutBitmap()
        {
            try
            {
                if (_bitmapFrame == null || _border == null) return null;

                var left = (int)Canvas.GetLeft(_border);
                var top = (int)Canvas.GetTop(_border);
                var width = (int)_border.ActualWidth;
                var height = (int)_border.ActualHeight;

                if (width <= 0 || height <= 0) return null;

                left = Math.Max(0, Math.Min(left, _bitmapFrame.PixelWidth - width));
                top = Math.Max(0, Math.Min(top, _bitmapFrame.PixelHeight - height));
                width = Math.Min(width, _bitmapFrame.PixelWidth - left);
                height = Math.Min(height, _bitmapFrame.PixelHeight - top);

                if (width <= 0 || height <= 0) return null;

                var croppedBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Pbgra32, null);
                var stride = (int)(_bitmapFrame.PixelWidth * (_bitmapFrame.Format.BitsPerPixel / 8.0));
                var pixelData = new byte[stride * _bitmapFrame.PixelHeight];
                _bitmapFrame.CopyPixels(pixelData, stride, 0);

                var sourceOffset = left * (_bitmapFrame.Format.BitsPerPixel / 8) + top * stride;
                croppedBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixelData, stride, sourceOffset);

                return croppedBitmap;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CutBitmap error: {ex.Message}");
                return null;
            }
        }
    }
}