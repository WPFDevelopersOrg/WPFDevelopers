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

        static NavigateMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigateMenu), new FrameworkPropertyMetadata(typeof(NavigateMenu)));
        }
     
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _listBox = GetTemplateChild(ListBoxTemplateName) as ListBox;
            _rectangle = GetTemplateChild(RectangleTemplateName) as Rectangle;
            _translateTransform = GetTemplateChild(TranslateTransformTemplateName) as TranslateTransform;
        }
        public NavigateMenu()
        {
            this.Loaded += (s, e) =>
            {
                SelectedIndex = 0;
            };
        }
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
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
        }
       

    }
}
