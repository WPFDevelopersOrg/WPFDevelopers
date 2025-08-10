using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Win32;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Core.Helpers
{
    public class OSVersionHelper
    {
        private const double LogicalDpi = 96.0;

        private static bool? _isSnapLayoutSupported;


        static OSVersionHelper()
        {
            var dC = Win32.GetDC(IntPtr.Zero);
            if (dC != IntPtr.Zero)
            {
                const int logicPixelsX = 88;
                const int logicPixelsY = 90;
                DeviceDpiX = Win32.GetDeviceCaps(dC, logicPixelsX);
                DeviceDpiY = Win32.GetDeviceCaps(dC, logicPixelsY);
                Win32.ReleaseDC(IntPtr.Zero, dC);
            }
            else
            {
                DeviceDpiX = LogicalDpi;
                DeviceDpiY = LogicalDpi;
            }

            var identity = Matrix.Identity;
            var identity2 = Matrix.Identity;
            identity.Scale(DeviceDpiX / LogicalDpi, DeviceDpiY / LogicalDpi);
            identity2.Scale(LogicalDpi / DeviceDpiX, LogicalDpi / DeviceDpiY);
            TransformFromDevice = new MatrixTransform(identity2);
            TransformFromDevice.Freeze();
            TransformToDevice = new MatrixTransform(identity);
            TransformToDevice.Freeze();
        }

        public static MatrixTransform TransformFromDevice { get; }
        public static MatrixTransform TransformToDevice { get; }

        public static double DeviceDpiX { get; }

        public static double DeviceDpiY { get; }
        public static double DeviceUnitsScalingFactorX => TransformToDevice.Matrix.M11;

        public static double DeviceUnitsScalingFactorY => TransformToDevice.Matrix.M22;

        public static bool IsSnapLayoutSupported()
        {
            if (_isSnapLayoutSupported.HasValue) return _isSnapLayoutSupported.Value;

            _isSnapLayoutSupported = CheckSnapLayoutSupport();
            return _isSnapLayoutSupported.Value;
        }

        private static bool CheckSnapLayoutSupport()
        {
            var version = GetWindowsBuildNumber();
            return version >= 22000;
        }

        private static int GetWindowsBuildNumber()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
                {
                    if (key != null)
                    {
                        var buildNumber = key.GetValue("CurrentBuildNumber");
                        if (buildNumber != null && int.TryParse(buildNumber.ToString(), out var result)) return result;
                    }
                }
            }
            catch
            {
            }

            return 0;
        }

        #region SnapLayout

        private const double DPI_SCALE = 1.5;
        public const int HTMAXBUTTON = 9;

        private static HwndSource GetWindowHwndSource(DependencyObject dependencyObject)
        {
            if (dependencyObject is Window window) return PresentationSource.FromVisual(window) as HwndSource;

            if (dependencyObject is ToolTip tooltip) return PresentationSource.FromVisual(tooltip) as HwndSource;

            if (dependencyObject is Popup popup)
            {
                if (popup.Child is null)
                    return null;

                return PresentationSource.FromVisual(popup.Child) as HwndSource;
            }

            if (dependencyObject is Visual visual)
            {
                return PresentationSource.FromVisual(visual) as HwndSource;
            }

            return null;
        }

        #endregion
    }
}