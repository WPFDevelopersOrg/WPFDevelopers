using System.Windows.Controls;
using System.Windows.Documents;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// ThumbDragAndAngleExample.xaml 的交互逻辑
    /// </summary>
    public partial class ThumbDragAndAngleExample : UserControl
    {
        public ThumbDragAndAngleExample()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                var al = AdornerLayer.GetAdornerLayer(_border);
                al.Add(new ElementAdorner(_border));
            };
        }
    }
}
