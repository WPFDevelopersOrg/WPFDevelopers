using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public enum LampEffect
    {
        OuterGlow,
        Eclipse,
        Ripple,
        Streamer
    }

    public class BreathLamp : ContentControl
    {
        static BreathLamp()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BreathLamp), new FrameworkPropertyMetadata(typeof(BreathLamp)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }


        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(BreathLamp), new PropertyMetadata(new CornerRadius(60d)));

        public static readonly DependencyProperty LampEffectProperty =
            DependencyProperty.Register("LampEffect", typeof(LampEffect), typeof(BreathLamp), new PropertyMetadata(default(LampEffect), OnLampEffectPropertyChangedCallBack));

        public static readonly DependencyProperty IsLampStartProperty =
            DependencyProperty.Register("IsLampStart", typeof(bool), typeof(BreathLamp), new PropertyMetadata(true));


        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public LampEffect LampEffect
        {
            get { return (LampEffect)GetValue(LampEffectProperty); }
            set { SetValue(LampEffectProperty, value); }
        }

        private static void OnLampEffectPropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public bool IsLampStart
        {
            get { return (bool)GetValue(IsLampStartProperty); }
            set { SetValue(IsLampStartProperty, value); }
        }
    }
}
