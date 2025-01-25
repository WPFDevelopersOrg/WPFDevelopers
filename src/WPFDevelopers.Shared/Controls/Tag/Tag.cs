using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFDevelopers.Controls
{
    public class Tag :ContentControl
    {
        private static RoutedCommand _closeCommand = null;
        public bool IsClose
        {
            get { return (bool)GetValue(IsCloseProperty); }
            set { SetValue(IsCloseProperty, value); }
        }

        public static readonly DependencyProperty IsCloseProperty =
            DependencyProperty.Register("IsClose", typeof(bool), typeof(Tag), new PropertyMetadata(true));


        public event RoutedEventHandler Close
        {
            add { AddHandler(CloseEvent, value); }
            remove { RemoveHandler(CloseEvent, value); }
        }

        public static readonly RoutedEvent CloseEvent =
            EventManager.RegisterRoutedEvent("Close", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Tag));

        public static RoutedCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        static Tag()
        {
            _closeCommand = new RoutedCommand("Close", typeof(Tag));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Tag), new FrameworkPropertyMetadata(typeof(Tag)));
            CommandManager.RegisterClassCommandBinding(typeof(Tag),new CommandBinding(CloseCommand, OnCloseExecuted));
        }
        private static void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is Tag tag)
                tag.OnClose();  
        }

        protected virtual void OnClose()
        {
            var args = new RoutedEventArgs(CloseEvent,this);
            RaiseEvent(args);
        }
    }
}
