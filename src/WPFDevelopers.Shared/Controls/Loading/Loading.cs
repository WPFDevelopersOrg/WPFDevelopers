using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using WPFDevelopers.Helpers;
using WPFDevelopers.Utilities;

namespace WPFDevelopers.Controls
{
    public class Loading : BaseControl
    {
        public static readonly DependencyProperty IsShowProperty =
           DependencyProperty.RegisterAttached("IsShow", typeof(bool), typeof(Loading),
               new PropertyMetadata(false, OnIsLoadingChanged));
        private const short SIZE = 25;
        private const double MINSIZE = 40;
        private static FrameworkElement oldFrameworkElement;
        private static void OnIsLoadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool isMask && d is FrameworkElement parent)
            {
                if (isMask)
                {
                    if (!parent.IsLoaded)
                        parent.Loaded += Parent_Loaded;
                    else
                        CreateMask(parent);
                }
                else
                {
                    parent.Loaded -= Parent_Loaded;
                    CreateMask(parent, true);
                }
            }
        }
        private static void Parent_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is UIElement element)
                CreateMask(element);
        }
        static void CreateMask(UIElement uIElement, bool isRemove = false)
        {
            var layer = AdornerLayer.GetAdornerLayer(uIElement);
            if (layer == null) return;
            if (isRemove && uIElement != null)
            {
                var adorners = layer.GetAdorners(uIElement);
                if (adorners != null)
                {
                    foreach (var item in adorners)
                    {
                        if (item is AdornerContainer container)
                        {
                            var isAddChild = (bool)Loading.GetIsAddChild(uIElement);
                            if (!isAddChild)
                                Loading.SetChild(uIElement, null);
                            container.Child = null;
                            layer.Remove(container);
                        }
                    }
                }
                return;
            }
            var adornerContainer = new AdornerContainer(uIElement);
            var value = Loading.GetChild(uIElement);
            if (value == null)
            {
                var isLoading = GetIsShow(uIElement);
                if (isLoading)
                {
                    var w = (double)uIElement.GetValue(ActualWidthProperty);
                    var h = (double)uIElement.GetValue(ActualHeightProperty);
                    var defaultLoading = new DefaultLoading();
                    if (w < MINSIZE || h < MINSIZE)
                    {
                        defaultLoading.Width = SIZE;
                        defaultLoading.Height = SIZE;
                        defaultLoading.StrokeArray = new DoubleCollection { 10, 100 };
                    }
                    SetChild(uIElement, defaultLoading);
                    value = Loading.GetChild(uIElement);
                }
                if (value != null)
                    adornerContainer.Child = new MaskControl(uIElement) { Content = value, Background = ControlsHelper.Brush };
            }
            else
            {
                var normalLoading = (FrameworkElement)value;
                var frameworkElement = (FrameworkElement)uIElement;
                Loading.SetIsAddChild(uIElement, true);

                if (oldFrameworkElement != null)
                    value = oldFrameworkElement;
                else
                {
                    string xaml = XamlWriter.Save(normalLoading);
                    oldFrameworkElement = (FrameworkElement) XamlReader.Parse(xaml);
                }

                var _size = frameworkElement.ActualHeight < frameworkElement.ActualWidth ? frameworkElement.ActualHeight : frameworkElement.ActualWidth;
                if(_size < MINSIZE)
                {
                    normalLoading.Width = SIZE;
                    normalLoading.Height = SIZE;
                    value = normalLoading;
                }
                
                adornerContainer.Child = new MaskControl(uIElement) { Content = value, Background = ControlsHelper.Brush };

            }
            layer.Add(adornerContainer);

        }
        public static bool GetIsShow(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsShowProperty);
        }

        public static void SetIsShow(DependencyObject obj, bool value)
        {
            obj.SetValue(IsShowProperty, value);
        }
    }
}