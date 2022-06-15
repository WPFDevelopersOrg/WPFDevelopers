using System.Windows.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// CropAvatarExample.xaml 的交互逻辑
    /// </summary>
    public partial class CropAvatarExample : UserControl
    {
        public CropAvatarExample()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var cropAvatarWindow = new CropAvatarWindow();
            if (cropAvatarWindow.ShowDialog() == true)
            {
                MyImage.Source = cropAvatarWindow.CropAvatarImage.Source;
            }
        }
    }
}
