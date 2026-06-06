using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFDevelopers.Controls
{
    public class StatefulControlBase : Control
    {
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(ControlState), typeof(StatefulControlBase),
                new PropertyMetadata(ControlState.None));

        public static readonly RoutedEvent CompletedEvent =
            EventManager.RegisterRoutedEvent("Completed", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
                typeof(StatefulControlBase));

        public static readonly DependencyProperty CompletedCommandProperty =
            DependencyProperty.Register("CompletedCommand", typeof(ICommand), typeof(StatefulControlBase),
                new PropertyMetadata(null));

        public ControlState State
        {
            get => (ControlState)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        public event RoutedEventHandler Completed
        {
            add { AddHandler(CompletedEvent, value); }
            remove { RemoveHandler(CompletedEvent, value); }
        }

        public ICommand CompletedCommand
        {
            get => (ICommand)GetValue(CompletedCommandProperty);
            set => SetValue(CompletedCommandProperty, value);
        }

        protected void RaiseCompleted(object parameter = null)
        {
            RaiseEvent(new RoutedEventArgs(CompletedEvent));
            CompletedCommand?.Execute(parameter);
        }
    }
}
