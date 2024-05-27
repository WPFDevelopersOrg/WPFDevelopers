using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// LoadingExample.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingExample : UserControl
    {
        public LoadingExample()
        {
            InitializeComponent();
        }

        private void Btn_LoadingExtClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var loading = new LoadingExt();
            loading.Show();
            var task = new Task(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    //模拟卡UI线程
                    Application.Current.Dispatcher.Invoke(new System.Action(() =>
                    {
                        Thread.Sleep(1000);
                    }));
                }

            });
            task.ContinueWith(previousTask =>
            {
                loading.Close();
            }, TaskScheduler.FromCurrentSynchronizationContext());
            task.Start();
        }
    }
}
