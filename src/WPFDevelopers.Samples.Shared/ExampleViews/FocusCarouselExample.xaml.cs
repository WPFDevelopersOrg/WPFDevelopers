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
    /// Interaction logic for FocusCarouselExample.xaml
    /// </summary>
    public partial class FocusCarouselExample : UserControl
    {
        public CarouselExampleViewModel ViewModel { get; }
        public FocusCarouselExample()
        {
            InitializeComponent();
            ViewModel = new CarouselExampleViewModel();
            DataContext = ViewModel;
        }
       
        private void Carousel_ItemClick(object sender, RoutedEventArgs e)
        {
            var carousel = sender as FocusCarousel;
            if (carousel != null)
                Toast.Push($"Event: 点击了第 {carousel.SelectedIndex} 个", ToastImage.Success, true);
        }

    }
}
