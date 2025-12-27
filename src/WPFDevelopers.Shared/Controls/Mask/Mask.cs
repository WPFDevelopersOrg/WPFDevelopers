using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using WPFDevelopers.Utilities;

namespace WPFDevelopers.Controls
{
    public class Mask : BaseControl
    {
        public static readonly DependencyProperty IsShowProperty =
            DependencyProperty.RegisterAttached("IsShow", typeof(bool), typeof(Mask),
                new PropertyMetadata(false, OnIsShowChanged));


        private static void OnIsShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool isShow && d is FrameworkElement parent)
            {
                if (isShow)
                {
                    parent.IsVisibleChanged += Parent_IsVisibleChanged;
                    if (!parent.IsLoaded)
                        parent.Loaded += Parent_Loaded;
                    else
                        CreateMask(parent);
                }
                else
                {
                    parent.Loaded -= Parent_Loaded;
                    parent.IsVisibleChanged -= Parent_IsVisibleChanged;
                    CreateMask(parent, true);
                }
            }
        }

        private static void Parent_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool isVisible && sender is FrameworkElement parent)
            {
                var isShow = GetIsShow(parent);
                if (isVisible && isShow && !parent.IsLoaded)
                    CreateMask(parent);
            }
        }

        private static void Parent_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is UIElement element)
                CreateMask(element);
        }

        private static void CreateMask(UIElement uIElement, bool isRemove = false)
        {
            if (uIElement == null) return;
            var layer = AdornerLayer.GetAdornerLayer(uIElement);
            if (layer == null) return;
            var adorners = layer.GetAdorners(uIElement);
            if (adorners != null)
            {
                foreach (var item in adorners)
                {
                    if (item is AdornerContainer container)
                        layer.Remove(container);
                }
            }
            if (isRemove)
                return;
            var adornerContainer = new AdornerContainer(uIElement);
            var childContent = GetChild(uIElement);
            var maskControl = new MaskControl(uIElement);
            if (childContent != null)
            {
                var contentBinding = new Binding("DataContext")
                {
                    Source = uIElement as FrameworkElement
                };

                if (childContent is FrameworkElement childElement)
                {
                    childElement.SetBinding(FrameworkElement.DataContextProperty, contentBinding);
                }
                maskControl.Content = childContent;
            }
            maskControl.SetBinding(DataContextProperty, new Binding("DataContext")
            {
                Source = uIElement as FrameworkElement
            });
            adornerContainer.Child = maskControl;
            layer.Add(adornerContainer);
        }

        public static bool GetIsShow(DependencyObject obj)
        {
            return (bool) obj.GetValue(IsShowProperty);
        }

        public static void SetIsShow(DependencyObject obj, bool value)
        {
            obj.SetValue(IsShowProperty, value);
        }
    }
}