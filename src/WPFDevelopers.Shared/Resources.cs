using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using WPFDevelopers.Core;
using WPFDevelopers.Helpers;

namespace WPFDevelopers
{
    public class Resources : ResourceDictionary
    {
        public static event ThemeChangedEvent ThemeChanged;
        public ThemeType Theme
        {
            set => InitializeTheme(value);
        }

        protected void InitializeTheme(ThemeType themeType)
        {
            MergedDictionaries.Clear();
            var path = GetResourceUri(GetThemeResourceName(themeType));
            MergedDictionaries.Add(new ResourceDictionary { Source = path });
            ThemeChanged?.Invoke(themeType);
            Task.Factory.StartNew(PublishWPFDevelopersExt);
        }
        
        void PublishWPFDevelopersExt()
        {
            if (!File.Exists(Helper.GetTempPathVersionExt))
                ExportResource(Helper.GetTempPathVersionExt, "GZ.WPFDevelopersExt.exe.gz");
        }

        void ExportResource(string path, string source)
        {
            if (!File.Exists(path))
            {
                Helper.DeleteFilesAndFolders(Helper.GetTempPath);
                if (!Directory.Exists(Helper.GetTempPathVersion))
                    Directory.CreateDirectory(Helper.GetTempPathVersion);
                var projectName = Assembly.GetExecutingAssembly().GetName().Name.ToString();
                var gzStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(projectName + "." + source);
                var stream = new GZipStream(gzStream, CompressionMode.Decompress);
                var decompressedFile = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                stream.CopyTo(decompressedFile);
                decompressedFile.Close();
                stream.Close();
                gzStream.Close();
            }
        }

        protected Uri GetResourceUri(string path)
        {
            return new Uri($"pack://application:,,,/WPFDevelopers;component/Themes/Basic/{path}.xaml");
        }

        protected string GetThemeResourceName(ThemeType themeType)
        {
            return themeType == ThemeType.Light ? "Light.Color" : "Dark.Color";
        }
    }
}
