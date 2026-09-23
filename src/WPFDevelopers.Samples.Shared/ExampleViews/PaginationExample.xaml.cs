using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFDevelopers.Controls;
using WPFDevelopers.Samples.Helpers;
using WPFDevelopers.Samples.ViewModels;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// PaginationExample.xaml 的交互逻辑
    /// </summary>
    public partial class PaginationExample : UserControl
    {
        public PaginationExampleVM NormalPaginationViewModel { get; set; } = new PaginationExampleVM();
        public PaginationExampleVM LitePaginationViewModel { get; set; } = new PaginationExampleVM();

        public PaginationExample()
        {
            InitializeComponent();
        }

        private void PaginationPrevClick(object sender, RoutedEventArgs e)
        {
            if (sender is Pagination pagination)
            {
                Toast.Push($"Prev click: Current={pagination.Current}", ToastImage.Info);
            }
        }

        private void PaginationNextClick(object sender, RoutedEventArgs e)
        {
            if (sender is Pagination pagination)
            {
                Toast.Push($"Next click: Current={pagination.Current}", ToastImage.Success);
            }
        }

        private void PaginationPageClick(object sender, RoutedEventArgs e)
        {
            if (sender is Pagination pagination)
            {
                Toast.Push($"Page click: Current={pagination.Current}", ToastImage.Info, true);
            }
        }

        private void PaginationJumpPageClick(object sender, RoutedEventArgs e)
        {
            if (sender is Pagination pagination)
            {
                Toast.Push($"Jump page click: Current={pagination.Current}", ToastImage.Warning, true);
            }
        }

        public ICommand PrevClickCommand => new RelayCommand(param =>
        {
            Toast.Push($"Command prev click: {param}", ToastImage.Info);
        });

        public ICommand NextClickCommand => new RelayCommand(param =>
        {
            Toast.Push($"Command next click: {param}", ToastImage.Success);
        });

        public ICommand PageClickCommand => new RelayCommand(param =>
        {
            if (param is int page)
            {
                Toast.Push($"Command page click: {page}", ToastImage.Info, true);
            }
        });

        public ICommand JumpPageClickCommand => new RelayCommand(param =>
        {
            if (param is int page)
            {
                Toast.Push($"Command jump page click: {page}", ToastImage.Warning, true);
            }
        });
    }
}
