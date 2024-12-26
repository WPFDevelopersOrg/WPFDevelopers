using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace WPFDevelopers
{
    public class Helper
    {
        public const int WM_USER = 0x03FC;
        public const int MY_MESSAGE = WM_USER + 1;
        public const int MY_MESSAGEFULLPATH = MY_MESSAGE + 1;
        public static string GetTempPath = Path.Combine(Path.GetTempPath(), Assembly.GetExecutingAssembly().GetName().Name);
        public static string GetTempPathVersion = Path.Combine(GetTempPath, GetMD5Hash(GetCurrentDllPath()));
        public const string GetExtName = "WPFDevelopersExt.exe";
        public static string GetTempPathVersionExt = Path.Combine(GetTempPathVersion, GetExtName);

        private static long _tick = DateTime.Now.Ticks;
        public static Random GetRandom = new Random((int)(_tick & 0xffffffffL) | (int)(_tick >> 32));
        private static readonly Regex _regexNumber = new Regex("[^0-9]+");


        public static void DeleteFilesAndFolders(string targetDir)
        {
            if (!Directory.Exists(targetDir))
                return;
            var directory = new DirectoryInfo(targetDir);
            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
            {
                DeleteFilesAndFolders(subDirectory.FullName);
                subDirectory.Delete();
            }
            foreach (FileInfo file in directory.GetFiles())
            {
                file.Delete();
            }
        }

        static string GetCurrentDllPath()
        {
            return Assembly.GetExecutingAssembly().Location;
        }

        static string GetMD5Hash(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var hashBytes = md5.ComputeHash(stream);
                    var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                    return hashString;
                }
            }
        }

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

        public static Uri ImageUri()
        {
            Uri uri = null;
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "图像文件(*.jpg;*.jpeg;*.png;)|*.jpg;*.jpeg;*.png;";
            if (openFileDialog.ShowDialog() == true) uri = new Uri(openFileDialog.FileName);
            return uri;
        }

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

    }
}
