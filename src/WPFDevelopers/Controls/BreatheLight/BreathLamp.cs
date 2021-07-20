using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public enum LampEffect
    {
        OuterGlow,
        Eclipse,
        Ripple
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


        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(BreathLamp), new PropertyMetadata(new CornerRadius(60d)));

        // Using a DependencyProperty as the backing store for LampEffect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LampEffectProperty =
            DependencyProperty.Register("LampEffect", typeof(LampEffect), typeof(BreathLamp), new PropertyMetadata(default(LampEffect), OnLampEffectPropertyChangedCallBack));

        // Using a DependencyProperty as the backing store for IsLampStart.  This enables animation, styling, binding, etc...
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
            //throw new NotImplementedException();
        }

        public bool IsLampStart
        {
            get { return (bool)GetValue(IsLampStartProperty); }
            set { SetValue(IsLampStartProperty, value); }
        }
    }
}
