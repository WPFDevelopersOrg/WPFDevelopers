using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace WPFDevelopers.Controls
{
    public class MultiSelectComboBox : ListBox
    {
        private const string PART_Popup = "PART_Popup";
        private const string PART_CheckBoxAll = "PART_CheckBoxAll";

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(MultiSelectComboBox),
                new PropertyMetadata(false));

        public static readonly DependencyProperty MaxDropDownHeightProperty
            = DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(MultiSelectComboBox),
                new PropertyMetadata(SystemParameters.PrimaryScreenHeight / 3));

        public static readonly DependencyProperty SelectAllContentProperty =
            DependencyProperty.Register("SelectAllContent", typeof(object), typeof(MultiSelectComboBox),
                new PropertyMetadata(LanguageManager.Instance["SelectAll"]));

        public static readonly DependencyProperty IsSelectAllActiveProperty =
            DependencyProperty.Register("IsSelectAllActive", typeof(bool), typeof(MultiSelectComboBox),
                new PropertyMetadata(false));

        public static readonly DependencyProperty DelimiterProperty =
            DependencyProperty.Register("Delimiter", typeof(string), typeof(MultiSelectComboBox),
                new PropertyMetadata(";"));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(MultiSelectComboBox),
                new PropertyMetadata(string.Empty, OnTextChanged));

        private bool _ignoreTextValueChanged;
        private MultiSelectComboBoxItem _multiSelectComboBoxItem;
        private Popup _popup;

        public bool IsDropDownOpen
        {
            get => (bool) GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        [Bindable(true)]
        [Category("Layout")]
        [TypeConverter(typeof(LengthConverter))]
        public double MaxDropDownHeight
        {
            get => (double) GetValue(MaxDropDownHeightProperty);
            set => SetValue(MaxDropDownHeightProperty, value);
        }

        public object SelectAllContent
        {
            get => GetValue(SelectAllContentProperty);
            set => SetValue(SelectAllContentProperty, value);
        }

        public bool IsSelectAllActive
        {
            get => (bool) GetValue(IsSelectAllActiveProperty);
            set => SetValue(IsSelectAllActiveProperty, value);
        }

        public string Delimiter
        {
            get => (string) GetValue(DelimiterProperty);
            set => SetValue(DelimiterProperty, value);
        }

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var MultiSelectComboBox = (MultiSelectComboBox) d;

            if (!(bool) e.NewValue)
                MultiSelectComboBox.Dispatcher.BeginInvoke(new Action(() => { Mouse.Capture(null); }),
                    DispatcherPriority.Send);
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is MultiSelectComboBoxItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MultiSelectComboBoxItem();
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            UpdateText();
            base.OnSelectionChanged(e);
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            LanguageManager.Instance.PropertyChanged += Instance_PropertyChanged;
            _popup = GetTemplateChild(PART_Popup) as Popup;
            _multiSelectComboBoxItem = GetTemplateChild(PART_CheckBoxAll) as MultiSelectComboBoxItem;
            _multiSelectComboBoxItem.Selected += _MultiSelectComboBoxItem_Selected;
            _multiSelectComboBoxItem.Unselected += _MultiSelectComboBoxItem_Unselected;
        }

        private void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SelectAllContent = LanguageManager.Instance["SelectAll"];
        }

        private void _MultiSelectComboBoxItem_Unselected(object sender, RoutedEventArgs e)
        {
            if (_ignoreTextValueChanged) return;
            _ignoreTextValueChanged = true;
            UnselectAll();
            _ignoreTextValueChanged = false;
            UpdateText();
        }

        private void _MultiSelectComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            if (_ignoreTextValueChanged) return;
            _ignoreTextValueChanged = true;
            SelectAll();
            _ignoreTextValueChanged = false;
            UpdateText();
        }

        protected virtual void UpdateText()
        {
            if (_ignoreTextValueChanged) return;
            var newValue = string.Join(Delimiter, SelectedItems.Cast<object>().Select(x => GetItemDisplayValue(x)));
            if (string.IsNullOrWhiteSpace(Text) || !Text.Equals(newValue))
            {
                _ignoreTextValueChanged = true;
                if (_multiSelectComboBoxItem != null)
                    _multiSelectComboBoxItem.SetCurrentValue(IsSelectedProperty, SelectedItems.Count == Items.Count);
                SetCurrentValue(TextProperty, newValue);
                _ignoreTextValueChanged = false;
            }
        }

        protected object GetItemDisplayValue(object item)
        {
            if (string.IsNullOrWhiteSpace(DisplayMemberPath))
            {
                var property = item.GetType().GetProperty("Content");
                if (property != null)
                    return property.GetValue(item, null);
            }

            var nameParts = DisplayMemberPath.Split('.');
            if (nameParts.Length == 1)
            {
                var property = item.GetType().GetProperty(DisplayMemberPath);
                if (property != null)
                    return property.GetValue(item, null);
            }

            return item;
        }
    }
}