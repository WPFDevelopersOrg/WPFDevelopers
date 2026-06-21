using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    public partial class SplitButtonExample : UserControl
    {
        public ObservableCollection<string> MenuItems { get; } = new ObservableCollection<string>
        {
            "Save in PDF",
            "Save in Word",
            "Save in Excel",
            "Save in Image"
        };

        public SplitButtonExample()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void SplitButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = (SplitButton)sender;
            Toast.Push($"click main {btn.Content.ToString()}", ToastImage.Success);
        }

        private void SplitButton_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Toast.Push($"选择了: {e.NewValue}", ToastImage.Success);
        }
    }
}
