using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class CycleLoading : ContentControl
    {
        static CycleLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CycleLoading), new FrameworkPropertyMetadata(typeof(CycleLoading)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }



        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(CycleLoading), new PropertyMetadata(100d));



        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(CycleLoading), new PropertyMetadata(0d, OnValuePropertyChangedCallBack));



        internal string ValueDescription
        {
            get { return (string)GetValue(ValueDescriptionProperty); }
            set { SetValue(ValueDescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValueDescription.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ValueDescriptionProperty =
            DependencyProperty.Register("ValueDescription", typeof(string), typeof(CycleLoading), new PropertyMetadata(default(string)));




        private static void OnValuePropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is CycleLoading loading))
                return;

            if (!double.TryParse(e.NewValue?.ToString(), out double value))
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

            double dValue = value / loading.MaxValue;
            loading.ValueDescription = dValue.ToString("P0");
        }

        public bool IsStart
        {
            get { return (bool)GetValue(IsStartProperty); }
            set { SetValue(IsStartProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsStart.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsStartProperty =
            DependencyProperty.Register("IsStart", typeof(bool), typeof(CycleLoading), new PropertyMetadata(true));



        public string LoadTitle
        {
            get { return (string)GetValue(LoadTitleProperty); }
            set { SetValue(LoadTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LoadTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoadTitleProperty =
            DependencyProperty.Register("LoadTitle", typeof(string), typeof(CycleLoading), new PropertyMetadata(default(string)));



    }
}
