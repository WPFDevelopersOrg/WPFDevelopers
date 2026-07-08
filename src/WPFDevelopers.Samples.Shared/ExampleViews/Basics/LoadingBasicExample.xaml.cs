using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews.Basics
{
    public partial class LoadingBasicExample : UserControl
    {
        private CancellationTokenSource tokenSource;
        public LoadingBasicExample() { InitializeComponent(); }

        private void Loading_Click(object sender, RoutedEventArgs e)
        {
            var task = new Task(() => { Thread.Sleep(5000); });
            task.ContinueWith(_ => Loading.SetIsShow(MyBasicLoading, false),
                TaskScheduler.FromCurrentSynchronizationContext());
            Loading.SetIsShow(MyBasicLoading, true);
            task.Start();
        }

        private void BtnOffTask_Click(object sender, RoutedEventArgs e)
        {
            if (tokenSource == null) return;
            tokenSource.Cancel();
            Loading.SetIsShow(btnLoadingTask, false);
        }

        private void LoadingTask_Click(object sender, RoutedEventArgs e)
        {
            tokenSource = new CancellationTokenSource();
            var cancellationToken = tokenSource.Token;
            var task = new Task(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    if (tokenSource.IsCancellationRequested) return;
                    Thread.Sleep(1000);
                }
            }, cancellationToken);
            task.ContinueWith(_ =>
            {
                if (!tokenSource.IsCancellationRequested)
                    Loading.SetIsShow(btnLoadingTask, false);
            }, TaskScheduler.FromCurrentSynchronizationContext());
            Loading.SetIsShow(btnLoadingTask, true);
            task.Start();
        }

        private void BtnLoading_Click(object sender, RoutedEventArgs e)
        {
            var task = new Task(() => { Thread.Sleep(50000); });
            task.ContinueWith(_ => Loading.SetIsShow(btnLoading, false),
                TaskScheduler.FromCurrentSynchronizationContext());
            Loading.SetIsShow(btnLoading, true);
            task.Start();
        }
    }
}
