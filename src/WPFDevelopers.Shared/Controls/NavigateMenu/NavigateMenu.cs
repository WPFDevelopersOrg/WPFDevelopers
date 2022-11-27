using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = ListBoxTemplateName, Type = typeof(ListBox))]
    [TemplatePart(Name = RectangleTemplateName, Type = typeof(Rectangle))]
    [TemplatePart(Name = TranslateTransformTemplateName, Type = typeof(TranslateTransform))]
    public class NavigateMenu: ListBox
    {
        private const string ListBoxTemplateName = "PART_ListBox";
        private const string RectangleTemplateName = "PART_RectangleSlider";
        private const string TranslateTransformTemplateName = "PART_TranslateTransform";

        private ListBox _listBox;
        private Rectangle _rectangle;
        private TranslateTransform _translateTransform;

        public Dock TabStripPlacement
        {
            get { return (Dock)GetValue(TabStripPlacementProperty); }
            set { SetValue(TabStripPlacementProperty, value); }
        }

        public static readonly DependencyProperty TabStripPlacementProperty =
            DependencyProperty.Register("TabStripPlacement", typeof(Dock), typeof(NavigateMenu), new PropertyMetadata(Dock.Left));



        public double RectangleSelectWidth
        {
            get { return (double)GetValue(RectangleSelectWidthProperty); }
            set { SetValue(RectangleSelectWidthProperty, value); }
        }

        public static readonly DependencyProperty RectangleSelectWidthProperty =
            DependencyProperty.Register("RectangleSelectWidth", typeof(double), typeof(NavigateMenu), new PropertyMetadata(40d));



        static NavigateMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigateMenu), new FrameworkPropertyMetadata(typeof(NavigateMenu)));
        }
     
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.ItemsSource == null )
            {

            }
            _listBox = GetTemplateChild(ListBoxTemplateName) as ListBox;
            _rectangle = GetTemplateChild(RectangleTemplateName) as Rectangle;
            _translateTransform = GetTemplateChild(TranslateTransformTemplateName) as TranslateTransform;
            SelectedIndex = 0;

        }
        public NavigateMenu()
        {
            //this.Loaded += (s, e) =>
            //{
            //    SelectedIndex = 0;
            //};
        }
      
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            switch (TabStripPlacement)
            {
                case Dock.Left:
                case Dock.Right:
                    if (_listBox == null) return;
                    object viewItem = _listBox.ItemContainerGenerator.ContainerFromItem(SelectedItem);
                    if (viewItem == null) return;
                    var currentItem = viewItem as ListBoxItem;
                    if (currentItem == null) return;
                    var offset = currentItem.TranslatePoint(new System.Windows.Point(0, 0), _rectangle).Y;
                    var animation = new DoubleAnimation()
                    {
                        To = offset,
                        Duration = TimeSpan.FromSeconds(0.3),
                    };
                    _translateTransform.BeginAnimation(TranslateTransform.YProperty, animation);
                    break;
                case Dock.Top:
                case Dock.Bottom:
                    viewItem = this.ItemContainerGenerator.ContainerFromItem(SelectedItem);
                    if (viewItem == null) return;
                    currentItem = viewItem as ListBoxItem;
                    RectangleSelectWidth = currentItem.ActualWidth;
                    offset = currentItem.TranslatePoint(new System.Windows.Point(0, 0), _rectangle).X;
                    animation = new DoubleAnimation()
                    {
                        To = offset,
                        Duration = TimeSpan.FromSeconds(0.3),
                    };
                    _translateTransform.BeginAnimation(TranslateTransform.XProperty, animation);
                    base.OnSelectionChanged(e);
                    break;
                default:
                    break;
            }
            
            //if (_listBox == null) return;
            //object viewItem = _listBox.ItemContainerGenerator.ContainerFromItem(SelectedItem);
            //if (viewItem == null) return;
            //var currentItem = viewItem as ListBoxItem;
            //if (currentItem == null) return;
            //RectangleSelectWidth = currentItem.ActualWidth;
            //var offset = 0d;
            //var animation = new DoubleAnimation()
            //{
            //    To = offset,
            //    Duration = TimeSpan.FromSeconds(0.3),
            //};
            //switch (TabStripPlacement)
            //{
            //    case Dock.Left:
            //    case Dock.Right:
            //        offset = currentItem.TranslatePoint(new System.Windows.Point(0, 0), _rectangle).Y;
            //        animation.To = offset;
            //        _translateTransform.BeginAnimation(TranslateTransform.YProperty, animation);
            //        break;
            //    case Dock.Top:
            //    case Dock.Bottom:
            //        offset = currentItem.TranslatePoint(new System.Windows.Point(0, 0), _rectangle).X;
            //        animation.To = offset;
            //        _translateTransform.BeginAnimation(TranslateTransform.XProperty, animation);
            //        break;
            //}


            //var offset = currentItem.TranslatePoint(new System.Windows.Point(0, 0), _rectangle).Y;
            //var animation = new DoubleAnimation()
            //{
            //    To = offset,
            //    Duration = TimeSpan.FromSeconds(0.3),
            //};
            //_translateTransform.BeginAnimation(TranslateTransform.YProperty, animation);
        }


    }
}
