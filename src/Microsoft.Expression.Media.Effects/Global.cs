using System;
using System.Reflection;

namespace Microsoft.Expression.Media.Effects
{
    internal static class Global
    {
        public static Uri MakePackUri(string relativeFile)
        {
            string uriString = "pack://application:,,,/" + Global.AssemblyShortName + ";component/" + relativeFile;
            return new Uri(uriString);
        }

        private static string AssemblyShortName
        {
            get
            {
                if (Global.assemblyShortName == null)
                {
                    Assembly assembly = typeof(Global).Assembly;
                    Global.assemblyShortName = assembly.ToString().Split(new char[]
                    {
                        ','
                    })[0];
                }
                return Global.assemblyShortName;
            }
        }

        private static string assemblyShortName;
    }
}
