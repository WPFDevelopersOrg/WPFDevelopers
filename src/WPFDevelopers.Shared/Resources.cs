using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WPFDevelopers.Core;
using WPFDevelopers.Helpers;

namespace WPFDevelopers
{
    public class Resources : ResourceDictionary
    {

        private string[] resourceKeys = { "WindowBorderColor", "PrimaryColor" };

        public static event ThemeChangedEvent ThemeChanged;

        private ThemeType _theme;
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
                    OnPropertyChanged(nameof(Color));
                    UpdateResourceDictionaryColor(key: "Primary", color: _color);
                    UpdateResourceDictionaryColor(key: "WindowBorder", color: _color);
                    UpdateResourceDictionaryColor(key: "PrimaryMouseOver", color: _color);
                }
            }
        }

        public Resources()
        {
            var resourceUri = new Uri("pack://application:,,,/WPFDevelopers;component/Themes/Theme.xaml");
            var resourceDictionary = new ResourceDictionary { Source = resourceUri };
            if (Application.Current?.Resources != null 
                &&
                !Application.Current.Resources.MergedDictionaries.Any(dictionary => dictionary.Source != null && dictionary.Source == resourceUri))
            {
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }
            _color = GetColorFromResource("Primary");
        }

        protected void InitializeTheme(ThemeType themeType)
        {
            _theme = themeType;
            MergedDictionaries.Clear();
            var path = GetResourceUri(GetThemeResourceName(themeType));
            MergedDictionaries.Add(new ResourceDictionary { Source = path });
            ThemeChanged?.Invoke(themeType);
            UpdateColorOnThemeChange();
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

        

        Color GetColorFromResource(string colorName)
        {
            Color color = default;
            if (Application.Current?.TryFindResource(GenerateColorResourceKey(colorName)) is Color colorResource)
                color = colorResource;
            return color;
        }

        string GenerateColorResourceKey(string key)
        {
            return $"WD.{key}Color";
        }

        void UpdateResourceDictionaryColor(string key = "Primary", Color color = default)
        {
            var newKey = GenerateColorResourceKey(key);
            if (Application.Current?.TryFindResource(newKey) is Color colorRes)
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
                            Application.Current?.TryFindResource(GenerateColorResourceKey("Base")) is Color colorBase
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
            Application.Current.Resources[key] = color;
            var newBrush = new SolidColorBrush(color);
            newBrush.Freeze();
            var keyBrush = key.Replace("Color", "Brush");
            Application.Current.Resources[keyBrush] = newBrush;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
