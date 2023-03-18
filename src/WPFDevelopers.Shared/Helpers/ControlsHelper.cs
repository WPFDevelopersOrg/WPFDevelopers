using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using WPFDevelopers.Controls;
using WPFDevelopers.Utilities;

namespace WPFDevelopers.Helpers
{
    public class ControlsHelper : DependencyObject
    {
        private static Win32ApiHelper.DeskTopSize size;
        private static readonly Regex _regexNumber = new Regex("[^0-9]+");
        public static Brush Brush = (Brush)Application.Current.TryFindResource("BackgroundSolidColorBrush");

        public static Brush WindowForegroundBrush =
            (Brush)Application.Current.TryFindResource("PrimaryTextSolidColorBrush");

        private static bool _IsCurrentDark;

        public static void OnSubThemeChanged()
        {
            if (!_IsCurrentDark)
            {
                var vBrush = (Brush)Application.Current.TryFindResource("PrimaryNormalSolidColorBrush");
                Application.Current.Resources["WindowBorderBrushSolidColorBrush"] = vBrush;
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
                    var vBrush = (Brush)Application.Current.TryFindResource("PrimaryNormalSolidColorBrush");
                    Application.Current.Resources["WindowBorderBrushSolidColorBrush"] = vBrush;
                    WindowForegroundBrush = (Brush)Application.Current.TryFindResource("PrimaryTextSolidColorBrush");
                    if (Application.Current.TryFindResource("DefaultBackgroundColor") is Color color)
                        Application.Current.Resources["DefaultBackgroundSolidColorBrush"] = new SolidColorBrush(color);
                }
                else
                {
                    if (Application.Current.TryFindResource("WindowBorderBrushColor") is Color color)
                    {
                        var colorBrush = new SolidColorBrush(color);
                        Application.Current.Resources["WindowBorderBrushSolidColorBrush"] = colorBrush;
                        Application.Current.Resources["DefaultBackgroundSolidColorBrush"] = colorBrush;
                    }

                    WindowForegroundBrush = (Brush)Application.Current.TryFindResource("DefaultBackgroundSolidColorBrush");
                }

                Brush = (Brush)Application.Current.TryFindResource("BackgroundSolidColorBrush");
                //WindowForegroundBrush = Application.Current.TryFindResource["PrimaryTextSolidColorBrush"] as Brush;
                _IsCurrentDark = isDark;
                ThemeRefresh();
            }
        }

        public static void ThemeRefresh()
        {
            //var themePath = "pack://application:,,,/WPFDevelopers;component/Themes/Theme.xaml";
            //var themeResourceDictionary =
            //    Application.Current.TryFindResource.MergedDictionaries.FirstOrDefault(x =>
            //        x.Source != null && x.Source.Equals(themePath));
            //if (themeResourceDictionary == null) return;
            //Application.Current.TryFindResource.MergedDictionaries.Remove(themeResourceDictionary);
            //Application.Current.TryFindResource.MergedDictionaries.Add(themeResourceDictionary);
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
            var hDC = Win32ApiHelper.GetDC(Win32ApiHelper.GetDesktopWindow());
            var hMemDC = Win32ApiHelper.CreateCompatibleDC(hDC);
            size.cx = Win32ApiHelper.GetSystemMetrics(0);
            size.cy = Win32ApiHelper.GetSystemMetrics(1);
            hBitmap = Win32ApiHelper.CreateCompatibleBitmap(hDC, size.cx, size.cy);
            if (hBitmap != IntPtr.Zero)
            {
                var hOld = Win32ApiHelper.SelectObject(hMemDC, hBitmap);
                Win32ApiHelper.BitBlt(hMemDC, 0, 0, size.cx, size.cy, hDC, 0, 0,
                    Win32ApiHelper.TernaryRasterOperations.SRCCOPY);
                Win32ApiHelper.SelectObject(hMemDC, hOld);
                Win32ApiHelper.DeleteDC(hMemDC);
                Win32ApiHelper.ReleaseDC(Win32ApiHelper.GetDesktopWindow(), hDC);
                var bsource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                Win32ApiHelper.DeleteObject(hBitmap);
                GC.Collect();
                return bsource;
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