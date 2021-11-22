using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class RingLoading : Control
    {
        static RingLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RingLoading), new FrameworkPropertyMetadata(typeof(RingLoading)));
        }

        public bool IsStart
        {
            get { return (bool)GetValue(IsStartProperty); }
            set { SetValue(IsStartProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsStart.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsStartProperty =
            DependencyProperty.Register("IsStart", typeof(bool), typeof(RingLoading), new PropertyMetadata(default));


        public double ProgressValue
        {
            get { return (double)GetValue(ProgressValueProperty); }
            set { SetValue(ProgressValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProgressValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressValueProperty =
            DependencyProperty.Register("ProgressValue", typeof(double), typeof(RingLoading), new PropertyMetadata(0d, OnProgressValueChangedCallBack));

        private static void OnProgressValueChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is RingLoading control))
                return;

            if (!double.TryParse(e.NewValue?.ToString(), out double value))
                return;

            double progress = value / control.Maximum;
            control.SetCurrentValue(ProgressProperty, progress.ToString("P0"));
        }


        internal string Progress
        {
            get { return (string)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Progress.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(string), typeof(RingLoading), new PropertyMetadata(default));


        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(RingLoading), new PropertyMetadata(100d, OnMaximumPropertyChangedCallBack));

        private static void OnMaximumPropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is RingLoading control))
                return;

            if (!double.TryParse(e.NewValue?.ToString(), out double maxValue))
                return;

            if (maxValue <= 0)
                return;

            double progress = control.ProgressValue / maxValue;
            control.SetCurrentValue(ProgressProperty, progress.ToString("P0"));
        }

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(RingLoading), new PropertyMetadata(default));






    }
}
