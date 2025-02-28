using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
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
    [TemplatePart(Name = Part_ContentDockName, Type = typeof(Canvas))]
    [TemplatePart(Name = Part_ButtonDockName, Type = typeof(StackPanel))]
    public class MasterCarousel : Control, IAddChild
    {
        private const string Part_ContentDockName = "PART_ContentDock";
        private const string Part_ButtonDockName = "PART_ButtonDock";

        // Using a DependencyProperty as the backing store for IsStartAinimation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsStartAinimationProperty =
            DependencyProperty.Register("IsStartAinimation", typeof(bool), typeof(MasterCarousel),
                new PropertyMetadata(default(bool), OnIsStartAinimationPropertyChangedCallback));

        // Using a DependencyProperty as the backing store for PlaySpeed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaySpeedProperty =
            DependencyProperty.Register("PlaySpeed", typeof(double), typeof(MasterCarousel),
                new PropertyMetadata(2000d, OnPlaySpeedPropertyChangedCallBack));

        // Using a DependencyProperty as the backing store for Childrens.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(MasterCarousel),
                new PropertyMetadata(default(IEnumerable), OnItemsSourcePropertyChangedCallBack));

        #region timer

        private readonly Timer _PlayTimer = new Timer();

        #endregion


        static MasterCarousel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterCarousel),
                new FrameworkPropertyMetadata(typeof(MasterCarousel)));
        }

        public MasterCarousel()
        {
            //_uiElementCollection = new List<object>();
            //SizeChanged += MasterCarousel_SizeChanged;

            LoadeTimer();
            Loaded += MasterCarousel_Loaded;
        }

        public bool IsStartAinimation
        {
            get => (bool)GetValue(IsStartAinimationProperty);
            set => SetValue(IsStartAinimationProperty, value);
        }


        public double PlaySpeed
        {
            get => (double)GetValue(PlaySpeedProperty);
            set => SetValue(PlaySpeedProperty, value);
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
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

            _Part_ContentDock = GetTemplateChild(Part_ContentDockName) as Canvas;
            _Part_ButtonDock = GetTemplateChild(Part_ButtonDockName) as StackPanel;

            if (_Part_ContentDock == null || _Part_ButtonDock == null)
                throw new Exception("Some element is not in template!");
        }

        #endregion

        private static void OnIsStartAinimationPropertyChangedCallback(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d == null)
                return;

            if (!(d is MasterCarousel control))
                return;

            if (bool.TryParse(e.NewValue?.ToString(), out var bResult))
            {
                if (bResult)
                    control.Start();
                else
                    control.Stop();
            }
        }

        private static void OnPlaySpeedPropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null)
                return;

            if (!(d is MasterCarousel control))
                return;

            if (!double.TryParse(e.NewValue?.ToString(), out var vResult))
                return;

            control.Stop();
            control.ResetInterval(vResult);
            control.Start();
        }

        private static void OnItemsSourcePropertyChangedCallBack(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (!(d is MasterCarousel carousel))
                return;

            var vOldEvent = e.OldValue?.GetType()?.GetEvent("CollectionChanged");
            if (vOldEvent != null)
                vOldEvent.RemoveEventHandler(e.OldValue,
                    new NotifyCollectionChangedEventHandler(carousel.ChildrenPropertyChanged));

            var vEvent = e.NewValue?.GetType()?.GetEvent("CollectionChanged");
            if (vEvent != null)
                vEvent.AddEventHandler(e.NewValue,
                    new NotifyCollectionChangedEventHandler(carousel.ChildrenPropertyChanged));
        }


        private void ChildrenPropertyChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
            }
        }


        private bool LoadeTimer()
        {
            _PlayTimer.Interval = PlaySpeed;
            _PlayTimer.Elapsed += PlayTimer_Elapsed;

            return true;
        }

        private bool ResetInterval(double interval)
        {
            if (interval <= 0)
                return false;

            _PlayTimer.Interval = interval;

            return true;
        }

        private bool CalculationShellReletiveProperty()
        {
            //计算当前目标区域的尺寸
            if (_Part_ContentDock == null)
                return false;

            var vWidth = _Part_ContentDock.ActualWidth;
            var vHeight = _Part_ContentDock.ActualHeight;

            if (vWidth == 0 || vHeight == 0)
                return false;

            if (vWidth == _ShellWidth && vHeight == _ShellHeight)
                return false;

            _ShellWidth = vWidth;
            _ShellHeight = vHeight;

            //计算 元素的默认长度和宽度
            _ElementWidth = _ShellWidth * _ElementScale;
            _ElementHeight = _ShellHeight;

            _LeftDockLeft = 0;
            _CenterDockLeft = 0 + _ShellWidth * _DockOffset;
            _RightDockLeft = _ShellWidth - _ElementWidth;

            return true;
        }

        private bool LoadCarousel()
        {
            if (Children.Count <= 0 && ItemsSource == null)
                return false;

            if (_Part_ButtonDock != null)
                foreach (var item in _Part_ButtonDock.Children)
                    if (item is FrameworkElement frameworkElement)
                    {
                        frameworkElement.MouseEnter -= Border_MouseEnter;
                        frameworkElement.PreviewMouseDown -= Border_PreviewMouseDown;
                    }

            _mapFrameworkes.Clear();
            _mapCarouselLocationFramewokes.Clear();
            _BufferLinkedList.Clear();

            _Part_ContentDock?.Children.Clear();
            _Part_ButtonDock?.Children.Clear();


            if (Children.Count > 0)
            {
                _CarouselSize = Children.Count;

                for (var i = 0; i < _CarouselSize; i++)
                {
                    var vItem = Children[i];
                    FrameworkElement frameworkElement;
                    if (vItem is FrameworkElement)
                    {
                        frameworkElement = vItem as FrameworkElement;
                    }
                    else
                    {
                        var vContent = new ContentControl();
                        vContent.HorizontalContentAlignment = HorizontalAlignment.Center;
                        vContent.VerticalContentAlignment = VerticalAlignment.Center;
                        vContent.Content = vItem;
                        frameworkElement = vContent;
                    }

                    frameworkElement.Width = _ElementWidth;
                    frameworkElement.Height = _ElementHeight;

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

                    var border = new Border
                    {
                        Margin = new Thickness(5),
                        Width = 20,
                        Height = 6,
                        //CornerRadius = new CornerRadius(20),
                        Background = Brushes.Gray,
                        Tag = i
                    };

                    border.MouseEnter += Border_MouseEnter;
                    border.PreviewMouseDown += Border_PreviewMouseDown;

                    _mapResources[i] = vItem;
                    _mapFrameworkes[i] = frameworkElement;

                    _Part_ContentDock?.Children.Add(frameworkElement);
                    _Part_ButtonDock?.Children.Add(border);

                    //第一个元素居中并且放大显示
                    if (i == 0)
                    {
                        var vScaleTransform = vTransformGroup.Children[0] as ScaleTransform;
                        vScaleTransform.ScaleY = _ScaleRatioEx;
                        frameworkElement.SetValue(Canvas.LeftProperty, _CenterDockLeft);
                        Panel.SetZIndex(frameworkElement, (int)CarouselZIndex.Center);
                        _mapCarouselLocationFramewokes.Add(CarouselLoacation.Center, i);
                    }
                    else if (i == 1)
                    {
                        frameworkElement.SetValue(Canvas.LeftProperty, _RightDockLeft);
                        Panel.SetZIndex(frameworkElement, (int)CarouselZIndex.Right);
                        _mapCarouselLocationFramewokes.Add(CarouselLoacation.Right, i);
                    }
                    else if (i == _CarouselSize - 1)
                    {
                        frameworkElement.SetValue(Canvas.LeftProperty, _LeftDockLeft);
                        Panel.SetZIndex(frameworkElement, (int)CarouselZIndex.Left);
                        _mapCarouselLocationFramewokes.Add(CarouselLoacation.Left, i);
                    }
                    else
                    {
                        _BufferLinkedList.AddLast(i);
                        frameworkElement.SetValue(Canvas.LeftProperty, _CenterDockLeft);
                        Panel.SetZIndex(frameworkElement, i);
                    }
                }
            }
            else
            {
                _CarouselSize = ItemsSource.Count();

                var nIndex = 0;
                foreach (var item in ItemsSource)
                {
                    FrameworkElement frameworkElement;
                    if (item is FrameworkElement)
                    {
                        frameworkElement = item as FrameworkElement;
                    }
                    else
                    {
                        var vContent = new ContentControl();
                        vContent.HorizontalContentAlignment = HorizontalAlignment.Center;
                        vContent.VerticalContentAlignment = VerticalAlignment.Center;
                        vContent.Content = item;
                        frameworkElement = vContent;
                    }

                    frameworkElement.Width = _ElementWidth;
                    frameworkElement.Height = _ElementHeight;

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

                    var border = new Border
                    {
                        Width = 25,
                        Height = 25,
                        CornerRadius = new CornerRadius(25),
                        Background = Brushes.Gray,
                        Tag = nIndex
                    };

                    border.MouseEnter += Border_MouseEnter;
                    border.PreviewMouseDown += Border_PreviewMouseDown;

                    _mapResources[nIndex] = item;
                    _mapFrameworkes[nIndex] = frameworkElement;

                    _Part_ContentDock?.Children.Add(frameworkElement);
                    _Part_ButtonDock?.Children.Add(border);

                    //第一个元素居中并且放大显示
                    if (nIndex == 0)
                    {
                        var vScaleTransform = vTransformGroup.Children[0] as ScaleTransform;
                        vScaleTransform.ScaleY = _ScaleRatioEx;
                        frameworkElement.SetValue(Canvas.LeftProperty, _CenterDockLeft);
                        Panel.SetZIndex(frameworkElement, (int)CarouselZIndex.Center);
                        _mapCarouselLocationFramewokes.Add(CarouselLoacation.Center, nIndex);
                    }
                    else if (nIndex == 1)
                    {
                        frameworkElement.SetValue(Canvas.LeftProperty, _RightDockLeft);
                        Panel.SetZIndex(frameworkElement, (int)CarouselZIndex.Right);
                        _mapCarouselLocationFramewokes.Add(CarouselLoacation.Right, nIndex);
                    }
                    else if (nIndex == _CarouselSize - 1)
                    {
                        frameworkElement.SetValue(Canvas.LeftProperty, _LeftDockLeft);
                        Panel.SetZIndex(frameworkElement, (int)CarouselZIndex.Left);
                        _mapCarouselLocationFramewokes.Add(CarouselLoacation.Left, nIndex);
                    }
                    else
                    {
                        _BufferLinkedList.AddLast(nIndex);
                        frameworkElement.SetValue(Canvas.LeftProperty, _CenterDockLeft);
                        Panel.SetZIndex(frameworkElement, nIndex);
                    }
                }
            }

            return true;
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            _IsStoryboardWorking = false;
        }

        private void PlayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher?.BeginInvoke(new Action(() => PlayCarouselRightToLeft()),
                DispatcherPriority.Background);
        }

        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;

            if (sender is FrameworkElement frameworkElement)
                if (int.TryParse(frameworkElement.Tag?.ToString(), out var nResult))
                    PlayCarouselWithIndex(nResult);
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is FrameworkElement frameworkElement)
                if (int.TryParse(frameworkElement.Tag?.ToString(), out var nResult))
                    PlayCarouselWithIndex(nResult);
        }

        private void MasterCarousel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Stop();

            if (CalculationShellReletiveProperty())
            {
                LoadCarousel();
                Start();
            }
        }

        private void MasterCarousel_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
                return;

            Stop();

            if (CalculationShellReletiveProperty())
            {
                LoadCarousel();
                Start();
            }

            _isLoaded = true;
        }

        #region feild

        private bool _isLoaded;
        private Canvas _Part_ContentDock;
        private StackPanel _Part_ButtonDock;

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

        private int _CarouselSize;

        #endregion

        #region

        private readonly Dictionary<int, object> _mapResources = new Dictionary<int, object>();
        private readonly Dictionary<int, FrameworkElement> _mapFrameworkes = new Dictionary<int, FrameworkElement>();

        private readonly Dictionary<CarouselLoacation, int> _mapCarouselLocationFramewokes =
            new Dictionary<CarouselLoacation, int>();

        private readonly LinkedList<int> _BufferLinkedList = new LinkedList<int>();

        #endregion

        #region StoryBoard

        private Storyboard _Storyboard;
        private readonly double _AnimationTime = 0.5;
        private readonly double _DelayAnimationTime = 0.7;

        private bool _IsAinimationStart;
        private bool _IsStoryboardWorking;

        #endregion


        #region 动画

        //从左边向右依次播放
        private bool PlayCarouselLeftToRight()
        {
            if (_Storyboard == null)
            {
                _Storyboard = new Storyboard();
                _Storyboard.Completed += Storyboard_Completed;
            }

            if (_IsStoryboardWorking)
                return false;

            _IsStoryboardWorking = true;

            _Storyboard?.Children.Clear();

            var nNextIndex = -1;

            //右边的动画移动到中间 后层
            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLoacation.Right, -1);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);

                if (vFrameworker != null)
                {
                    //置于后层
                    var animation1 = new Int32Animation
                    {
                        To = vResult + 1,
                        Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _Storyboard.Children.Add(animation1);

                    //右边移动到中间
                    var animation2 = new DoubleAnimation
                    {
                        To = _CenterDockLeft,
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _Storyboard.Children.Add(animation2);

                    //缩放
                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        To = _ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _Storyboard.Children.Add(animation3);

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
                    //置于左边上层
                    var animation1 = new Int32Animation
                    {
                        To = (int)CarouselZIndex.Right,
                        Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _Storyboard.Children.Add(animation1);

                    //从中间到左边
                    var animation2 = new DoubleAnimation
                    {
                        //BeginTime = TimeSpan.FromSeconds(_DelayAnimationTime),
                        To = _RightDockLeft,
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _Storyboard.Children.Add(animation2);

                    //缩放
                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        To = _ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _Storyboard.Children.Add(animation3);

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
                    //置于上层
                    var animation1 = new Int32Animation
                    {
                        To = (int)CarouselZIndex.Center,
                        Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _Storyboard.Children.Add(animation1);

                    //从左到中
                    var animation2 = new DoubleAnimation
                    {
                        To = _CenterDockLeft,
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _Storyboard.Children.Add(animation2);

                    //缩放
                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        To = _ScaleRatioEx,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _Storyboard.Children.Add(animation3);

                    _mapCarouselLocationFramewokes[CarouselLoacation.Center] = vResult;

                    nNextIndex = vResult - 1;
                    if (nNextIndex < 0)
                        nNextIndex = _CarouselSize - 1;
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
                    //右侧置顶
                    var animation1 = new Int32Animation
                    {
                        To = (int)CarouselZIndex.Left,
                        Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _Storyboard.Children.Add(animation1);

                    //从中间移动到右侧
                    var animation2 = new DoubleAnimation
                    {
                        To = _LeftDockLeft,
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _Storyboard.Children.Add(animation2);

                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        To = _ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _Storyboard.Children.Add(animation3);

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
                        //右侧置顶
                        var animation1 = new Int32Animation
                        {
                            To = (int)CarouselZIndex.Left,
                            Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };
                        Storyboard.SetTarget(animation1, vFrameworker);
                        Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                        _Storyboard.Children.Add(animation1);

                        //从中间移动到右侧
                        var animation2 = new DoubleAnimation
                        {
                            To = _LeftDockLeft,
                            Duration = TimeSpan.FromSeconds(_AnimationTime),
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };
                        Storyboard.SetTarget(animation2, vFrameworker);
                        Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                        _Storyboard.Children.Add(animation2);

                        var animation3 = new DoubleAnimation
                        {
                            Duration = TimeSpan.FromSeconds(_AnimationTime),
                            To = _ScaleRatio,
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };
                        Storyboard.SetTarget(animation3, vFrameworker);
                        Storyboard.SetTargetProperty(animation3,
                            new PropertyPath(
                                "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                        _Storyboard.Children.Add(animation3);

                        _mapCarouselLocationFramewokes[CarouselLoacation.Left] = vResult;
                    }
                }
            }

            _Storyboard?.Begin();

            return true;
        }

        //从右向左依次播放
        private bool PlayCarouselRightToLeft()
        {
            if (_Storyboard == null)
            {
                _Storyboard = new Storyboard();
                _Storyboard.Completed += Storyboard_Completed;
            }

            _IsStoryboardWorking = true;

            _Storyboard?.Children.Clear();

            var nNextIndex = -1;

            //左边的动画移动到中间 后层
            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLoacation.Left, -1);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);

                if (vFrameworker != null)
                {
                    //置于后层
                    var animation1 = new Int32Animation
                    {
                        To = vResult + 1,
                        Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _Storyboard.Children.Add(animation1);

                    //从左到中
                    var animation2 = new DoubleAnimation
                    {
                        To = _CenterDockLeft,
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _Storyboard.Children.Add(animation2);

                    //缩放
                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        To = _ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _Storyboard.Children.Add(animation3);

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
                    //置于左边上层
                    var animation1 = new Int32Animation
                    {
                        To = (int)CarouselZIndex.Left,
                        Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _Storyboard.Children.Add(animation1);

                    //从中间到左边
                    var animation2 = new DoubleAnimation
                    {
                        //BeginTime = TimeSpan.FromSeconds(_DelayAnimationTime),
                        To = _LeftDockLeft,
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _Storyboard.Children.Add(animation2);

                    //缩放
                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        To = _ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _Storyboard.Children.Add(animation3);

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
                    //置于上层
                    var animation1 = new Int32Animation
                    {
                        To = (int)CarouselZIndex.Center,
                        Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _Storyboard.Children.Add(animation1);

                    //右边移动到中间
                    var animation2 = new DoubleAnimation
                    {
                        To = _CenterDockLeft,
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _Storyboard.Children.Add(animation2);

                    //缩放
                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        To = _ScaleRatioEx,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _Storyboard.Children.Add(animation3);

                    _mapCarouselLocationFramewokes[CarouselLoacation.Center] = vResult;

                    nNextIndex = vResult + 1;
                    if (nNextIndex >= _CarouselSize)
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
                    //右侧置顶
                    var animation1 = new Int32Animation
                    {
                        To = (int)CarouselZIndex.Right,
                        Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _Storyboard.Children.Add(animation1);

                    //从中间移动到右侧
                    var animation2 = new DoubleAnimation
                    {
                        To = _RightDockLeft,
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _Storyboard.Children.Add(animation2);

                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        To = _ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _Storyboard.Children.Add(animation3);

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
                        //右侧置顶
                        var animation1 = new Int32Animation
                        {
                            To = (int)CarouselZIndex.Right,
                            Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };
                        Storyboard.SetTarget(animation1, vFrameworker);
                        Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                        _Storyboard.Children.Add(animation1);

                        //从中间移动到右侧
                        var animation2 = new DoubleAnimation
                        {
                            To = _RightDockLeft,
                            Duration = TimeSpan.FromSeconds(_AnimationTime),
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };
                        Storyboard.SetTarget(animation2, vFrameworker);
                        Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                        _Storyboard.Children.Add(animation2);

                        var animation3 = new DoubleAnimation
                        {
                            Duration = TimeSpan.FromSeconds(_AnimationTime),
                            To = _ScaleRatio,
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };
                        Storyboard.SetTarget(animation3, vFrameworker);
                        Storyboard.SetTargetProperty(animation3,
                            new PropertyPath(
                                "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                        _Storyboard.Children.Add(animation3);

                        _mapCarouselLocationFramewokes[CarouselLoacation.Right] = vResult;
                    }
                }
            }

            _Storyboard?.Begin();

            return true;
        }

        //当用户点击其中某个位置的动画时
        private bool PlayCarouselWithIndex(int nIndex)
        {
            //检查 nIndex是否有效
            if (nIndex < 0 || nIndex >= _CarouselSize)
                return false;

            //判断当前选中的是否处于中间播放位置
            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLoacation.Center, -1);
                if (vResult == nIndex)
                    return true;
            }

            //判断如果当前选中的在左侧等待区 播放顺序是从左向右
            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLoacation.Left, -1);
                if (vResult == nIndex)
                    return PlayCarouselLeftToRight();
            }

            //判断如果当前选中的在右侧等待区 播放顺序是从右向左
            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLoacation.Right, -1);
                if (vResult == nIndex)
                    return PlayCarouselRightToLeft();
            }

            //其他情况 
            return PlayCarouselWithIndexOutRange(nIndex);
        }

        private bool PlayCarouselWithIndexOutRange(int nIndex)
        {
            //检查 nIndex是否有效
            if (nIndex < 0 || nIndex >= _CarouselSize)
                return false;

            //计算前后动画位置
            var vPre = nIndex - 1;
            if (vPre < 0)
                vPre = _CarouselSize - 1;

            var vNext = nIndex + 1;
            if (vNext >= _CarouselSize)
                vNext = 0;

            if (_Storyboard == null)
            {
                _Storyboard = new Storyboard();
                _Storyboard.Completed += Storyboard_Completed;
            }

            if (_IsStoryboardWorking)
                return false;

            _IsStoryboardWorking = true;

            _Storyboard?.Children.Clear();

            //清空队列
            _BufferLinkedList.Clear();

            //先将队列归位 全部置于中间后面隐藏
            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLoacation.Right, -1);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);
                if (vFrameworker != null)
                {
                    //置于后层
                    var animation1 = new Int32Animation
                    {
                        To = vResult + 1,
                        Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _Storyboard.Children.Add(animation1);

                    //回到中间
                    var animation2 = new DoubleAnimation
                    {
                        To = _CenterDockLeft,
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _Storyboard.Children.Add(animation2);

                    //缩放
                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        To = _ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _Storyboard.Children.Add(animation3);
                }

                _mapCarouselLocationFramewokes[CarouselLoacation.Right] = -1;
            }

            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLoacation.Center, -1);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);
                if (vFrameworker != null)
                {
                    //置于后层
                    var animation1 = new Int32Animation
                    {
                        To = vResult + 1,
                        Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _Storyboard.Children.Add(animation1);

                    //回到中间
                    var animation2 = new DoubleAnimation
                    {
                        To = _CenterDockLeft,
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _Storyboard.Children.Add(animation2);

                    //缩放
                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        To = _ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _Storyboard.Children.Add(animation3);
                }

                _mapCarouselLocationFramewokes[CarouselLoacation.Center] = -1;
            }

            {
                var vResult = _mapCarouselLocationFramewokes.GetValueOrDefault(CarouselLoacation.Left, -1);

                var vFrameworker = _mapFrameworkes.GetValueOrDefault(vResult);
                if (vFrameworker != null)
                {
                    //置于后层
                    var animation1 = new Int32Animation
                    {
                        To = vResult + 1,
                        Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation1, vFrameworker);
                    Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                    _Storyboard.Children.Add(animation1);

                    //回到中间
                    var animation2 = new DoubleAnimation
                    {
                        To = _CenterDockLeft,
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation2, vFrameworker);
                    Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                    _Storyboard.Children.Add(animation2);

                    //缩放
                    var animation3 = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(_AnimationTime),
                        To = _ScaleRatio,
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };
                    Storyboard.SetTarget(animation3, vFrameworker);
                    Storyboard.SetTargetProperty(animation3,
                        new PropertyPath(
                            "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                    _Storyboard.Children.Add(animation3);
                }

                _mapCarouselLocationFramewokes[CarouselLoacation.Left] = -1;
            }

            //再调出目标位置动画
            for (var i = 0; i < _CarouselSize; i++)
                if (i == vPre) //放左侧
                {
                    if (_mapFrameworkes.ContainsKey(i))
                    {
                        var vFrameworker = _mapFrameworkes.GetValueOrDefault(i);

                        if (vFrameworker != null)
                        {
                            //置于左边上层
                            var animation1 = new Int32Animation
                            {
                                To = (int)CarouselZIndex.Left,
                                Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation1, vFrameworker);
                            Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                            _Storyboard.Children.Add(animation1);

                            //从中间到左边
                            var animation2 = new DoubleAnimation
                            {
                                BeginTime = TimeSpan.FromSeconds(_DelayAnimationTime),
                                To = _LeftDockLeft,
                                Duration = TimeSpan.FromSeconds(_AnimationTime),
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation2, vFrameworker);
                            Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                            _Storyboard.Children.Add(animation2);

                            //缩放
                            var animation3 = new DoubleAnimation
                            {
                                Duration = TimeSpan.FromSeconds(_AnimationTime),
                                To = _ScaleRatio,
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation3, vFrameworker);
                            Storyboard.SetTargetProperty(animation3,
                                new PropertyPath(
                                    "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                            _Storyboard.Children.Add(animation3);

                            _mapCarouselLocationFramewokes[CarouselLoacation.Left] = i;
                        }
                    }
                }
                else if (i == nIndex) //放中间
                {
                    if (_mapFrameworkes.ContainsKey(i))
                    {
                        var vFrameworker = _mapFrameworkes.GetValueOrDefault(i);

                        if (vFrameworker != null)
                        {
                            //置于中间上层
                            var animation1 = new Int32Animation
                            {
                                To = (int)CarouselZIndex.Center,
                                Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation1, vFrameworker);
                            Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                            _Storyboard.Children.Add(animation1);

                            //到中间
                            var animation2 = new DoubleAnimation
                            {
                                BeginTime = TimeSpan.FromSeconds(_DelayAnimationTime),
                                To = _CenterDockLeft,
                                Duration = TimeSpan.FromSeconds(_AnimationTime),
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation2, vFrameworker);
                            Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                            _Storyboard.Children.Add(animation2);

                            //缩放
                            var animation3 = new DoubleAnimation
                            {
                                Duration = TimeSpan.FromSeconds(_AnimationTime),
                                To = _ScaleRatioEx,
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation3, vFrameworker);
                            Storyboard.SetTargetProperty(animation3,
                                new PropertyPath(
                                    "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                            _Storyboard.Children.Add(animation3);

                            _mapCarouselLocationFramewokes[CarouselLoacation.Center] = i;
                        }
                    }
                }
                else if (i == vNext) //放右侧
                {
                    if (_mapFrameworkes.ContainsKey(i))
                    {
                        var vFrameworker = _mapFrameworkes.GetValueOrDefault(i);

                        if (vFrameworker != null)
                        {
                            //置于右边上层
                            var animation1 = new Int32Animation
                            {
                                To = (int)CarouselZIndex.Right,
                                Duration = TimeSpan.FromSeconds(_DelayAnimationTime),
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation1, vFrameworker);
                            Storyboard.SetTargetProperty(animation1, new PropertyPath("(Panel.ZIndex)"));
                            _Storyboard.Children.Add(animation1);

                            //到右边
                            var animation2 = new DoubleAnimation
                            {
                                BeginTime = TimeSpan.FromSeconds(_DelayAnimationTime),
                                To = _RightDockLeft,
                                Duration = TimeSpan.FromSeconds(_AnimationTime),
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation2, vFrameworker);
                            Storyboard.SetTargetProperty(animation2, new PropertyPath("(Canvas.Left)"));
                            _Storyboard.Children.Add(animation2);

                            //缩放
                            var animation3 = new DoubleAnimation
                            {
                                Duration = TimeSpan.FromSeconds(_AnimationTime),
                                To = _ScaleRatio,
                                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                            };
                            Storyboard.SetTarget(animation3, vFrameworker);
                            Storyboard.SetTargetProperty(animation3,
                                new PropertyPath(
                                    "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                            _Storyboard.Children.Add(animation3);

                            _mapCarouselLocationFramewokes[CarouselLoacation.Right] = i;
                        }
                    }
                }
                else
                {
                    _BufferLinkedList.AddLast(i);
                }

            _Storyboard?.Begin();

            return true;
        }

        private bool Start()
        {
            if (!IsStartAinimation)
                return true;

            if (_IsAinimationStart)
                return true;

            _IsAinimationStart = true;
            _PlayTimer.Start();
            return true;
        }

        private bool Stop()
        {
            if (_IsAinimationStart)
            {
                _IsAinimationStart = false;
                _PlayTimer.Stop();
            }

            return true;
        }

        #endregion
    }
}