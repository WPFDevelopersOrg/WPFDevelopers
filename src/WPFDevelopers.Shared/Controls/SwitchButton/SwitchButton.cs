using System.Windows;
using System.Windows.Controls.Primitives;

namespace WPFDevelopers.Controls
{
    public class SwitchButton : ToggleButton
    {
        public static readonly DependencyProperty CheckedContentProperty =
            DependencyProperty.Register(nameof(CheckedContent), typeof(object), typeof(SwitchButton),
                new PropertyMetadata(null));

        public static readonly DependencyProperty UncheckedContentProperty =
            DependencyProperty.Register(nameof(UncheckedContent), typeof(object), typeof(SwitchButton),
                new PropertyMetadata(null));

        static SwitchButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SwitchButton),
                new FrameworkPropertyMetadata(typeof(SwitchButton)));
        }

        public object CheckedContent
        {
            get => GetValue(CheckedContentProperty);
            set => SetValue(CheckedContentProperty, value);
        }

        public object UncheckedContent
        {
            get => GetValue(UncheckedContentProperty);
            set => SetValue(UncheckedContentProperty, value);
        }
    }
}
