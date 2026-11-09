using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WPFDevelopers.Core;
using WPFDevelopers.Core.Helpers;
using WPFDevelopers.Helpers;

namespace WPFDevelopers
{
    public class Resources : ResourceDictionary, ISupportInitialize
    {

        private string[] resourceKeys = { "WindowBorderColor", "PrimaryColor" };

        public static event ThemeChangedEvent ThemeChanged;
        public event Action<ThemeMode> ThemeModeChanged;

        private static int _publishInProgress;

        private int _initCount;
        private readonly string _themePreferenceKey;

        private ThemeType _theme = ThemeType.Light;
        public ThemeType Theme
        {
            get => _theme;
            set
            {
                switch (value)
                {
                    case ThemeType.Default: ThemeMode = ThemeMode.System; break;
                    case ThemeType.Light: ThemeMode = ThemeMode.Light; break;
                    case ThemeType.Dark: ThemeMode = ThemeMode.Dark; break;
                    default: throw new ArgumentOutOfRangeException(nameof(value));
                }
            }
        }

        private ThemeMode _themeMode = ThemeMode.Light;
        public ThemeMode ThemeMode
        {
            get => _themeMode;
            set
            {
                if (!Enum.IsDefined(typeof(ThemeMode), value))
                    throw new ArgumentOutOfRangeException(nameof(value));
                ThemePreferenceStore.Save(_themePreferenceKey, value);
                SetThemeMode(value);
            }
        }

        private void SetThemeMode(ThemeMode value)
        {
            if (_themeMode == value) return;
            SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
            _themeMode = value;
            if (value == ThemeMode.System && OSVersionHelper.IsWindows10OrLater())
                SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
            ApplyTheme();
            ThemeModeChanged?.Invoke(value);
        }

        internal ThemeType ResolveTheme(ThemeMode mode)
        {
            switch (mode)
            {
                case ThemeMode.System:
                    return OSVersionHelper.IsWindows10OrLater() && IsDarkMode() ? ThemeType.Dark : ThemeType.Light;
                case ThemeMode.Light: return ThemeType.Light;
                case ThemeMode.Dark: return ThemeType.Dark;
                default: throw new ArgumentOutOfRangeException(nameof(mode));
            }
        }

        public bool IsDarkTheme => Theme == ThemeType.Dark;

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

        private double _radius;
        private bool _radiusSet;
       
        public double Radius
        {
            get => _radius;
            set
            {
                _radius = value;
                _radiusSet = true;
                ElementHelper.OverrideCornerRadius(value);
            }
        }

        public Resources() : this(ThemePreferenceStore.GetApplicationKey())
        {
        }

        internal Resources(string themePreferenceKey)
        {
            _themePreferenceKey = themePreferenceKey;
            var mode = ThemePreferenceStore.Load(_themePreferenceKey);
            AddTheme();
            SetThemeMode(mode);
        }

        void ISupportInitialize.BeginInit()
        {
            _initCount++;
        }

        void ISupportInitialize.EndInit()
        {
            _initCount--;
            if (_initCount == 0 && !_radiusSet && OSVersionHelper.IsWindows11OrLater())
            {
                ElementHelper.OverrideCornerRadius(4);
            }
        }

        void AddTheme()
        {
            var resourceUri = new Uri("pack://application:,,,/WPFDevelopers;component/Themes/Theme.xaml", UriKind.Absolute);
            var resourceDictionary = new ResourceDictionary { Source = resourceUri };
            MergedDictionaries.Add(resourceDictionary);
            _color = GetColorFromResource("Primary");
            ThemeManager.Instance.Resources = this;
            InitializeTheme(Theme,isInit: true);
        }
       
        protected void InitializeTheme(ThemeType themeType,bool isInit = false)
        {
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
            _ = Task.Factory.StartNew(
                PublishWPFDevelopersExtSafe,
                System.Threading.CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.Default);
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
            var theme = ResolveTheme(ThemeMode);
            if (Theme != theme)
                InitializeTheme(theme);
        }

        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (ThemeMode == ThemeMode.System && e.Category == UserPreferenceCategory.General)
            {
                var dispatcher = Application.Current?.Dispatcher;
                if (dispatcher != null && !dispatcher.CheckAccess())
                    dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (ThemeMode == ThemeMode.System) ApplyTheme();
                    }));
                else
                    ApplyTheme();
            }
        }

        void PublishWPFDevelopersExtSafe()
        {
            try
            {
                PublishWPFDevelopersExt();
            }
            catch
            {
                
            }
        }

        void PublishWPFDevelopersExt()
        {
            if (Interlocked.Exchange(ref _publishInProgress, 1) == 1)
                return;

            lock (typeof(Resources))
            {
                try
                {
                    string exePath = Helper.GetTempPathVersionExt;
                    if (!File.Exists(exePath))
                        ExportResource(exePath, "GZ.WPFDevelopersExt.exe.gz");

                    CleanupOldPublishedVersions();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException($"An error occurred while publishing WPFDevelopersExt {ex.Message}");
                }
                finally
                {
                    Interlocked.Exchange(ref _publishInProgress, 0);
                }
            }
        }

        void CleanupOldPublishedVersions()
        {
            try
            {
                if (!Directory.Exists(Helper.GetTempPath))
                    return;

                var currentVersionPath = Path.GetFullPath(Helper.GetTempPathVersion)
                    .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                foreach (var versionDir in Directory.GetDirectories(Helper.GetTempPath))
                {
                    var fullVersionDir = Path.GetFullPath(versionDir)
                        .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                    if (string.Equals(fullVersionDir, currentVersionPath, StringComparison.OrdinalIgnoreCase))
                        continue;

                    TryDeleteDirectorySilently(fullVersionDir);
                }
            }
            catch
            {
            }
        }

        void TryDeleteDirectorySilently(string directory)
        {
            try
            {
                if (!Directory.Exists(directory))
                    return;

                Directory.Delete(directory, true);
            }
            catch
            {
            }
        }

        void ExportResource(string path, string source)
        {
            if (File.Exists(path))
                return;

            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var projectName = Assembly.GetExecutingAssembly().GetName().Name.ToString();
            var resourceName = projectName + "." + source;
            using (var gzStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (gzStream == null)
                    throw new FileNotFoundException($"Embedded resource not found: {resourceName}");

                var tempPath = path + ".tmp";
                using (var stream = new GZipStream(gzStream, CompressionMode.Decompress))
                using (var decompressedFile = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    stream.CopyTo(decompressedFile);
                }

                if (File.Exists(path))
                {
                    File.Delete(tempPath);
                }
                else
                {
                    File.Move(tempPath, path);
                }
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
                            IsDarkTheme)
                            tempColor = colorBase;
                        ThemeManager.Instance.ColorCache[newColor][keyTuple] = tempColor;
                        newColor = tempColor;
                    }
                }
                SetResourceColorAndBrush(newKey, newColor);
            }
        }

        void SetResourceColorAndBrush(string key, Color color)
        {
            UpdateResource(key, color);
            var brushKey = key.Replace("Color", "Brush");
            Brush newBrush = new SolidColorBrush(color);
            newBrush.Freeze();
            UpdateResource(brushKey, newBrush);
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

    public enum ThemeMode
    {
        System,
        Light,
        Dark,
    }

    public enum ThemeType
    {
        Default,
        Light,
        Dark,
    }
}
