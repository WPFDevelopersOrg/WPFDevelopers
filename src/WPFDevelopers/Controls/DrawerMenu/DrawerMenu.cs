using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class DrawerMenu : ContentControl
    {
        public new static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(List<DrawerMenuItem>), typeof(DrawerMenu),
                new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(DrawerMenu), new PropertyMetadata(true));

        public static readonly DependencyProperty MenuIconColorProperty =
            DependencyProperty.Register("MenuIconColor", typeof(Brush), typeof(DrawerMenu),
                new PropertyMetadata(Brushes.White));

        public static readonly DependencyProperty SelectionIndicatorColorProperty =
            DependencyProperty.Register("SelectionIndicatorColor", typeof(Brush), typeof(DrawerMenu),
                new PropertyMetadata(DrawingContextHelper.Brush));

        public static readonly DependencyProperty MenuItemForegroundProperty =
            DependencyProperty.Register("MenuItemForeground", typeof(Brush), typeof(DrawerMenu),
                new PropertyMetadata(Brushes.Transparent));

        static DrawerMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DrawerMenu),
                new FrameworkPropertyMetadata(typeof(DrawerMenu)));
        }

        public new List<DrawerMenuItem> Content
        {
            get => (List<DrawerMenuItem>)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }


        public Brush MenuIconColor
        {
            get => (Brush)GetValue(MenuIconColorProperty);
            set => SetValue(MenuIconColorProperty, value);
        }


        public Brush SelectionIndicatorColor
        {
            get => (Brush)GetValue(SelectionIndicatorColorProperty);
            set => SetValue(SelectionIndicatorColorProperty, value);
        }

        public Brush MenuItemForeground
        {
            get => (Brush)GetValue(MenuItemForegroundProperty);
            set => SetValue(MenuItemForegroundProperty, value);
        }

        public override void BeginInit()
        {
            Content = new List<DrawerMenuItem>();
            base.BeginInit();
        }
    }
}