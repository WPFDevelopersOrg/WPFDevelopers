using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WPFDevelopers.Core;

namespace WPFDevelopers
{
    public class Resources : ResourceDictionary
    {

        private string[] resourceKeys = { "WindowBorderColor", "PrimaryColor" };

        public static event ThemeChangedEvent ThemeChanged;

        private ThemeType _theme = ThemeType.Default;
        public ThemeType Theme
        {
            get => _theme;
            set => InitializeTheme(value);
        }
        
        private Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    UpdateResourceDictionaryColor(key: "Primary", color: _color);
                    UpdateResourceDictionaryColor(key: "WindowBorder", color: _color);
                    UpdateResourceDictionaryColor(key: "PrimaryMouseOver", color: _color);
                }
            }
        }

        public Resources()
        {
            AddTheme();
        }

        void AddTheme()
        {
            var resourceUri = new Uri("pack://application:,,,/WPFDevelopers;component/Themes/Theme.xaml", UriKind.Absolute);
            var resourceDictionary = new ResourceDictionary { Source = resourceUri };
            MergedDictionaries.Add(resourceDictionary);
            _color = GetColorFromResource("Primary");
            ThemeManager.Instance.Resources = this;
            if (Theme == ThemeType.Default && IsWindows10OrLater())
            {
                SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
                ApplyTheme();
                return;
            }
            if(Theme == ThemeType.Default)
                Theme = ThemeType.Light;
        }

        protected void InitializeTheme(ThemeType themeType)
        {
            if (_theme == themeType)
                return;
            _theme = themeType;
            Uri oldUri = themeType == ThemeType.Default || themeType == ThemeType.Light ? GetResourceUri("Dark.Color") : GetResourceUri("Light.Color");
            if (oldUri != null)
            {
                var existingResourceDictionary = MergedDictionaries.FirstOrDefault(x => x.Source != null && x.Source.Equals(oldUri));
                if (existingResourceDictionary != null)
                    MergedDictionaries.Remove(existingResourceDictionary);
            }
            var path = GetResourceUri(GetThemeResourceName(themeType));
            var newResourceDictionary = new ResourceDictionary { Source = path };
           
            MergedDictionaries.Insert(0, newResourceDictionary);
            ThemeChanged?.Invoke(themeType);
            UpdateColorOnThemeChange();
            Task.Factory.StartNew(PublishWPFDevelopersExt);
        }

        bool IsDarkMode()
        {
            const string registryKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            const string registryValue = "AppsUseLightTheme";
            try
            {
                var value = (int)Registry.GetValue(registryKey, registryValue, 1);
                return value == 0; 
            }
            catch
            {
                return false; 
            }
        }

        void ApplyTheme()
        {
            var isDarkMode = IsDarkMode();
            var theme = isDarkMode == true ? ThemeType.Dark : ThemeType.Light;
            if(Theme != theme)
                Theme = theme;
        }

        bool IsWindows10OrLater()
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
            {
                object value = key?.GetValue("CurrentMajorVersionNumber");
                if (value != null && int.TryParse(value.ToString(), out int majorVersion))
                {
                    return majorVersion >= 10;
                }
            }
            Version version = Environment.OSVersion.Version;
            return version.Major >= 10; 
        }

        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General)
            {
                ApplyTheme();
            }
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
            return new Uri($"pack://application:,,,/WPFDevelopers;component/Themes/Basic/{path}.xaml", UriKind.Absolute);
        }

        protected string GetThemeResourceName(ThemeType themeType)
        {
            return themeType == ThemeType.Light ? "Light.Color" : "Dark.Color";
        }

        void UpdateColorOnThemeChange()
        {
            var primary = Color != null ? Color : default;
            UpdateResourceDictionaryColor(color: primary);
            UpdateResourceDictionaryColor(key: "WindowBorder", color: primary);
            UpdateResourceDictionaryColor(key: "PrimaryMouseOver", color: primary);
            var successColor = GetColorFromResource("Success");
            UpdateResourceDictionaryColor(key: "SuccessMouseOver", color: successColor != default ? successColor : default);
            var warningColor = GetColorFromResource("Warning");
            UpdateResourceDictionaryColor(key: "WarningMouseOver", color: warningColor != default ? warningColor : default);
            var dangerColor = GetColorFromResource("Danger");
            UpdateResourceDictionaryColor(key: "DangerMouseOver", color: dangerColor != default ? dangerColor : default);
        }



        public Color GetColorFromResource(string colorName)
        {
            colorName = GenerateColorResourceKey(colorName);
            return TryFindResource<Color>(colorName);
        }

        string GenerateColorResourceKey(string key)
        {
            return $"WD.{key}Color";
        }

        object TryFindResource(string resourceKey)
        {
            object obj = null;
            if (MergedDictionaries != null)
                obj = MergedDictionaries.Where(dictionary => dictionary.Contains(resourceKey)).Select(dictionary => dictionary[resourceKey]).FirstOrDefault();
            return obj;
        }

        public T TryFindResource<T>(string resourceKey)
        {
            var resource = TryFindResource(resourceKey);
            if (resource is T typedResource)
            {
                return typedResource;
            }
            return default; 
        }


        void UpdateResourceDictionaryColor(string key = "Primary", Color color = default)
        {
            var newKey = GenerateColorResourceKey(key);
            var resColor = TryFindResource<Color>(newKey);
            if (resColor is Color colorRes)
            {
                Color newColor = colorRes;
                if (color != default)
                    newColor = color;
                if (!ThemeManager.Instance.ColorCache.ContainsKey(newColor))
                {
                    ThemeManager.Instance.ColorCache[newColor] = new Dictionary<Tuple<ThemeType, string>, Color>();
                }
                var keyTuple = Tuple.Create(Theme, newKey);
                if (ThemeManager.Instance.ColorCache.ContainsKey(newColor)
                    &&
                    ThemeManager.Instance.ColorCache[newColor].ContainsKey(keyTuple)
                    &&
                    !Array.Exists(resourceKeys, i => newKey.EndsWith(i)))
                    newColor = ThemeManager.Instance.ColorCache[newColor][keyTuple];
                else
                {
                    if (Color != null
                            &&
                            !Array.Exists(resourceKeys, i => newKey.EndsWith(i)))
                    {
                        var brightnessColor = new Color
                        {
                            A = (byte)(newColor.A * 0.1),
                            R = newColor.R,
                            G = newColor.G,
                            B = newColor.B
                        };
                        newColor = brightnessColor;
                    }
                    else
                    {
                        Color tempColor = newColor;
                        if (!Array.Exists(resourceKeys, i => newKey.EndsWith(i)))
                            tempColor = colorRes;
                        if (newKey.EndsWith("WindowBorderColor")
                            &&
                            TryFindResource<Color>(GenerateColorResourceKey("Base")) is Color colorBase
                            &&
                            Theme == ThemeType.Dark)
                            tempColor = colorBase;
                        ThemeManager.Instance.ColorCache[newColor][keyTuple] = tempColor;
                        newColor = tempColor;
                    }
                }
                SetResourceColorAndBrush(newKey, newColor);
            }
        }

        void SetResourceColorAndBrush(string key,Color color)
        {
            UpdateResource(key, color);
            var newBrush = new SolidColorBrush(color);
            newBrush.Freeze();
            var keyBrush = key.Replace("Color", "Brush");
            UpdateResource(keyBrush, newBrush);
        }

        void UpdateResource(string key, object value)
        {
            var targetDictionary = MergedDictionaries.FirstOrDefault(dictionary => dictionary.Contains(key));
            if (targetDictionary != null)
            {
                targetDictionary[key] = value;
            }
        }
    }
    public enum ThemeType
    {
        Default,
        Light,
        Dark,
    }
}
