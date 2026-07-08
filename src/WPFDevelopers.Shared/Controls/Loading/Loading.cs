using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using WPFDevelopers.Helpers;
using WPFDevelopers.Utilities;

namespace WPFDevelopers.Controls
{
    public class Loading : BaseControl
    {
        private const short SIZE = 25;
        private const double MINSIZE = 40;

        private static readonly Dictionary<UIElement, Grid> _wrapperMap = new Dictionary<UIElement, Grid>();
        private static readonly Dictionary<UIElement, UIElement> _loadingMap = new Dictionary<UIElement, UIElement>();
        private static readonly Dictionary<UIElement, LoadingAdorner> _adornerMap = new Dictionary<UIElement, LoadingAdorner>();

        public static readonly DependencyProperty IsShowProperty =
            DependencyProperty.RegisterAttached("IsShow", typeof(bool), typeof(Loading),
                new PropertyMetadata(false, OnIsShowChanged));

        public static readonly DependencyProperty LoadingTypeProperty =
            DependencyProperty.RegisterAttached("LoadingType", typeof(LoadingType), typeof(Loading),
                new PropertyMetadata(LoadingType.Default));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.RegisterAttached("Value", typeof(double), typeof(LoadingType),
                new PropertyMetadata(0d, OnValueChanged));


        public static double GetValue(DependencyObject obj)
        {
            return (double) obj.GetValue(ValueProperty);
        }

        public static void SetValue(DependencyObject obj, double value)
        {
            obj.SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                var isShow = GetIsShow(element);
                if (!isShow) return;
                var newValue = (double) e.NewValue;
                if (newValue >= 100)
                {
                    SetIsShow(element, false);
                    return;
                }

                if (_loadingMap.TryGetValue(element, out var loadingElement) &&
                    loadingElement is ProgressLoading progressLoading)
                {
                    var value = newValue / 100.0 * 360;
                    progressLoading.Value = value;
                }
            }
        }

        public static LoadingType GetLoadingType(DependencyObject obj)
        {
            return (LoadingType) obj.GetValue(LoadingTypeProperty);
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

            if (isRemove)
            {
                UnwrapElement(uIElement);
                return;
            }

            if (_wrapperMap.ContainsKey(uIElement) || _adornerMap.ContainsKey(uIElement) || _loadingMap.ContainsKey(uIElement))
                return;

            var type = GetLoadingType(uIElement);
            var isLoading = GetIsShow(uIElement);
            if (!isLoading) return;

            _loadingMap[uIElement] = null;

            UIElement value = null;
            var w = (double) uIElement.GetValue(FrameworkElement.ActualWidthProperty);
            var h = (double) uIElement.GetValue(FrameworkElement.ActualHeightProperty);

            switch (type)
            {
                case LoadingType.Default:
                    if (isLoading)
                    {
                        var frameworkElement = (FrameworkElement) uIElement;
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
                        defaultLoading.StrokeArray = new DoubleCollection {10, 100};
                    }

                    value = defaultLoading;
                    break;
                case LoadingType.Progress:
                    if (isLoading)
                    {
                        var frameworkElement = (FrameworkElement) uIElement;
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

            if (value == null)
                return;

            WrapElement(uIElement, value);
        }

        private static void WrapElement(UIElement target, UIElement loadingContent)
        {
            var parent = VisualTreeHelper.GetParent(target);
            if (parent == null)
                return;

            if (parent is ContentPresenter)
            {
                var cornerRadius = ElementHelper.GetCornerRadius(target);
                var mask = new MaskControl(target)
                {
                    Content = loadingContent,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Visibility = Visibility.Visible
                };

                mask.SetBinding(MaskControl.CornerRadiusProperty, new Binding
                {
                    Path = new PropertyPath("(0)", ElementHelper.CornerRadiusProperty),
                    Source = target
                });

                var adorner = new LoadingAdorner(target, mask);
                _loadingMap[target] = mask;

                if (target is FrameworkElement targetFe && !targetFe.IsLoaded)
                {
                    RoutedEventHandler loadedHandler = null;
                    loadedHandler = (s, e) =>
                    {
                        var layer = AdornerLayer.GetAdornerLayer(target);
                        if (layer != null)
                        {
                            layer.Add(adorner);
                            _adornerMap[target] = adorner;
                        }
                        targetFe.Loaded -= loadedHandler;
                    };
                    targetFe.Loaded += loadedHandler;
                }
                else
                {
                    var layer = AdornerLayer.GetAdornerLayer(target);
                    if (layer != null)
                    {
                        layer.Add(adorner);
                        _adornerMap[target] = adorner;
                    }
                }
                return;
            }

            var cornerRadius2 = ElementHelper.GetCornerRadius(target);
            var mask2 = new MaskControl(target)
            {
                Content = loadingContent,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Visibility = Visibility.Visible
            };

            mask2.SetBinding(MaskControl.CornerRadiusProperty, new Binding
            {
                Path = new PropertyPath("(0)", ElementHelper.CornerRadiusProperty),
                Source = target
            });

            var grid = new Grid();
            if (target is FrameworkElement fe)
            {
                grid.HorizontalAlignment = fe.HorizontalAlignment;
                grid.VerticalAlignment = fe.VerticalAlignment;
                grid.Margin = fe.Margin;

                if (parent is Grid parentGrid)
                {
                    Grid.SetRow(grid, Grid.GetRow(fe));
                    Grid.SetColumn(grid, Grid.GetColumn(fe));
                    Grid.SetRowSpan(grid, Grid.GetRowSpan(fe));
                    Grid.SetColumnSpan(grid, Grid.GetColumnSpan(fe));
                }

                if (!double.IsNaN(fe.Width)) grid.Width = fe.Width;
                if (!double.IsNaN(fe.Height)) grid.Height = fe.Height;
                if (!double.IsNaN(fe.MinWidth)) grid.MinWidth = fe.MinWidth;
                if (!double.IsNaN(fe.MinHeight)) grid.MinHeight = fe.MinHeight;
                if (!double.IsNaN(fe.MaxWidth)) grid.MaxWidth = fe.MaxWidth;
                if (!double.IsNaN(fe.MaxHeight)) grid.MaxHeight = fe.MaxHeight;

                fe.HorizontalAlignment = HorizontalAlignment.Stretch;
                fe.VerticalAlignment = VerticalAlignment.Stretch;
                fe.Margin = new Thickness();
            }
            _wrapperMap[target] = grid;
            _loadingMap[target] = mask2;
            if (cornerRadius2 != new CornerRadius(0))
            {
                var clip = new RectangleGeometry
                {
                    Rect = new Rect(0, 0, grid.ActualWidth, grid.ActualHeight),
                    RadiusX = cornerRadius2.TopLeft,
                    RadiusY = cornerRadius2.TopLeft
                };
                grid.Clip = clip;
            }
            grid.SizeChanged += (s, e) =>
            {
                var cr = ElementHelper.GetCornerRadius(target);
                var clip = grid.Clip as RectangleGeometry;
                if (cr != new CornerRadius(0))
                {
                    if (clip == null)
                    {
                        clip = new RectangleGeometry();
                        grid.Clip = clip;
                    }
                    clip.Rect = new Rect(0, 0, grid.ActualWidth, grid.ActualHeight);
                    clip.RadiusX = cr.TopLeft;
                    clip.RadiusY = cr.TopLeft;
                }
                else if (clip != null)
                {
                    grid.Clip = null;
                }
            };

            if (parent is Panel panel)
            {
                var index = panel.Children.IndexOf(target);
                if (index < 0) return;
                panel.Children.RemoveAt(index);
                grid.Children.Add(target);
                grid.Children.Add(mask2);
                panel.Children.Insert(index, grid);
            }
            else if (parent is Decorator decorator)
            {
                if (decorator.Child != target) return;
                decorator.Child = null;
                grid.Children.Add(target);
                grid.Children.Add(mask2);
                decorator.Child = grid;
            }
            else if (parent is ContentControl contentControl)
            {
                if (contentControl.Content != target) return;
                contentControl.Content = null;
                grid.Children.Add(target);
                grid.Children.Add(mask2);
                contentControl.Content = grid;
            }
            else
            {
                _wrapperMap.Remove(target);
                _loadingMap.Remove(target);
                return;
            }
        }

        private static void UnwrapElement(UIElement target)
        {
            if (_adornerMap.TryGetValue(target, out var adorner))
            {
                var layer = AdornerLayer.GetAdornerLayer(target);
                if (layer != null)
                    layer.Remove(adorner);
                _adornerMap.Remove(target);
                _loadingMap.Remove(target);
                return;
            }

            if (!_wrapperMap.TryGetValue(target, out var grid))
                return;

            var parent = VisualTreeHelper.GetParent(grid);
            if (parent == null)
                return;

            grid.Children.Remove(target);

            if (target is FrameworkElement fe)
            {
                fe.HorizontalAlignment = grid.HorizontalAlignment;
                fe.VerticalAlignment = grid.VerticalAlignment;
                fe.Margin = grid.Margin;
                if (!double.IsNaN(grid.Width)) fe.Width = grid.Width;
                if (!double.IsNaN(grid.Height)) fe.Height = grid.Height;
                if (!double.IsNaN(grid.MinWidth)) fe.MinWidth = grid.MinWidth;
                if (!double.IsNaN(grid.MinHeight)) fe.MinHeight = grid.MinHeight;
                if (!double.IsNaN(grid.MaxWidth)) fe.MaxWidth = grid.MaxWidth;
                if (!double.IsNaN(grid.MaxHeight)) fe.MaxHeight = grid.MaxHeight;

                if (parent is Grid parentGrid)
                {
                    Grid.SetRow(fe, Grid.GetRow(grid));
                    Grid.SetColumn(fe, Grid.GetColumn(grid));
                    Grid.SetRowSpan(fe, Grid.GetRowSpan(grid));
                    Grid.SetColumnSpan(fe, Grid.GetColumnSpan(grid));
                }
            }

            if (parent is Panel panel)
            {
                var index = panel.Children.IndexOf(grid);
                if (index < 0) return;
                panel.Children.RemoveAt(index);
                panel.Children.Insert(index, target);
            }
            else if (parent is Decorator decorator)
            {
                if (decorator.Child != grid) return;
                decorator.Child = target;
            }
            else if (parent is ContentControl contentControl)
            {
                if (contentControl.Content != grid) return;
                contentControl.Content = target;
            }

            _wrapperMap.Remove(target);
            _loadingMap.Remove(target);
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

    internal class LoadingAdorner : Adorner
    {
        private readonly UIElement _overlay;

        public LoadingAdorner(UIElement adornedElement, UIElement overlay)
            : base(adornedElement)
        {
            _overlay = overlay;
            AddVisualChild(_overlay);
            AddLogicalChild(_overlay);
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int childIndex)
        {
            if (childIndex == 0) return _overlay;
            throw new ArgumentOutOfRangeException(nameof(childIndex));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _overlay.Measure(AdornedElement.RenderSize);
            return AdornedElement.RenderSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _overlay.Arrange(new Rect(finalSize));
            return finalSize;
        }
    }
}
