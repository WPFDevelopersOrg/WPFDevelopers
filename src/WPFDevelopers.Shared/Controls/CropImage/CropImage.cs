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


        private AdornerLayer adornerLayer;

        private BitmapFrame bitmapFrame;
        private bool isDragging;
        private double offsetX, offsetY;
        private ScreenCutAdorner screenCutAdorner;

        static CropImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CropImage),
                new FrameworkPropertyMetadata(typeof(CropImage)));
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
                crop.DrawImage();
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
            DrawImage();
        }

        private void DrawImage()
        {
            if (Source == null)
            {
                _border.Visibility = Visibility.Collapsed;
                if (adornerLayer == null) return;
                adornerLayer.Remove(screenCutAdorner);
                screenCutAdorner = null;
                adornerLayer = null;
                return;
            }

            _border.Visibility = Visibility.Visible;
            var bitmap = (BitmapImage)Source;
            bitmapFrame = ControlsHelper.CreateResizedImage(bitmap, (int)bitmap.Width, (int)bitmap.Height, 0);
            _canvas.Width = bitmap.Width;
            _canvas.Height = bitmap.Height;
            _canvas.Background = new ImageBrush(bitmap);
            if (IsRatioScale && !ScaleSize.IsEmpty &&
                ScaleSize.Width > double.MinValue &&
                ScaleSize.Height > double.MinValue &&
                ScaleSize.Width != ScaleSize.Height)
            {
                if(ScaleSize.Width > ScaleSize.Height)
                {
                    _border.Width = bitmap.Width * RectScale;
                    _border.Height = _border.Width / ScaleSize.Width;
                }
                else
                {
                    _border.Height = bitmap.Height * RectScale;
                    _border.Width = _border.Height * ScaleSize.Height;
                }
            }
            else
            {
                _border.Width = bitmap.Width * RectScale;
                _border.Height = bitmap.Height * RectScale;
            }

            var cx = _canvas.Width / 2 - _border.Width / 2;
            var cy = _canvas.Height / 2 - _border.Height / 2;
            Canvas.SetLeft(_border, cx);
            Canvas.SetTop(_border, cy);
            if (adornerLayer != null) return;
            adornerLayer = AdornerLayer.GetAdornerLayer(_border);
            screenCutAdorner = new ScreenCutAdorner(_border, IsRatioScale, ScaleSize);
            adornerLayer.Add(screenCutAdorner);
            _border.SizeChanged -= Border_SizeChanged;
            _border.SizeChanged += Border_SizeChanged;
            _border.MouseDown -= Border_MouseDown;
            _border.MouseDown += Border_MouseDown;
            _border.MouseMove -= Border_MouseMove;
            _border.MouseMove += Border_MouseMove;
            _border.MouseUp -= Border_MouseUp;
            _border.MouseUp += Border_MouseUp;
        }


        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            var draggableControl = sender as UIElement;
            draggableControl.ReleaseMouseCapture();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!isDragging)
            {
                isDragging = true;
                var draggableControl = sender as UIElement;
                var position = e.GetPosition(this);
                offsetX = position.X - Canvas.GetLeft(draggableControl);
                offsetY = position.Y - Canvas.GetTop(draggableControl);
                draggableControl.CaptureMouse();
            }
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                var draggableControl = sender as UIElement;
                var position = e.GetPosition(this);
                var x = position.X - offsetX;
                x = x < 0 ? 0 : x;
                x = x + _border.Width > _canvas.Width ? _canvas.Width - _border.Width : x;
                var y = position.Y - offsetY;
                y = y < 0 ? 0 : y;
                y = y + _border.Height > _canvas.Height ? _canvas.Height - _border.Height : y;
                Canvas.SetLeft(draggableControl, x);
                Canvas.SetTop(draggableControl, y);
                Render();
            }
        }

        private void Render()
        {
            var cy = Canvas.GetTop(_border);
            cy = cy < 0 ? 0 : cy;
            var borderLeft = Canvas.GetLeft(_border);
            borderLeft = borderLeft < 0 ? 0 : borderLeft;
            _rectangleLeft.Width = borderLeft;
            _rectangleLeft.Height = _border.ActualHeight;
            Canvas.SetTop(_rectangleLeft, cy);

            _rectangleTop.Width = _canvas.Width;
            _rectangleTop.Height = cy;

            var rx = borderLeft + _border.ActualWidth;
            rx = rx > _canvas.Width ? _canvas.Width : rx;
            _rectangleRight.Width = _canvas.Width - rx;
            _rectangleRight.Height = _border.ActualHeight;
            Canvas.SetLeft(_rectangleRight, rx);
            Canvas.SetTop(_rectangleRight, cy);

            var by = cy + _border.ActualHeight;
            by = by < 0 ? 0 : by;
            _rectangleBottom.Width = _canvas.Width;
            var rby = _canvas.Height - by;
            _rectangleBottom.Height = rby < 0 ? 0 : rby;
            Canvas.SetTop(_rectangleBottom, by);

            var bitmap = CutBitmap();
            if (bitmap == null) return;
            var frame = BitmapFrame.Create(bitmap);
            CurrentAreaBitmap = frame;
        }

        private void Border_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Render();
        }

        private CroppedBitmap CutBitmap()
        {
            try
            {
                var width = _border.Width;
                var height = _border.Height;
                if (double.IsNaN(width) || double.IsNaN(height))
                    return null;
                var left = Canvas.GetLeft(_border);
                var top = Canvas.GetTop(_border);
                CurrentRect = new Rect(left, top, width, height);
                if (CurrentRect.IsEmpty || CurrentRect.Width <= 0 || CurrentRect.Height <= 0) return null;
                return new CroppedBitmap(bitmapFrame,
                    new Int32Rect((int)CurrentRect.X, (int)CurrentRect.Y, (int)CurrentRect.Width,
                        (int)CurrentRect.Height));
            }
            catch
            {
                throw;
            }
        }
    }
}