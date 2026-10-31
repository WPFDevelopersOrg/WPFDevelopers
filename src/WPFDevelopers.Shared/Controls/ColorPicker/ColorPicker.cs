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
    [TemplatePart(Name = ColorTypeSelectorTemplateName, Type = typeof(ComboBox))]
    public partial class ColorPicker : Control
    {
        private const string HueSliderColorTemplateName = "PART_HueSlider";

        private const string CanvasTemplateName = "PART_Canvas";

        private const string ThumbTemplateName = "PART_Thumb";

        internal const string ColorTypeSelectorTemplateName = "PART_ColorTypeSelector";

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
                new FrameworkPropertyMetadata(ColorTypeEnum.RGB, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private Canvas _canvas;

        private Slider _hueSliderColor;

        private bool _isInnerUpdateSelectedColor;

        private bool _hasPendingSelectedColor = true;

        private Thumb _thumb;


        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker),
                new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }

        public Color HueColor => (Color)GetValue(HueColorProperty);

        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public HSB HSB => (HSB)GetValue(HSBHProperty);


        public ColorTypeEnum ColorType
        {
            get => (ColorTypeEnum)GetValue(ColorTypeProperty);
            set => SetValue(ColorTypeProperty, value);
        }

        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (ColorPicker)d;
            ctrl.UpdateSelectedColor((Color)e.NewValue);
            var color = (Color)e.NewValue;
            ctrl.SetValue(ColorTextPropertyKey, color.A == 255
                ? $"#{color.R:X2}{color.G:X2}{color.B:X2}"
                : color.ToString());
            ctrl.RaiseEvent(new RoutedPropertyChangedEventArgs<Color>(
                (Color)e.OldValue, color, SelectedColorChangedEvent));
        }

        private void UpdateSelectedColor(Color color)
        {
            if (_isInnerUpdateSelectedColor)
            {
                _isInnerUpdateSelectedColor = false;
                return;
            }
            if (_hueSliderColor == null)
            {
                _hasPendingSelectedColor = true;
                return;
            }
            double h = 0, s = 0, b = 0;
            ColorUtil.HsbFromColor(color, ref h, ref s, ref b);
            var hsb = new HSB { H = h, S = s, B = b };
            SetValue(HueColorPropertyKey, ColorUtil.ColorFromHsb(hsb.H, 1, 1));
            SetValue(HSBPropertyKey, hsb);
            _hueSliderColor.ValueChanged -= HueSliderColor_OnValueChanged;
            _hueSliderColor.Value = 1 - h;
            _hueSliderColor.ValueChanged += HueSliderColor_OnValueChanged;
            UpdateThumbPosition();
        }

        public override void OnApplyTemplate()
        {
            DetachPopupTemplate();
            if (_hueSliderColor != null)
                _hueSliderColor.ValueChanged -= HueSliderColor_OnValueChanged;
            if (_canvas != null)
                _canvas.MouseUp -= Canvas_MouseUp;
            if (_thumb != null)
                _thumb.DragDelta -= Thumb_DragDelta;
            base.OnApplyTemplate();
            _canvas = GetTemplateChild(CanvasTemplateName) as Canvas;
            if (_canvas != null)
            {
                _canvas.MouseUp -= Canvas_MouseUp;
                _canvas.MouseUp += Canvas_MouseUp;
            }

            _thumb = GetTemplateChild(ThumbTemplateName) as Thumb;
            if (_thumb != null)
                _thumb.DragDelta += Thumb_DragDelta;
            _hueSliderColor = GetTemplateChild(HueSliderColorTemplateName) as Slider;
            if (_hueSliderColor != null)
                _hueSliderColor.ValueChanged += HueSliderColor_OnValueChanged;

            _hasPendingSelectedColor = true;
            LoadedColor();
            ApplyPopupTemplate();
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_canvas == null) return;
            var canvasPosition = e.GetPosition(_canvas);
            GetHSB(canvasPosition);
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_canvas == null) return;
            var canvasPosition = e.GetPosition(_canvas);
            GetHSB(canvasPosition);
        }

        private void GetHSB(Point point, bool isMove = true)
        {
            if (_canvas == null || _thumb == null || _hueSliderColor == null) return;
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
                H = HSB.H,
                S = (newLeft + thumbW) / _canvas.ActualWidth,
                B = 1 - (newTop + thumbH) / _canvas.ActualHeight
            };
            SetValue(HSBPropertyKey, hsb);
            var currentColor = ColorUtil.ColorFromAhsb(1, HSB.H, HSB.S, HSB.B);
            if (SelectedColor != currentColor)
            {
                _isInnerUpdateSelectedColor = true;
                SetCurrentValue(SelectedColorProperty, currentColor);
            }
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (_canvas == null) return;
            var point = Mouse.GetPosition(_canvas);
            GetHSB(point);
        }

        private void LoadedColor()
        {
            if (_canvas == null || _thumb == null || _hueSliderColor == null) return;
            if (_hasPendingSelectedColor)
            {
                _hasPendingSelectedColor = false;
                var color = SelectedColor;
                double h = 0, s = 0, b = 0;
                ColorUtil.HsbFromColor(color, ref h, ref s, ref b);
                SetValue(HueColorPropertyKey, ColorUtil.ColorFromHsb(h, 1, 1));
                SetValue(HSBPropertyKey, new HSB { H = h, S = s, B = b });
                _hueSliderColor.ValueChanged -= HueSliderColor_OnValueChanged;
                _hueSliderColor.Value = 1 - h;
                _hueSliderColor.ValueChanged += HueSliderColor_OnValueChanged;
            }
            UpdateThumbPosition();
        }

        private void HueSliderColor_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DoubleUtil.AreClose(HSB.H, e.NewValue))
                return;
            var hsb = new HSB { H = 1 - e.NewValue, S = HSB.S, B = HSB.B };
            SetValue(HSBPropertyKey, hsb);
            SetValue(HueColorPropertyKey, ColorUtil.ColorFromHsb(HSB.H, 1, 1));

            var newLeft = Canvas.GetLeft(_thumb);
            var newTop = Canvas.GetTop(_thumb);
            var point = new Point(newLeft, newTop);
            GetHSB(point, false);
        }

        private void UpdateThumbPosition()
        {
            if (_canvas == null || _thumb == null) return;
            var color = SelectedColor;
            double h = 0, s = 0, b = 0;
            ColorUtil.HsbFromColor(color, ref h, ref s, ref b);
            Canvas.SetLeft(_thumb, s * _canvas.ActualWidth - _thumb.ActualWidth / 2);
            Canvas.SetTop(_thumb, (1 - b) * _canvas.ActualHeight - _thumb.ActualHeight / 2);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            UpdateThumbPosition();
        }
    }

    public enum ColorTypeEnum
    {
        RGB,
        HSL,
        HEX
    }
}