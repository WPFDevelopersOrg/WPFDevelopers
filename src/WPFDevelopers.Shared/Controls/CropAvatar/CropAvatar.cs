using System;
using System.Diagnostics;
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
    [TemplatePart(Name = GridTemplateName, Type = typeof(SmallPanel))]
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

        private BitmapFrame _bitmapFrame;
        private Canvas _canvas;
        private CroppedBitmap _crop;
        private SmallPanel _grid;
        private Image _image;
        private int _initialX, _initialY, _voffsetX, _voffsetY;
        private bool _isDown;
        private bool? _isLeft;
        private Path _path;
        private Point _point;
        private Button _replaceButton, _addButton;
        private double _vNewStartX, _vNewStartY, _startX, _startY, _centerX, _centerY;


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
            _canvas = GetTemplateChild(CanvasTemplateName) as Canvas;
            if(_canvas != null)
            {
                _canvas.Loaded -= OnCanvas_Loaded;
                _canvas.Loaded += OnCanvas_Loaded;
            }
            _grid = GetTemplateChild(GridTemplateName) as SmallPanel;
            _image = GetTemplateChild(ImageTemplateName) as Image;
            if(_image != null)
            {
                _image.MouseDown -= OnImage_MouseDown;
                _image.MouseDown += OnImage_MouseDown;
                _image.MouseMove -= OnImage_MouseMove;
                _image.MouseMove += OnImage_MouseMove;
                _image.MouseUp -= OnImage_MouseUp;
                _image.MouseUp += OnImage_MouseUp;
                _image.MouseLeave -= OnImage_MouseLeave;
                _image.MouseLeave += OnImage_MouseLeave;
            }
            _path = GetTemplateChild(PathTemplateName) as Path;
            _replaceButton = GetTemplateChild(ReplaceButtonTemplateName) as Button;
            if(_replaceButton != null)
            {
                _replaceButton.Click -= OnReplaceButton_Click;
                _replaceButton.Click += OnReplaceButton_Click;
            }
            _addButton = GetTemplateChild(AddButtonTemplateName) as Button;
            if (_addButton != null)
            {
                _addButton.Click -= OnAddButton_Click;
                _addButton.Click += OnAddButton_Click;
            }
        }

        private void OnCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Canvas canvas)
            {
                var width = canvas.ActualWidth;
                var height = canvas.ActualHeight;
                _centerX = (width - _path.Width) / 2.0d;
                _centerY = (height - _path.Height) / 2.0d;
                canvas.Clip = new RectangleGeometry(new Rect(_centerX, _centerY, 200, 200));
                Canvas.SetLeft(_path, _centerX);
                Canvas.SetTop(_path, _centerY);
                Canvas.SetLeft(_grid, _centerX);
                Canvas.SetTop(_grid, _centerY);
            }
        }

        private void OnImage_MouseLeave(object sender, MouseEventArgs e)
        {
            _isDown = false;
            SettingPoint();
        }

        private void SettingPoint()
        {
            if (_isLeft == true)
            {
                _startX = Canvas.GetLeft(_image);
                _initialX = _voffsetX;
            }
            else if (_isLeft == false)
            {
                _startY = Canvas.GetTop(_image);
                _initialY = _voffsetY;
            }
        }

        private void OnImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDown)
                SettingPoint();
        }

        private void OnImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _isDown)
            {
                var vPoint = e.GetPosition(this);
                if (_isLeft == true)
                {
                    var voffset = vPoint.X - _point.X;
                    _vNewStartX = _startX + voffset;
                    var xPath = Canvas.GetLeft(_path);
                    if (_vNewStartX <= xPath && _vNewStartX >= -(_bitmapFrame.Width - 200 - xPath))
                    {
                        Canvas.SetLeft(_image, _vNewStartX);
                        _voffsetX = _initialX - (int)Math.Round(voffset);
                        _voffsetX = _voffsetX < 0 ? 0 : _voffsetX;
                        _crop = new CroppedBitmap(_bitmapFrame,
                            new Int32Rect(_voffsetX + _size > _bitmapFrame.Width ? Convert.ToInt32(_bitmapFrame.Width - _size) : _voffsetX, 0, _size, _size));
                    }
                }
                else if (_isLeft == false)
                {
                    var voffset = vPoint.Y - _point.Y;
                    _vNewStartY = _startY + voffset;
                    var yPath = Canvas.GetTop(_path);
                    if (_vNewStartY <= yPath && _vNewStartY >= -(_bitmapFrame.Height - 200 - yPath))
                    {
                        Canvas.SetTop(_image, _vNewStartY);
                        _voffsetY = _initialY - (int)Math.Round(voffset);
                        _voffsetY = _voffsetY < 0 ? 0 : _voffsetY;
                        _crop = new CroppedBitmap(_bitmapFrame,
                            new Int32Rect(0, _voffsetY + _size > _bitmapFrame.Height ? Convert.ToInt32(_bitmapFrame.Height - _size) : _voffsetY, _size, _size));
                    }
                }
                OutImageSource = _crop;
            }
        }

        private void OnImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDown = true;
            _point = e.GetPosition(this);
        }

        private void OnReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            InitialImage();
        }

        private void OnAddButton_Click(object sender, RoutedEventArgs e)
        {
            InitialImage();
        }

        private void InitialImage()
        {
            _vNewStartX = 0;
            _vNewStartY = 0;
            var uri = Helper.ImageUri();
            if (uri == null) return;
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = uri;
            bitmap.EndInit();
            if (bitmap.Height > bitmap.Width)
            {
                var scale = bitmap.Width / _path.Width;
                _image.Width = _size;
                _image.Height = bitmap.Height / scale;
                _isLeft = false;
            }
            else if (bitmap.Width > bitmap.Height)
            {
                var scale = bitmap.Height / _path.Height;
                _image.Width = bitmap.Width / scale;
                _image.Height = _size;
                _isLeft = true;
            }
            else
            {
                _image.Width = _size;
                _image.Height = _size;
                _isLeft = null;
            }

            _bitmapFrame = ControlsHelper.CreateResizedImage(bitmap, (int)_image.Width, (int)_image.Height, 0);
            _image.Source = _bitmapFrame;
            if (_image.Source != null)
            {
                _replaceButton.Visibility = Visibility.Visible;
                _addButton.Visibility = Visibility.Collapsed;
            }
            Canvas.SetLeft(_grid, _centerX);
            Canvas.SetTop(_grid, _centerY);
            _startX = (_canvas.ActualWidth - _image.Width) / 2.0d;
            _startY = (_canvas.ActualHeight - _image.Height) / 2.0d;
            Canvas.SetLeft(_image, _startX);
            Canvas.SetTop(_image, _startY);
            if (_isLeft == true)
            {
                _initialX = ((int)_image.Width - 200) / 2;
                _initialY = 0;
                _crop = new CroppedBitmap(_bitmapFrame,
                            new Int32Rect(_voffsetX + _size > _bitmapFrame.Width ? Convert.ToInt32(_bitmapFrame.Width - _size) : _voffsetX, 0, _size, _size));
            }
            else if (_isLeft == false)
            {
                _initialY = ((int)_image.Height - 200) / 2;
                _initialX = 0;
                _crop = new CroppedBitmap(_bitmapFrame,
                             new Int32Rect(0, _voffsetY + _size > _bitmapFrame.Height ? Convert.ToInt32(_bitmapFrame.Height - _size) : _voffsetY, _size, _size));
            }
            else
            {
                _crop = new CroppedBitmap(_bitmapFrame, new Int32Rect(0, 0, _size, _size));
            }
            OutImageSource = _crop;
        }
    }
}