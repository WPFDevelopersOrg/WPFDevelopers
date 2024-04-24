using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WPFDevelopers.Samples.ExampleViews
{
    public class TransformThumb : ContentControl
    {
        public Guid Id { get; private set; }
        public TransformThumb()
        {
            Id = Guid.NewGuid();
        }
        public Type ContentType
        {
            get { return (Type)GetValue(ContentTypeProperty); }
            set { SetValue(ContentTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentTypeProperty =
            DependencyProperty.Register("ContentType", typeof(Type), typeof(TransformThumb));

        // 选中事件

        // 点击事件

        public event EventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
        "Click", RoutingStrategy.Bubble, typeof(EventHandler), typeof(TransformThumb));


        public PrintState PrintState
        {
            get { return (PrintState)GetValue(PrintStateProperty); }
            set { SetValue(PrintStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PrintState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrintStateProperty =
            DependencyProperty.Register("PrintState", typeof(PrintState), typeof(TransformThumb), new PropertyMetadata(PrintState.Printing));




        public bool IsSeleted
        {
            get { return (bool)GetValue(IsSeletedProperty); }
            set { SetValue(IsSeletedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSeleted.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSeletedProperty =
            DependencyProperty.Register("IsSeleted", typeof(bool), typeof(TransformThumb), new FrameworkPropertyMetadata(false, (s, e) => {

                // 显示遮罩
                TransformThumb transform = s as TransformThumb;
                if (transform != null)
                {
                    // 判定为选中还是非选中
                    bool selected = (bool)e.NewValue;
                    string status = selected ? "选中" : "非选中";
                    // 控制选中效果
                    Debug.WriteLine($"节点：{transform.Id} 节点状态：{status}");
                    //transform.SetValue(ThumbTypeProperty, ThumbType.Style3);
                    UIElement element = transform as UIElement;
                    if (element == null)
                    {
                        return;
                    }
                    var adornerLayer = AdornerLayer.GetAdornerLayer(element);
                    if (adornerLayer != null)
                    {
                        var adorners = adornerLayer.GetAdorners(element);
                        Type oldtype = null;
                        switch (transform.ThumbType)
                        {
                            case ThumbType.Style1:
                                oldtype = typeof(ElementAdorner);
                                break;
                            case ThumbType.Style2:
                                oldtype = typeof(ThumbAdorner);
                                break;
                            case ThumbType.Style3:
                                oldtype = typeof(ThumbAdorner2);
                                break;
                            default:
                                oldtype = typeof(ThumbAdorner2);
                                break;
                        }
                        foreach (var adorner in adorners)
                        {
                            if (adorner.GetType() == oldtype)
                            {
                                adorner.Visibility = selected? Visibility.Visible:Visibility.Hidden;
                            }
                        }
                    }
                }
            }));


        public ShapeType ShapeType
        {
            get { return (ShapeType)GetValue(ShapeTypeProperty); }
            set { SetValue(ShapeTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShapeTypeProperty =
            DependencyProperty.Register("ShapeType", typeof(ShapeType), typeof(TransformThumb));

        public ThumbType ThumbType
        {
            get { return (ThumbType)GetValue(ThumbTypeProperty); }
            set { SetValue(ThumbTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThumbTypeProperty =
            DependencyProperty.Register("ThumbType", typeof(ThumbType), typeof(TransformThumb),new FrameworkPropertyMetadata(ThumbType.Style1, onPropertyChanged));

        private static void onPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateAdorner(d, e);
        }

        static TransformThumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TransformThumb), new FrameworkPropertyMetadata(typeof(TransformThumb)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            IsVisibleChanged += TransformThumb_IsVisibleChanged;
            CreateAdorner();
        }
        void CreateAdorner()
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(this);
            if (adornerLayer != null)
            {
                Adorner adorner;
                switch (ThumbType)
                {
                    case ThumbType.Style1:
                        adorner = new ElementAdorner(this);
                        break;
                    case ThumbType.Style2:
                        adorner = new ThumbAdorner(this);
                        break;
                    case ThumbType.Style3:
                        adorner = new ThumbAdorner2(this);
                        break;
                    default:
                        adorner = new ThumbAdorner2(this);
                        break;
                }
                if (adorner != null)
                {
                    adorner.Visibility = Visibility.Hidden;
                    adornerLayer.Add(adorner);
                }
            }
        }

        static void UpdateAdorner(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = d as UIElement;
            if (element == null)
            {
                return;
            }
            var adornerLayer = AdornerLayer.GetAdornerLayer(element as Visual);
            if (adornerLayer != null)
            {
                ThumbType oldvalue = (ThumbType)e.OldValue;
                var adorners = adornerLayer.GetAdorners(element as UIElement);
                Type oldtype = null;
                switch (oldvalue)
                {
                    case ThumbType.Style1:
                        oldtype = typeof(ElementAdorner);
                        break;
                    case ThumbType.Style2:
                        oldtype = typeof(ThumbAdorner);
                        break;
                    case ThumbType.Style3:
                        oldtype = typeof(ThumbAdorner2);
                        break;
                    default:
                        oldtype = typeof(ThumbAdorner2);
                        break;
                }
                foreach (var adorner in adorners) {
                    if (adorner.GetType() == oldtype)
                    {
                        adornerLayer.Remove(adorner);
                    }
                }

                ThumbType newvalue = (ThumbType)e.NewValue;
                Adorner Newadorner;
                switch (newvalue)
                {
                    case ThumbType.Style1:
                        Newadorner = new ElementAdorner(element);
                        break;
                    case ThumbType.Style2:
                        Newadorner = new ThumbAdorner(element);
                        break;
                    case ThumbType.Style3:
                        Newadorner = new ThumbAdorner2(element);
                        break;
                    default:
                        Newadorner = new ThumbAdorner2(element);
                        break;
                }
                if (Newadorner != null)
                {
                    adornerLayer.Add(Newadorner);
                }
            }
        }
        private void TransformThumb_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool isVisible)
            {
                if (isVisible)
                {
                    CreateAdorner();
                }

            }
        }

        public override string ToString()
        {
            return $"{Id}";
        }
    }
}
