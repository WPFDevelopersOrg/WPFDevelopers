using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using WPFDevelopers.Controls;
using WPFDevelopers.Samples.Helpers;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// StepExample.xaml 的交互逻辑
    /// </summary>
    public partial class StepExample : UserControl
    {
        public ObservableCollection<string> Steps
        {
            get;
            set;
        }
        public StepExample()
        {
            InitializeComponent();
            Steps = new ObservableCollection<string>();
            Steps.Add("Step 1");
            Steps.Add("Step 2");
            Steps.Add("Step 3");
            Steps.Add("Step 4");
            this.DataContext = this;
        }
        public ICommand NextCommand => new RelayCommand(new Action<object>((sender) =>
        {
            var uniformGrid = sender as UniformGrid;
            if (uniformGrid == null) return;
            foreach (var step in uniformGrid.Children.OfType<Step>())
                step.Next();

        }));
        public ICommand PreviousCommand => new RelayCommand(new Action<object>((sender) =>
        {
            var uniformGrid = sender as UniformGrid;
            if (uniformGrid == null) return;
            foreach (var step in uniformGrid.Children.OfType<Step>())
                step.Previous();
        }));
    }
}
