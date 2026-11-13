using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WPFDevelopers.Helpers
{
    internal static class ThemePreferenceStore
    {
        private const string PreferenceFileName = "theme-mode.txt";

        internal static ThemeMode Load()
        {
            var path = GetPreferencePath();
            if (!File.Exists(path))
                return ThemeMode.System;

            switch (File.ReadAllText(path).Trim())
            {
                case "System": return ThemeMode.System;
                case "Light": return ThemeMode.Light;
                case "Dark": return ThemeMode.Dark;
                default: throw new InvalidDataException("Invalid theme mode in '" + path + "'.");
            }
        }

        internal static void Save(ThemeMode mode)
        {
            if (!Enum.IsDefined(typeof(ThemeMode), mode))
                throw new ArgumentOutOfRangeException(nameof(mode));

            var path = GetPreferencePath();
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, mode.ToString());
        }

        private static string GetPreferencePath()
        {
            var applicationName = Assembly.GetEntryAssembly()?.GetName().Name
                ?? AppDomain.CurrentDomain.FriendlyName;
            var invalidCharacters = Path.GetInvalidFileNameChars();
            var safeApplicationName = string.Concat(applicationName.Select(character =>
                invalidCharacters.Contains(character) || character == '/' || character == '\\'
                    ? '_'
                    : character));
            if (string.IsNullOrWhiteSpace(safeApplicationName))
                throw new InvalidOperationException("The application name cannot be used for theme preferences.");

            var localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (string.IsNullOrWhiteSpace(localApplicationData))
                throw new IOException("The local application data directory is unavailable.");

            return Path.Combine(localApplicationData, "WPFDevelopers", safeApplicationName, PreferenceFileName);
        }
    }
}
