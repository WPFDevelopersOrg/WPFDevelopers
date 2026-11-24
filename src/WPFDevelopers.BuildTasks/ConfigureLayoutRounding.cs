using System;
using System.Xml;
using Microsoft.Build.Framework;

namespace WPFDevelopers.BuildTasks
{
    public sealed class ConfigureLayoutRounding : XmlConfigurationTask
    {
        private const string SwitchName = "Switch.MS.Internal.DoNotApplyLayoutRoundingToMarginsAndBorderThickness";

        [Required]
        public string TargetFrameworkIdentifier { get; set; }

        [Required]
        public string TargetFrameworkVersion { get; set; }

        protected override string DefaultFileName => "App.config";

        protected override bool ShouldConfigure()
        {
            if (!string.Equals(TargetFrameworkIdentifier, ".NETFramework", StringComparison.OrdinalIgnoreCase))
                return false;

            Version version;
            if (string.IsNullOrWhiteSpace(TargetFrameworkVersion) ||
                !Version.TryParse(TargetFrameworkVersion.Trim().TrimStart('v', 'V'), out version))
            {
                Log.LogError("Invalid TargetFrameworkVersion '{0}'. Expected a version such as 'v4.5.2'.", TargetFrameworkVersion);
                return false;
            }

            return version < new Version(4, 6);
        }

        protected override bool Configure(XmlDocument document)
        {
            var changed = false;
            if (document.DocumentElement == null)
            {
                document.AppendChild(document.CreateElement("configuration"));
                changed = true;
            }

            ValidateRoot(document.DocumentElement, "configuration", "");
            var runtime = EnsureChild(document.DocumentElement, "runtime", "", ref changed);
            var overrides = EnsureChild(runtime, "AppContextSwitchOverrides", "", ref changed);
            var value = overrides.GetAttribute("value");
            var found = false;
            foreach (var entry in value.Split(';'))
            {
                var separator = entry.IndexOf('=');
                var key = separator < 0 ? entry.Trim() : entry.Substring(0, separator).Trim();
                if (key != SwitchName)
                    continue;

                found = true;
                if (separator < 0 || !string.Equals(entry.Substring(separator + 1).Trim(), "false", StringComparison.OrdinalIgnoreCase))
                    Log.LogWarning("WPFDevelopers preserved the existing layout rounding switch '{0}' in '{1}'; expected '{2}=false'.",
                        entry, FilePath ?? DefaultFileName, SwitchName);
            }

            if (!found)
            {
                var prefix = string.IsNullOrWhiteSpace(value) ? "" : value.TrimEnd();
                if (prefix.Length > 0 && !prefix.EndsWith(";", StringComparison.Ordinal))
                    prefix += ";";
                overrides.SetAttribute("value", prefix + SwitchName + "=false");
                changed = true;
            }
            return changed;
        }
    }
}
