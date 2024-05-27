using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WPFDevelopers.Controls
{
    public class SliderRepeatButton: RepeatButton
    {
        public RadiusOrientation RadiusOrientation
        {
            get { return (RadiusOrientation)GetValue(RadiusOrientationProperty); }
            set { SetValue(RadiusOrientationProperty, value); }
        }

        public static readonly DependencyProperty RadiusOrientationProperty =
            DependencyProperty.Register("RadiusOrientation", typeof(RadiusOrientation), typeof(SliderRepeatButton), new PropertyMetadata(null));

    }

    [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    public enum RadiusOrientation
    {
        Down = ExpandDirection.Down,
        Up = ExpandDirection.Up,
        Left = ExpandDirection.Left,
        Right = ExpandDirection.Right,
        TopLeft,
        TopRight,
        BottomRight,
        BottomLeft
    }
}
