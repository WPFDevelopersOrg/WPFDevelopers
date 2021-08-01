using System.Windows.Controls;
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
            this.DataContext = this;
        }
    }
}
