using System;
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
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    [ContentProperty("Children")]
    [TemplatePart(Name = PARTContentName, Type = typeof(Canvas))]
    [TemplatePart(Name = PARTDotsName, Type = typeof(ItemsControl))]
    [TemplatePart(Name = PARTPrevButtonName, Type = typeof(Button))]
    [TemplatePart(Name = PARTNextButtonName, Type = typeof(Button))]
    public class Carousel : CarouselBase, IAddChild
    {
        private const string PARTContentName = "PART_Content";
        private const string PARTDotsName = "PART_Dots";
        private const string PARTPrevButtonName = "PART_PrevButton";
        private const string PARTNextButtonName = "PART_NextButton";

        private static readonly Type _typeofSelf = typeof(Carousel);

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

        private readonly ObservableCollection<int> _dotsItems = new ObservableCollection<int>();

        public ObservableCollection<int> DotsItems => _dotsItems;

        private Canvas _contentCanvas;
        private ItemsControl _dotsPanel;
        private Button _prevButton;
        private Button _nextButton;
        private bool _isAnimating;
        private double _slideWidth;
        private int _itemCount;
        private TranslateTransform _translateTransform;
        private readonly List<object> _cachedItems = new List<object>();

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
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<object> Children { get; } = new List<object>();

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

        protected override void OnItemsSourceChangedCore(object oldValue, object newValue)
        {
            BuildSlides();
            UpdateItems();
        }

        protected override void OnSelectedIndexChangedCore(int oldIndex, int newIndex)
        {
            if (_translateTransform != null && _slideWidth > 0)
            {
                var expectedOffset = -newIndex * _slideWidth;
                if (Math.Abs(_translateTransform.X - expectedOffset) < 1)
                    return;
            }
            AnimateToIndex(newIndex);
        }

        protected override void OnItemsSourceCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            BuildSlides();
            UpdateItems();
        }

        protected override void OnAutoPlayTick()
        {
            GoToNext();
        }

        private void Carousel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BuildSlides();
        }

        private void HandleSlideClick(Point pos)
        {
            if (_slideWidth <= 0 || _itemCount == 0 || _contentCanvas == null) return;

            var index = (int)(pos.X / _slideWidth);
            if (index < 0 || index >= _itemCount) return;

            var item = _cachedItems[index];

            SelectedItem = item;

            RaiseItemClick(item);
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

        private static void OnItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (Carousel)d;
            ctrl.BuildSlides();
            ctrl.UpdateItems();
        }

        private void BuildSlides()
        {
            if (_contentCanvas == null) return;

            _contentCanvas.Children.Clear();
            _cachedItems.Clear();
            if (Children.Count > 0)
                _cachedItems.AddRange(Children);
            else if (ItemsSource != null)
                foreach (var item in ItemsSource)
                    _cachedItems.Add(item);

            _itemCount = _cachedItems.Count;
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
                var content = _cachedItems[i];
                FrameworkElement fe = CreateSlideElement(content, width, height);
                if (fe == null) continue;
                Canvas.SetLeft(fe, i * width);
                Canvas.SetTop(fe, 0);
                fe.Tag = content;
                _contentCanvas.Children.Add(fe);
            }
            if (_itemCount > 1)
            {
                var firstContent = _cachedItems[0];
                FrameworkElement clone = CreateCloneElement(firstContent, width, height);
                if (clone != null)
                {
                    Canvas.SetLeft(clone, _itemCount * width);
                    Canvas.SetTop(clone, 0);
                    _contentCanvas.Children.Add(clone);
                }
            }

            AnimateToIndex(SelectedIndex, false);
        }

        private FrameworkElement CreateCloneElement(object content, double width, double height)
        {
            if (content is Image img && img.Source != null)
            {
                return new Image
                {
                    Source = img.Source,
                    Width = width,
                    Height = height
                };
            }
            if (content is FrameworkElement element)
            {
                var brush = new VisualBrush(element);
                return new Border
                {
                    Background = brush,
                    Width = width,
                    Height = height
                };
            }
            return CreateSlideElement(content, width, height);
        }

        private FrameworkElement CreateSlideElement(object content, double width, double height)
        {
            FrameworkElement fe;
            if (ItemTemplate != null)
            {
                return new ContentPresenter
                {
                    Content = content,
                    ContentTemplate = ItemTemplate,
                    Width = width,
                    Height = height
                };
            }

            if (content is FrameworkElement element)
            {
                element.Width = width;
                element.Height = height;
                return element;
            }

            if (content is string uri)
            {
                var bitmap = ControlsHelper.CreateBitmapImage(uri, (int)width, (int)height);
                return new Image
                {
                    Source = bitmap,
                    Stretch = Stretch.UniformToFill,
                    Width = width,
                    Height = height
                };
            }

            if (!string.IsNullOrEmpty(DisplayMemberPath))
            {
                var imageUrl = GetDisplayValue(content, DisplayMemberPath);
                var bitmap = ControlsHelper.CreateBitmapImage(imageUrl, (int)width, (int)height);
                return new Image
                {
                    Source = bitmap,
                    Stretch = Stretch.UniformToFill,
                    Width = width,
                    Height = height
                };
            }

            return new ContentPresenter
            {
                Content = content,
                Width = width,
                Height = height
            };
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
    }
}
