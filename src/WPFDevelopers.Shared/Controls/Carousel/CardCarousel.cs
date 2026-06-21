using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    internal enum CarouselLoacation
    {
        Left,
        Right,
        Center
    }

    internal enum CarouselZIndex
    {
        Left = 20,
        Center = 30,
        Right = 20,
        LeftMask = 40,
        RightMask = 40
    }


    [DefaultProperty("Children")]
    [ContentProperty("Children")]
    [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    [TemplatePart(Name = PARTContentDockName, Type = typeof(Canvas))]
    [TemplatePart(Name = PARTPrevButtonName, Type = typeof(Button))]
    [TemplatePart(Name = PARTNextButtonName, Type = typeof(Button))]
    [TemplatePart(Name = PARTDotsName, Type = typeof(ItemsControl))]
    public class CardCarousel : CarouselBase, IAddChild
    {
        private const string PARTContentDockName = "PART_ContentDock";
        private const string PARTDotsName = "PART_Dots";
        private const string PARTPrevButtonName = "PART_PrevButton";
        private const string PARTNextButtonName = "PART_NextButton";

        public static readonly DependencyProperty ShowDotsProperty =
            DependencyProperty.Register(nameof(ShowDots), typeof(bool), typeof(CardCarousel),
                new PropertyMetadata(true));

        public static readonly DependencyProperty ShowArrowsProperty =
            DependencyProperty.Register(nameof(ShowArrows), typeof(bool), typeof(CardCarousel),
                new PropertyMetadata(false));

        private readonly ObservableCollection<int> _dotsItems = new ObservableCollection<int>();

        public ObservableCollection<int> DotsItems => _dotsItems;

        static CardCarousel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CardCarousel),
                new FrameworkPropertyMetadata(typeof(CardCarousel)));
        }

        public CardCarousel()
        {
            Loaded += CardCarousel_Loaded;
            SizeChanged += CardCarousel_SizeChanged;
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<object> Children { get; } = new List<object>();

        public void AddChild(object value)
        {
            throw new NotImplementedException();
        }

        public void AddText(string text)
        {
            throw new NotImplementedException();
        }

        #region override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _contentDock = GetTemplateChild(PARTContentDockName) as Canvas;
            _prevButton = GetTemplateChild(PARTPrevButtonName) as Button;
            _nextButton = GetTemplateChild(PARTNextButtonName) as Button;
            _dots = GetTemplateChild(PARTDotsName) as ItemsControl;

            if (_prevButton != null)
                _prevButton.Click += (s, e) => GoToPrevious();
            if (_nextButton != null)
                _nextButton.Click += (s, e) => GoToNext();
            if (_dots != null)
                _dots.PreviewMouseLeftButtonDown += Dots_PreviewMouseLeftButtonDown;
        }

        #endregion

        protected override void OnItemsSourceChangedCore(object oldValue, object newValue)
        {
            BuildSlides();
        }

        protected override void OnItemsSourceCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            BuildSlides();
        }

        protected override void OnSelectedIndexChangedCore(int oldIndex, int newIndex)
        {
            PlayCarouselWithIndex(newIndex);
        }

        protected override void OnAutoPlayTick()
        {
            PlayCarouselRightToLeft();
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            HandleSlideClick(e.GetPosition(_contentDock));
        }

        protected override void OnTouchUp(TouchEventArgs e)
        {
            base.OnTouchUp(e);
            HandleSlideClick(e.GetTouchPoint(_contentDock).Position);
        }

        private void HandleSlideClick(Point pos)
        {
            if (_contentDock == null || _carouselSize == 0) return;

            var hit = _contentDock.InputHitTest(pos);
            if (hit is FrameworkElement fe)
            {
                if (_mapFrameworkes.ContainsValue(fe))
                {
                    foreach (var kvp in _mapFrameworkes)
                    {
                        if (kvp.Value == fe)
                        {
                            var item = _mapResources[kvp.Key];
                            SelectedItem = item;
                            RaiseItemClick(item);
                            return;
                        }
                    }
                }
            }
            else if (hit is DependencyObject depObj)
            {
                var element = depObj as FrameworkElement;
                while (element != null)
                {
                    if (ReferenceEquals(element.Parent, _contentDock))
                    {
                        foreach (var kvp in _mapFrameworkes)
                        {
                            if (kvp.Value == element)
                            {
                                var item = _mapResources[kvp.Key];
                                SelectedItem = item;
                                RaiseItemClick(item);
                                return;
                            }
                        }
                        break;
                    }
                    element = element.Parent as FrameworkElement;
                }
            }
        }

        private void CardCarousel_Loaded(object sender, RoutedEventArgs e)
        {
            BuildSlides();
        }

        private void CardCarousel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_carouselSize == 0)
                return;

            StopAutoPlay();
            BuildSlides();
            if (AutoPlay)
                StartAutoPlay();
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

        private FrameworkElement CreateItemElement(object content, double width, double height)
        {
            FrameworkElement fe;

            if (content is FrameworkElement element)
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
                    Width = width,
                    Height = height
                };
            }
            else if (!string.IsNullOrEmpty(DisplayMemberPath))
            {
                var imageUrl = GetDisplayValue(content, DisplayMemberPath);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    var bitmap = ControlsHelper.CreateBitmapImage(imageUrl, (int)width, (int)height);
                    if (bitmap != null)
                    {
                        fe = new Image
                        {
                            Source = bitmap,
                            Width = width,
                            Height = height
                        };
                    }
                    else
                    {
                        fe = new ContentPresenter { Content = content, Width = width, Height = height };
                    }
                }
                else
                {
                    fe = new ContentPresenter { Content = content, Width = width, Height = height };
                }
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

            return fe;
        }

        private void BuildSlides()
        {
            if (_contentDock == null) return;

            StopAutoPlay();
            _mapCarouselLocationFramewokes.Clear();
            _mapResources.Clear();
            _BufferLinkedList.Clear();

            _contentDock.Children.Clear();

            var items = GetItems();
            _carouselSize = items.Count;
            if (_carouselSize == 0) return;

            if (!TryCalculateShellProperties()) return;

            double width = _ElementWidth;
            double height = _ElementHeight;

            for (var i = 0; i < _carouselSize; i++)
            {
                var item = items[i];
                var frameworkElement = CreateItemElement(item, width, height);

                frameworkElement.RenderTransformOrigin = new Point(0.5, 1);
                var vTransformGroup = new TransformGroup
                {
                    Children =
                    {
                        new ScaleTransform { ScaleY = _ScaleRatio },
                        new SkewTransform(),
                        new RotateTransform(),
                        new TranslateTransform()
                    }
                };
                frameworkElement.RenderTransform = vTransformGroup;

                _mapResources[i] = item;
                _mapFrameworkes[i] = frameworkElement;

                _contentDock.Children.Add(frameworkElement);

                if (i == 0)
                {
                    var vScaleTransform = vTransformGroup.Children[0] as ScaleTransform;
                    vScaleTransform.ScaleY = _ScaleRatioEx;
                    frameworkElement.SetValue(Canvas.LeftProperty, _CenterDockLeft);
                    Panel.SetZIndex(frameworkElement, (int)CarouselZIndex.Center);
                    _mapCarouselLocationFramewokes[CarouselLoacation.Center] = i;
                }
                else if (i == 1)
                {
                    frameworkElement.SetValue(Canvas.LeftProperty, _RightDockLeft);
                    Panel.SetZIndex(frameworkElement, (int)CarouselZIndex.Right);
                    _mapCarouselLocationFramewokes[CarouselLoacation.Right] = i;
                }
                else if (i == _carouselSize - 1)
                {
                    frameworkElement.SetValue(Canvas.LeftProperty, _LeftDockLeft);
                    Panel.SetZIndex(frameworkElement, (int)CarouselZIndex.Left);
                    _mapCarouselLocationFramewokes[CarouselLoacation.Left] = i;
                }
                else
                {
                    _BufferLinkedList.AddLast(i);
                    frameworkElement.SetValue(Canvas.LeftProperty, _CenterDockLeft);
                    Panel.SetZIndex(frameworkElement, i);
                }
            }

            _isLoaded = true;
            UpdateDots();
            if (AutoPlay)
                StartAutoPlay();
        }

        private bool TryCalculateShellProperties()
        {
            if (_contentDock == null)
                return false;

            var vWidth = _contentDock.ActualWidth;
            var vHeight = _contentDock.ActualHeight;

            if (vWidth == 0 || vHeight == 0)
                return false;

            _ShellWidth = vWidth;
            _ShellHeight = vHeight;
            _ElementWidth = _ShellWidth * _ElementScale;
            _ElementHeight = _ShellHeight;
            _LeftDockLeft = 0;
            _CenterDockLeft = _ShellWidth * _DockOffset;
            _RightDockLeft = _ShellWidth - _ElementWidth;

            return true;
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            _isStoryboardWorking = false;
        }

        private void Dots_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is DependencyObject clicked && _dots != null)
            {
                var container = _dots.ContainerFromElement(clicked);
                if (container != null)
                {
                    var idx = _dots.ItemContainerGenerator.IndexFromContainer(container);
                    if (idx >= 0) SelectedIndex = idx;
                }
            }
        }

        public void GoToNext()
        {
            if (_carouselSize == 0) return;
            int next = SelectedIndex + 1;
            if (next >= _carouselSize) next = 0;
            SelectedIndex = next;
        }

        public void GoToPrevious()
        {
            if (_carouselSize == 0) return;
            int prev = SelectedIndex - 1;
            if (prev < 0) prev = _carouselSize - 1;
            SelectedIndex = prev;
        }

        #region feild

        private bool _isLoaded;
        private Canvas _contentDock;
        private ItemsControl _dots;
        private Button _prevButton;
        private Button _nextButton;

        #endregion

        #region render relative

        private const double _ScaleRatio = 0.95;
        private const double _ScaleRatioEx = 1;

        private double _ShellWidth;
        private double _ShellHeight;

        private double _ElementWidth;
        private double _ElementHeight;
        private readonly double _ElementScale = 0.6;

        private double _CenterDockLeft;
        private double _LeftDockLeft;
        private double _RightDockLeft;
        private readonly double _DockOffset = 0.2;

        private int _carouselSize;

        #endregion

        #region

        private readonly Dictionary<int, object> _mapResources = new Dictionary<int, object>();
        private readonly Dictionary<int, FrameworkElement> _mapFrameworkes = new Dictionary<int, FrameworkElement>();

        private readonly Dictionary<CarouselLoacation, int> _mapCarouselLocationFramewokes =
            new Dictionary<CarouselLoacation, int>();

        private readonly LinkedList<int> _BufferLinkedList = new LinkedList<int>();

        #endregion

        #region StoryBoard

        private Storyboard _storyboard;
        private readonly double _animationTime = 0.5;
        private readonly double _delayAnimationTime = 0.7;

        private bool _isStoryboardWorking;

        #endregion


        #region 动画

        //从左边向右依次播放
        private bool PlayCarouselLeftToRight()
        {
            if (_storyboard == null)
            {
                _storyboard = new Storyboard();
                _storyboard.Completed += Storyboard_Completed;
            }

            if (_isStoryboardWorking)
                return false;

            _isStoryboardWorking = true;

            _storyboard?.Stop();
            _storyboard?.Children.Clear();

            var nNextIndex = -1;

            //右边的动画移动到中间 后层
            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLoacation.Right, -1);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);

                if (vFrameworker != null)
                {
                    var animation1 = new Int32Animation
                    {
                        To = vResult + 1,
                        Duration = TimeSpan.FromSeconds(_delayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _storyboard.Children.Add(animation1);

                    var animation2 = new DoubleAnimation
                    {
                        To = _CenterDockLeft,
                        Duration = TimeSpan.FromSeconds(_animationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _storyboard.Children.Add(animation2);

                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_animationTime),
                        To = _ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _storyboard.Children.Add(animation3);

                    _BufferLinkedList.AddFirst(vResult);
                }

                _mapCarouselLocationFramewokes[CarouselLoacation.Right] = -1;
            }

            //中间的动画移动到右边
            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLoacation.Center, -1);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);

                if (vFrameworker != null)
                {
                    var animation1 = new Int32Animation
                    {
                        To = (int)CarouselZIndex.Right,
                        Duration = TimeSpan.FromSeconds(_delayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _storyboard.Children.Add(animation1);

                    var animation2 = new DoubleAnimation
                    {
                        To = _RightDockLeft,
                        Duration = TimeSpan.FromSeconds(_animationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _storyboard.Children.Add(animation2);

                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_animationTime),
                        To = _ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _storyboard.Children.Add(animation3);

                    _mapCarouselLocationFramewokes[CarouselLoacation.Right] = vResult;
                }

                _mapCarouselLocationFramewokes[CarouselLoacation.Center] = -1;
            }

            //左边的动画移动到中间 上层
            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLoacation.Left, -1);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);

                if (vFrameworker != null)
                {
                    var animation1 = new Int32Animation
                    {
                        To = (int)CarouselZIndex.Center,
                        Duration = TimeSpan.FromSeconds(_delayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _storyboard.Children.Add(animation1);

                    var animation2 = new DoubleAnimation
                    {
                        To = _CenterDockLeft,
                        Duration = TimeSpan.FromSeconds(_animationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _storyboard.Children.Add(animation2);

                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_animationTime),
                        To = _ScaleRatioEx,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _storyboard.Children.Add(animation3);

                    _mapCarouselLocationFramewokes[CarouselLoacation.Center] = vResult;

                    SelectedIndex = vResult;

                    nNextIndex = vResult - 1;
                    if (nNextIndex < 0)
                        nNextIndex = _carouselSize - 1;
                }

                _mapCarouselLocationFramewokes[CarouselLoacation.Left] = -1;
            }

            //后层记录推送到前台左侧

            if (nNextIndex >= 0)
            {
                _BufferLinkedList.Remove(nNextIndex);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(nNextIndex);

                if (vFrameworker != null)
                {
                    var animation1 = new Int32Animation
                    {
                        To = (int)CarouselZIndex.Left,
                        Duration = TimeSpan.FromSeconds(_delayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _storyboard.Children.Add(animation1);

                    var animation2 = new DoubleAnimation
                    {
                        To = _LeftDockLeft,
                        Duration = TimeSpan.FromSeconds(_animationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _storyboard.Children.Add(animation2);

                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_animationTime),
                        To = _ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _storyboard.Children.Add(animation3);

                    _mapCarouselLocationFramewokes[CarouselLoacation.Left] = nNextIndex;
                }
            }
            else
            {
                if (_BufferLinkedList.Count > 0)
                {
                    var vResult = _BufferLinkedList.LastOrDefault();
                    _BufferLinkedList.RemoveLast();

                    var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);

                    if (vFrameworker != null)
                    {
                        var animation1 = new Int32Animation
                        {
                            To = (int)CarouselZIndex.Left,
                            Duration = TimeSpan.FromSeconds(_delayAnimationTime),
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };
                        Storyboard.SetTarget(animation1, vFrameworker);
                        Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                        _storyboard.Children.Add(animation1);

                        var animation2 = new DoubleAnimation
                        {
                            To = _LeftDockLeft,
                            Duration = TimeSpan.FromSeconds(_animationTime),
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };
                        Storyboard.SetTarget(animation2, vFrameworker);
                        Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                        _storyboard.Children.Add(animation2);

                        var animation3 = new DoubleAnimation
                        {
                            Duration = TimeSpan.FromSeconds(_animationTime),
                            To = _ScaleRatio,
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };
                        Storyboard.SetTarget(animation3, vFrameworker);
                        Storyboard.SetTargetProperty(animation3,
                            new PropertyPath(
                                "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                        _storyboard.Children.Add(animation3);

                        _mapCarouselLocationFramewokes[CarouselLoacation.Left] = vResult;
                    }
                }
            }

            _storyboard?.Begin();

            return true;
        }

        //从右向左依次播放
        private bool PlayCarouselRightToLeft()
        {
            if (_storyboard == null)
            {
                _storyboard = new Storyboard();
                _storyboard.Completed += Storyboard_Completed;
            }

            _isStoryboardWorking = true;

            _storyboard?.Stop();
            _storyboard?.Children.Clear();

            int nNextIndex = -1;

            //左边的动画移动到中间 后层
            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLoacation.Left, -1);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);

                if (vFrameworker != null)
                {
                    var animation1 = new Int32Animation
                    {
                        To = vResult + 1,
                        Duration = TimeSpan.FromSeconds(_delayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _storyboard.Children.Add(animation1);

                    var animation2 = new DoubleAnimation
                    {
                        To = _CenterDockLeft,
                        Duration = TimeSpan.FromSeconds(_animationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _storyboard.Children.Add(animation2);

                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_animationTime),
                        To = _ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _storyboard.Children.Add(animation3);

                    _BufferLinkedList.AddLast(vResult);
                }

                _mapCarouselLocationFramewokes[CarouselLoacation.Left] = -1;
            }

            //中间的动画移动到左边
            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLoacation.Center, -1);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);

                if (vFrameworker != null)
                {
                    var animation1 = new Int32Animation
                    {
                        To = (int)CarouselZIndex.Left,
                        Duration = TimeSpan.FromSeconds(_delayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _storyboard.Children.Add(animation1);

                    var animation2 = new DoubleAnimation
                    {
                        To = _LeftDockLeft,
                        Duration = TimeSpan.FromSeconds(_animationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _storyboard.Children.Add(animation2);

                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_animationTime),
                        To = _ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _storyboard.Children.Add(animation3);

                    _mapCarouselLocationFramewokes[CarouselLoacation.Left] = vResult;
                }

                _mapCarouselLocationFramewokes[CarouselLoacation.Center] = -1;
            }

            //右边的动画移动到中间
            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLoacation.Right, -1);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);

                if (vFrameworker != null)
                {
                    var animation1 = new Int32Animation
                    {
                        To = (int)CarouselZIndex.Center,
                        Duration = TimeSpan.FromSeconds(_delayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _storyboard.Children.Add(animation1);

                    var animation2 = new DoubleAnimation
                    {
                        To = _CenterDockLeft,
                        Duration = TimeSpan.FromSeconds(_animationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _storyboard.Children.Add(animation2);

                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_animationTime),
                        To = _ScaleRatioEx,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _storyboard.Children.Add(animation3);

                    _mapCarouselLocationFramewokes[CarouselLoacation.Center] = vResult;

                    SelectedIndex = vResult;

                    nNextIndex = vResult + 1;
                    if (nNextIndex >= _carouselSize)
                        nNextIndex = 0;
                }
                _mapCarouselLocationFramewokes[CarouselLoacation.Right] = -1;
            }

            //后层记录推送到前台

            if (nNextIndex >= 0)
            {
                _BufferLinkedList.Remove(nNextIndex);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(nNextIndex);

                if (vFrameworker != null)
                {
                    var animation1 = new Int32Animation
                    {
                        To = (int)CarouselZIndex.Right,
                        Duration = TimeSpan.FromSeconds(_delayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _storyboard.Children.Add(animation1);

                    var animation2 = new DoubleAnimation
                    {
                        To = _RightDockLeft,
                        Duration = TimeSpan.FromSeconds(_animationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _storyboard.Children.Add(animation2);

                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_animationTime),
                        To = _ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _storyboard.Children.Add(animation3);

                    _mapCarouselLocationFramewokes[CarouselLoacation.Right] = nNextIndex;
                }
            }
            else
            {
                if (_BufferLinkedList.Count > 0)
                {
                    var vResult = _BufferLinkedList.FirstOrDefault();
                    _BufferLinkedList.RemoveFirst();

                    var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);

                    if (vFrameworker != null)
                    {
                        var animation1 = new Int32Animation
                        {
                            To = (int)CarouselZIndex.Right,
                            Duration = TimeSpan.FromSeconds(_delayAnimationTime),
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };
                        Storyboard.SetTarget(animation1, vFrameworker);
                        Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                        _storyboard.Children.Add(animation1);

                        var animation2 = new DoubleAnimation
                        {
                            To = _RightDockLeft,
                            Duration = TimeSpan.FromSeconds(_animationTime),
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };
                        Storyboard.SetTarget(animation2, vFrameworker);
                        Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                        _storyboard.Children.Add(animation2);

                        var animation3 = new DoubleAnimation
                        {
                            Duration = TimeSpan.FromSeconds(_animationTime),
                            To = _ScaleRatio,
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };
                        Storyboard.SetTarget(animation3, vFrameworker);
                        Storyboard.SetTargetProperty(animation3,
                            new PropertyPath(
                                "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                        _storyboard.Children.Add(animation3);

                        _mapCarouselLocationFramewokes[CarouselLoacation.Right] = vResult;
                    }
                }
            }

            _storyboard?.Begin();

            return true;
        }

        void UpdateDots()
        {
            _dotsItems.Clear();
            for (int i = 0; i < _carouselSize; i++)
                _dotsItems.Add(i);
        }

        //当用户点击其中某个位置的动画时
        private bool PlayCarouselWithIndex(int nIndex)
        {
            if (nIndex < 0 || nIndex >= _carouselSize)
                return false;

            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLoacation.Center, -1);
                if (vResult == nIndex)
                    return true;
            }

            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLoacation.Left, -1);
                if (vResult == nIndex)
                    return PlayCarouselLeftToRight();
            }

            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLoacation.Right, -1);
                if (vResult == nIndex)
                    return PlayCarouselRightToLeft();
            }

            return PlayCarouselWithIndexOutRange(nIndex);
        }

        private bool PlayCarouselWithIndexOutRange(int nIndex)
        {
            if (nIndex < 0 || nIndex >= _carouselSize)
                return false;

            var vPre = nIndex - 1;
            if (vPre < 0)
                vPre = _carouselSize - 1;

            var vNext = nIndex + 1;
            if (vNext >= _carouselSize)
                vNext = 0;

            if (_storyboard == null)
            {
                _storyboard = new Storyboard();
                _storyboard.Completed += Storyboard_Completed;
            }

            if (_isStoryboardWorking)
                return false;

            _isStoryboardWorking = true;

            _storyboard?.Stop();
            _storyboard?.Children.Clear();

            _BufferLinkedList.Clear();

            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLoacation.Right, -1);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);
                if (vFrameworker != null)
                {
                    var animation1 = new Int32Animation
                    {
                        To = vResult + 1,
                        Duration = TimeSpan.FromSeconds(_delayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _storyboard.Children.Add(animation1);

                    var animation2 = new DoubleAnimation
                    {
                        To = _CenterDockLeft,
                        Duration = TimeSpan.FromSeconds(_animationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _storyboard.Children.Add(animation2);

                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_animationTime),
                        To = _ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _storyboard.Children.Add(animation3);
                }

                _mapCarouselLocationFramewokes[CarouselLoacation.Right] = -1;
            }

            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLoacation.Center, -1);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);
                if (vFrameworker != null)
                {
                    var animation1 = new Int32Animation
                    {
                        To = vResult + 1,
                        Duration = TimeSpan.FromSeconds(_delayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _storyboard.Children.Add(animation1);

                    var animation2 = new DoubleAnimation
                    {
                        To = _CenterDockLeft,
                        Duration = TimeSpan.FromSeconds(_animationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _storyboard.Children.Add(animation2);

                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_animationTime),
                        To = _ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _storyboard.Children.Add(animation3);
                }

                _mapCarouselLocationFramewokes[CarouselLoacation.Center] = -1;
            }

            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLoacation.Left, -1);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);
                if (vFrameworker != null)
                {
                    var animation1 = new Int32Animation
                    {
                        To = vResult + 1,
                        Duration = TimeSpan.FromSeconds(_delayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _storyboard.Children.Add(animation1);

                    var animation2 = new DoubleAnimation
                    {
                        To = _CenterDockLeft,
                        Duration = TimeSpan.FromSeconds(_animationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _storyboard.Children.Add(animation2);

                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_animationTime),
                        To = _ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _storyboard.Children.Add(animation3);
                }

                _mapCarouselLocationFramewokes[CarouselLoacation.Left] = -1;
            }

            for (var i = 0; i < _carouselSize; i++)
                if (i == vPre)
                {
                    if (_mapFrameworkes.ContainsKey(i))
                    {
                        var vFrameworker = _mapFrameworkes.GetValueOrDefault(i);

                        if (vFrameworker != null)
                        {
                            var animation1 = new Int32Animation
                            {
                                To = (int)CarouselZIndex.Left,
                                Duration = TimeSpan.FromSeconds(_delayAnimationTime),
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation1, vFrameworker);
                            Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                            _storyboard.Children.Add(animation1);

                            var animation2 = new DoubleAnimation
                            {
                                BeginTime = TimeSpan.FromSeconds(_delayAnimationTime),
                                To = _LeftDockLeft,
                                Duration = TimeSpan.FromSeconds(_animationTime),
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation2, vFrameworker);
                            Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                            _storyboard.Children.Add(animation2);

                            var animation3 = new DoubleAnimation
                            {
                                Duration = TimeSpan.FromSeconds(_animationTime),
                                To = _ScaleRatio,
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation3, vFrameworker);
                            Storyboard.SetTargetProperty(animation3,
                                new PropertyPath(
                                    "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                            _storyboard.Children.Add(animation3);

                            _mapCarouselLocationFramewokes[CarouselLoacation.Left] = i;
                        }
                    }
                }
                else if (i == nIndex)
                {
                    if (_mapFrameworkes.ContainsKey(i))
                    {
                        var vFrameworker = _mapFrameworkes.GetValueOrDefault(i);

                        if (vFrameworker != null)
                        {
                            var animation1 = new Int32Animation
                            {
                                To = (int)CarouselZIndex.Center,
                                Duration = TimeSpan.FromSeconds(_delayAnimationTime),
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation1, vFrameworker);
                            Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                            _storyboard.Children.Add(animation1);

                            var animation2 = new DoubleAnimation
                            {
                                BeginTime = TimeSpan.FromSeconds(_delayAnimationTime),
                                To = _CenterDockLeft,
                                Duration = TimeSpan.FromSeconds(_animationTime),
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation2, vFrameworker);
                            Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                            _storyboard.Children.Add(animation2);

                            var animation3 = new DoubleAnimation
                            {
                                Duration = TimeSpan.FromSeconds(_animationTime),
                                To = _ScaleRatioEx,
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation3, vFrameworker);
                            Storyboard.SetTargetProperty(animation3,
                                new PropertyPath(
                                    "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                            _storyboard.Children.Add(animation3);

                            _mapCarouselLocationFramewokes[CarouselLoacation.Center] = i;
                        }
                    }
                }
                else if (i == vNext)
                {
                    if (_mapFrameworkes.ContainsKey(i))
                    {
                        var vFrameworker = _mapFrameworkes.GetValueOrDefault(i);

                        if (vFrameworker != null)
                        {
                            var animation1 = new Int32Animation
                            {
                                To = (int)CarouselZIndex.Right,
                                Duration = TimeSpan.FromSeconds(_delayAnimationTime),
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation1, vFrameworker);
                            Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                            _storyboard.Children.Add(animation1);

                            var animation2 = new DoubleAnimation
                            {
                                BeginTime = TimeSpan.FromSeconds(_delayAnimationTime),
                                To = _RightDockLeft,
                                Duration = TimeSpan.FromSeconds(_animationTime),
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation2, vFrameworker);
                            Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                            _storyboard.Children.Add(animation2);

                            var animation3 = new DoubleAnimation
                            {
                                Duration = TimeSpan.FromSeconds(_animationTime),
                                To = _ScaleRatio,
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation3, vFrameworker);
                            Storyboard.SetTargetProperty(animation3,
                                new PropertyPath(
                                    "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                            _storyboard.Children.Add(animation3);

                            _mapCarouselLocationFramewokes[CarouselLoacation.Right] = i;
                        }
                    }
                }
                else
                {
                    _BufferLinkedList.AddLast(i);
                }

            _storyboard?.Begin();

            return true;
        }


        #endregion
    }
}
