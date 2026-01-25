using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Linq;
using WPFDevelopers.Controls.Runtimes;
using WPFDevelopers.Controls.Runtimes.Interop;
using WPFDevelopers.Controls.Runtimes.Shell32;
using WPFDevelopers.Controls.Runtimes.User32;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public class NotifyIcon : FrameworkElement, IDisposable
    {
        private static NotifyIcon NotifyIconCache;

        public static readonly DependencyProperty ContextContentProperty = DependencyProperty.Register(
            "ContextContent", typeof(object), typeof(NotifyIcon), new PropertyMetadata(default));

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(NotifyIcon),
                new PropertyMetadata(default, OnIconPropertyChanged));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(NotifyIcon),
                new PropertyMetadata(default, OnTitlePropertyChanged));

        public static readonly RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(NotifyIcon));

        public static readonly RoutedEvent MouseDoubleClickEvent =
            EventManager.RegisterRoutedEvent("MouseDoubleClick", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(NotifyIcon));
        /// <summary>
        /// 托盘图标闪烁间隔
        /// </summary>
        public static readonly DependencyProperty TwinkIntervalProperty =
            DependencyProperty.Register("TwinkInterval",
                typeof(TimeSpan), typeof(NotifyIcon), new PropertyMetadata(TimeSpan.FromMilliseconds(500), OnTwinkIntervalChanged));
        /// <summary>
        /// 托盘图标是否开启闪烁
        /// </summary>
        public static readonly DependencyProperty IsTwinkProperty =
            DependencyProperty.Register("IsTwink",
                typeof(bool), typeof(NotifyIcon), new PropertyMetadata(false, OnIsTwinkChanged));

        private static bool s_Loaded = false;

        private static NotifyIcon s_NotifyIcon;

        //这是窗口名称
        private readonly string _TrayWndClassName;

        //这个是窗口消息名称
        private readonly string _TrayWndMessage;

        //这个是窗口消息回调（窗口消息都需要在此捕获）
        private readonly WndProc _TrayWndProc;
        private Popup _contextContent;

        private bool _doubleClick;

        //图标句柄
        private IntPtr _hIcon = IntPtr.Zero;
        private ImageSource _icon;
        private IntPtr _iconHandle;

        private int _IsShowIn;

        //托盘对象
        private NOTIFYICONDATA _NOTIFYICONDATA;

        //这个是传递给托盘的鼠标消息id
        private int _TrayMouseMessage;

        //窗口句柄
        private IntPtr _TrayWindowHandle = IntPtr.Zero;

        //通过注册窗口消息可以获取唯一标识Id
        private int _WmTrayWindowMessage;

        private bool disposedValue;

        private IntPtr _tempIconHandle;
        //闪烁定时器
        private DispatcherTimer _dispatcherTimerTwink;

        public NotifyIcon()
        {
            _TrayWndClassName = $"WPFDevelopers_{Guid.NewGuid()}";
            _TrayWndProc = WndProc_CallBack;
            _TrayWndMessage = "TrayWndMessageName";
            _TrayMouseMessage = (int)WM.USER + 1024;
            Start();
            if (Application.Current != null)
            {
                WPFDevelopers.Resources.ThemeChanged += Resources_ThemeChanged;
                Application.Current.Exit += (s, e) => Dispose();
            }
            NotifyIconCache = this;
        }
        static NotifyIcon()
        {
            DataContextProperty.OverrideMetadata(typeof(NotifyIcon), new FrameworkPropertyMetadata(DataContextPropertyChanged));
            ContextMenuProperty.OverrideMetadata(typeof(NotifyIcon), new FrameworkPropertyMetadata(ContextMenuPropertyChanged));
        }

        private void Resources_ThemeChanged(ThemeType currentTheme)
        {
#if NET40
            UpdateDefaultStyle();
#else
            ContextMenu.UpdateDefaultStyle();
#endif
        }

        void UpdateDefaultStyle()
        {
            var currentContextMenu = ContextMenu;
            var newContextMenu = new ContextMenu();
            var itemsToMove = currentContextMenu.Items.OfType<MenuItem>().ToList();
            currentContextMenu.Items.Clear();
            itemsToMove.ForEach(item => newContextMenu.Items.Add(item));
            ContextMenu = newContextMenu;
        }

        private static void DataContextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            ((NotifyIcon)d).OnDataContextPropertyChanged(e);

        private void OnDataContextPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateDataContext(_contextContent, e.OldValue, e.NewValue);
            UpdateDataContext(ContextMenu, e.OldValue, e.NewValue);
        }

        private void UpdateDataContext(FrameworkElement target, object oldValue, object newValue)
        {
            if (target == null || BindingOperations.GetBindingExpression(target, DataContextProperty) != null) return;
            if (ReferenceEquals(this, target.DataContext) || Equals(oldValue, target.DataContext))
            {
                target.DataContext = newValue ?? this;
            }
        }

        private static void ContextMenuPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (NotifyIcon)d;
            ctl.OnContextMenuPropertyChanged(e);
        }

        private void OnContextMenuPropertyChanged(DependencyPropertyChangedEventArgs e) =>
            UpdateDataContext((ContextMenu)e.NewValue, null, DataContext);
        public object ContextContent
        {
            get => GetValue(ContextContentProperty);
            set => SetValue(ContextContentProperty, value);
        }

        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }


        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        /// <summary>
        /// 托盘图标闪烁间隔
        /// </summary>
        public TimeSpan TwinkInterval
        {
            get => (TimeSpan)GetValue(TwinkIntervalProperty);
            set => SetValue(TwinkIntervalProperty, value);
        }
        /// <summary>
        /// 托盘图标是否开启闪烁
        /// </summary>
        public bool IsTwink
        {
            get => (bool)GetValue(IsTwinkProperty);
            set => SetValue(IsTwinkProperty, value);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private static void OnTwinkIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NotifyIcon trayService)
            {
                var notifyIcon = (NotifyIcon)d;
                notifyIcon._dispatcherTimerTwink.Interval = (TimeSpan)e.NewValue;
            }
        }
        private static void OnIsTwinkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NotifyIcon trayService)
            {
                var notifyIcon = (NotifyIcon)d;
                if (notifyIcon.Visibility != Visibility.Visible) return;
                if ((bool)e.NewValue)
                {
                    if (notifyIcon._dispatcherTimerTwink == null)
                    {
                        notifyIcon._dispatcherTimerTwink = new DispatcherTimer
                        {
                            Interval = notifyIcon.TwinkInterval
                        };
                        notifyIcon._dispatcherTimerTwink.Tick += notifyIcon.DispatcherTimerTwinkTick;
                    }
                    notifyIcon._tempIconHandle = notifyIcon._hIcon;
                    notifyIcon._dispatcherTimerTwink.Start();
                }
                else
                {
                    notifyIcon._dispatcherTimerTwink?.Stop();
                    notifyIcon._dispatcherTimerTwink.Tick -= notifyIcon.DispatcherTimerTwinkTick;
                    notifyIcon._dispatcherTimerTwink = null;
                    notifyIcon._iconHandle = notifyIcon._tempIconHandle;
                    notifyIcon.ChangeIcon(false, notifyIcon._iconHandle);
                    notifyIcon._tempIconHandle = IntPtr.Zero;
                }
            }

        }
        private static void OnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NotifyIcon trayService)
                trayService.ChangeTitle(e.NewValue?.ToString());
        }

        private static void OnIconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NotifyIcon trayService)
            {
                var notifyIcon = (NotifyIcon)d;
                notifyIcon._icon = (ImageSource)e.NewValue;
                trayService.ChangeIcon();
            }
        }

        public event RoutedEventHandler Click
        {
            add => AddHandler(ClickEvent, value);
            remove => RemoveHandler(ClickEvent, value);
        }

        public event RoutedEventHandler MouseDoubleClick
        {
            add => AddHandler(MouseDoubleClickEvent, value);
            remove => RemoveHandler(MouseDoubleClickEvent, value);
        }

        private void DispatcherTimerTwinkTick(object sender, EventArgs e)
        {
            if (Visibility != Visibility.Visible) return;
            _iconHandle = _iconHandle != IntPtr.Zero ? IntPtr.Zero : _tempIconHandle;
            ChangeIcon(false, _iconHandle);
        }
        private static void Current_Exit(object sender, ExitEventArgs e)
        {
            s_NotifyIcon?.Dispose();
            s_NotifyIcon = default;
        }


        public bool Start()
        {
            if (DesignerHelper.IsInDesignMode == true) return false;
            RegisterClass(_TrayWndClassName, _TrayWndProc, _TrayWndMessage);
            LoadNotifyIconData(string.Empty);
            Show();

            return true;
        }

        public bool Stop()
        {
            //销毁窗体
            if (_TrayWindowHandle != IntPtr.Zero)
                if (User32Interop.IsWindow(_TrayWindowHandle))
                    User32Interop.DestroyWindow(_TrayWindowHandle);

            //反注册窗口类
            if (!string.IsNullOrWhiteSpace(_TrayWndClassName))
                User32Interop.UnregisterClassName(_TrayWndClassName, Kernel32Interop.GetModuleHandle(default));

            //销毁Icon
            if (_hIcon != IntPtr.Zero)
                User32Interop.DestroyIcon(_hIcon);

            Hide();

            return true;
        }

        /// <summary>
        ///     注册并创建窗口对象
        /// </summary>
        /// <param name="className">窗口名称</param>
        /// <param name="messageName">窗口消息名称</param>
        /// <returns></returns>
        private bool RegisterClass(string className, WndProc wndproccallback, string messageName)
        {
            var wndClass = new WNDCLASSEX
            {
                cbSize = Marshal.SizeOf(typeof(WNDCLASSEX)),
                style = 0,
                lpfnWndProc = wndproccallback,
                cbClsExtra = 0,
                cbWndExtra = 0,
                hInstance = IntPtr.Zero,
                hCursor = IntPtr.Zero,
                hbrBackground = IntPtr.Zero,
                lpszMenuName = string.Empty,
                lpszClassName = className
            };

            //注册窗体对象
            User32Interop.RegisterClassEx(ref wndClass);
            //注册消息获取对应消息标识id
            _WmTrayWindowMessage = User32Interop.RegisterWindowMessage(messageName);
            //创建窗体（本质上托盘在创建时需要一个窗口句柄，完全可以将主窗体的句柄给进去，但是为了更好的管理消息以及托盘的生命周期，通常会创建一个独立不可见的窗口）
            _TrayWindowHandle = User32Interop.CreateWindowEx(0, className, "", 0, 0, 0, 1, 1, IntPtr.Zero, IntPtr.Zero,
                IntPtr.Zero, IntPtr.Zero);

            return true;
        }

        /// <summary>
        ///     创建托盘对象
        /// </summary>
        /// <param name="icon">图标路径，可以修改托盘图标（本质上是可以接受用户传入一个图片对象，然后将图片转成Icon，但是算了这个有点复杂）</param>
        /// <param name="title">托盘的tooltip</param>
        /// <returns></returns>
        private bool LoadNotifyIconData(string title)
        {
            lock (this)
            {
                _NOTIFYICONDATA = NOTIFYICONDATA.GetDefaultNotifyData(_TrayWindowHandle);

                if (_TrayMouseMessage != 0)
                    _NOTIFYICONDATA.uCallbackMessage = (uint)_TrayMouseMessage;
                else
                    _TrayMouseMessage = (int)_NOTIFYICONDATA.uCallbackMessage;

                if (_iconHandle == IntPtr.Zero)
                {
                    var processPath = Kernel32Interop.GetModuleFileName(new HandleRef());
                    if (!string.IsNullOrWhiteSpace(processPath))
                    {
                        var index = IntPtr.Zero;
                        var hIcon = Shell32Interop.ExtractAssociatedIcon(IntPtr.Zero, processPath, ref index);
                        _NOTIFYICONDATA.hIcon = hIcon;
                        _hIcon = hIcon;
                    }
                }

                if (!string.IsNullOrWhiteSpace(title))
                    _NOTIFYICONDATA.szTip = title;
            }

            return true;
        }

        private bool Show()
        {
            var command = NotifyCommand.NIM_Add;
            if (Thread.VolatileRead(ref _IsShowIn) == 1)
                command = NotifyCommand.NIM_Modify;
            else
                Thread.VolatileWrite(ref _IsShowIn, 1);

            lock (this)
            {
                return Shell32Interop.Shell_NotifyIcon(command, ref _NOTIFYICONDATA);
            }
        }

        internal static int AlignToBytes(double original, int nBytesCount)
        {
            var nBitsCount = 8 << (nBytesCount - 1);
            return ((int)Math.Ceiling(original) + (nBitsCount - 1)) / nBitsCount * nBitsCount;
        }

        private static byte[] GenerateMaskArray(int width, int height, byte[] colorArray)
        {
            var nCount = width * height;
            var bytesPerScanLine = AlignToBytes(width, 2) / 8;
            var bitsMask = new byte[bytesPerScanLine * height];

            for (var i = 0; i < nCount; i++)
            {
                var hPos = i % width;
                var vPos = i / width;
                var byteIndex = hPos / 8;
                var offsetBit = (byte)(0x80 >> (hPos % 8));

                if (colorArray[i * 4 + 3] == 0x00)
                    bitsMask[byteIndex + bytesPerScanLine * vPos] |= offsetBit;
                else
                    bitsMask[byteIndex + bytesPerScanLine * vPos] &= (byte)~offsetBit;

                if (hPos == width - 1 && width == 8) bitsMask[1 + bytesPerScanLine * vPos] = 0xff;
            }

            return bitsMask;
        }

        private byte[] BitmapImageToByteArray(BitmapImage bmp)
        {
            byte[] bytearray = null;
            try
            {
                var smarket = bmp.StreamSource;
                if (smarket != null && smarket.Length > 0)
                {
                    //设置当前位置
                    smarket.Position = 0;
                    using (var br = new BinaryReader(smarket))
                    {
                        bytearray = br.ReadBytes((int)smarket.Length);
                    }
                }
            }
            catch
            {
                throw;
            }

            return bytearray;
        }

        private byte[] ConvertBitmapSourceToBitmapImage(
            BitmapSource bitmapSource)
        {
            byte[] imgByte = default;
            if (!(bitmapSource is BitmapImage bitmapImage))
            {
                bitmapImage = new BitmapImage();

                var encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (var memoryStream = new MemoryStream())
                {
                    encoder.Save(memoryStream);
                    memoryStream.Position = 0;

                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.EndInit();
                    imgByte = BitmapImageToByteArray(bitmapImage);
                }
            }

            return imgByte;
        }

        internal static IconHandle CreateIconCursor(byte[] xor, int width, int height, int xHotspot,
            int yHotspot, bool isIcon)
        {
            var bits = IntPtr.Zero;

            BitmapHandle colorBitmap = null;
            var bi = new BITMAPINFO(width, -height, 32)
            {
                bmiHeader_biCompression = 0
            };

            colorBitmap = Gdi32Interop.CreateDIBSection(new HandleRef(null, IntPtr.Zero), ref bi, 0, ref bits, null, 0);

            if (colorBitmap.IsInvalid || bits == IntPtr.Zero) return IconHandle.GetInvalidIcon();
            Marshal.Copy(xor, 0, bits, xor.Length);
            var maskArray = GenerateMaskArray(width, height, xor);
            var maskBitmap = Gdi32Interop.CreateBitmap(width, height, 1, 1, maskArray);
            if (maskBitmap.IsInvalid) return IconHandle.GetInvalidIcon();
            var iconInfo = new Gdi32Interop.ICONINFO
            {
                fIcon = isIcon,
                xHotspot = xHotspot,
                yHotspot = yHotspot,
                hbmMask = maskBitmap,
                hbmColor = colorBitmap
            };

            return User32Interop.CreateIconIndirect(iconInfo);
        }

        private bool ChangeIcon(bool destroyOldIcon = false, IntPtr? newIntprt = null)
        {
            if (DesignerHelper.IsInDesignMode == true) return false;
            var bitmapFrame = _icon as BitmapFrame;
            if (bitmapFrame != null && bitmapFrame.Decoder != null)
                if (bitmapFrame.Decoder is IconBitmapDecoder)
                {
                    var w = bitmapFrame.PixelWidth;
                    var h = bitmapFrame.PixelHeight;
                    var bpp = bitmapFrame.Format.BitsPerPixel;
                    var stride = (bpp * w + 31) / 32 * 4;
                    var sizeCopyPixels = stride * h;
                    var xor = new byte[sizeCopyPixels];
                    bitmapFrame.CopyPixels(xor, stride, 0);

                    var iconHandle = CreateIconCursor(xor, w, h, 0, 0, true);
                    _iconHandle = iconHandle.CriticalGetHandle();
                }

            if (Thread.VolatileRead(ref _IsShowIn) != 1)
                return false;

            if (destroyOldIcon && _hIcon != IntPtr.Zero)
            {
                User32Interop.DestroyIcon(_hIcon);
                _hIcon = IntPtr.Zero;
            }
            lock (this)
            {
                if (_iconHandle != IntPtr.Zero)
                {
                    var hIcon = _iconHandle;
                    _NOTIFYICONDATA.hIcon = hIcon;
                    _hIcon = hIcon;
                }
                else
                {
                    _NOTIFYICONDATA.hIcon = IntPtr.Zero;
                }
                return Shell32Interop.Shell_NotifyIcon(NotifyCommand.NIM_Modify, ref _NOTIFYICONDATA);
            }
        }

        private bool ChangeTitle(string title)
        {
            if (Thread.VolatileRead(ref _IsShowIn) != 1)
                return false;

            lock (this)
            {
                _NOTIFYICONDATA.szTip = title;
                return Shell32Interop.Shell_NotifyIcon(NotifyCommand.NIM_Modify, ref _NOTIFYICONDATA);
            }
        }

        public static void ShowBalloonTip(string title, string content, NotifyIconInfoType infoType)
        {
            if (NotifyIconCache != null)
                NotifyIconCache.ShowBalloonTips(title, content, infoType);
        }

        public void ShowBalloonTips(string title, string content, NotifyIconInfoType infoType)
        {
            if (Thread.VolatileRead(ref _IsShowIn) != 1)
                return;
            var _ShowNOTIFYICONDATA = NOTIFYICONDATA.GetDefaultNotifyData(_TrayWindowHandle);
            _ShowNOTIFYICONDATA.uFlags = NIFFlags.NIF_INFO;
            _ShowNOTIFYICONDATA.szInfoTitle = title ?? string.Empty;
            _ShowNOTIFYICONDATA.szInfo = content ?? string.Empty;

            switch (infoType)
            {
                case NotifyIconInfoType.Info:
                    _ShowNOTIFYICONDATA.dwInfoFlags = NIIFFlags.NIIF_INFO;
                    break;
                case NotifyIconInfoType.Warning:
                    _ShowNOTIFYICONDATA.dwInfoFlags = NIIFFlags.NIIF_WARNING;
                    break;
                case NotifyIconInfoType.Error:
                    _ShowNOTIFYICONDATA.dwInfoFlags = NIIFFlags.NIIF_ERROR;
                    break;
                case NotifyIconInfoType.None:
                    _ShowNOTIFYICONDATA.dwInfoFlags = NIIFFlags.NIIF_NONE;
                    break;
            }

            Shell32Interop.Shell_NotifyIcon(NotifyCommand.NIM_Modify, ref _ShowNOTIFYICONDATA);
        }

        private bool Hide()
        {
            var isShow = Thread.VolatileRead(ref _IsShowIn);
            if (isShow != 1)
                return true;

            Thread.VolatileWrite(ref _IsShowIn, 0);

            lock (this)
            {
                return Shell32Interop.Shell_NotifyIcon(NotifyCommand.NIM_Delete, ref _NOTIFYICONDATA);
            }
        }

        private IntPtr WndProc_CallBack(IntPtr hwnd, WM msg, IntPtr wParam, IntPtr lParam)
        {
            //这是窗口相关的消息
            if ((int)msg == _WmTrayWindowMessage)
            {
            }
            else if ((int)msg == _TrayMouseMessage) //这是托盘上鼠标相关的消息
            {
                switch ((WM)(long)lParam)
                {
                    case WM.LBUTTONDOWN:
                        break;
                    case WM.LBUTTONUP:
                        WMMouseUp(MouseButton.Left);
                        break;
                    case WM.LBUTTONDBLCLK:
                        WMMouseDown(MouseButton.Left, 2);
                        break;
                    case WM.RBUTTONDOWN:
                        break;
                    case WM.RBUTTONUP:
                        OpenMenu();
                        break;
                    case WM.MOUSEMOVE:
                        break;
                    case WM.MOUSEWHEEL:
                        break;
                }
            }
            else if (msg == WM.COMMAND)
            {
            }


            return User32Interop.DefWindowProc(hwnd, msg, wParam, lParam);
        }

        private void WMMouseUp(MouseButton button)
        {
            if (!_doubleClick && button == MouseButton.Left)
                RaiseEvent(new MouseButtonEventArgs(
                    Mouse.PrimaryDevice,
                    Environment.TickCount, button)
                {
                    RoutedEvent = ClickEvent
                });
            _doubleClick = false;
        }

        private void WMMouseDown(MouseButton button, int clicks)
        {
            if (clicks == 2)
            {
                RaiseEvent(new MouseButtonEventArgs(
                    Mouse.PrimaryDevice,
                    Environment.TickCount, button)
                {
                    RoutedEvent = MouseDoubleClickEvent
                });
                _doubleClick = true;
            }
        }

        private void OpenMenu()
        {
            if (ContextContent != null)
            {
                _contextContent = new Popup
                {
                    Placement = PlacementMode.Mouse,
                    AllowsTransparency = true,
                    StaysOpen = false,
                    UseLayoutRounding = true,
                    SnapsToDevicePixels = true
                };

                _contextContent.Child = new ContentControl
                {
                    Content = ContextContent
                };
                UpdateDataContext(_contextContent, null, DataContext);
                _contextContent.IsOpen = true;
                User32Interop.SetForegroundWindow(_contextContent.Child.GetHandle());
            }
            else if (ContextMenu != null)
            {
                if (ContextMenu.Items.Count == 0) return;

                ContextMenu.InvalidateProperty(StyleProperty);
                foreach (var item in ContextMenu.Items)
                    if (item is MenuItem menuItem)
                    {
                        menuItem.InvalidateProperty(StyleProperty);
                    }
                    else
                    {
                        var container = ContextMenu.ItemContainerGenerator.ContainerFromItem(item) as MenuItem;
                        container?.InvalidateProperty(StyleProperty);
                    }
                ContextMenu.Placement = PlacementMode.Mouse;
                ContextMenu.IsOpen = true;

                User32Interop.SetForegroundWindow(ContextMenu.GetHandle());
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    Stop();

                disposedValue = true;
            }
        }
    }

    public enum NotifyIconInfoType
    {
        /// <summary>
        ///     No Icon.
        /// </summary>
        None,

        /// <summary>
        ///     A Information Icon.
        /// </summary>
        Info,

        /// <summary>
        ///     A Warning Icon.
        /// </summary>
        Warning,

        /// <summary>
        ///     A Error Icon.
        /// </summary>
        Error
    }
}