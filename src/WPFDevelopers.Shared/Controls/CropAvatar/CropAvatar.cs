using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = CanvasTemplateName, Type = typeof(Canvas))]
    [TemplatePart(Name = ImageTemplateName, Type = typeof(Image))]
    [TemplatePart(Name = PathTemplateName, Type = typeof(Path))]
    [TemplatePart(Name = GridTemplateName, Type = typeof(Grid))]
    [TemplatePart(Name = ReplaceButtonTemplateName, Type = typeof(Button))]
    [TemplatePart(Name = AddButtonTemplateName, Type = typeof(Button))]
    public class CropAvatar : Control
    {
        private const string CanvasTemplateName = "PART_Canvas";
        private const string ImageTemplateName = "PART_Image";
        private const string PathTemplateName = "PART_Layout";
        private const string GridTemplateName = "PART_Grid";
        private const string ReplaceButtonTemplateName = "PART_ReplaceButton";
        private const string AddButtonTemplateName = "PART_AddButton";
        private const int _size = 200;

        public static readonly DependencyProperty OutImageSourceProperty =
            DependencyProperty.Register("OutImageSource", typeof(ImageSource), typeof(CropAvatar),
                new PropertyMetadata(null));

        private BitmapFrame bitmapFrame;
        private Canvas canvas;
        private CroppedBitmap crop;
        private Grid grid;
        private Image image;
        private int initialX, initialY, voffsetX, voffsetY;
        private bool isDown;
        private bool isLeft;
        private Path path;
        private Point point;
        private Button replaceButton, addButton;
        private double vNewStartX, vNewStartY, _StartX, _StartY, centerX, centerY;


        static CropAvatar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CropAvatar),
                new FrameworkPropertyMetadata(typeof(CropAvatar)));
        }

        public ImageSource OutImageSource
        {
            get => (ImageSource)GetValue(OutImageSourceProperty);
            set => SetValue(OutImageSourceProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            canvas = GetTemplateChild(CanvasTemplateName) as Canvas;
            canvas.Loaded += Canvas_Loaded;
            grid = GetTemplateChild(GridTemplateName) as Grid;
            image = GetTemplateChild(ImageTemplateName) as Image;
            image.MouseDown += Image_MouseDown;
            image.MouseMove += Image_MouseMove;
            image.MouseUp += Image_MouseUp;
            image.MouseLeave += Image_MouseLeave;
            path = GetTemplateChild(PathTemplateName) as Path;
            replaceButton = GetTemplateChild(ReplaceButtonTemplateName) as Button;
            replaceButton.Click += ReplaceButton_Click;
            addButton = GetTemplateChild(AddButtonTemplateName) as Button;
            addButton.Click += AddButton_Click;
        }

        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Canvas canvas)
            {
                var width = canvas.ActualWidth;
                var height = canvas.ActualHeight;
                centerX = (width - path.Width) / 2.0d;
                centerY = (height - path.Height) / 2.0d;
                canvas.Clip = new RectangleGeometry(new Rect(centerX, centerY, 200, 200));
                Canvas.SetLeft(path, centerX);
                Canvas.SetTop(path, centerY);
                Canvas.SetLeft(grid, centerX);
                Canvas.SetTop(grid, centerY);
            }
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            isDown = false;
            SettingPoint();
        }

        void SettingPoint()
        {
            if (isLeft)
            {
                _StartX = Canvas.GetLeft(image);
                initialX = voffsetX;
            }

            else
            {
                _StartY = Canvas.GetTop(image);
                initialY = voffsetY;
            }
        }
        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isDown)
                SettingPoint();
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && isDown)
            {
                var vPoint = e.GetPosition(this);
                if (isLeft)
                {
                    var voffset = vPoint.X - point.X;
                    vNewStartX = _StartX + voffset;
                    var xPath = Canvas.GetLeft(path);
                    if (vNewStartX <= xPath && vNewStartX >= -(bitmapFrame.Width - 200 - xPath))
                    {
                        Canvas.SetLeft(image, vNewStartX);
                        voffsetX = initialX - (int)Math.Round(voffset);
                        voffsetX = voffsetX < 0 ? 0 : voffsetX;
                        crop = new CroppedBitmap(bitmapFrame, new Int32Rect(voffsetX, 0, _size, _size));
                    }
                }
                else
                {
                    var voffset = vPoint.Y - point.Y;
                    vNewStartY = _StartY + voffset;
                    var yPath = Canvas.GetTop(path);
                    if (vNewStartY <= yPath && vNewStartY >= -(bitmapFrame.Height - 200 - yPath))
                    {
                        Canvas.SetTop(image, vNewStartY);
                        voffsetY = initialY - (int)Math.Round(voffset);
                        voffsetY = voffsetY < 0 ? 0 : voffsetY;
                        crop = new CroppedBitmap(bitmapFrame, new Int32Rect(0, voffsetY, _size, _size));
                    }
                }

                OutImageSource = crop;
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDown = true;
            point = e.GetPosition(this);
        }

        private void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            InitialImage();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            InitialImage();
        }

        private void InitialImage()
        {
            vNewStartX = 0;
            vNewStartY = 0;
            var uri = ControlsHelper.ImageUri();
            if (uri == null) return;
            var bitmap = new BitmapImage(uri);
            if (bitmap.Height > bitmap.Width)
            {
                var scale = bitmap.Width / path.Width;
                image.Width = _size;
                image.Height = bitmap.Height / scale;
                isLeft = false;
            }
            else if (bitmap.Width > bitmap.Height)
            {
                var scale = bitmap.Height / path.Height;
                image.Width = bitmap.Width / scale;
                image.Height = _size;
                isLeft = true;
            }

            bitmapFrame = ControlsHelper.CreateResizedImage(bitmap, (int)image.Width, (int)image.Height, 0);
            image.Source = bitmapFrame;
            if (image.Source != null)
            {
                replaceButton.Visibility = Visibility.Visible;
                addButton.Visibility = Visibility.Collapsed;
            }

            Canvas.SetLeft(grid, centerX);
            Canvas.SetTop(grid, centerY);
            _StartX = (canvas.ActualWidth - image.Width) / 2.0d;
            _StartY = (canvas.ActualHeight - image.Height) / 2.0d;
            Canvas.SetLeft(image, _StartX);
            Canvas.SetTop(image, _StartY);
            if (isLeft)
            {
                initialX = ((int)image.Width - 200) / 2;
                initialY = 0;
                crop = new CroppedBitmap(bitmapFrame, new Int32Rect(initialX, 0, _size, _size));
            }
            else
            {
                initialY = ((int)image.Height - 200) / 2;
                initialX = 0;
                crop = new CroppedBitmap(bitmapFrame, new Int32Rect(0, initialY, _size, _size));
            }

            OutImageSource = crop;
        }
    }
}