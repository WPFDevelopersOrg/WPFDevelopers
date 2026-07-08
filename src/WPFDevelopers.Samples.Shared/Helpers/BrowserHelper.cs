using System;
using System.Diagnostics;

namespace WPFDevelopers.Helpers
{
    public static class BrowserHelper
    {
        public static void OpenUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;

            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                return;

            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch
            {
            }
        }

        public static void OpenUrl(Uri uri)
        {
            if (uri == null) return;
            OpenUrl(uri.AbsoluteUri);
        }
    }
}
