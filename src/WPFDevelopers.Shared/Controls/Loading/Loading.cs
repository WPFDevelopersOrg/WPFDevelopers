using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using WPFDevelopers.Core;
using WPFDevelopers.Helpers;
using WPFDevelopers.Utilities;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

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



        public static double GetValue(DependencyObject obj)
        {
            return (double)obj.GetValue(ValueProperty);
        }

        public static void SetValue(DependencyObject obj, double value)
        {
            obj.SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.RegisterAttached("Value", typeof(double), typeof(LoadingType), new PropertyMetadata(0d, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                var isShow = GetIsShow(element);
                if (!isShow) return;
                var newValue = (double)e.NewValue;
                if (newValue >= 100)
                {
                    SetIsShow(element, false);
                    return;
                }
                var layer = AdornerLayer.GetAdornerLayer(element);
                if (layer != null)
                {
                    var adorners = layer.GetAdorners(element);
                    if (adorners != null)
                    {
                        foreach (var adorner in adorners)
                        {
                            if (adorner is AdornerContainer container &&
                                container.Child is MaskControl maskControl &&
                                maskControl.Content is ProgressLoading progressLoading)
                            {
                                var value = ((double)newValue / 100.0) * 360;
                                progressLoading.Value = value;
                            }
                        }
                    }
                }
            }
        }

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
            if (uIElement == null) return;
            UIElement value = null;
            var layer = AdornerLayer.GetAdornerLayer(uIElement);
            if (layer == null) return;
            var adorners = layer.GetAdorners(uIElement);
            if (adorners != null)
            {
                foreach (var item in adorners)
                {
                    if (item is AdornerContainer container)
                    {
                        if (isRemove)
                            SetChild(uIElement, null);
                        container.Child = null;
                        layer.Remove(container);
                    }
                }
            }
            if (isRemove)
                return;
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
                case LoadingType.Progress:
                    if (isLoading)
                    {
                        var frameworkElement = (FrameworkElement)uIElement;
                        var progressLoading = new ProgressLoading();
                        var _size = frameworkElement.ActualHeight < frameworkElement.ActualWidth
                            ? frameworkElement.ActualHeight
                            : frameworkElement.ActualWidth;
                        if (_size < MINSIZE)
                        {
                            progressLoading.Width = SIZE;
                            progressLoading.Height = SIZE;
                        }
                        value = progressLoading;
                    }
                    break;
            }

            if (value != null)
            {
                var cornerRadius = ElementHelper.GetCornerRadius(uIElement);
                adornerContainer.Child = new MaskControl(uIElement)
                {
                    CornerRadius = cornerRadius,
                    Content = value,
                };
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
        Normal,
        /// <summary>
        ///     进度
        /// </summary>
        Progress
    }
}