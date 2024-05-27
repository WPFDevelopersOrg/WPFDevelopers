using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public class AnimationNavigationBar3DItem : ListBoxItem
    {
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(AnimationNavigationBar3DItem),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ContentBackProperty =
            DependencyProperty.Register("ContentBack", typeof(object), typeof(AnimationNavigationBar3DItem),
                new PropertyMetadata(null));

        static AnimationNavigationBar3DItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimationNavigationBar3DItem),
                new FrameworkPropertyMetadata(typeof(AnimationNavigationBar3DItem)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (ContentBack == null) 
                ContentBack = ControlsHelper.GetXmlReader(Content);
        }

        /// <summary>
        ///  Color fore
        /// </summary>
        public Brush Fill
        {
            get => (Brush)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        /// <summary>
        ///  The content after the mouse is moved in
        /// </summary>
        public object ContentBack
        {
            get => (object)GetValue(ContentBackProperty);
            set => SetValue(ContentBackProperty, value);
        }
    }
}