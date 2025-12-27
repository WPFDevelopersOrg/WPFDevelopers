using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// LoadingExample.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingExample : UserControl
    {
        private DispatcherTimer _timer = new DispatcherTimer();
        private int _currentProgress = 3;
        public LoadingExample()
        {
            InitializeComponent();
            Loaded += OnLoadingExample_Loaded;
        }

        private void OnLoadingExample_Loaded(object sender, RoutedEventArgs e)
        {
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += (s, args) =>
            {
                if (_currentProgress <= 100)
                    _currentProgress += 3;
                else
                    _timer.Stop();
                Loading.SetValue(BtnProgress, _currentProgress);
            };
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

        private void MyCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Loading.SetLoadingType(BtnProgress, LoadingType.Progress);
            Loading.SetValue(BtnProgress, _currentProgress);
            Loading.SetIsShow(BtnProgress, true);
            _timer.Start();
        }
        private void MyCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _currentProgress = 0;
            Loading.SetIsShow(BtnProgress, false);
            _timer.Stop();
        }
    }
}
