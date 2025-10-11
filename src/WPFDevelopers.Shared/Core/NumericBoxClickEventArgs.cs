using System.Windows;

namespace WPFDevelopers.Core
{
    public class NumericBoxClickEventArgs : RoutedEventArgs
    {
        public bool SkipStepChange { get; set; }
        public NumericBoxClickEventArgs(RoutedEvent routedEvent) : base(routedEvent) { }
    }
}
