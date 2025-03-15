using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFDevelopers.Core;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = ImageTemplateName, Type = typeof(Image))]
    public class VerifyCode : Control
    {
        private const string ImageTemplateName = "PART_Image";
        private const string strCode = "abcdefhkmnprstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(VerifyCode),
                new PropertyMetadata(null));


        public string VerifyCodeText
        {
            get { return (string)GetValue(VerifyCodeTextProperty); }
            private set { SetValue(VerifyCodeTextProperty, value); }
        }

        public static readonly DependencyProperty VerifyCodeTextProperty =
            DependencyProperty.Register("VerifyCodeText", typeof(string), 
                typeof(VerifyCode), new PropertyMetadata(string.Empty));



        private Image _image;
        private Size _size;

        public VerifyCode()
        {
            Foreground = ThemeManager.Instance.PrimaryBrush;
            _size = new Size(70, 23);
            Loaded += CheckCode_Loaded;
        }

        /// <summary>
        ///     随机生成的验证码
        /// </summary>
        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        private void CheckCode_Loaded(object sender, RoutedEventArgs e)
        {
            ImageSource = CreateCheckCodeImage(CreateCode(4), (int)ActualWidth, (int)ActualHeight);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _image = GetTemplateChild(ImageTemplateName) as Image;
            if (_image != null)
                _image.PreviewMouseDown += _image_PreviewMouseDown;
        }

        private void _image_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsLoaded)
                return;

            ImageSource = CreateCheckCodeImage(CreateCode(4), (int)ActualWidth, (int)ActualHeight);
        }

        private string CreateCode(int strLength)
        {
            var _charArray = strCode.ToCharArray();
            var randomCode = "";
            var temp = -1;
            var rand = new Random(Guid.NewGuid().GetHashCode());
            for (var i = 0; i < strLength; i++)
            {
                if (temp != -1)
                    rand = new Random(i * temp * (int)DateTime.Now.Ticks);
                var t = rand.Next(strCode.Length - 1);
                if (!string.IsNullOrWhiteSpace(randomCode))
                    while (randomCode.ToLower().Contains(_charArray[t].ToString().ToLower()))
                        t = rand.Next(strCode.Length - 1);
                if (temp == t)
                    return CreateCode(strLength);
                temp = t;

                randomCode += _charArray[t];
            }
            VerifyCodeText = randomCode;
            return randomCode;
        }

        private ImageSource CreateCheckCodeImage(string checkCode, int width, int height)
        {
            if (string.IsNullOrWhiteSpace(checkCode))
                return null;
            if (width <= 0 || height <= 0)
                return null;
            var drawingVisual = new DrawingVisual();
            var random = new Random(Guid.NewGuid().GetHashCode());
            using (var dc = drawingVisual.RenderOpen())
            {
                dc.DrawRectangle(Brushes.White, new Pen(Foreground, 1), new Rect(_size));
                var formattedText = DrawingContextHelper.GetFormattedText(checkCode, Foreground,
                    FlowDirection.LeftToRight, 20, FontWeights.Bold);
                dc.DrawText(formattedText,
                    new Point((_size.Width - formattedText.Width) / 2, (_size.Height - formattedText.Height) / 2));

                for (var i = 0; i < 10; i++)
                {
                    var x1 = random.Next(width - 1);
                    var y1 = random.Next(height - 1);
                    var x2 = random.Next(width - 1);
                    var y2 = random.Next(height - 1);

                    dc.DrawGeometry(Brushes.Silver, new Pen(Brushes.Silver, 0.5D),
                        new LineGeometry(new Point(x1, y1), new Point(x2, y2)));
                }

                for (var i = 0; i < 100; i++)
                {
                    var x = random.Next(width - 1);
                    var y = random.Next(height - 1);
                    var c = new SolidColorBrush(Color.FromRgb((byte)random.Next(0, 255), (byte)random.Next(0, 255),
                        (byte)random.Next(0, 255)));
                    dc.DrawGeometry(c, new Pen(c, 1D),
                        new LineGeometry(new Point(x - 0.5, y - 0.5), new Point(x + 0.5, y + 0.5)));
                }

                dc.Close();
            }

            var renderBitmap = new RenderTargetBitmap(70, 23, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(drawingVisual);
            return BitmapFrame.Create(renderBitmap);
        }
    }
}