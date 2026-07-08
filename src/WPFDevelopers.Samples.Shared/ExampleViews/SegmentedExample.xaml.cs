using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFDevelopers.Controls;
using WPFDevelopers.Samples.Helpers;

namespace WPFDevelopers.Samples.ExampleViews
{
    public partial class SegmentedExample : UserControl
    {
        public ObservableCollection<OptionItem> Options { get; } = new ObservableCollection<OptionItem>
        {
            new OptionItem { Name = "Day", Id = 1 },
            new OptionItem { Name = "Week", Id = 2 },
            new OptionItem { Name = "Month", Id = 3 },
            new OptionItem { Name = "Year", Id = 4 }
        };



        public int SelectedOptionIndex
        {
            get { return (int)GetValue(SelectedOptionIndexProperty); }
            set { SetValue(SelectedOptionIndexProperty, value); }
        }

        public static readonly DependencyProperty SelectedOptionIndexProperty =
            DependencyProperty.Register(nameof(SelectedOptionIndex), typeof(int), typeof(SegmentedExample), new PropertyMetadata(1));



        public ICommand ItemClickCommand => new RelayCommand(obj =>
        {
            Toast.Push($"MVVM 模式点击: {obj}", ToastImage.Success);
        });

        public SegmentedExample()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Segmented_ItemClick(object sender, RoutedEventArgs e)
        {
            Toast.Push($"点击: {e.OriginalSource.ToString()}", ToastImage.Success, true);
        }


    }

    public class OptionItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
