using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Helpers
{
    public class TreeViewHelper
    {
        private static readonly DependencyProperty MouseHandlerProperty =
    DependencyProperty.RegisterAttached("MouseHandler", typeof(MouseButtonEventHandler), typeof(TreeViewHelper), new PropertyMetadata(null));

        private static readonly DependencyProperty ExpandHandlerProperty =
            DependencyProperty.RegisterAttached("ExpandHandler", typeof(RoutedEventHandler), typeof(TreeViewHelper), new PropertyMetadata(null));

        public static readonly DependencyProperty IsScrollAnimationProperty =
            DependencyProperty.RegisterAttached("IsScrollAnimation", typeof(bool), typeof(TreeViewHelper),
                new FrameworkPropertyMetadata(false));

        public static object GetIsScrollAnimation(DependencyObject obj)
        {
            return obj.GetValue(IsScrollAnimationProperty);
        }

        public static void SetIsScrollAnimation(DependencyObject obj, object value)
        {
            obj.SetValue(IsScrollAnimationProperty, value);
        }

        public static readonly DependencyProperty PreserveScrollOnExpandProperty =
            DependencyProperty.RegisterAttached(
                "PreserveScrollOnExpand",
                typeof(bool),
                typeof(TreeViewHelper),
                new PropertyMetadata(false, OnPreserveScrollOnExpandChanged));

        public static void SetPreserveScrollOnExpand(DependencyObject element, bool value) =>
           element.SetValue(PreserveScrollOnExpandProperty, value);

        public static bool GetPreserveScrollOnExpand(DependencyObject element) =>
            (bool)element.GetValue(PreserveScrollOnExpandProperty);


        public static readonly DependencyProperty LastOffsetProperty =
    DependencyProperty.RegisterAttached("LastOffset", typeof(double), typeof(TreeViewHelper), new PropertyMetadata(0.0));

        internal static void SetLastOffset(DependencyObject obj, double value) => obj.SetValue(LastOffsetProperty, value);
        public static double GetLastOffset(DependencyObject obj) => (double)obj.GetValue(LastOffsetProperty);

        private static void OnPreserveScrollOnExpandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeView treeView)
            {
                if ((bool)e.NewValue)
                {
                    var mouseHandler = new MouseButtonEventHandler(OnMouseDown);
                    var expandHandler = new RoutedEventHandler(OnItemExpanded);

                    treeView.SetValue(MouseHandlerProperty, mouseHandler);
                    treeView.SetValue(ExpandHandlerProperty, expandHandler);

                    treeView.AddHandler(TreeViewItem.PreviewMouseLeftButtonDownEvent, mouseHandler, true);
                    treeView.AddHandler(TreeViewItem.ExpandedEvent, expandHandler, true);
                }
                else
                {
                    if (treeView.GetValue(MouseHandlerProperty) is MouseButtonEventHandler mouseHandler)
                        treeView.RemoveHandler(TreeViewItem.PreviewMouseLeftButtonDownEvent, mouseHandler);

                    if (treeView.GetValue(ExpandHandlerProperty) is RoutedEventHandler expandHandler)
                        treeView.RemoveHandler(TreeViewItem.ExpandedEvent, expandHandler);
                }
            }
        }
        private static void OnMouseDown(object sender, MouseButtonEventArgs args)
        {
            if (sender is TreeView treeView)
            {
                var viewer = ControlsHelper.FindVisualChild<ScrollViewer>(treeView);
                if (viewer != null)
                    SetLastOffset(treeView, viewer.VerticalOffset);
            }
        }

        private static void OnItemExpanded(object sender, RoutedEventArgs args)
        {
            if (sender is TreeView treeView)
            {
                var scrollViewer = ControlsHelper.FindVisualChild<ScrollViewer>(treeView);
                if (scrollViewer == null)
                    return;

                var lastOffset = GetLastOffset(treeView);
                treeView.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                {
                    if (scrollViewer is WDScrollViewer wd)
                        wd.AnimateScroll(lastOffset);
                    else
                        scrollViewer.ScrollToVerticalOffset(lastOffset);
                }));
            }
        }
    }
}
