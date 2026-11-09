using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;

namespace WPFDevelopers.Helpers
{
    internal static class ThemePreferenceStore
    {
        internal static string GetApplicationKey()
        {
            var assembly = Assembly.GetEntryAssembly();
            var name = assembly == null ? AppDomain.CurrentDomain.FriendlyName : assembly.GetName().Name;
            return @"Software\WPFDevelopers\Applications\" + name.Replace('\\', '_').Replace('/', '_');
        }

        internal static ThemeMode Load(string path)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(path))
            {
                var value = key?.GetValue("ThemeMode");
                if (value == null) return ThemeMode.System;
                switch (value)
                {
                    case "System": return ThemeMode.System;
                    case "Light": return ThemeMode.Light;
                    case "Dark": return ThemeMode.Dark;
                    default: throw new InvalidDataException("Invalid ThemeMode in HKCU\\" + path + ".");
                }
            }
        }

        internal static void Save(string path, ThemeMode mode)
        {
            if (!Enum.IsDefined(typeof(ThemeMode), mode))
                throw new ArgumentOutOfRangeException(nameof(mode));
            using (var key = Registry.CurrentUser.CreateSubKey(path))
            {
                if (key == null)
                    throw new IOException("Unable to open theme preferences at HKCU\\" + path + ".");
                key.SetValue("ThemeMode", mode.ToString(), RegistryValueKind.String);
            }
        }
    }
}