using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class DrawerMenu : ContentControl
    {
        public new List<DrawerMenuItem> Content
        {
            get { return (List<DrawerMenuItem>)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public new static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(List<DrawerMenuItem>), typeof(DrawerMenu),new FrameworkPropertyMetadata(null));

        static DrawerMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DrawerMenu), new FrameworkPropertyMetadata(typeof(DrawerMenu)));
        }

        public override void BeginInit()
        {
            Content = new List<DrawerMenuItem>();
            base.BeginInit();
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set
            {
                SetValue(IsOpenProperty, value);
            }
        }

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(DrawerMenu), new PropertyMetadata(true));


        public Brush MenuIconColor
        {
            get { return (Brush)GetValue(MenuIconColorProperty); }
            set { SetValue(MenuIconColorProperty, value); }
        }

        public static readonly DependencyProperty MenuIconColorProperty =
            DependencyProperty.Register("MenuIconColor", typeof(Brush), typeof(DrawerMenu), new PropertyMetadata(Brushes.White));


        public Brush SelectionIndicatorColor
        {
            get { return (Brush)GetValue(SelectionIndicatorColorProperty); }
            set { SetValue(SelectionIndicatorColorProperty, value); }
        }

        public static readonly DependencyProperty SelectionIndicatorColorProperty =
            DependencyProperty.Register("SelectionIndicatorColor", typeof(Brush), typeof(DrawerMenu), new PropertyMetadata(DrawingContextHelper.Brush));

        public Brush MenuItemForeground
        {
            get { return (Brush)GetValue(MenuItemForegroundProperty); }
            set { SetValue(MenuItemForegroundProperty, value); }
        }

        public static readonly DependencyProperty MenuItemForegroundProperty =
            DependencyProperty.Register("MenuItemForeground", typeof(Brush), typeof(DrawerMenu), new PropertyMetadata(Brushes.Transparent));
    }
}
