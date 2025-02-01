using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public class NavMenu3DItem : ListBoxItem
    {
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(NavMenu3DItem),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ContentBackProperty =
            DependencyProperty.Register("ContentBack", typeof(object), typeof(NavMenu3DItem),
                new PropertyMetadata(null));

        static NavMenu3DItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavMenu3DItem),
                new FrameworkPropertyMetadata(typeof(NavMenu3DItem)));
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