using System.Windows;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// CropAvatarWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CropAvatarWindow 
    {
        public CropAvatarWindow()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
