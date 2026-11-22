using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Samples.ViewModels;

namespace WPFDevelopers.Samples.ExampleViews
{
    public partial class SidePanelExample : UserControl
    {
        private int _taskCount = 3;

        public SidePanelExampleViewModel ViewModel { get; } = new SidePanelExampleViewModel();

        public SidePanelExample()
        {
            InitializeComponent();
        }

        private void NewSidePanelTask_Click(object sender, RoutedEventArgs e)
        {
            _taskCount++;
            var task = new TextBlock { Margin = new Thickness(0, 6, 0, 6) };
            SampleLanguageManager.Instance.SetLocalizedText(task, TextBlock.TextProperty, $"任务 {_taskCount}");
            SidePanelTaskList.Children.Insert(0, task);
        }
    }
}
