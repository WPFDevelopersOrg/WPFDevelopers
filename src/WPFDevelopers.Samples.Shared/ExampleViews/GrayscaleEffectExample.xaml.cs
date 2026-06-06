using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Samples;

namespace WPFDevelopers.Sample.ExampleViews
{
    public partial class GrayscaleEffectExample : UserControl
    {
        public GrayscaleEffectExample()
        {
            InitializeComponent();
        }

        private void FactorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (factorText != null)
                factorText.Text = e.NewValue.ToString("F2");

            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.SetWindowGrayscale(e.NewValue);
            }
        }
    }
}
