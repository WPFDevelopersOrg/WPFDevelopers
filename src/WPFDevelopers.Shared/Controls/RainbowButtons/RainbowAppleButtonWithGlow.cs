using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
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