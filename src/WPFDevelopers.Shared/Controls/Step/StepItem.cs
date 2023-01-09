using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class StepItem : ContentControl
    {

        public static readonly DependencyProperty IndexProperty = DependencyProperty.Register(
            "Index", typeof(int), typeof(StepItem), new PropertyMetadata(-1));

        public int Index
        {
            get => (int)GetValue(IndexProperty);
            internal set => SetValue(IndexProperty, value);
        }


        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(
            "Status", typeof(Status), typeof(StepItem), new PropertyMetadata(Status.Waiting));


        public Status Status
        {
            get => (Status)GetValue(StatusProperty);
            internal set => SetValue(StatusProperty, value);
        }
    }
}
