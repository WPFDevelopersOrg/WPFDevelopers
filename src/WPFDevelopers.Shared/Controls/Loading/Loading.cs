using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using WPFDevelopers.Core;
using WPFDevelopers.Utilities;

namespace WPFDevelopers.Controls
{
    public class Loading : BaseControl
    {
        private const short SIZE = 25;
        private const double MINSIZE = 40;

        public static readonly DependencyProperty IsShowProperty =
            DependencyProperty.RegisterAttached("IsShow", typeof(bool), typeof(Loading),
                new PropertyMetadata(false, OnIsShowChanged));

        public static readonly DependencyProperty LoadingTypeProperty =
            DependencyProperty.RegisterAttached("LoadingType", typeof(LoadingType), typeof(Loading),
                new PropertyMetadata(LoadingType.Default));


        public static LoadingType GetLoadingType(DependencyObject obj)
        {
            return (LoadingType)obj.GetValue(LoadingTypeProperty);
        }

        public static void SetLoadingType(DependencyObject obj, LoadingType value)
        {
            obj.SetValue(LoadingTypeProperty, value);
        }

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
                        CreateLoading(parent);
                }
                else
                {
                    parent.Loaded -= Parent_Loaded;
                    parent.IsVisibleChanged -= Parent_IsVisibleChanged;
                    CreateLoading(parent, true);
                }
            }
        }

        private static void Parent_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool isVisible && sender is FrameworkElement parent)
            {
                var isShow = GetIsShow(parent);
                if (isVisible && isShow && !parent.IsLoaded)
                    CreateLoading(parent);
            }
        }

        private static void Parent_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is UIElement element)
                CreateLoading(element);
        }

        private static void CreateLoading(UIElement uIElement, bool isRemove = false)
        {
            UIElement value = null;
            var layer = AdornerLayer.GetAdornerLayer(uIElement);
            if (layer == null) return;
            var adorners = layer.GetAdorners(uIElement);
            if (isRemove || adorners != null)
                foreach (var item in adorners)
                    if (item is AdornerContainer container)
                    {
                        if (isRemove)
                            SetChild(uIElement, null);
                        container.Child = null;
                        layer.Remove(container);
                    }

            var adornerContainer = new AdornerContainer(uIElement);
            var type = GetLoadingType(uIElement);
            var isLoading = GetIsShow(uIElement);
            if (!isLoading) return;
            var w = (double)uIElement.GetValue(ActualWidthProperty);
            var h = (double)uIElement.GetValue(ActualHeightProperty);
            switch (type)
            {
                case LoadingType.Default:
                    if (isLoading)
                    {
                        var frameworkElement = (FrameworkElement)uIElement;
                        var normalLoading = new DefaultLoading();
                        var _size = frameworkElement.ActualHeight < frameworkElement.ActualWidth
                            ? frameworkElement.ActualHeight
                            : frameworkElement.ActualWidth;
                        if (_size < MINSIZE)
                        {
                            normalLoading.Width = SIZE;
                            normalLoading.Height = SIZE;
                        }
                        value = normalLoading;
                    }

                    break;
                case LoadingType.Normal:
                    var defaultLoading = new NormalLoading();
                    if (w < MINSIZE || h < MINSIZE)
                    {
                        defaultLoading.Width = SIZE;
                        defaultLoading.Height = SIZE;
                        defaultLoading.StrokeArray = new DoubleCollection { 10, 100 };
                    }
                    value = defaultLoading;
                    break;
            }

            if (value != null)
                adornerContainer.Child = new MaskControl(uIElement)
                { Content = value, Background = ThemeManager.Instance.BackgroundBrush };
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

    /// <summary>
    ///     Loading类型
    /// </summary>
    public enum LoadingType
    {
        /// <summary>
        ///     默认
        /// </summary>
        Default,

        /// <summary>
        ///     普通
        /// </summary>
        Normal
    }
}