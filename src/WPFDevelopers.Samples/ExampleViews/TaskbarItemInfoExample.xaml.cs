using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// TaskbarItemInfoExample.xaml 的交互逻辑
    /// </summary>
    public partial class TaskbarItemInfoExample : MetroWindow
    {
        int iconWidth = 20;
        int iconHeight = 20;
        RenderTargetBitmap renderTargetBitmap;
        ContentControl contentControl;
        public TaskbarItemInfoExample()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                renderTargetBitmap = new RenderTargetBitmap(iconWidth, iconHeight, 96, 96, PixelFormats.Default);
                contentControl = new ContentControl();
                contentControl.ContentTemplate = ((DataTemplate)Resources["TaskbarIcon"]);
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            switch (btn.Tag.ToString())
            {
                case "add":
                    CurrentTaskbarItemInfo("99");
                    break;
                case "remove":
                    TaskbarItemInfo.Overlay = null;
                    break;
                default:
                    break;
            }
        }
        void CurrentTaskbarItemInfo(string value)
        {
            contentControl.Content = value;
            contentControl.Arrange(new Rect(0, 0, iconWidth, iconHeight));
            renderTargetBitmap.Render(contentControl);
            TaskbarItemInfo.Overlay = (ImageSource)renderTargetBitmap;
        }
    }
}
