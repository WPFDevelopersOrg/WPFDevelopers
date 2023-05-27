using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Samples.Controls
{
    public class RainbowAppleButtonWithGlow : Button
    {
        static RainbowAppleButtonWithGlow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RainbowAppleButtonWithGlow),
                new FrameworkPropertyMetadata(typeof(RainbowAppleButtonWithGlow)));
        }
    }
}