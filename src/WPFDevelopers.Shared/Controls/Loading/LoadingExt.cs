using System.Diagnostics;
using System.IO;
using System.Windows;

namespace WPFDevelopers.Controls
{
    public class LoadingExt
    {
        private Process process;

        public void Show()
        {
            if (Helper.GetTempPathVersionExt != null && File.Exists(Helper.GetTempPathVersionExt))
            {
                process = new Process();
                process.StartInfo.FileName = Helper.GetTempPathVersionExt;
                process.StartInfo.Arguments = $"Loading {Application.Current.MainWindow.Title}"; 
                process.EnableRaisingEvents = true;
                process.Exited += (s, x) =>
                {
                    process = null;
                };
                process.Start();
            }
        }
        public void Close()
        {
            if (process != null)
                process.Kill();
        }
    }
}
