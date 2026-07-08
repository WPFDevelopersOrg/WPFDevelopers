using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WPFDevelopers.Controls;
using WPFDevelopers.Samples.Helpers;

namespace WPFDevelopers.Samples.ExampleViews
{
    public class CarouselExampleViewModel
    {
        public ObservableCollection<CarouselSlideModel> ImagePaths { get; }

        public CarouselExampleViewModel()
        {
            var imgs = new List<CarouselSlideModel>
            {
                new CarouselSlideModel{ Title ="Slide0", URL="pack://application:,,,/WPFDevelopers.Samples;component/Resources/Images/Craouse/Slide0.png" },
                new CarouselSlideModel{ Title ="Slide1", URL="pack://application:,,,/WPFDevelopers.Samples;component/Resources/Images/Craouse/Slide1.png" },
                new CarouselSlideModel{ Title ="Slide2", URL="pack://application:,,,/WPFDevelopers.Samples;component/Resources/Images/Craouse/Slide2.png" },
                new CarouselSlideModel{ Title ="Slide3", URL="pack://application:,,,/WPFDevelopers.Samples;component/Resources/Images/Craouse/Slide3.png" },
            };
            ImagePaths = new ObservableCollection<CarouselSlideModel>(imgs);
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
