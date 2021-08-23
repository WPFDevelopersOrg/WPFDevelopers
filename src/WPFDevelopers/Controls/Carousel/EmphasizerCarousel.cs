using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WPFDevelopers.Controls.Helpers;

namespace WPFDevelopers.Controls
{

    [DefaultProperty("Children")]
    [ContentProperty("Children")]
    [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    [TemplatePart(Name = Part_BackCanvasName, Type = typeof(Canvas))]
    public class EmphasizerCarousel : Control, IAddChild
    {
        static EmphasizerCarousel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EmphasizerCarousel), new FrameworkPropertyMetadata(typeof(EmphasizerCarousel)));
        }

        public EmphasizerCarousel()
        {
            Loaded += EmphasizerCarousel_Loaded;
            Unloaded += EmphasizerCarousel_Unloaded;
            SizeChanged += EmphasizerCarousel_SizeChanged;
            Children.CollectionChanged += OnItems_CollectionChanged;
        }


        [ReadOnly(true)]
        const string Part_BackCanvasName = "PART_BackCanvas";

        Canvas Part_BackCanvas = default;

        private const int _maxSimpleHeight = 320;

        private int _SimpleCount = 0;

        private FrameworkElement _DisplayItem = null;

        private double _SimpleTop = 0;
        private double _SimpleHeight = 0;
        private double _SimpleWidth = 0;
        //private double _SimpleOffset = 10;

        private double _DisplayHeight = 0;
        private double _DisplayWidth = 0;
        private double _DisplayOffset = 10d;



        private Dictionary<int, Point> _mapCanvasPoint = new Dictionary<int, Point>();

        private Dictionary<int, FrameworkElement> _mapUIwithIndex = new Dictionary<int, FrameworkElement>();

        private ObservableCollection<FrameworkElement> _Children = new ObservableCollection<FrameworkElement>();
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<FrameworkElement> Children => _Children;

        private void OnItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is FrameworkElement element)
                    {
                        element.PreviewMouseLeftButtonDown += EmphasizerCarousel_MouseLeftButtonDown;

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
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is FrameworkElement element)
                        element.PreviewMouseLeftButtonDown -= EmphasizerCarousel_MouseLeftButtonDown;

                    if (item == _DisplayItem)
                        _DisplayItem = null;
                }
            }

            OnSizeChangedCallback();
        }

        #region override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Part_BackCanvas = GetTemplateChild(Part_BackCanvasName) as Canvas;

            if (Part_BackCanvas == null)
                throw new Exception("Some element is not in template!");
        }


        #endregion



        void IAddChild.AddChild(object value)
        {
            throw new NotImplementedException();
        }

        void IAddChild.AddText(string text)
        {
            throw new NotImplementedException();
        }


        private bool OnSizeChangedCallback()
        {
            if (Part_BackCanvas == null)
                return false;

            var vHeight = Part_BackCanvas.ActualHeight;
            var vWidth = Part_BackCanvas.ActualWidth;

            if (vHeight == Double.NaN || vWidth == Double.NaN)
                return false;

            if (vHeight == 0 || vWidth == 0)
                return false;

            Part_BackCanvas.Children.Clear();

            _mapUIwithIndex.Clear();
            _mapCanvasPoint.Clear();

            var vItemCount = Children.Count;
            if (vItemCount <= 0)
                return false;

            _SimpleCount = vItemCount - 1;
            if (_SimpleCount == 0)
            {
                Children[0].Width = vWidth;
                Children[0].Height = vHeight;

                Part_BackCanvas.Children.Add(Children[0]);
                return true;
            }

            //计算并划分显示区域
            var vSimpleHeight = vHeight * 0.4;
            _SimpleHeight = vSimpleHeight;
            if (_SimpleHeight > _maxSimpleHeight)
                _SimpleHeight = _maxSimpleHeight;

            var vSimpleWidth = vWidth / _SimpleCount;
            _SimpleWidth = vSimpleWidth;
            _SimpleTop = vHeight - _SimpleHeight;

            _DisplayHeight = vHeight - _SimpleHeight;
            _DisplayHeight -= _DisplayOffset;

            _DisplayWidth = vWidth;

            if (_DisplayItem == null)
                _DisplayItem = Children[0];

            int nIndex = 0;
            int nPosIndex = 0;
            double Left = 0;

            foreach (var item in Children)
            {
                Part_BackCanvas.Children.Add(item);
                item.Tag = nIndex;

                if (_DisplayItem == item)
                {
                    item.Width = _DisplayWidth;
                    item.Height = _DisplayHeight;
                    item.SetValue(Canvas.LeftProperty, 0d);
                    item.SetValue(Canvas.TopProperty, 0d);
                }
                else
                {
                    item.Width = _SimpleWidth;
                    item.Height = _SimpleHeight;
                    item.SetValue(Canvas.LeftProperty, Left);
                    item.SetValue(Canvas.TopProperty, _SimpleTop);

                    _mapCanvasPoint[nPosIndex] = new Point(Left, _SimpleTop);
                    _mapUIwithIndex[nPosIndex] = item;

                    Left += _SimpleWidth;
                    ++nPosIndex;
                }
                ++nIndex;
            }
            return true;
        }

        private void EmphasizerCarousel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == _DisplayItem)
                return;

            e.Handled = true;

            var vFrameWorker = sender as FrameworkElement;
            if (vFrameWorker == null)
                return;

            if (!int.TryParse(_DisplayItem.Tag.ToString(), out int nIndex))
                return;

            if (!int.TryParse(vFrameWorker.Tag.ToString(), out int nLeaveIndex))
                return;

            int offset = 500;

            var vLeft = (_DisplayWidth - _SimpleWidth) / 2d;
            var vTop = (_DisplayHeight - _SimpleHeight) / 2d;

            //一系列计算 计算得到当前展示页要回到的Dock位置  
            //一系列计算 计算得到当前点击页要移除的Dock位置

            int nTargertIndex = nIndex;

            int nLeaveDockIndex = 0;
            foreach (var item in _mapUIwithIndex)
            {
                if (!int.TryParse(item.Value.Tag.ToString(), out int nItemIndex))
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
            {
                if (int.TryParse(vTargetFrame.Tag.ToString(), out int vTargetFrameIndex))
                {
                    //先判定 后续动作是 左移还是右移
                    bool? bLeft2Right = null;

                    if (nTargertIndex > nLeaveDockIndex)
                        bLeft2Right = false;
                    else if (nTargertIndex < nLeaveDockIndex)
                        bLeft2Right = true;

                    if (bLeft2Right == true)
                    {
                        if (vTargetFrameIndex < nIndex)
                            nTargertIndex++;
                    }

                    if (bLeft2Right == false)
                    {
                        if (vTargetFrameIndex > nIndex)
                            nTargertIndex--;
                    }
                }
            }

            if (nIndex >= _mapCanvasPoint.Count)
                nTargertIndex = _mapCanvasPoint.Count - 1;

            if (nIndex < 0)
                nTargertIndex = 0;

            Point point = _mapCanvasPoint.GetValueOrDefault(nTargertIndex);

            //定义动画 
            Storyboard storyboard = new Storyboard
            {
                SpeedRatio = 2,
            };

            int nBegin = 250;
            if (nTargertIndex < nLeaveDockIndex)
            {
                //全部右移
                for (int i = nLeaveDockIndex - 1; i >= nTargertIndex; --i)
                {
                    var vUI = _mapUIwithIndex.GetValueOrDefault(i);
                    if (vUI == null)
                        continue;

                    var vPoint = _mapCanvasPoint.GetValueOrDefault(i + 1);

                    DoubleAnimation animation = new DoubleAnimation()
                    {
                        To = vPoint.X,
                        BeginTime = TimeSpan.FromMilliseconds(nBegin),
                        Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut },
                    };

                    Storyboard.SetTarget(animation, vUI);
                    Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Left)"));
                    storyboard.Children.Add(animation);

                    if (_mapUIwithIndex.ContainsKey(i + 1))
                        _mapUIwithIndex[i + 1] = vUI;

                    _mapUIwithIndex[i] = null;

                    //nBegin += nBegin;
                }
            }
            else if (nTargertIndex > nLeaveDockIndex)
            {
                //全部左移
                for (int i = nLeaveDockIndex + 1; i <= nTargertIndex; ++i)
                {
                    var vUI = _mapUIwithIndex.GetValueOrDefault(i);
                    if (vUI == null)
                        continue;

                    var vPoint = _mapCanvasPoint.GetValueOrDefault(i - 1);
                    DoubleAnimation animation = new DoubleAnimation()
                    {
                        To = vPoint.X,
                        BeginTime = TimeSpan.FromMilliseconds(nBegin),
                        Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                        EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut },
                    };

                    Storyboard.SetTarget(animation, vUI);
                    Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Left)"));
                    storyboard.Children.Add(animation);

                    if (_mapUIwithIndex.ContainsKey(i - 1))
                        _mapUIwithIndex[i - 1] = vUI;

                    _mapUIwithIndex[i] = null;

                    //nBegin += nBegin;
                }
            }

            if (_mapUIwithIndex.ContainsKey(nTargertIndex))
                _mapUIwithIndex[nTargertIndex] = _DisplayItem;

            //当前打开的界面 先缩放 位移 后 移到等待区
            {
                DoubleAnimation animation = new DoubleAnimation()
                {
                    To = _SimpleWidth,
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut },
                };

                Storyboard.SetTarget(animation, _DisplayItem);
                Storyboard.SetTargetProperty(animation, new PropertyPath("Width"));
                storyboard.Children.Add(animation);
            }

            {
                DoubleAnimation animation = new DoubleAnimation()
                {
                    To = _SimpleHeight,
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut },
                };

                Storyboard.SetTarget(animation, _DisplayItem);
                Storyboard.SetTargetProperty(animation, new PropertyPath("Height"));
                storyboard.Children.Add(animation);
            }

            {
                DoubleAnimation animation = new DoubleAnimation()
                {
                    To = vLeft,
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut },
                };

                Storyboard.SetTarget(animation, _DisplayItem);
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Left)"));
                storyboard.Children.Add(animation);
            }

            {
                DoubleAnimation animation = new DoubleAnimation()
                {
                    To = vTop,
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut },
                };

                Storyboard.SetTarget(animation, _DisplayItem);
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Top)"));
                storyboard.Children.Add(animation);
            }

            {

                DoubleAnimation animation = new DoubleAnimation()
                {
                    To = point.X,
                    BeginTime = TimeSpan.FromMilliseconds(offset),
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut },
                };

                Storyboard.SetTarget(animation, _DisplayItem);
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Left)"));
                storyboard.Children.Add(animation);
            }

            {
                DoubleAnimation animation = new DoubleAnimation()
                {
                    To = point.Y,
                    BeginTime = TimeSpan.FromMilliseconds(offset),
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut },
                };

                Storyboard.SetTarget(animation, _DisplayItem);
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Top)"));
                storyboard.Children.Add(animation);
            }

            //当前选中的界面 移动到目标位置 再放大位移
            {
                DoubleAnimation animation = new DoubleAnimation()
                {
                    To = vLeft,
                    BeginTime = TimeSpan.FromMilliseconds(offset),
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut },
                };

                Storyboard.SetTarget(animation, vFrameWorker);
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Left)"));
                storyboard.Children.Add(animation);
            }

            {
                DoubleAnimation animation = new DoubleAnimation()
                {
                    To = vTop,
                    BeginTime = TimeSpan.FromMilliseconds(offset),
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut },
                };

                Storyboard.SetTarget(animation, vFrameWorker);
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Top)"));
                storyboard.Children.Add(animation);
            }

            {
                DoubleAnimation animation = new DoubleAnimation()
                {
                    To = _DisplayWidth,
                    BeginTime = TimeSpan.FromMilliseconds(offset * 2),
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut },
                };

                Storyboard.SetTarget(animation, vFrameWorker);
                Storyboard.SetTargetProperty(animation, new PropertyPath("Width"));
                storyboard.Children.Add(animation);
            }

            {
                DoubleAnimation animation = new DoubleAnimation()
                {
                    To = _DisplayHeight,
                    BeginTime = TimeSpan.FromMilliseconds(offset * 2),
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut },
                };

                Storyboard.SetTarget(animation, vFrameWorker);
                Storyboard.SetTargetProperty(animation, new PropertyPath("Height"));
                storyboard.Children.Add(animation);
            }

            {
                DoubleAnimation animation = new DoubleAnimation()
                {
                    To = 0,
                    BeginTime = TimeSpan.FromMilliseconds(offset * 2),
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut },
                };

                Storyboard.SetTarget(animation, vFrameWorker);
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Top)"));
                storyboard.Children.Add(animation);
            }

            {
                DoubleAnimation animation = new DoubleAnimation()
                {
                    To = 0,
                    BeginTime = TimeSpan.FromMilliseconds(offset * 2),
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, offset)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut },
                };

                Storyboard.SetTarget(animation, vFrameWorker);
                Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Left)"));
                storyboard.Children.Add(animation);
            }

            _DisplayItem = vFrameWorker;
            storyboard.Begin(vFrameWorker);
        }

        private void EmphasizerCarousel_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void EmphasizerCarousel_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void EmphasizerCarousel_SizeChanged(object sender, SizeChangedEventArgs e) => OnSizeChangedCallback();

    }
}
