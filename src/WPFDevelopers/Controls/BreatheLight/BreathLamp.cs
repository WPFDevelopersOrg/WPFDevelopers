using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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


        public Color EffectColor
        {
            get { return (Color)GetValue(EffectColorProperty); }
            set { SetValue(EffectColorProperty, value); }
        }

        public static readonly DependencyProperty EffectColorProperty =
            DependencyProperty.Register("EffectColor", typeof(Color), typeof(BreathLamp), new PropertyMetadata(Application.Current.Resources["DangerColor"]));


        public Color GradientStopColor1
        {
            get { return (Color)GetValue(GradientStopColor1Property); }
            set { SetValue(GradientStopColor1Property, value); }
        }
        public static readonly DependencyProperty GradientStopColor1Property =
            DependencyProperty.Register("GradientStopColor1", typeof(Color), typeof(BreathLamp), new PropertyMetadata(null));

        public Color GradientStopColor2
        {
            get { return (Color)GetValue(GradientStopColor2Property); }
            set { SetValue(GradientStopColor2Property, value); }
        }
        public static readonly DependencyProperty GradientStopColor2Property =
            DependencyProperty.Register("GradientStopColor2", typeof(Color), typeof(BreathLamp), new PropertyMetadata(null));


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
