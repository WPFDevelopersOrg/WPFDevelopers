using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WPFDevelopers.Controls
{
    [DefaultProperty("Items")]
    [ContentProperty(nameof(Items))]
    [TemplatePart(Name = IndicatorTemplateName, Type = typeof(Border))]
    [TemplatePart(Name = IndicatorTransformTemplateName, Type = typeof(TranslateTransform))]
    [TemplatePart(Name = ItemsPresenterTemplateName, Type = typeof(FrameworkElement))]
    public class Segmented : ItemsControlBase
    {
        private const string IndicatorTemplateName = "PART_Indicator";
        private const string IndicatorTransformTemplateName = "PART_IndicatorTransform";
        private const string ItemsPresenterTemplateName = "PART_ItemsPresenter";

        private static readonly Type _typeofSelf = typeof(Segmented);

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), _typeofSelf,
                new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

        public static readonly DependencyProperty IndicatorBrushProperty =
            DependencyProperty.Register(nameof(IndicatorBrush), typeof(Brush), _typeofSelf,
                new PropertyMetadata(null));

        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register(nameof(AnimationDuration), typeof(double), _typeofSelf,
                new PropertyMetadata(0.25));

        private readonly string _groupName = Guid.NewGuid().ToString("N");

        private Border _indicator;
        private TranslateTransform _indicatorTransform;
        private FrameworkElement _itemsPresenter;

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public Brush IndicatorBrush
        {
            get => (Brush)GetValue(IndicatorBrushProperty);
            set => SetValue(IndicatorBrushProperty, value);
        }

        public double AnimationDuration
        {
            get => (double)GetValue(AnimationDurationProperty);
            set => SetValue(AnimationDurationProperty, value);
        }

        public Segmented()
        {
            ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        protected override DependencyObject GetContainerForItemOverride()
            => new SegmentedItem { GroupName = _groupName };

        protected override bool IsItemItsOwnContainerOverride(object item)
            => item is SegmentedItem;

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (element is SegmentedItem container)
            {
                container.GroupName = _groupName;
                container.Checked += Container_Checked;

                if (item is SegmentedItem) return;

                if (!string.IsNullOrEmpty(DisplayMemberPath))
                {
                    var binding = new Binding(DisplayMemberPath) { Source = item };
                    container.SetBinding(ContentControl.ContentProperty, binding);
                }
                else
                {
                    container.Content = item;
                }
            }
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is SegmentedItem container)
                container.Checked -= Container_Checked;
            base.ClearContainerForItemOverride(element, item);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _indicator = GetTemplateChild(IndicatorTemplateName) as Border;
            _indicatorTransform = GetTemplateChild(IndicatorTransformTemplateName) as TranslateTransform;
            _itemsPresenter = GetTemplateChild(ItemsPresenterTemplateName) as FrameworkElement;
            SizeChanged += Segmented_SizeChanged;
        }

        private void Segmented_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (SelectedIndex >= 0)
                AnimateIndicator(SelectedIndex, false);
        }

        protected override void OnSelectedIndexChanged(int oldIndex, int newIndex)
        {
            base.OnSelectedIndexChanged(oldIndex, newIndex);
            Dispatcher.BeginInvoke(new Action(() => AnimateIndicator(newIndex)),
                System.Windows.Threading.DispatcherPriority.Render);
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (Segmented)d;
            if (ctrl.SelectedIndex >= 0)
                ctrl.Dispatcher.BeginInvoke(new Action(() => ctrl.AnimateIndicator(ctrl.SelectedIndex, false)),
                    System.Windows.Threading.DispatcherPriority.Render);
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                SyncSelectedIndexToContainer();
                Dispatcher.BeginInvoke(new Action(() => AnimateIndicator(SelectedIndex, false)),
                    System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        private void Container_Checked(object sender, RoutedEventArgs e)
        {
            if (_isSyncing) return;
            if (!(sender is SegmentedItem container)) return;

            var idx = ItemContainerGenerator.IndexFromContainer(container);
            if (idx >= 0)
            {
                SyncSelectedIndex(idx);
                Dispatcher.BeginInvoke(new Action(() => AnimateIndicator(idx)),
                    System.Windows.Threading.DispatcherPriority.Render);
                RaiseItemClick(container.Content);
            }
        }

        private void SyncSelectedIndexToContainer()
        {
            var index = SelectedIndex;
            if (index < 0 || index >= Items.Count) return;

            if (ItemContainerGenerator.ContainerFromIndex(index) is SegmentedItem container)
            {
                _isSyncing = true;
                try
                {
                    container.IsChecked = true;
                }
                finally
                {
                    _isSyncing = false;
                }
            }
        }

        private void AnimateIndicator(int index, bool animate = true)
        {
            if (_indicator == null || _indicatorTransform == null || _itemsPresenter == null) return;
            if (index < 0 || index >= Items.Count) return;

            var container = ItemContainerGenerator.ContainerFromIndex(index) as FrameworkElement;
            if (container == null || container.ActualWidth <= 0) return;

            var pos = container.TranslatePoint(new Point(0, 0), _itemsPresenter);

            if (Orientation == Orientation.Horizontal)
            {
                _indicator.Width = container.ActualWidth;
                _indicator.Height = double.NaN;
                _indicator.HorizontalAlignment = HorizontalAlignment.Left;
                _indicator.VerticalAlignment = VerticalAlignment.Stretch;
                if (animate)
                {
                    var anim = new DoubleAnimation(pos.X, TimeSpan.FromSeconds(AnimationDuration))
                    {
                        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                    };
                    _indicatorTransform.BeginAnimation(TranslateTransform.XProperty, anim);
                }
                else
                {
                    _indicatorTransform.X = pos.X;
                }
            }
            else
            {
                _indicator.Width = _itemsPresenter.ActualWidth;
                _indicator.Height = container.ActualHeight;
                _indicator.HorizontalAlignment = HorizontalAlignment.Stretch;
                _indicator.VerticalAlignment = VerticalAlignment.Top;
                if (animate)
                {
                    var anim = new DoubleAnimation(pos.Y, TimeSpan.FromSeconds(AnimationDuration))
                    {
                        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                    };
                    _indicatorTransform.BeginAnimation(TranslateTransform.YProperty, anim);
                }
                else
                {
                    _indicatorTransform.Y = pos.Y;
                }
            }
        }
    }
}
