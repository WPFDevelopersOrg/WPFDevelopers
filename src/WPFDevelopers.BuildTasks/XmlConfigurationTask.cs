using System;
using System.IO;
using System.Security;
using System.Text;
using System.Xml;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace WPFDevelopers.BuildTasks
{
    public abstract class XmlConfigurationTask : Task
    {
        [Required]
        public string ProjectDirectory { get; set; }

        public string FilePath { get; set; }

        [Output]
        public string ConfiguredFile { get; private set; }

        [Output]
        public bool Changed { get; private set; }

        protected abstract string DefaultFileName { get; }

        public override bool Execute()
        {
            ConfiguredFile = null;
            Changed = false;
            try
            {
                if (!ShouldConfigure())
                    return !Log.HasLoggedErrors;

                var path = Path.GetFullPath(Path.Combine(ProjectDirectory,
                    string.IsNullOrWhiteSpace(FilePath) ? DefaultFileName : FilePath));
                var document = new XmlDocument { PreserveWhitespace = true, XmlResolver = null };
                var exists = File.Exists(path);
                if (exists)
                {
                    using (var reader = XmlReader.Create(path, new XmlReaderSettings
                    {
                        DtdProcessing = DtdProcessing.Prohibit,
                        XmlResolver = null
                    }))
                        document.Load(reader);
                }

                if (Configure(document))
                {
                    Save(document, path, exists);
                    Changed = true;
                    Log.LogMessage(MessageImportance.High, "WPFDevelopers configured '{0}'.", path);
                }

                ConfiguredFile = path;
                return !Log.HasLoggedErrors;
            }
            catch (Exception exception) when (exception is IOException ||
                                              exception is InvalidDataException ||
                                              exception is UnauthorizedAccessException ||
                                              exception is XmlException ||
                                              exception is ArgumentException ||
                                              exception is NotSupportedException ||
                                              exception is SecurityException)
            {
                Log.LogErrorFromException(exception, false);
                return false;
            }
        }

        protected virtual bool ShouldConfigure()
        {
            return true;
        }

        protected abstract bool Configure(XmlDocument document);

        protected static XmlElement FindChild(XmlElement parent, string name, string namespaceUri)
        {
            XmlElement result = null;
            foreach (XmlNode child in parent.ChildNodes)
            {
                var element = child as XmlElement;
                if (element == null || element.LocalName != name || element.NamespaceURI != namespaceUri)
                    continue;

                if (result != null)
                    throw new InvalidDataException("Multiple '" + name + "' elements were found in '" + parent.Name + "'.");
                result = element;
            }
            return result;
        }

        protected static XmlElement EnsureChild(XmlElement parent, string name, string namespaceUri, ref bool changed)
        {
            var element = FindChild(parent, name, namespaceUri);
            if (element == null)
            {
                element = parent.OwnerDocument.CreateElement(name, namespaceUri);
                parent.AppendChild(element);
                changed = true;
            }
            return element;
        }

        protected static void ValidateRoot(XmlElement root, string name, string namespaceUri)
        {
            if (root.LocalName != name || root.NamespaceURI != namespaceUri)
                throw new InvalidDataException("Expected root element '" + name + "' in namespace '" + namespaceUri + "'.");
        }

        private static void Save(XmlDocument document, string path, bool exists)
        {
            var directory = Path.GetDirectoryName(path);
            Directory.CreateDirectory(directory);
            var temporaryPath = Path.Combine(directory, "." + Path.GetFileName(path) + "." + Guid.NewGuid().ToString("N") + ".tmp");
            try
            {
                var declaration = document.FirstChild as XmlDeclaration;
                var encoding = declaration != null && !string.IsNullOrEmpty(declaration.Encoding)
                    ? Encoding.GetEncoding(declaration.Encoding)
                    : new UTF8Encoding(false);
                using (var writer = XmlWriter.Create(temporaryPath, new XmlWriterSettings
                {
                    Encoding = encoding,
                    Indent = true,
                    NewLineHandling = NewLineHandling.None
                }))
                    document.Save(writer);

                if (exists)
                    File.Replace(temporaryPath, path, null);
                else
                    File.Move(temporaryPath, path);
            }
            finally
            {
                if (File.Exists(temporaryPath))
                    File.Delete(temporaryPath);
            }
        }
    }
}
