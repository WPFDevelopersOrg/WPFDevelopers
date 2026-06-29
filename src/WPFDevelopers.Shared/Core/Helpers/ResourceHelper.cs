using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace WPFDevelopers.Helpers
{
    public static class ResourceHelper
    {
        public static List<string> GetResourceUris(
            string relativePath,
            string extension = null,
            Assembly assembly = null)
        {
            if (assembly == null)
            {
                assembly = Assembly.GetCallingAssembly();
            }

            var uris = new List<string>();
            var assemblyName = assembly.GetName().Name;
            var resourceFileName = assemblyName + ".g.resources";
            var searchPattern = relativePath.ToLowerInvariant();

            using (var stream = assembly.GetManifestResourceStream(resourceFileName))
            {
                if (stream == null)
                {
                    return uris;
                }

                using (var reader = new ResourceReader(stream))
                {
                    var enumerator = reader.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        var key = enumerator.Key as string;
                        if (string.IsNullOrEmpty(key)) continue;
                        if (key.IndexOf(searchPattern, StringComparison.OrdinalIgnoreCase) < 0)
                            continue;
                        if (!string.IsNullOrEmpty(extension) &&
                            !key.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                            continue;
                        var uri = BuildPackUri(assemblyName, key);
                        uris.Add(uri);
                    }
                }
            }

            return uris.OrderBy(u => u).ToList();
        }

        public static List<string> GetResourceUris<T>(
            string relativePath,
            string extension = null) where T : class
        {
            var assembly = typeof(T).Assembly;
            return GetResourceUris(relativePath, extension, assembly);
        }

        private static string BuildPackUri(string assemblyName, string resourceKey)
        {
            var resourcePath = resourceKey.Replace("/", ".");

            var lastDot = resourcePath.LastIndexOf('.');
            if (lastDot > 0)
            {
                var pathPart = resourcePath.Substring(0, lastDot).Replace('.', '/');
                var extension = resourcePath.Substring(lastDot);
                resourcePath = pathPart + extension;
            }

            return $"pack://application:,,,/{assemblyName};component/{resourcePath}";
        }
        
        public static List<string> GetResourceNames(
            string relativePath,
            string extension = null,
            Assembly assembly = null)
        {
            var uris = GetResourceUris(relativePath, extension, assembly);
            return uris.Select(u => System.IO.Path.GetFileNameWithoutExtension(u)).ToList();
        }

        public static List<ResourceInfo> GetResourceInfos(
            string relativePath,
            string extension = null,
            Assembly assembly = null)
        {
            var uris = GetResourceUris(relativePath, extension, assembly);
            return uris.Select(u => new ResourceInfo
            {
                Uri = u,
                FileName = System.IO.Path.GetFileName(u),
                NameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(u)
            }).ToList();
        }
    }
    
    public class ResourceInfo
    {
        public string Uri { get; set; }
        public string FileName { get; set; }
        public string NameWithoutExtension { get; set; }
    }
}
