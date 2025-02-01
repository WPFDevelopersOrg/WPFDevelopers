using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class NavMenu3D : ListBox
    {
        static NavMenu3D()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavMenu3D),
                new FrameworkPropertyMetadata(typeof(NavMenu3D)));
        }
        public NavMenu3D()
        {
            SelectionMode = SelectionMode.Single;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is NavMenu3DItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new NavMenu3DItem();
        }
    }
}