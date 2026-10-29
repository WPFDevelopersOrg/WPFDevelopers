using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = PopupTemplateName, Type = typeof(Popup))]
    [TemplatePart(Name = ColorPickerTemplateName, Type = typeof(ColorPicker))]
    [TemplatePart(Name = ToggleButtonTemplateName, Type = typeof(ToggleButton))]
    public partial class ColorPicker
    {
        private const string PopupTemplateName = "PART_Popup";
        private const string ColorPickerTemplateName = "PART_ColorPicker";
        private const string ToggleButtonTemplateName = "PART_ToggleButton";

        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode", typeof(ColorPickerDisplayMode), typeof(ColorPicker),
                new PropertyMetadata(ColorPickerDisplayMode.Inline, OnDisplayModeChanged),
                value => (ColorPickerDisplayMode)value == ColorPickerDisplayMode.Inline ||
                         (ColorPickerDisplayMode)value == ColorPickerDisplayMode.Popup);

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(ColorPicker),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    null, CoerceIsDropDownOpen));

        public static readonly DependencyProperty ShowColorTextProperty =
            DependencyProperty.Register("ShowColorText", typeof(bool), typeof(ColorPicker),
                new PropertyMetadata(false));

        public static readonly DependencyProperty SwatchSizeProperty =
            DependencyProperty.Register("SwatchSize", typeof(double), typeof(ColorPicker),
                new PropertyMetadata(18.0), value =>
                {
                    var size = (double)value;
                    return size > 0 && !double.IsInfinity(size) && !double.IsNaN(size);
                });

        private static readonly DependencyPropertyKey ColorTextPropertyKey =
            DependencyProperty.RegisterReadOnly("ColorText", typeof(string), typeof(ColorPicker),
                new PropertyMetadata("#FF0000"));

        public static readonly DependencyProperty ColorTextProperty = ColorTextPropertyKey.DependencyProperty;

        public static readonly RoutedEvent SelectedColorChangedEvent =
            EventManager.RegisterRoutedEvent("SelectedColorChanged", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<Color>), typeof(ColorPicker));

        private Popup _popup;
        private ColorPicker _colorPicker;
        private ToggleButton _toggleButton;
        private ComboBox _colorTypeSelector;

        public ColorPicker()
        {
            Unloaded += OnUnloaded;
            IsEnabledChanged += OnIsEnabledChanged;
        }

        public ColorPickerDisplayMode DisplayMode
        {
            get => (ColorPickerDisplayMode)GetValue(DisplayModeProperty);
            set => SetValue(DisplayModeProperty, value);
        }

        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        public bool ShowColorText
        {
            get => (bool)GetValue(ShowColorTextProperty);
            set => SetValue(ShowColorTextProperty, value);
        }

        public double SwatchSize
        {
            get => (double)GetValue(SwatchSizeProperty);
            set => SetValue(SwatchSizeProperty, value);
        }

        public string ColorText => (string)GetValue(ColorTextProperty);

        public event RoutedPropertyChangedEventHandler<Color> SelectedColorChanged
        {
            add { AddHandler(SelectedColorChangedEvent, value); }
            remove { RemoveHandler(SelectedColorChangedEvent, value); }
        }

        private void DetachPopupTemplate()
        {
            DetachColorTypeSelector();
            if (_popup != null)
            {
                _popup.Opened -= OnPopupOpened;
                _popup.IsOpen = false;
            }
            if (_colorPicker != null)
            {
                _colorPicker.KeyDown -= OnPickerKeyDown;
                _colorPicker.SelectedColorChanged -= OnPanelSelectedColorChanged;
            }
        }

        private void ApplyPopupTemplate()
        {
            _popup = GetTemplateChild(PopupTemplateName) as Popup;
            _colorPicker = GetTemplateChild(ColorPickerTemplateName) as ColorPicker;
            _toggleButton = GetTemplateChild(ToggleButtonTemplateName) as ToggleButton;
            if (_popup != null)
                _popup.Opened += OnPopupOpened;
            if (_colorPicker != null)
            {
                _colorPicker.KeyDown += OnPickerKeyDown;
                _colorPicker.SelectedColorChanged += OnPanelSelectedColorChanged;
            }
            if (_popup != null && _popup.IsOpen)
                OnPopupOpened(_popup, System.EventArgs.Empty);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Handled || DisplayMode != ColorPickerDisplayMode.Popup)
                return;
            if (e.Key == Key.F4 || (e.SystemKey == Key.Down && (Keyboard.Modifiers & ModifierKeys.Alt) != 0))
            {
                SetCurrentValue(IsDropDownOpenProperty, !IsDropDownOpen);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape && IsDropDownOpen)
            {
                CloseAndFocus();
                e.Handled = true;
            }
        }

        private static void OnDisplayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var picker = (ColorPicker)d;
            picker.SetCurrentValue(IsDropDownOpenProperty, false);
            picker.CoerceValue(IsDropDownOpenProperty);
        }

        private static object CoerceIsDropDownOpen(DependencyObject d, object baseValue)
        {
            var picker = (ColorPicker)d;
            return picker.DisplayMode == ColorPickerDisplayMode.Popup && picker.IsEnabled && (bool)baseValue;
        }

        private void OnPanelSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            e.Handled = true;
        }

        private void OnPopupOpened(object sender, System.EventArgs e)
        {
            DetachColorTypeSelector();
            if (_colorPicker != null)
            {
                _colorPicker.ApplyTemplate();
                _colorTypeSelector = _colorPicker.Template?.FindName(
                    ColorPicker.ColorTypeSelectorTemplateName, _colorPicker) as ComboBox;
                if (_colorTypeSelector != null)
                {
                    _colorTypeSelector.DropDownOpened += OnFormatDropDownOpened;
                    _colorTypeSelector.DropDownClosed += OnFormatDropDownClosed;
                }
            }
            _colorPicker?.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }

        private void DetachColorTypeSelector()
        {
            if (_colorTypeSelector == null)
                return;
            _colorTypeSelector.DropDownOpened -= OnFormatDropDownOpened;
            _colorTypeSelector.DropDownClosed -= OnFormatDropDownClosed;
            _colorTypeSelector = null;
        }

        private void OnFormatDropDownOpened(object sender, System.EventArgs e)
        {
            // Let the nested ComboBox own capture until it has finished releasing it.
            _popup?.SetCurrentValue(Popup.StaysOpenProperty, true);
        }

        private void OnFormatDropDownClosed(object sender, System.EventArgs e)
        {
            var popup = _popup;
            var selector = _colorTypeSelector;
            var clickedOutside = _colorPicker != null && !_colorPicker.IsMouseOver &&
                (Mouse.LeftButton == MouseButtonState.Pressed || Mouse.RightButton == MouseButtonState.Pressed);
            // DropDownClosed runs before ComboBox releases capture; reclaim it on the next input turn.
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new System.Action(() =>
            {
                if (popup == null || popup != _popup || selector != _colorTypeSelector ||
                    (selector != null && selector.IsDropDownOpen))
                    return;
                popup.SetCurrentValue(Popup.StaysOpenProperty, false);
                if (clickedOutside)
                    SetCurrentValue(IsDropDownOpenProperty, false);
            }));
        }

        private void OnPickerKeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Handled && e.Key == Key.Escape)
            {
                CloseAndFocus();
                e.Handled = true;
            }
        }

        private void CloseAndFocus()
        {
            SetCurrentValue(IsDropDownOpenProperty, false);
            _toggleButton?.Focus();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            SetCurrentValue(IsDropDownOpenProperty, false);
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
                SetCurrentValue(IsDropDownOpenProperty, false);
            CoerceValue(IsDropDownOpenProperty);
        }
    }

    public enum ColorPickerDisplayMode
    {
        Inline,
        Popup
    }
}
