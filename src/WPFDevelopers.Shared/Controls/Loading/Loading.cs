using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public static class Loading
    {
        /// <summary>
        /// End your current task after loading the control and exiting
        /// </summary>
        public static event EventHandler<EventArgs> LoadingQuitEvent;
        /// <summary>
        /// Is Loading Run
        /// </summary>
        public static bool IsLoadingRun;

        private static ContentControl win;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentControl">ContentControl</param>
        /// <param name="size">LoadingSize</param>
        /// <param name="borderBrush">LoadingBrush</param>
        public static void Show(ContentControl contentControl,double size = 18d, Brush borderBrush = null)
        {
            if (contentControl == null || IsLoadingRun) return;
            Close();
            win = contentControl;
            var original = new Grid();
            original.Opacity = .4;
            if (win.Content is UIElement)
            {
                var uIElement = win.Content as UIElement;
                win.Content = null;
                original.Children.Add(uIElement);
            }
            else
            {
                if (win.Content.GetType() == typeof(string))
                {
                    var text = new TextBlock { Text = win.Content.ToString(),Foreground = win.Foreground };
                    win.Content = null;
                    original.Children.Add(new Border { Child = text });
                }
            }

            var layer = new Grid();
            var loading = new WPFLoading();
            var _size = contentControl.ActualHeight < contentControl.ActualWidth ? contentControl.ActualHeight : contentControl.ActualWidth;
            size = size < _size / 2 ? size : _size / 2 ;
            loading.Width = size;
            loading.Height = size;
            if (borderBrush != null)
                loading.BorderBrush = borderBrush;
            layer.Children.Add(loading);
            var container = new Grid();
            container.Children.Add(original);
            container.Children.Add(layer);
            win.Content = container;
            Console.WriteLine($"02win.ActualHeight:{win.ActualHeight}");
            IsLoadingRun = true;
          

            //var loading = new WPFLoading();
            //loading.Width = size;
            //loading.Height = size;
            //if (borderBrush != null)
            //    loading.BorderBrush = borderBrush;
            //layer.Children.Add(loading);

            //win.Content = layer;
            //IsLoadingRun = true;
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="showOff">Whether to enable the close button</param>
        public static void Show(bool showOff = false)
        {
            Close();
            if (Application.Current.Windows.Count > 0)
                win = Application.Current.Windows.OfType<Window>().FirstOrDefault(o => o.IsActive);
            if (win != null)
            {
                var layer = new Grid();
                layer.Children.Add(new Rectangle { Fill = ControlsHelper.Brush, Opacity = .7 });
                var loading = new WPFLoading();
                layer.Children.Add(loading);
                if (showOff)
                {
                    var btnClose = new Button()
                    {
                        Style = Application.Current.FindResource("PathButton") as Style,
                        Content = new Path
                        {
                            Data = Application.Current.FindResource("PathMetroWindowClose") as Geometry,
                            Stretch = Stretch.Fill,
                            Width = 10,
                            Height = 10,
                            Fill = Application.Current.Resources["PrimaryTextSolidColorBrush"] as Brush,//ControlHelper.WindowForegroundBrush,
                        }
                    };
                    btnClose.Click += delegate
                    {
                        Close();
                        LoadingQuitEvent?.Invoke(null, EventArgs.Empty);
                    };
                    layer.Children.Add(btnClose);
                }
                var original = win.Content as UIElement;
                win.Content = null;
                var container = new Grid();
                container.Children.Add(original);
                container.Children.Add(layer);
                win.Content = container;
                IsLoadingRun = true;
            }
        }
        /// <summary>
        /// Exit Loading
        /// </summary>
        public static void Close()
        {
            if (!IsLoadingRun) return;
            if (win == null) return;
            var grid = win.Content as Grid;
            if (grid == null) return;
            var original = VisualTreeHelper.GetChild(grid, 0) as UIElement;
            grid.Children.Remove(original);
            if(original.Opacity != 1)original.Opacity = 1;
            win.Content = original;
            IsLoadingRun = false;
        }
    }
}