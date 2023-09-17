using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace WPFDevelopers.Helpers
{
    public class ControlsHelper : DependencyObject
    {
        private static Win32.DeskTopSize size;
        private static readonly Regex _regexNumber = new Regex("[^0-9]+");
        public static Brush Brush = (Brush)Application.Current.TryFindResource("WD.BackgroundSolidColorBrush");

        /// <summary>
        /// PrimaryNormalBrush
        /// </summary>
        public static Brush PrimaryNormalBrush = (Brush)Application.Current.TryFindResource("WD.PrimaryNormalSolidColorBrush");

        public static Brush WindowForegroundBrush =
            (Brush)Application.Current.TryFindResource("WD.PrimaryTextSolidColorBrush");

        private static bool _IsCurrentDark;

        public static void OnSubThemeChanged()
        {
            if (!_IsCurrentDark)
            {
                PrimaryNormalBrush = (Brush)Application.Current.TryFindResource("WD.PrimaryNormalSolidColorBrush");
                Application.Current.Resources["WD.WindowBorderBrushSolidColorBrush"] = PrimaryNormalBrush;
            }
        }

        public static void ToggleLightAndDark(bool isDark = false)
        {
            var type = isDark ? ThemeType.Dark : ThemeType.Light;

            var existingResourceDictionary =
                (Resources)Application.Current.Resources.MergedDictionaries.FirstOrDefault(x => x is Resources);
            if (existingResourceDictionary != null)
            {
                existingResourceDictionary.Theme = type;
                if (type == ThemeType.Light)
                {
                    PrimaryNormalBrush = (Brush)Application.Current.TryFindResource("WD.PrimaryNormalSolidColorBrush");
                    Application.Current.Resources["WD.WindowBorderBrushSolidColorBrush"] = PrimaryNormalBrush;
                    WindowForegroundBrush = (Brush)Application.Current.TryFindResource("WD.PrimaryTextSolidColorBrush");
                    if (Application.Current.TryFindResource("WD.DefaultBackgroundColor") is Color color)
                    {
                        var solidColorBrush = new SolidColorBrush(color);
                        Application.Current.Resources["WD.DefaultBackgroundSolidColorBrush"] = solidColorBrush;

                    }
                }
                else
                {
                    if (Application.Current.TryFindResource("WD.WindowBorderBrushColor") is Color color)
                    {
                        var colorBrush = new SolidColorBrush(color);
                        Application.Current.Resources["WD.WindowBorderBrushSolidColorBrush"] = colorBrush;
                        Application.Current.Resources["WD.DefaultBackgroundSolidColorBrush"] = colorBrush;
                    }

                    WindowForegroundBrush = (Brush)Application.Current.TryFindResource("WD.DefaultBackgroundSolidColorBrush");
                }

                Brush = (Brush)Application.Current.TryFindResource("WD.BackgroundSolidColorBrush");
                _IsCurrentDark = isDark;
                ThemeRefresh();
            }
        }

        public static void ThemeRefresh()
        {
            OnSubThemeChanged();
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ControlsHelper),
                new PropertyMetadata(new CornerRadius(4)));

        /// <summary>
        ///     Get angle
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static double CalculeAngle(Point start, Point end)
        {
            var radian = Math.Atan2(end.Y - start.Y, end.X - start.X);
            var angle = (radian * (180 / Math.PI) + 360) % 360;
            return angle;
        }

        public static bool IsDifferenceOne(double a, double b)
        {
            return Math.Abs(a / b - 1) < 0.0001 || Math.Abs(b / a - 1) < 0.0001;
        }
        public static CornerRadius GetCornerRadius(DependencyObject obj)
        {
            return (CornerRadius)obj.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }

        public static void WindowShake(Window window = null)
        {
            if (window == null)
                if (Application.Current.Windows.Count > 0)
                    window = Application.Current.Windows.OfType<Window>().FirstOrDefault(o => o.IsActive);

            var doubleAnimation = new DoubleAnimation
            {
                From = window.Left,
                To = window.Left + 15,
                Duration = TimeSpan.FromMilliseconds(50),
                AutoReverse = true,
                RepeatBehavior = new RepeatBehavior(3),
                FillBehavior = FillBehavior.Stop
            };
            window.BeginAnimation(Window.LeftProperty, doubleAnimation);
            var wavUri = new Uri(@"pack://application:,,,/WPFDevelopers;component/Resources/Audio/shake.wav");
            var streamResource = Application.GetResourceStream(wavUri);
            var player1 = new SoundPlayer(streamResource.Stream);
            player1.Play();
        }

        public static Uri ImageUri()
        {
            Uri uri = null;
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "图像文件(*.jpg;*.jpeg;*.png;)|*.jpg;*.jpeg;*.png;";
            if (openFileDialog.ShowDialog() == true) uri = new Uri(openFileDialog.FileName);
            return uri;
        }

        public static BitmapFrame CreateResizedImage(ImageSource source, int width, int height, int margin)
        {
            var rect = new Rect(margin, margin, width - margin * 2, height - margin * 2);

            var group = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.HighQuality);
            group.Children.Add(new ImageDrawing(source, rect));

            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawDrawing(group);
            }

            var resizedImage = new RenderTargetBitmap(
                width, height,
                96, 96,
                PixelFormats.Default);
            resizedImage.Render(drawingVisual);
            return BitmapFrame.Create(resizedImage);
        }
        private static long _tick = DateTime.Now.Ticks;
        public static Random GetRandom = new Random((int)(_tick & 0xffffffffL) | (int)(_tick >> 32));

        public static double NextDouble(double miniDouble, double maxiDouble)
        {
            if (GetRandom != null)
            {
                return GetRandom.NextDouble() * (maxiDouble - miniDouble) + miniDouble;
            }
            else
            {
                return 0.0d;
            }
        }
        public static Brush RandomBrush()
        {
            var R = GetRandom.Next(255);
            var G = GetRandom.Next(255);
            var B = GetRandom.Next(255);
            var color = Color.FromRgb((byte)R, (byte)G, (byte)B);
            var solidColorBrush = new SolidColorBrush(color);
            return solidColorBrush;
        }

        public static bool IsNumber(string text)
        {
            return _regexNumber.IsMatch(text);
        }

        public static BitmapSource Capture()
        {
            IntPtr hBitmap;
            var hDC = Win32.GetDC(Win32.GetDesktopWindow());
            var hMemDC = Win32.CreateCompatibleDC(hDC);
            size.cx = Win32.GetSystemMetrics(0);
            size.cy = Win32.GetSystemMetrics(1);
            hBitmap = Win32.CreateCompatibleBitmap(hDC, size.cx, size.cy);
            if (hBitmap != IntPtr.Zero)
            {
                var hOld = Win32.SelectObject(hMemDC, hBitmap);
                Win32.BitBlt(hMemDC, 0, 0, size.cx, size.cy, hDC, 0, 0,
                    Win32.TernaryRasterOperations.SRCCOPY);
                Win32.SelectObject(hMemDC, hOld);
                Win32.DeleteDC(hMemDC);
                Win32.ReleaseDC(Win32.GetDesktopWindow(), hDC);
                var bsource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                Win32.DeleteObject(hBitmap);
                GC.Collect();
                return bsource;
            }
            return null;
        }
        public static AdornerLayer GetAdornerLayer(Visual visual)
        {
            var decorator = visual as AdornerDecorator;
            if (decorator != null)
                return decorator.AdornerLayer;
            var presenter = visual as ScrollContentPresenter;
            if (presenter != null)
                return presenter.AdornerLayer;
            var visualContent = (visual as Window)?.Content as Visual;
            return AdornerLayer.GetAdornerLayer(visualContent ?? visual);
        }

        public static Window GetDefaultWindow()
        {
            Window window = null;
            if (Application.Current.Windows.Count > 0)
            {
                window = Application.Current.Windows.OfType<Window>().FirstOrDefault(o => o.IsActive);
                if (window == null)
                    window = Enumerable.FirstOrDefault(Application.Current.Windows.OfType<Window>());
            }
            return window;
        }
        public static Thickness GetPadding(FrameworkElement element)
        {
            Type elementType = element.GetType();
            PropertyInfo paddingProperty = elementType.GetProperty("Padding");
            if (paddingProperty != null)
                return (Thickness)paddingProperty.GetValue(element, null);
            return new Thickness();
        }

        public static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                    return typedChild;
                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }


    }


    #region 是否设计时模式
    public class DesignerHelper
    {
        private static bool? _isInDesignMode;

        public static bool IsInDesignMode
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                {
                    _isInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty,
                        typeof(FrameworkElement)).Metadata.DefaultValue;
                }
                return _isInDesignMode.Value;
            }
        }
    }
    #endregion
}