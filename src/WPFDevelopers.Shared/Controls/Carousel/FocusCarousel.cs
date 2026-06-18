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
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    [DefaultProperty("Children")]
    [ContentProperty("Children")]
    [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    [TemplatePart(Name = Part_BackCanvasName, Type = typeof(Canvas))]
    public class FocusCarousel : CarouselBase, IAddChild
    {
        [ReadOnly(true)] private const string Part_BackCanvasName = "PART_BackCanvas";

        private const int _maxSimpleHeight = 320;
        private double _displayHeight;
        private FrameworkElement _displayItem;
        private readonly double _displayOffset = 10d;
        private double _displayWidth;
        private readonly Dictionary<int, Point> _mapCanvasPoint = new Dictionary<int, Point>();
        private readonly Dictionary<int, FrameworkElement> _mapUIwithIndex = new Dictionary<int, FrameworkElement>();
        private int _simpleCount;
        private double _simpleHeight;
        private double _simpleTop;
        private double _simpleWidth;
        private Canvas _part_BackCanvas;

        static FocusCarousel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FocusCarousel),
                new FrameworkPropertyMetadata(typeof(FocusCarousel)));
        }

        public FocusCarousel()
        {
            Loaded += FocusCarousel_Loaded;
            Unloaded += FocusCarousel_Unloaded;
            SizeChanged += FocusCarousel_SizeChanged;
            Children.CollectionChanged += OnItems_CollectionChanged;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<FrameworkElement> Children { get; } = new ObservableCollection<FrameworkElement>();

        void IAddChild.AddChild(object value)
        {
            throw new NotImplementedException();
        }

        void IAddChild.AddText(string text)
        {
            throw new NotImplementedException();
        }

        private bool _isUpdatingItemsSource;

        private void OnItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                foreach (var item in e.NewItems)
                    if (item is FrameworkElement element)
                    {
                        element.PreviewMouseLeftButtonDown += FocusCarousel_MouseLeftButtonDown;

                        element.RenderTransformOrigin = new Point(0.5, 0.5);
                        element.RenderTransform = new TransformGroup
                        {
                            Children =
                            {
                                new ScaleTransform(),
                                new SkewTransform(),
                                new RotateTransform(),
                                new TranslateTransform()
                            }
                        };
                    }

            if (e.Action == NotifyCollectionChangedAction.Remove)
                foreach (var item in e.OldItems)
                {
                    if (item is FrameworkElement element)
                        element.PreviewMouseLeftButtonDown -= FocusCarousel_MouseLeftButtonDown;

                    if (item == _displayItem)
                        _displayItem = null;
                }

            if (_isUpdatingItemsSource)
                return;

            if (_part_BackCanvas != null)
                OnSizeChangedCallback();
        }

        #region override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _part_BackCanvas = GetTemplateChild(Part_BackCanvasName) as Canvas;

            if (_part_BackCanvas == null)
                throw new Exception("Some element is not in template!");

            Dispatcher.BeginInvoke(new Action(() => OnSizeChangedCallback()));
        }

        protected override void OnItemsSourceChangedCore(object oldValue, object newValue)
        {
            _isUpdatingItemsSource = true;
            try
            {
                Children.Clear();
                if (newValue is IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        FrameworkElement fe = null;
                        if (item is FrameworkElement element)
                        {
                            fe = element;
                        }
                        else if (!string.IsNullOrEmpty(DisplayMemberPath))
                        {
                            var imageUrl = GetDisplayValue(item, DisplayMemberPath);
                            if (!string.IsNullOrEmpty(imageUrl))
                            {
                                fe = new Image { Tag = imageUrl };
                            }
                        }
                        if (fe == null)
                        {
                            fe = new ContentControl { HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Content = item };
                        }
                        Children.Add(fe);
                    }
                }
            }
            finally
            {
                _isUpdatingItemsSource = false;
            }
            if (_part_BackCanvas != null)
                OnSizeChangedCallback();
        }

        protected override void OnSelectedIndexChangedCore(int oldIndex, int newIndex)
        {
            if (newIndex >= 0 && newIndex < Children.Count)
            {
                _displayItem = Children[newIndex];
                OnSizeChangedCallback();
            }
        }

        protected override void OnItemsSourceCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            OnItemsSourceChangedCore(null, ItemsSource);
        }

        protected override void OnAutoPlayTick()
        {
            if (Children.Count == 0) return;
            int next = SelectedIndex + 1;
            if (next >= Children.Count) next = 0;
            SelectedIndex = next;
        }

        #endregion


        public bool OnSizeChangedCallback()
        {
            var vItemCount = Children.Count;
            if (vItemCount <= 0)
                return false;
            if (_part_BackCanvas == null)
                return false;
            var vHeight = _part_BackCanvas.ActualHeight;
            var vWidth = _part_BackCanvas.ActualWidth;

            if (vHeight == double.NaN || vWidth == double.NaN)
                return false;

            if (vHeight == 0 || vWidth == 0)
                return false;

            _part_BackCanvas.Children.Clear();

            _mapUIwithIndex.Clear();
            _mapCanvasPoint.Clear();
            _simpleCount = vItemCount - 1;
            if (_simpleCount == 0)
            {
                Children[0].Width = vWidth;
                Children[0].Height = vHeight;

                _part_BackCanvas.Children.Add(Children[0]);
                return true;
            }

            //计算并划分显示区域
            var vSimpleHeight = vHeight * 0.4;
            _simpleHeight = vSimpleHeight;
            if (_simpleHeight > _maxSimpleHeight)
                _simpleHeight = _maxSimpleHeight;

            var vSimpleWidth = vWidth / _simpleCount;
            _simpleWidth = vSimpleWidth;
            _simpleTop = vHeight - _simpleHeight;

            _displayHeight = vHeight - _simpleHeight;
            _displayHeight -= _displayOffset;

            _displayWidth = vWidth;

            if (_displayItem == null)
                _displayItem = Children[0];

            var nIndex = 0;
            var nPosIndex = 0;
            double Left = 0;

            foreach (var item in Children)
            {
                EnsureImageLoaded(item, _displayItem == item ? _displayWidth : _simpleWidth,
                    _displayItem == item ? _displayHeight : _simpleHeight);

                _part_BackCanvas.Children.Add(item);
                item.Tag = nIndex;


                if (_displayItem == item)
                {
                    var storyboard = new Storyboard();
                    {
                        if (_displayWidth == double.NaN)
                        {
                        }

                        var animation = new DoubleAnimation
                        {
                            To = _displayWidth,
                            Duration = new Duration(new TimeSpan(0, 0, 0, 0, 0)),
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };

                        Storyboard.SetTarget(animation, _displayItem);
                        Storyboard.SetTargetProperty(animation, new PropertyPath("Width"));
                        storyboard.Children.Add(animation);
                    }
                    {
                        var animation = new DoubleAnimation
                        {
                            To = _displayHeight,
                            Duration = new Duration(new TimeSpan(0, 0, 0, 0, 0)),
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };

                        Storyboard.SetTarget(animation, _displayItem);
                        Storyboard.SetTargetProperty(animation, new PropertyPath("Height"));
                        storyboard.Children.Add(animation);
                    }

                    {
                        var animation = new DoubleAnimation
                        {
                            To = 0d,
                            Duration = new Duration(new TimeSpan(0, 0, 0, 0, 0)),
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };

                        Storyboard.SetTarget(animation, _displayItem);
                        Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Left)"));
                        storyboard.Children.Add(animation);
                    }
                    {
                        var animation = new DoubleAnimation
                        {
                            To = 0d,
                            Duration = new Duration(new TimeSpan(0, 0, 0, 0, 0)),
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };

                        Storyboard.SetTarget(animation, _displayItem);
                        Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Top)"));
                        storyboard.Children.Add(animation);
                    }
                    storyboard.Begin();

                    item.Width = _displayWidth;
                    item.Height = _displayHeight;
                    item.SetValue(Canvas.LeftProperty, 0d);
                    item.SetValue(Canvas.TopProperty, 0d);
                }
                else
                {
                    var storyboard = new Storyboard();

                    {
                        var animation = new DoubleAnimation
                        {
                            To = _simpleWidth,
                            Duration = new Duration(new TimeSpan(0, 0, 0, 0, 0)),
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };

                        Storyboard.SetTarget(animation, item);
                        Storyboard.SetTargetProperty(animation, new PropertyPath("Width"));
                        storyboard.Children.Add(animation);
                    }
                    {
                        var animation = new DoubleAnimation
                        {
                            To = _simpleHeight,
                            Duration = new Duration(new TimeSpan(0, 0, 0, 0, 0)),
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };

                        Storyboard.SetTarget(animation, item);
                        Storyboard.SetTargetProperty(animation, new PropertyPath("Height"));
                        storyboard.Children.Add(animation);
                    }

                    {
                        var animation = new DoubleAnimation
                        {
                            To = Left,
                            Duration = new Duration(new TimeSpan(0, 0, 0, 0, 0)),
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };

                        Storyboard.SetTarget(animation, item);
                        Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Left)"));
                        storyboard.Children.Add(animation);
                    }
                    {
                        var animation = new DoubleAnimation
                        {
                            To = _simpleTop,
                            Duration = new Duration(new TimeSpan(0, 0, 0, 0, 0)),
                            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                        };

                        Storyboard.SetTarget(animation, item);
                        Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Top)"));
                        storyboard.Children.Add(animation);
                    }
                    storyboard.Begin();

                    item.Width = _simpleWidth;
                    item.Height = _simpleHeight;
                    item.SetValue(Canvas.LeftProperty, Left);
                    item.SetValue(Canvas.TopProperty, _simpleTop);

                    _mapCanvasPoint[nPosIndex] = new Point(Left, _simpleTop);
                    _mapUIwithIndex[nPosIndex] = item;

                    Left += _simpleWidth;
                    ++nPosIndex;
                }

                ++nIndex;
            }

            return true;
        }

        private void FocusCarousel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == _displayItem)
                return;

            e.Handled = true;

            var vFrameWorker = sender as FrameworkElement;
            if (vFrameWorker == null)
                return;

            if (_displayItem?.Tag == null || !int.TryParse(_displayItem.Tag.ToString(), out var nIndex))
                return;

            if (!int.TryParse(vFrameWorker.Tag.ToString(), out var nLeaveIndex))
                return;

            var offset = 500;

            var vLeft = (_displayWidth - _simpleWidth) / 2d;
            var vTop = (_displayHeight - _simpleHeight) / 2d;

            //一系列计算 计算得到当前展示页要回到的Dock位置  
            //一系列计算 计算得到当前点击页要移除的Dock位置

            var nTargertIndex = nIndex;

            var nLeaveDockIndex = 0;
            foreach (var item in _mapUIwithIndex)
            {
                if (!int.TryParse(item.Value.Tag.ToString(), out var nItemIndex))
                    continue;

                if (nItemIndex == nLeaveIndex)
                {
                    nLeaveDockIndex = item.Key;
                    _mapUIwithIndex[item.Key] = null;
                    break;
                }
            }

            //如果目标位置Index是1那么他可以放在 0号位也可以放在1号位 主要是看他的前一个位置上的对象的Index是大还是小
            //判定 模拟演练 目标位置放入对象时 目标位置当前的对象时前移还是不动
            var vTargetFrame = _mapUIwithIndex.GetValueOrDefault(nTargertIndex);
            if (vTargetFrame != null)
                if (int.TryParse(vTargetFrame.Tag.ToString(), out var vTargetFrameIndex))
                {
                    //先判定 后续动作是 左移还是右移
                    bool? bLeft2Right = null;

                    if (nTargertIndex > nLeaveDockIndex)
                        bLeft2Right = false;
                    else if (nTargertIndex < nLeaveDockIndex)
                        bLeft2Right = true;

                    if (bLeft2Right == true)
                        if (vTargetFrameIndex < nIndex)
                            nTargertIndex++;

                    if (bLeft2Right == false)
                        if (vTargetFrameIndex > nIndex)
                            nTargertIndex--;
                }

            if (nIndex >= _mapCanvasPoint.Count)
                nTargertIndex = _mapCanvasPoint.Count - 1;

            if (nIndex < 0)
                nTargertIndex = 0;

            var point = _mapCanvasPoint.GetValueOrDefault(nTargertIndex);

            //定义动画 
            var storyboard = new Storyboard
            {
                SpeedRatio = 2
            };

            var nBegin = 250;
            if (nTargertIndex < nLeaveDockIndex)
                //全部右移
                for (var i = nLeaveDockIndex - 1; i >= nTargertIndex; --i)
                {
                    var vUI = _mapUIwithIndex.GetValueOrDefault(i);
                    if (vUI == null)
                        continue;

                    var vPoint = _mapCanvasPoint.GetValueOrDefault(i + 1);

                    var animation = new DoubleAnimation
                    {
                        To = vPoint.X,
                        BeginTime = TimeSpan.FromMilliseconds(nBegin),
                        Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };

                    Storyboard.SetTarget(animation, vUI);
                    Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Left)"));
                    storyboard.Children.Add(animation);

                    if (_mapUIwithIndex.ContainsKey(i + 1))
                        _mapUIwithIndex[i + 1] = vUI;

                    _mapUIwithIndex[i] = null;
                    //nBegin += nBegin;
                }
            else if (nTargertIndex > nLeaveDockIndex)
                //全部左移
                for (var i = nLeaveDockIndex + 1; i <= nTargertIndex; ++i)
                {
                    var vUI = _mapUIwithIndex.GetValueOrDefault(i);
                    if (vUI == null)
                        continue;

                    var vPoint = _mapCanvasPoint.GetValueOrDefault(i - 1);
                    var animation = new DoubleAnimation
                    {
                        To = vPoint.X,
                        BeginTime = TimeSpan.FromMilliseconds(nBegin),
                        Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                    };

                    Storyboard.SetTarget(animation, vUI);
                    Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Left)"));
                    storyboard.Children.Add(animation);

                    if (_mapUIwithIndex.ContainsKey(i - 1))
                        _mapUIwithIndex[i - 1] = vUI;

                    _mapUIwithIndex[i] = null;
                    //nBegin += nBegin;
                }

            if (_mapUIwithIndex.ContainsKey(nTargertIndex))
                _mapUIwithIndex[nTargertIndex] = _displayItem;

            //当前打开的界面 先缩放 位移 后 移到等待区
            {
                var animation = new DoubleAnimation
                {
                    To = _simpleWidth,
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                };

                Storyboard.SetTarget(animation, _displayItem);
                Storyboard.SetTargetProperty(animation, new PropertyPath("Width"));
                storyboard.Children.Add(animation);
            }

            {
                var animation = new DoubleAnimation
                {
                    To = _simpleHeight,
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                };

                Storyboard.SetTarget(animation, _displayItem);
                Storyboard.SetTargetProperty(animation, new PropertyPath("Height"));
                storyboard.Children.Add(animation);
            }

            {
                var animation = new DoubleAnimation
                {
                    To = vLeft,
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                };

                Storyboard.SetTarget(animation, _displayItem);
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Left)"));
                storyboard.Children.Add(animation);
            }

            {
                var animation = new DoubleAnimation
                {
                    To = vTop,
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                };

                Storyboard.SetTarget(animation, _displayItem);
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Top)"));
                storyboard.Children.Add(animation);
            }

            {
                var animation = new DoubleAnimation
                {
                    To = point.X,
                    BeginTime = TimeSpan.FromMilliseconds(offset),
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                };

                Storyboard.SetTarget(animation, _displayItem);
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Left)"));
                storyboard.Children.Add(animation);
            }

            {
                var animation = new DoubleAnimation
                {
                    To = point.Y,
                    BeginTime = TimeSpan.FromMilliseconds(offset),
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                };

                Storyboard.SetTarget(animation, _displayItem);
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Top)"));
                storyboard.Children.Add(animation);
            }

            //当前选中的界面 移动到目标位置 再放大位移
            {
                var animation = new DoubleAnimation
                {
                    To = vLeft,
                    BeginTime = TimeSpan.FromMilliseconds(offset),
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                };

                Storyboard.SetTarget(animation, vFrameWorker);
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Left)"));
                storyboard.Children.Add(animation);
            }

            {
                var animation = new DoubleAnimation
                {
                    To = vTop,
                    BeginTime = TimeSpan.FromMilliseconds(offset),
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                };

                Storyboard.SetTarget(animation, vFrameWorker);
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Top)"));
                storyboard.Children.Add(animation);
            }

            {
                var animation = new DoubleAnimation
                {
                    To = _displayWidth,
                    BeginTime = TimeSpan.FromMilliseconds(offset * 2),
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                };

                Storyboard.SetTarget(animation, vFrameWorker);
                Storyboard.SetTargetProperty(animation, new PropertyPath("Width"));
                storyboard.Children.Add(animation);
            }

            {
                var animation = new DoubleAnimation
                {
                    To = _displayHeight,
                    BeginTime = TimeSpan.FromMilliseconds(offset * 2),
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                };

                Storyboard.SetTarget(animation, vFrameWorker);
                Storyboard.SetTargetProperty(animation, new PropertyPath("Height"));
                storyboard.Children.Add(animation);
            }

            {
                var animation = new DoubleAnimation
                {
                    To = 0,
                    BeginTime = TimeSpan.FromMilliseconds(offset * 2),
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                };

                Storyboard.SetTarget(animation, vFrameWorker);
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Top)"));
                storyboard.Children.Add(animation);
            }

            {
                var animation = new DoubleAnimation
                {
                    To = 0,
                    BeginTime = TimeSpan.FromMilliseconds(offset * 2),
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
                };

                Storyboard.SetTarget(animation, vFrameWorker);
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Left)"));
                storyboard.Children.Add(animation);
            }

            ReplaceImageBitmap(vFrameWorker, _displayWidth, _displayHeight);
            ReplaceImageBitmap(_displayItem, _simpleWidth, _simpleHeight);

            _displayItem = vFrameWorker;
            storyboard.Begin(vFrameWorker);

            RaiseItemClick(vFrameWorker);
        }

        private void FocusCarousel_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private static void EnsureImageLoaded(FrameworkElement fe, double width, double height)
        {
            if (fe is Image img && img.Tag is string uri)
            {
                img.Source = ControlsHelper.CreateBitmapImage(uri, (int)width, (int)height);
                img.Tag = null;
            }
        }

        private static void ReplaceImageBitmap(FrameworkElement fe, double width, double height)
        {
            if (fe is Image img && img.Source is BitmapSource)
            {
                if (img.Tag is string uri)
                {
                    img.Source = ControlsHelper.CreateBitmapImage(uri, (int)width, (int)height);
                }
                else
                {
                    var w = (int)width;
                    var h = (int)height;
                    img.Source = ControlsHelper.CreateBitmapImage(
                        (img.Source as BitmapSource).ToString(), w, h);
                }
            }
        }

        private void FocusCarousel_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void FocusCarousel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            OnSizeChangedCallback();
        }
    }
}