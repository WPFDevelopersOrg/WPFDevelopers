using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFDevelopers.Controls;
using WPFDevelopers.Samples.Helpers;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// CarouselExample.xaml 的交互逻辑
    /// </summary>
    public partial class CarouselExample : UserControl
    {
        public ObservableCollection<CarouselSlideModel> ImagePaths
        {
            get { return (ObservableCollection<CarouselSlideModel>)GetValue(ImagePathsProperty); }
            set { SetValue(ImagePathsProperty, value); }
        }

        public static readonly DependencyProperty ImagePathsProperty =
            DependencyProperty.Register(nameof(ImagePaths), typeof(ObservableCollection<CarouselSlideModel>), typeof(CarouselExample), new PropertyMetadata(null));

        public CarouselExample()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += CarouselExample_Loaded;
        }
       
        private void CarouselExample_Loaded(object sender, RoutedEventArgs e)
        {
            var imgs = new List<CarouselSlideModel>
            { 
                new CarouselSlideModel{ Title ="Slide0",URL="pack://application:,,,/WPFDevelopers.Samples;component/Resources/Images/Craouse/0.jpg" },
                new CarouselSlideModel{ Title ="Slide1",URL="pack://application:,,,/WPFDevelopers.Samples;component/Resources/Images/Craouse/1.jpg" },
                new CarouselSlideModel{ Title ="Slide2",URL="pack://application:,,,/WPFDevelopers.Samples;component/Resources/Images/Craouse/2.jpg" },
                new CarouselSlideModel{ Title ="Slide3",URL="pack://application:,,,/WPFDevelopers.Samples;component/Resources/Images/Craouse/3.jpg" },
                new CarouselSlideModel{ Title ="Slide4",URL="pack://application:,,,/WPFDevelopers.Samples;component/Resources/Images/Craouse/4.jpg" },
            };
            ImagePaths = new ObservableCollection<CarouselSlideModel>(imgs);
        }
       
        private void Carousel_ItemClick(object sender, RoutedEventArgs e)
        {
            var carousel = sender as Carousel;
            if (carousel != null)
                Toast.Push($"Event: 点击了第 {carousel.SelectedIndex} 个", ToastImage.Success, true);
        }

        public ICommand CarouselClickCommand => new RelayCommand(param =>
        {
            if (param is CarouselSlideModel model)
            {
                Toast.Push($"Command: 点击了图片 - {model.Title}", ToastImage.Success, true);
            }
        });
    }

    public class CarouselSlideModel
    {
        public string Title { get; set; }
        public string URL { get; set; }
    }
}
