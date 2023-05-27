using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Win32;
#if NET40
using Microsoft.Windows.Shell;
using WPFDevelopers.Helpers;
#else
using System.Windows.Shell;
#endif


namespace WPFDevelopers.Controls
{
    internal enum AccentState
    {
        ACCENT_DISABLED = 0,
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
        ACCENT_INVALID_STATE = 5
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public uint AccentFlags;
        public uint GradientColor;
        public uint AnimationId;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    internal enum WindowCompositionAttribute
    {
        // ...
        WCA_ACCENT_POLICY = 19
        // ...
    }

    internal class WindowOldConfig
    {
        public bool AllowsTransparency;
        public Brush Background;
        public WindowChrome WindowChrome;

        public WindowStyle WindowStyle = WindowStyle.SingleBorderWindow;
    }


    internal class WindowOSHelper
    {
        public static Version GetWindowOSVersion()
        {
            var regKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion");

            int major;
            int minor;
            int build;
            int revision;
            try
            {
                var str = regKey.GetValue("CurrentMajorVersionNumber")?.ToString();
                int.TryParse(str, out major);

                str = regKey.GetValue("CurrentMinorVersionNumber")?.ToString();
                int.TryParse(str, out minor);

                str = regKey.GetValue("CurrentBuildNumber")?.ToString();
                int.TryParse(str, out build);

                str = regKey.GetValue("BaseBuildRevisionNumber")?.ToString();
                int.TryParse(str, out revision);

                return new Version(major, minor, build, revision);
            }
            catch (Exception)
            {
                return new Version(0, 0, 0, 0);
            }
            finally
            {
                regKey.Close();
            }
        }
    }


    public class WindowAcrylicBlur : Freezable
    {
        private static readonly Color _BackgtoundColor = Color.FromArgb(0x01, 0, 0, 0); //设置透明色 防止穿透

        [DllImport(Win32.User32)]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        private static bool EnableAcrylicBlur(Window window, Color color, double opacity, bool enable)
        {
            if (window == null)
                return false;

            AccentState accentState;
            var vOsVersion = WindowOSHelper.GetWindowOSVersion();
            if (vOsVersion > new Version(10, 0, 17763)) //1809
                accentState = enable ? AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND : AccentState.ACCENT_DISABLED;
            else if (vOsVersion > new Version(10, 0))
                accentState = enable ? AccentState.ACCENT_ENABLE_BLURBEHIND : AccentState.ACCENT_DISABLED;
            else
                accentState = AccentState.ACCENT_DISABLED;

            if (opacity > 1)
                opacity = 1;

            var windowHelper = new WindowInteropHelper(window);

            var accent = new AccentPolicy();

            var opacityIn = (uint) (255 * opacity);

            accent.AccentState = accentState;

            if (enable)
            {
                var blurColor = (uint) ((color.R << 0) | (color.G << 8) | (color.B << 16) | (color.A << 24));
                var blurColorIn = blurColor;
                if (opacityIn > 0)
                    blurColorIn = (opacityIn << 24) | (blurColor & 0xFFFFFF);
                else if (opacityIn == 0 && color.A == 0)
                    blurColorIn = (0x01 << 24) | (blurColor & 0xFFFFFF);

                if (accent.GradientColor == blurColorIn)
                    return true;

                accent.GradientColor = blurColorIn;
            }

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);

            return true;
        }

        private static void Window_Initialized(object sender, EventArgs e)
        {
            if (!(sender is Window window))
                return;

            var config = new WindowOldConfig
            {
                WindowStyle = window.WindowStyle,
                AllowsTransparency = window.AllowsTransparency,
                Background = window.Background
            };

            var vWindowChrome = WindowChrome.GetWindowChrome(window);

            if (vWindowChrome == null)
            {
                window.WindowStyle = WindowStyle.None; //一定要将窗口的背景色改为透明才行
                window.AllowsTransparency = true; //一定要将窗口的背景色改为透明才行
                window.Background = new SolidColorBrush(_BackgtoundColor); //一定要将窗口的背景色改为透明才行
            }
            else
            {
                config.WindowChrome = new WindowChrome
                {
                    GlassFrameThickness = vWindowChrome.GlassFrameThickness
                };
                window.Background = Brushes.Transparent; //一定要将窗口的背景色改为透明才行
                var vGlassFrameThickness = vWindowChrome.GlassFrameThickness;
                vWindowChrome.GlassFrameThickness = new Thickness(0, vGlassFrameThickness.Top, 0, 0);
            }

            SetWindowOldConfig(window, config);

            window.Initialized -= Window_Initialized;
        }

        private static void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is Window window))
                return;

            var vBlur = GetWindowAcrylicBlur(window);
            if (vBlur != null)
                EnableAcrylicBlur(window, vBlur.BlurColor, vBlur.Opacity, true);

            window.Loaded -= Window_Loaded;
        }


        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }

        protected override void OnChanged()
        {
            base.OnChanged();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        #region 开启Win11风格

        public static WindowAcrylicBlur GetWindowAcrylicBlur(DependencyObject obj)
        {
            return (WindowAcrylicBlur) obj.GetValue(WindowAcrylicBlurProperty);
        }

        public static void SetWindowAcrylicBlur(DependencyObject obj, WindowAcrylicBlur value)
        {
            obj.SetValue(WindowAcrylicBlurProperty, value);
        }

        public static readonly DependencyProperty WindowAcrylicBlurProperty =
            DependencyProperty.RegisterAttached("WindowAcrylicBlur", typeof(WindowAcrylicBlur),
                typeof(WindowAcrylicBlur),
                new PropertyMetadata(default(WindowAcrylicBlur), OnWindowAcryBlurPropertyChangedCallBack));

        private static void OnWindowAcryBlurPropertyChangedCallBack(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Window window))
                return;

            if (e.OldValue == null && e.NewValue == null)
                return;

            if (e.OldValue == null && e.NewValue != null)
            {
                window.Initialized += Window_Initialized;
                window.Loaded += Window_Loaded;
            }

            if (e.OldValue != null && e.NewValue == null)
            {
                var vConfig = GetWindowOldConfig(d);
                if (vConfig != null)
                {
                    window.WindowStyle = vConfig.WindowStyle;
                    window.AllowsTransparency = vConfig.AllowsTransparency;
                    window.Background = vConfig.Background;

                    if (vConfig.WindowChrome != null)
                    {
                        var vWindowChrome = WindowChrome.GetWindowChrome(window);
                        if (vWindowChrome != null)
                            vWindowChrome.GlassFrameThickness = vConfig.WindowChrome.GlassFrameThickness;
                    }
                }
            }

            if (e.OldValue == e.NewValue)
            {
                if (!window.IsLoaded)
                    return;

                var vBlur = e.NewValue as WindowAcrylicBlur;
                if (vBlur == null)
                    return;

                EnableAcrylicBlur(window, vBlur.BlurColor, vBlur.Opacity, true);
            }
        }

        #endregion


        #region 内部设置

        private static WindowOldConfig GetWindowOldConfig(DependencyObject obj)
        {
            return (WindowOldConfig) obj.GetValue(WindowOldConfigProperty);
        }

        private static void SetWindowOldConfig(DependencyObject obj, WindowOldConfig value)
        {
            obj.SetValue(WindowOldConfigProperty, value);
        }

        private static readonly DependencyProperty WindowOldConfigProperty =
            DependencyProperty.RegisterAttached("WindowOldConfig", typeof(WindowOldConfig), typeof(WindowAcrylicBlur),
                new PropertyMetadata(default(WindowOldConfig)));

        #endregion

        #region

        public Color BlurColor
        {
            get => (Color) GetValue(BlurColorProperty);
            set => SetValue(BlurColorProperty, value);
        }

        public static readonly DependencyProperty BlurColorProperty =
            DependencyProperty.Register("BlurColor", typeof(Color), typeof(WindowAcrylicBlur),
                new PropertyMetadata(default(Color)));

        public double Opacity
        {
            get => (double) GetValue(OpacityProperty);
            set => SetValue(OpacityProperty, value);
        }

        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register("Opacity", typeof(double), typeof(WindowAcrylicBlur),
                new PropertyMetadata(default(double)));

        #endregion
    }
}