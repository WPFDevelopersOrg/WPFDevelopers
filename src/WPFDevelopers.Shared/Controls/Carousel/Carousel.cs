using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    [ContentProperty("Children")]
    [TemplatePart(Name = PARTContentName, Type = typeof(Canvas))]
    [TemplatePart(Name = PARTDotsName, Type = typeof(ItemsControl))]
    [TemplatePart(Name = PARTPrevButtonName, Type = typeof(Button))]
    [TemplatePart(Name = PARTNextButtonName, Type = typeof(Button))]
    public class Carousel : Control, IAddChild
    {
        private const string PARTContentName = "PART_Content";
        private const string PARTDotsName = "PART_Dots";
        private const string PARTPrevButtonName = "PART_PrevButton";
        private const string PARTNextButtonName = "PART_NextButton";

        private static readonly Type _typeofSelf = typeof(Carousel);

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), _typeofSelf,
                new PropertyMetadata(null, OnItemsSourceChanged));

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(nameof(SelectedIndex), typeof(int), _typeofSelf,
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedIndexChanged));

        public static readonly DependencyProperty AutoPlayProperty =
            DependencyProperty.Register(nameof(AutoPlay), typeof(bool), _typeofSelf,
                new PropertyMetadata(true, OnAutoPlayChanged));

        public static readonly DependencyProperty AutoPlayIntervalProperty =
            DependencyProperty.Register(nameof(AutoPlayInterval), typeof(TimeSpan), _typeofSelf,
                new PropertyMetadata(TimeSpan.FromSeconds(3)));

        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register(nameof(AnimationDuration), typeof(double), _typeofSelf,
                new PropertyMetadata(0.5));

        public static readonly DependencyProperty ShowDotsProperty =
            DependencyProperty.Register(nameof(ShowDots), typeof(bool), _typeofSelf,
                new PropertyMetadata(true));

        public static readonly DependencyProperty ShowArrowsProperty =
            DependencyProperty.Register(nameof(ShowArrows), typeof(bool), _typeofSelf,
                new PropertyMetadata(true));

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), _typeofSelf,
                new PropertyMetadata(null, OnItemTemplateChanged));

        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(nameof(DisplayMemberPath), typeof(string), _typeofSelf,
                new PropertyMetadata(null, OnDisplayMemberPathChanged));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), _typeofSelf,
                new FrameworkPropertyMetadata(null, OnSelectedItemChanged));

        public static readonly RoutedEvent SelectedItemChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(SelectedItemChanged), RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<object>), _typeofSelf);

        public static readonly RoutedEvent ItemClickEvent =
            EventManager.RegisterRoutedEvent(nameof(ItemClick), RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), _typeofSelf);

        public static readonly DependencyProperty ItemClickCommandProperty =
            DependencyProperty.Register(nameof(ItemClickCommand), typeof(ICommand), _typeofSelf,
                new PropertyMetadata(null));

        private readonly ObservableCollection<int> _dotsItems = new ObservableCollection<int>();

        public ObservableCollection<int> DotsItems => _dotsItems;

        private Canvas _contentCanvas;
        private ItemsControl _dotsPanel;
        private Button _prevButton;
        private Button _nextButton;
        private DispatcherTimer _autoPlayTimer;
        private bool _isAnimating;
        private double _slideWidth;
        private int _itemCount;
        private TranslateTransform _translateTransform;

        static Carousel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(_typeofSelf,
                new FrameworkPropertyMetadata(_typeofSelf));
        }

        public Carousel()
        {
            Loaded += Carousel_Loaded;
            SizeChanged += Carousel_SizeChanged;
        }

        private void Carousel_Loaded(object sender, RoutedEventArgs e)
        {
            BuildSlides();
            UpdateItems();
            StartAutoPlay();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<object> Children { get; } = new List<object>();

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        public bool AutoPlay
        {
            get => (bool)GetValue(AutoPlayProperty);
            set => SetValue(AutoPlayProperty, value);
        }

        public TimeSpan AutoPlayInterval
        {
            get => (TimeSpan)GetValue(AutoPlayIntervalProperty);
            set => SetValue(AutoPlayIntervalProperty, value);
        }

        public double AnimationDuration
        {
            get => (double)GetValue(AnimationDurationProperty);
            set => SetValue(AnimationDurationProperty, value);
        }

        public bool ShowDots
        {
            get => (bool)GetValue(ShowDotsProperty);
            set => SetValue(ShowDotsProperty, value);
        }

        public bool ShowArrows
        {
            get => (bool)GetValue(ShowArrowsProperty);
            set => SetValue(ShowArrowsProperty, value);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public string DisplayMemberPath
        {
            get => (string)GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public event RoutedPropertyChangedEventHandler<object> SelectedItemChanged
        {
            add => AddHandler(SelectedItemChangedEvent, value);
            remove => RemoveHandler(SelectedItemChangedEvent, value);
        }

        public event RoutedEventHandler ItemClick
        {
            add => AddHandler(ItemClickEvent, value);
            remove => RemoveHandler(ItemClickEvent, value);
        }

        public ICommand ItemClickCommand
        {
            get => (ICommand)GetValue(ItemClickCommandProperty);
            set => SetValue(ItemClickCommandProperty, value);
        }

        void IAddChild.AddChild(object value)
        {
            if (value != null)
                Children.Add(value);
        }

        void IAddChild.AddText(string text)
        {
            if (!string.IsNullOrEmpty(text))
                Children.Add(text);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _contentCanvas = GetTemplateChild(PARTContentName) as Canvas;
            _dotsPanel = GetTemplateChild(PARTDotsName) as ItemsControl;
            _prevButton = GetTemplateChild(PARTPrevButtonName) as Button;
            _nextButton = GetTemplateChild(PARTNextButtonName) as Button;

            if (_prevButton != null)
                _prevButton.Click += (s, e) => GoToPrevious();
            if (_nextButton != null)
                _nextButton.Click += (s, e) => GoToNext();
            if (_dotsPanel != null)
                _dotsPanel.PreviewMouseLeftButtonDown += DotsPanel_PreviewMouseLeftButtonDown;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            HandleSlideClick(e.GetPosition(_contentCanvas));
        }

        protected override void OnTouchUp(TouchEventArgs e)
        {
            base.OnTouchUp(e);
            HandleSlideClick(e.GetTouchPoint(_contentCanvas).Position);
        }

        private void HandleSlideClick(Point pos)
        {
            if (_slideWidth <= 0 || _itemCount == 0 || _contentCanvas == null) return;

            var index = (int)(pos.X / _slideWidth);
            if (index < 0 || index >= _itemCount) return;

            var item = GetItems()[index];

            var clickArgs = new RoutedEventArgs(ItemClickEvent, item);
            RaiseEvent(clickArgs);

            SelectedItem = item;

            if (ItemClickCommand?.CanExecute(item) == true)
                ItemClickCommand.Execute(item);
        }

        private void DotsPanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is DependencyObject clicked && _dotsPanel != null)
            {
                var container = _dotsPanel.ContainerFromElement(clicked);
                if (container != null)
                {
                    var idx = _dotsPanel.ItemContainerGenerator.IndexFromContainer(container);
                    if (idx >= 0) SelectedIndex = idx;
                }
            }
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (Carousel)d;
            if (e.OldValue is INotifyCollectionChanged oldNcc)
                oldNcc.CollectionChanged -= ctrl.Items_CollectionChanged;
            if (e.NewValue is INotifyCollectionChanged newNcc)
                newNcc.CollectionChanged += ctrl.Items_CollectionChanged;
            ctrl.BuildSlides();
            ctrl.UpdateItems();
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (Carousel)d;
            var newIndex = (int)e.NewValue;
            if (ctrl._translateTransform != null && ctrl._slideWidth > 0)
            {
                var expectedOffset = -newIndex * ctrl._slideWidth;
                if (Math.Abs(ctrl._translateTransform.X - expectedOffset) < 1)
                    return;
            }
            ctrl.AnimateToIndex(newIndex);
        }

        private static void OnAutoPlayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (Carousel)d;
            if ((bool)e.NewValue)
                ctrl.StartAutoPlay();
            else
                ctrl.StopAutoPlay();
        }

        private static void OnItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (Carousel)d;
            ctrl.BuildSlides();
            ctrl.UpdateItems();
        }

        private static void OnDisplayMemberPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (Carousel)d;
            ctrl.BuildSlides();
            ctrl.UpdateItems();
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (Carousel)d;
            var args = new RoutedPropertyChangedEventArgs<object>(e.OldValue, e.NewValue, SelectedItemChangedEvent);
            ctrl.RaiseEvent(args);
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            BuildSlides();
            UpdateItems();
        }

        private void Carousel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_slideWidth > 0)
            {
                BuildSlides();
            }
        }

        private void BuildSlides()
        {
            if (_contentCanvas == null) return;
            _contentCanvas.Children.Clear();
            var items = GetItems();
            _itemCount = items.Count;
            if (_itemCount == 0) return;
            double width = ActualWidth;
            double height = ActualHeight;
            if (width <= 0 || double.IsNaN(width)) width = 800;
            if (height <= 0 || double.IsNaN(height)) height = 300;
            _slideWidth = width;
            _translateTransform = new TranslateTransform();
            _contentCanvas.RenderTransform = _translateTransform;
            for (int i = 0; i < _itemCount; i++)
            {
                var content = items[i];
                FrameworkElement fe;

                if (ItemTemplate != null)
                {
                    fe = new ContentPresenter
                    {
                        Content = content,
                        ContentTemplate = ItemTemplate,
                        Width = width,
                        Height = height
                    };
                }
                else if (content is FrameworkElement element)
                {
                    fe = element;
                    fe.Width = width;
                    fe.Height = height;
                }
                else if (content is string uri)
                {
                    var bitmap = ControlsHelper.CreateBitmapImage(uri, (int)width, (int)height);
                    fe = new Image
                    {
                        Source = bitmap,
                        Stretch = Stretch.UniformToFill,
                        Width = width,
                        Height = height
                    };
                }
                else if (!string.IsNullOrEmpty(DisplayMemberPath))
                {
                    var imageUrl = GetDisplayValue(content, DisplayMemberPath);
                    var bitmap = ControlsHelper.CreateBitmapImage(imageUrl, (int)width, (int)height);
                    fe = new Image
                    {
                        Source = bitmap,
                        Stretch = Stretch.UniformToFill,
                        Width = width,
                        Height = height
                    };
                }
                else
                {
                    fe = new ContentPresenter
                    {
                        Content = content,
                        Width = width,
                        Height = height
                    };
                }
                Canvas.SetLeft(fe, i * width);
                Canvas.SetTop(fe, 0);
                fe.Tag = content;
                _contentCanvas.Children.Add(fe);
            }
            if (_itemCount > 1)
            {
                var firstItem = items[0];
                FrameworkElement clone;

                if (ItemTemplate != null)
                {
                    clone = new ContentPresenter
                    {
                        Content = firstItem,
                        ContentTemplate = ItemTemplate,
                        Width = width,
                        Height = height
                    };
                }
                else if (firstItem is FrameworkElement element)
                {
                    var brush = new VisualBrush(element);
                    brush.Stretch = Stretch.UniformToFill;
                    clone = new Border
                    {
                        Background = brush,
                        Width = width,
                        Height = height
                    };
                }
                else if (firstItem is string uri)
                {
                    var bitmap = ControlsHelper.CreateBitmapImage(uri, (int)width, (int)height);
                    clone = new Image
                    {
                        Source = bitmap,
                        Stretch = Stretch.UniformToFill,
                        Width = width,
                        Height = height
                    };
                }
                else if (!string.IsNullOrEmpty(DisplayMemberPath))
                {
                    var imageUrl = GetDisplayValue(firstItem, DisplayMemberPath);
                    var bitmap = ControlsHelper.CreateBitmapImage(imageUrl, (int)width, (int)height);
                    clone = new Image
                    {
                        Source = bitmap,
                        Stretch = Stretch.UniformToFill,
                        Width = width,
                        Height = height
                    };
                }
                else
                {
                    clone = new ContentPresenter
                    {
                        Content = firstItem,
                        Width = width,
                        Height = height
                    };
                }
                Canvas.SetLeft(clone, _itemCount * width);
                Canvas.SetTop(clone, 0);
                _contentCanvas.Children.Add(clone);
            }

            AnimateToIndex(SelectedIndex, false);
        }

        private void UpdateItems()
        {
            _dotsItems.Clear();
            for (int i = 0; i < _itemCount; i++)
                _dotsItems.Add(i);
        }

        private void AnimateToIndex(int index, bool animate = true)
        {
            if (_contentCanvas == null) return;

            if (_itemCount == 0) return;

            index = Math.Max(0, Math.Min(index, _itemCount - 1));

            double offset = -index * _slideWidth;
            if (_slideWidth <= 0) return;

            if (_translateTransform == null) return;

            if (animate)
            {
                _isAnimating = true;

                var animation = new DoubleAnimation
                {
                    From = _translateTransform.X,
                    To = offset,
                    Duration = TimeSpan.FromSeconds(AnimationDuration),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
                animation.Completed += (s, e) =>
                {
                    _isAnimating = false;
                    double cloneOffset = -_itemCount * _slideWidth;
                    if (Math.Abs(_translateTransform.X - cloneOffset) < 1)
                    {
                        _translateTransform.BeginAnimation(TranslateTransform.XProperty, null);
                        _translateTransform.X = 0;
                    }
                };
                _translateTransform.BeginAnimation(TranslateTransform.XProperty, animation);
            }
            else
            {
                _translateTransform.X = offset;
            }
        }

        public void GoToNext()
        {
            if (_isAnimating) return;

            if (_itemCount == 0) return;

            int next = SelectedIndex + 1;
            if (next >= _itemCount)
            {
                double cloneOffset = -_itemCount * _slideWidth;
                _isAnimating = true;

                var animation = new DoubleAnimation
                {
                    From = _translateTransform.X,
                    To = cloneOffset,
                    Duration = TimeSpan.FromSeconds(AnimationDuration),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
                animation.Completed += (s, e) =>
                {
                    _isAnimating = false;
                    _translateTransform.BeginAnimation(TranslateTransform.XProperty, null);
                    _translateTransform.X = 0;
                    SelectedIndex = 0;
                };
                _translateTransform.BeginAnimation(TranslateTransform.XProperty, animation);
            }
            else
            {
                SelectedIndex = next;
            }
        }

        public void GoToPrevious()
        {
            if (_isAnimating) return;

            if (_itemCount == 0) return;

            int prev = SelectedIndex - 1;
            if (prev < 0) prev = _itemCount - 1;
            SelectedIndex = prev;
        }

        private void StartAutoPlay()
        {
            if (!AutoPlay) return;

            StopAutoPlay();
            _autoPlayTimer = new DispatcherTimer
            {
                Interval = AutoPlayInterval
            };
            _autoPlayTimer.Tick += (s, e) => GoToNext();
            _autoPlayTimer.Start();
        }

        private void StopAutoPlay()
        {
            _autoPlayTimer?.Stop();
            _autoPlayTimer = null;
        }

        private IList<object> GetItems()
        {
            var list = new List<object>();
            if (Children.Count > 0)
            {
                list.AddRange(Children);
            }
            else if (ItemsSource != null)
            {
                foreach (var item in ItemsSource)
                    list.Add(item);
            }
            return list;
        }

        private string GetDisplayValue(object item, string propertyPath)
        {
            if (item == null || string.IsNullOrEmpty(propertyPath))
                return null;
            var prop = TypeDescriptor.GetProperties(item).Find(propertyPath, true);
            return prop?.GetValue(item)?.ToString();
        }
    }
}
