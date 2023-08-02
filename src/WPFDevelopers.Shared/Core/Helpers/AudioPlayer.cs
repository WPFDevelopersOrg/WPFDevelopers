using System;
using System.Text;

namespace WPFDevelopers.Helpers
{
    public static class AudioPlayer
    {
        public static TimeSpan GetSoundLength(string fileName)
        {
            try
            {
                var sb = new StringBuilder(128);
                Win32.mciSendString(string.Format("status \"{0}\" length", fileName), sb, 128, IntPtr.Zero);
                var songlength = Convert.ToUInt64(sb.ToString());
                return TimeSpan.FromMilliseconds(songlength);
            }
            catch (Exception)
            {
                return TimeSpan.Zero;
            }
        }

        public static void PlaySong(string fileName, IntPtr handle)
        {
            Win32.mciSendString("close media", null, 0, IntPtr.Zero);
            Win32.mciSendString(string.Format("open \"{0}\" type mpegvideo alias media", fileName), null, 0,
                IntPtr.Zero);
            Win32.mciSendString("play media notify", null, 0, handle);
        }

        public static long Stop()
        {
            return Win32.mciSendString("close media", null, 0, IntPtr.Zero);
        }
    }
}