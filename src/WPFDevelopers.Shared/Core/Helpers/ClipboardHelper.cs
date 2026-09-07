using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace WPFDevelopers.Helpers
{
    public static class ClipboardHelper
    {
        public static bool TrySetText(string text, int maxAttempts = 3, int retryDelayMilliseconds = 50)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            var attempts = Math.Max(1, maxAttempts);
            var delay = Math.Max(0, retryDelayMilliseconds);

            for (var attempt = 0; attempt < attempts; attempt++)
            {
                try
                {
                    Clipboard.SetDataObject(text, false);
                    return true;
                }
                catch (COMException)
                {
                    if (attempt == attempts - 1)
                    {
                        return false;
                    }
                }
                catch (ExternalException)
                {
                    if (attempt == attempts - 1)
                    {
                        return false;
                    }
                }

                if (delay > 0)
                {
                    Thread.Sleep(delay);
                }
            }

            return false;
        }
    }
}
