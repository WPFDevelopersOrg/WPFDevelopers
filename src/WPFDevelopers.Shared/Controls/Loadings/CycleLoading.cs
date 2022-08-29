using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class CycleLoading : ContentControl
    {
        // Using a DependencyProperty as the backing store for MaxValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(CycleLoading), new PropertyMetadata(100d));

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(CycleLoading),
                new PropertyMetadata(0d, OnValuePropertyChangedCallBack));

        // Using a DependencyProperty as the backing store for ValueDescription.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ValueDescriptionProperty =
            DependencyProperty.Register("ValueDescription", typeof(string), typeof(CycleLoading),
                new PropertyMetadata(default(string)));

        // Using a DependencyProperty as the backing store for IsStart.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsStartProperty =
            DependencyProperty.Register("IsStart", typeof(bool), typeof(CycleLoading), new PropertyMetadata(true));

        // Using a DependencyProperty as the backing store for LoadTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoadTitleProperty =
            DependencyProperty.Register("LoadTitle", typeof(string), typeof(CycleLoading),
                new PropertyMetadata(default(string)));

        static CycleLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CycleLoading),
                new FrameworkPropertyMetadata(typeof(CycleLoading)));
        }


        public double MaxValue
        {
            get => (double)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }


        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }


        internal string ValueDescription
        {
            get => (string)GetValue(ValueDescriptionProperty);
            set => SetValue(ValueDescriptionProperty, value);
        }

        public bool IsStart
        {
            get => (bool)GetValue(IsStartProperty);
            set => SetValue(IsStartProperty, value);
        }


        public string LoadTitle
        {
            get => (string)GetValue(LoadTitleProperty);
            set => SetValue(LoadTitleProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }


        private static void OnValuePropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is CycleLoading loading))
                return;

            if (!double.TryParse(e.NewValue?.ToString(), out var value))
                return;

            if (value >= loading.MaxValue)
            {
                value = loading.MaxValue;

                if (loading.IsStart)
                    loading.IsStart = false;
            }
            else
            {
                if (!loading.IsStart)
                    loading.IsStart = true;
            }

            var dValue = value / loading.MaxValue;
            loading.ValueDescription = dValue.ToString("P0");
        }
    }
}