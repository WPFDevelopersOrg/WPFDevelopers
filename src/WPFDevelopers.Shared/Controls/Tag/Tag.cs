using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class Tag : ContentControl
    {
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(Tag), new PropertyMetadata(null));

        public static readonly DependencyProperty LabelBackgroundProperty =
            DependencyProperty.Register("LabelBackground", typeof(Brush), typeof(Tag), new PropertyMetadata(Brushes.DimGray));

        public static readonly DependencyProperty LabelForegroundProperty =
            DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(Tag), new PropertyMetadata(Brushes.White));

        public static readonly DependencyProperty IsCloseProperty =
            DependencyProperty.Register("IsClose", typeof(bool), typeof(Tag), new PropertyMetadata(true));

        public static readonly RoutedEvent CloseEvent =
            EventManager.RegisterRoutedEvent("Close", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Tag));

        static Tag()
        {
            CloseCommand = new RoutedCommand("Close", typeof(Tag));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Tag), new FrameworkPropertyMetadata(typeof(Tag)));
            CommandManager.RegisterClassCommandBinding(typeof(Tag), new CommandBinding(CloseCommand, OnCloseExecuted));
        }

        public bool IsClose
        {
            get => (bool) GetValue(IsCloseProperty);
            set => SetValue(IsCloseProperty, value);
        }

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public Brush LabelBackground
        {
            get => (Brush)GetValue(LabelBackgroundProperty);
            set => SetValue(LabelBackgroundProperty, value);
        }

        public Brush LabelForeground
        {
            get => (Brush)GetValue(LabelForegroundProperty);
            set => SetValue(LabelForegroundProperty, value);
        }

        public static RoutedCommand CloseCommand { get; }


        public event RoutedEventHandler Close
        {
            add => AddHandler(CloseEvent, value);
            remove => RemoveHandler(CloseEvent, value);
        }

        private static void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is Tag tag)
                tag.OnClose();
        }

        protected virtual void OnClose()
        {
            var args = new RoutedEventArgs(CloseEvent, this);
            RaiseEvent(args);
        }
    }
}