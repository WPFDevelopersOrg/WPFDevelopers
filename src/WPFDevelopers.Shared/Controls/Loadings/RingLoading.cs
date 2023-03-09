using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class RingLoading : Control
    {
        public static readonly DependencyProperty IsStartProperty =
            DependencyProperty.Register("IsStart", typeof(bool), typeof(RingLoading), new PropertyMetadata(default));

        public static readonly DependencyProperty ProgressValueProperty =
            DependencyProperty.Register("ProgressValue", typeof(double), typeof(RingLoading),
                new PropertyMetadata(0d, OnProgressValueChangedCallBack));

        internal static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(string), typeof(RingLoading), new PropertyMetadata(default));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(RingLoading),
                new PropertyMetadata(100d, OnMaximumPropertyChangedCallBack));

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(RingLoading),
                new PropertyMetadata(default));

        static RingLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RingLoading),
                new FrameworkPropertyMetadata(typeof(RingLoading)));
        }

        public bool IsStart
        {
            get => (bool)GetValue(IsStartProperty);
            set => SetValue(IsStartProperty, value);
        }


        public double ProgressValue
        {
            get => (double)GetValue(ProgressValueProperty);
            set => SetValue(ProgressValueProperty, value);
        }


        internal string Progress
        {
            get => (string)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }


        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        private static void OnProgressValueChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is RingLoading control))
                return;

            if (!double.TryParse(e.NewValue?.ToString(), out var value))
                return;

            var progress = value / control.Maximum;
            control.SetCurrentValue(ProgressProperty, progress.ToString("P0"));
        }

        private static void OnMaximumPropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is RingLoading control))
                return;

            if (!double.TryParse(e.NewValue?.ToString(), out var maxValue))
                return;

            if (maxValue <= 0)
                return;

            var progress = control.ProgressValue / maxValue;
            control.SetCurrentValue(ProgressProperty, progress.ToString("P0"));
        }
    }
}