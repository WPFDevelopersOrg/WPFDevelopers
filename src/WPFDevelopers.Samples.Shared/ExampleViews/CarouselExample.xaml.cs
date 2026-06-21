using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    public partial class CarouselExample : UserControl
    {
        public CarouselExampleViewModel ViewModel { get; }

        public CarouselExample()
        {
            InitializeComponent();
            ViewModel = new CarouselExampleViewModel();
            DataContext = ViewModel;
        }

        private void Carousel_ItemClick(object sender, RoutedEventArgs e)
        {
            if (sender is Carousel carousel)
                Toast.Push($"Event: 点击了第 {carousel.SelectedIndex} 个", ToastImage.Success, true);
        }
    }
}
