using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class DrawerMenu : ListBox
    {
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(DrawerMenu), new PropertyMetadata(true));

        public static readonly DependencyProperty MenuIconColorProperty =
            DependencyProperty.Register("MenuIconColor", typeof(Brush), typeof(DrawerMenu),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SelectionIndicatorColorProperty =
            DependencyProperty.Register("SelectionIndicatorColor", typeof(Brush), typeof(DrawerMenu),
                new PropertyMetadata(null));

        public static readonly DependencyProperty MenuItemForegroundProperty =
            DependencyProperty.Register("MenuItemForeground", typeof(Brush), typeof(DrawerMenu),
                new PropertyMetadata(null));

        public static readonly DependencyProperty OpenIconProperty =
        DependencyProperty.Register("OpenIcon", typeof(object), typeof(DrawerMenu),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ClosedIconProperty =
            DependencyProperty.Register("ClosedIcon", typeof(object), typeof(DrawerMenu),
                new PropertyMetadata(null));

        static DrawerMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DrawerMenu),
                new FrameworkPropertyMetadata(typeof(DrawerMenu)));
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DrawerMenuItem;
        }
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DrawerMenuItem();
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
        public object OpenIcon
        {
            get => GetValue(OpenIconProperty);
            set => SetValue(OpenIconProperty, value);
        }

        public object ClosedIcon
        {
            get => GetValue(ClosedIconProperty);
            set => SetValue(ClosedIconProperty, value);
        }
    }
}