using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class RainbowAppleButton : Button
    {
        static RainbowAppleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RainbowAppleButton), new FrameworkPropertyMetadata(typeof(RainbowAppleButton)));
        }
    }
}
