using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class AnimationNavigationBar3D : ListBox
    {
        static AnimationNavigationBar3D()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimationNavigationBar3D),
                new FrameworkPropertyMetadata(typeof(AnimationNavigationBar3D)));
        }
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is AnimationNavigationBar3DItem;
        }
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new AnimationNavigationBar3DItem();
        }
    }
}