using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using WPFDevelopers.Utilities;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = HueSliderColorTemplateName, Type = typeof(Slider))]
    [TemplatePart(Name = CanvasTemplateName, Type = typeof(Canvas))]
    [TemplatePart(Name = ThumbTemplateName, Type = typeof(Thumb))]
    [TemplatePart(Name = ButtonTemplateName, Type = typeof(Button))]
    public class ColorPicker : Control
    {
        private const string HueSliderColorTemplateName = "PART_HueSlider";

        private const string CanvasTemplateName = "PART_Canvas";

        private const string ThumbTemplateName = "PART_Thumb";

        private const string ButtonTemplateName = "PART_Button";

        private static readonly DependencyPropertyKey HueColorPropertyKey =
            DependencyProperty.RegisterReadOnly("HueColor", typeof(Color), typeof(ColorPicker),
                new PropertyMetadata(Colors.Red));

        public static readonly DependencyProperty HueColorProperty = HueColorPropertyKey.DependencyProperty;

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker),
                new FrameworkPropertyMetadata(Colors.Red, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedColorChanged));

        private static readonly DependencyPropertyKey HSBPropertyKey =
            DependencyProperty.RegisterReadOnly("HSB", typeof(HSB), typeof(ColorPicker),
                new PropertyMetadata(new HSB()));

        public static readonly DependencyProperty HSBHProperty = HSBPropertyKey.DependencyProperty;

        public static readonly DependencyProperty ColorTypeProperty =
            DependencyProperty.Register("ColorType", typeof(ColorTypeEnum), typeof(ColorPicker),
                new PropertyMetadata(ColorTypeEnum.RGB));

        private Button _button;

        private Canvas _canvas;

        private Slider _hueSliderColor;

        private bool _isInnerUpdateSelectedColor;

        private Thumb _thumb;

        private ColorTypeEnum[] colorTypeEnums;

        private int currentGridStateIndex;


        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker),
                new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }

        public Color HueColor => (Color) GetValue(HueColorProperty);

        public Color SelectedColor
        {
            get => (Color) GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public HSB HSB => (HSB) GetValue(HSBHProperty);


        public ColorTypeEnum ColorType
        {
            get => (ColorTypeEnum) GetValue(ColorTypeProperty);
            set => SetValue(ColorTypeProperty, value);
        }

        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as ColorPicker;
            if (ctrl._isInnerUpdateSelectedColor)
            {
                ctrl._isInnerUpdateSelectedColor = false;
                return;
            }

            var color = (Color) e.NewValue;
            double h = 0, s = 0, b = 0;
            ColorUtil.HsbFromColor(color, ref h, ref s, ref b);
            var hsb = new HSB {H = h, S = s, B = b};
            ctrl.SetValue(HueColorPropertyKey, ColorUtil.ColorFromHsb(hsb.H, 1, 1));
            ctrl.SetValue(HSBPropertyKey, hsb);
            Canvas.SetLeft(ctrl._thumb, s * ctrl._canvas.ActualWidth - ctrl._thumb.ActualWidth / 2);
            Canvas.SetTop(ctrl._thumb, (1 - b) * ctrl._canvas.ActualHeight - ctrl._thumb.ActualHeight / 2);
            ctrl._hueSliderColor.Value = 1 - h;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (_hueSliderColor != null)
                _hueSliderColor.ValueChanged -= HueSliderColor_OnValueChanged;
            _canvas = GetTemplateChild(CanvasTemplateName) as Canvas;
            if (_canvas != null)
            {
                _canvas.Loaded += Canvas_Loaded;
                _canvas.MouseUp += Canvas_MouseUp;
            }

            _thumb = GetTemplateChild(ThumbTemplateName) as Thumb;
            if (_thumb != null)
                _thumb.DragDelta += Thumb_DragDelta;
            _hueSliderColor = GetTemplateChild(HueSliderColorTemplateName) as Slider;
            if (_hueSliderColor != null)
                _hueSliderColor.ValueChanged += HueSliderColor_OnValueChanged;

            _button = GetTemplateChild(ButtonTemplateName) as Button;
            currentGridStateIndex = 0;
            colorTypeEnums = (ColorTypeEnum[]) Enum.GetValues(typeof(ColorTypeEnum));
            if (_button != null)
                _button.Click += Button_Click;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            currentGridStateIndex = (currentGridStateIndex + 1) % colorTypeEnums.Length;
            ColorType = colorTypeEnums[currentGridStateIndex];
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var canvasPosition = e.GetPosition(_canvas);
            GetHSB(canvasPosition);
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var canvasPosition = e.GetPosition(_canvas);
            GetHSB(canvasPosition);
        }

        private void GetHSB(Point point, bool isMove = true)
        {
            var newLeft = point.X - _thumb.ActualWidth / 2;
            var newTop = point.Y - _thumb.ActualHeight / 2;
            var thumbW = _thumb.ActualWidth / 2;
            var thumbH = _thumb.ActualHeight / 2;
            var canvasRight = _canvas.ActualWidth - thumbW;
            var canvasBottom = _canvas.ActualHeight - thumbH;
            if (newLeft < -thumbW)
                newLeft = -thumbW;
            else if (newLeft > canvasRight)
                newLeft = canvasRight;
            if (newTop < -thumbH)
                newTop = -thumbH;
            else if (newTop > canvasBottom)
                newTop = canvasBottom;

            if (isMove)
            {
                Canvas.SetLeft(_thumb, newLeft);
                Canvas.SetTop(_thumb, newTop);
            }

            var hsb = new HSB
            {
                H = HSB.H, S = (newLeft + thumbW) / _canvas.ActualWidth,
                B = 1 - (newTop + thumbH) / _canvas.ActualHeight
            };
            SetValue(HSBPropertyKey, hsb);
            var currentColor = ColorUtil.ColorFromAhsb(1, HSB.H, HSB.S, HSB.B);
            if (SelectedColor != currentColor)
            {
                _isInnerUpdateSelectedColor = true;
                SelectedColor = currentColor;
            }
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var point = Mouse.GetPosition(_canvas);
            GetHSB(point);
        }

        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            var width = (int) _canvas.ActualWidth;
            var height = (int) _canvas.ActualHeight;
            var point = new Point(width - _thumb.ActualWidth / 2, -_thumb.ActualHeight / 2);
            Canvas.SetLeft(_thumb, point.X);
            Canvas.SetTop(_thumb, point.Y);
            var hsb = new HSB {H = _hueSliderColor.Value, S = HSB.S, B = HSB.B};
            SetValue(HSBPropertyKey, hsb);
        }

        private void HueSliderColor_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DoubleUtil.AreClose(HSB.H, e.NewValue))
                return;
            var hsb = new HSB {H = 1 - e.NewValue, S = HSB.S, B = HSB.B};
            SetValue(HSBPropertyKey, hsb);
            SetValue(HueColorPropertyKey, ColorUtil.ColorFromHsb(HSB.H, 1, 1));

            var newLeft = Canvas.GetLeft(_thumb);
            var newTop = Canvas.GetTop(_thumb);
            var point = new Point(newLeft, newTop);
            GetHSB(point, false);
        }
    }

    public enum ColorTypeEnum
    {
        RGB,
        HSL,
        HEX
    }
}