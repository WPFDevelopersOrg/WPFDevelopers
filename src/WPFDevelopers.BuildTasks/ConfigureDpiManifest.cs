using System.Xml;

namespace WPFDevelopers.BuildTasks
{
    public sealed class ConfigureDpiManifest : XmlConfigurationTask
    {
        private const string AssemblyNamespace = "urn:schemas-microsoft-com:asm.v1";
        private const string ApplicationNamespace = "urn:schemas-microsoft-com:asm.v3";

        public string AssemblyName { get; set; } = "Application";

        protected override string DefaultFileName => "app.manifest";

        protected override bool Configure(XmlDocument document)
        {
            var changed = false;
            if (document.DocumentElement == null)
            {
                var root = document.CreateElement("assembly", AssemblyNamespace);
                root.SetAttribute("manifestVersion", "1.0");
                document.AppendChild(root);
                var identity = document.CreateElement("assemblyIdentity", AssemblyNamespace);
                identity.SetAttribute("version", "1.0.0.0");
                identity.SetAttribute("name", AssemblyName + ".app");
                root.AppendChild(identity);
                changed = true;
            }

            ValidateRoot(document.DocumentElement, "assembly", AssemblyNamespace);
            var application = EnsureChild(document.DocumentElement, "application", ApplicationNamespace, ref changed);
            var settings = EnsureChild(application, "windowsSettings", ApplicationNamespace, ref changed);
            var awareConflict = HasConflict(settings, "dpiAware",
                "http://schemas.microsoft.com/SMI/2005/WindowsSettings", "true/PM");
            var awarenessConflict = HasConflict(settings, "dpiAwareness",
                "http://schemas.microsoft.com/SMI/2016/WindowsSettings", "PerMonitorV2, PerMonitor");
            // A new dpiAwareness declaration can override an existing dpiAware choice.
            if (awareConflict || awarenessConflict)
                return false;

            ConfigureSetting(settings, "dpiAware", "http://schemas.microsoft.com/SMI/2005/WindowsSettings",
                "true/PM", ref changed);
            ConfigureSetting(settings, "dpiAwareness", "http://schemas.microsoft.com/SMI/2016/WindowsSettings",
                "PerMonitorV2, PerMonitor", ref changed);
            return changed;
        }

        private bool HasConflict(XmlElement settings, string name, string namespaceUri, string value)
        {
            var element = FindChild(settings, name, namespaceUri);
            if (element == null || element.InnerText.Trim() == value)
                return false;

            Log.LogWarning("WPFDevelopers skipped DPI configuration because {0}='{1}' in '{2}'; expected '{3}'.",
                name, element.InnerText, FilePath ?? DefaultFileName, value);
            return true;
        }

        private static void ConfigureSetting(XmlElement settings, string name, string namespaceUri, string value, ref bool changed)
        {
            var element = FindChild(settings, name, namespaceUri);
            if (element == null)
            {
                element = EnsureChild(settings, name, namespaceUri, ref changed);
                element.InnerText = value;
            }
        }
    }
}
