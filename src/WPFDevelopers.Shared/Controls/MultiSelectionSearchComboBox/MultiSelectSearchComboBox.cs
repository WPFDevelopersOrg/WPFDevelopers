using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_Selector", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_SelectAll", Type = typeof(CheckBox))]
    [TemplatePart(Name = "PART_SearchSelector", Type = typeof(ListBox))]
    public class MultiSelectionSearchComboBox : Control
    {
        private const string TextBoxTemplateName = "PART_TextBox";
        private const string PopupTemplateName = "PART_Popup";
        private const string ListBoxTemplateName = "PART_Selector";
        private const string CheckBoxTemplateName = "PART_SelectAll";
        private const string ListBoxTemplateNameSearch = "PART_SearchSelector";

        public static readonly RoutedEvent ClosedEvent =
            EventManager.RegisterRoutedEvent("Closed",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(MultiSelectionSearchComboBox));

        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath",
                typeof(string),
                typeof(MultiSelectionSearchComboBox),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register("SelectedValuePath",
                typeof(string),
                typeof(MultiSelectionSearchComboBox),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text",
                typeof(string),
                typeof(MultiSelectionSearchComboBox),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(MultiSelectionSearchComboBox),
                new PropertyMetadata());

        public static readonly DependencyProperty ItemsSourceSearchProperty =
            DependencyProperty.Register("ItemsSourceSearch", typeof(IEnumerable), typeof(MultiSelectionSearchComboBox),
                new PropertyMetadata());

        public static readonly DependencyProperty SelectAllContentProperty =
            DependencyProperty.Register("SelectAllContent", typeof(object), typeof(MultiSelectionSearchComboBox),
                new PropertyMetadata(LanguageManager.Instance["SelectAll"]));

        public static readonly DependencyProperty IsSelectAllActiveProperty =
            DependencyProperty.Register("IsSelectAllActive", typeof(bool), typeof(MultiSelectionSearchComboBox),
                new PropertyMetadata(false));

        public static readonly DependencyProperty DelimiterProperty =
            DependencyProperty.Register("Delimiter", typeof(string), typeof(MultiSelectionSearchComboBox),
                new PropertyMetadata(";"));

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(MultiSelectionSearchComboBox),
                new PropertyMetadata(false, OnIsDropDownOpenChanged));

        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(MultiSelectionSearchComboBox),
                new UIPropertyMetadata(SystemParameters.PrimaryScreenHeight / 3.0, OnMaxDropDownHeightChanged));

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(IList), typeof(MultiSelectionSearchComboBox),
                new FrameworkPropertyMetadata(new ArrayList(),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                    OnSelectedItemsChanged));

        public static readonly DependencyProperty SearchWatermarkProperty =
            DependencyProperty.Register("SearchWatermark",
                typeof(string),
                typeof(MultiSelectionSearchComboBox),
                new PropertyMetadata(string.Empty));

        private CheckBox _checkBox;
        private ListBox _listBox;
        private ListBox _listBoxSearch;
        private Popup _popup;
        private TextBox _textBox;
        private List<object> selectedItems;

        private List<object> selectedList;
        private List<object> selectedSearchList;

        private string theLastText;

        static MultiSelectionSearchComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiSelectionSearchComboBox),
                new FrameworkPropertyMetadata(typeof(MultiSelectionSearchComboBox)));
        }

        public string Delimiter
        {
            get => (string)GetValue(DelimiterProperty);
            set => SetValue(DelimiterProperty, value);
        }

        public string SelectedValuePath
        {
            get => (string)GetValue(SelectedValuePathProperty);
            set => SetValue(SelectedValuePathProperty, value);
        }

        public string DisplayMemberPath
        {
            get => (string)GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public IEnumerable ItemsSourceSearch
        {
            get => (IEnumerable)GetValue(ItemsSourceSearchProperty);
            set => SetValue(ItemsSourceSearchProperty, value);
        }

        public object SelectAllContent
        {
            get => GetValue(SelectAllContentProperty);
            set => SetValue(SelectAllContentProperty, value);
        }

        public bool IsSelectAllActive
        {
            get => (bool)GetValue(IsSelectAllActiveProperty);
            set => SetValue(IsSelectAllActiveProperty, value);
        }

        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        public double MaxDropDownHeight
        {
            get => (double)GetValue(MaxDropDownHeightProperty);
            set => SetValue(MaxDropDownHeightProperty, value);
        }

        public IList SelectedItems
        {
            get => (IList)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        public string SearchWatermark
        {
            get => (string)GetValue(SearchWatermarkProperty);
            set => SetValue(SearchWatermarkProperty, value);
        }

        [DllImport(Win32.User32)]
        private static extern IntPtr SetFocus(IntPtr hWnd);

        public event RoutedEventHandler Closed
        {
            add => AddHandler(ClosedEvent, value);
            remove => RemoveHandler(ClosedEvent, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            LanguageManager.Instance.PropertyChanged += Instance_PropertyChanged;
            selectedList = new List<object>();
            selectedSearchList = new List<object>();
            selectedItems = new List<object>();
            _textBox = GetTemplateChild(TextBoxTemplateName) as TextBox;
            _popup = GetTemplateChild(PopupTemplateName) as Popup;
            if (_popup != null)
                _popup.GotFocus += OnPopup_GotFocus;
            _listBox = GetTemplateChild(ListBoxTemplateName) as ListBox;

            _checkBox = GetTemplateChild(CheckBoxTemplateName) as CheckBox;
            _listBoxSearch = GetTemplateChild(ListBoxTemplateNameSearch) as ListBox;
            if (_textBox != null)
            {
                _textBox.TextChanged -= OnTextbox_TextChanged;
                _textBox.TextChanged += OnTextbox_TextChanged;
            }

            if (_checkBox != null)
            {
                _checkBox.Checked -= OnCheckBox_Checked;
                _checkBox.Unchecked -= OnCheckBox_Unchecked;
                _checkBox.Checked += OnCheckBox_Checked;
                _checkBox.Unchecked += OnCheckBox_Unchecked;
            }

            if (_listBox != null)
            {
                _listBox.IsVisibleChanged -= OnListBox_IsVisibleChanged;
                _listBox.IsVisibleChanged += OnListBox_IsVisibleChanged;
                _listBox.SelectionChanged -= OnListBox_SelectionChanged;
                _listBox.SelectionChanged += OnListBox_SelectionChanged;
            }

            if (_listBoxSearch != null)
            {
                _listBoxSearch.IsVisibleChanged -= OnListBoxSearch_IsVisibleChanged;
                _listBoxSearch.IsVisibleChanged += OnListBoxSearch_IsVisibleChanged;
                _listBoxSearch.SelectionChanged -= OnListBoxSearch_SelectionChanged;
                _listBoxSearch.SelectionChanged += OnListBoxSearch_SelectionChanged;
            }
        }

        private void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SelectAllContent = LanguageManager.Instance["SelectAll"];
        }

        private void OnListBoxSearch_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                UpdateIsChecked(_listBoxSearch);
        }

        private void OnListBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                foreach (var item in selectedSearchList)
                    if (!_listBox.SelectedItems.Contains(item))
                        _listBox.SelectedItems.Add(item);
                UpdateIsChecked(_listBox);
            }
        }

        private void UpdateIsChecked(ListBox listBox)
        {
            _checkBox.Checked -= OnCheckBox_Checked;
            if (listBox.Items.Count > 0 && listBox.Items.Count == listBox.SelectedItems.Count)
            {
                if (_checkBox.IsChecked != true)
                    _checkBox.IsChecked = true;
            }
            else
            {
                if (listBox.SelectedItems.Count == 0)
                    _checkBox.IsChecked = false;
                else
                    _checkBox.IsChecked = null;
            }

            _checkBox.Checked += OnCheckBox_Checked;
        }

        private void OnPopup_GotFocus(object sender, RoutedEventArgs e)
        {
            var source = (HwndSource)PresentationSource.FromVisual(_popup.Child);
            if (source != null)
            {
                SetFocus(source.Handle);
                _textBox.Focus();
            }
        }

        private void OnCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_listBoxSearch.Visibility == Visibility.Visible)
                _listBoxSearch.UnselectAll();
            else
                _listBox.UnselectAll();
        }

        private void OnCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (_listBoxSearch.Visibility == Visibility.Visible)
                _listBoxSearch.SelectAll();
            else
                _listBox.SelectAll();
        }

        private void Combination()
        {
            var seletedName = new List<string>();
            foreach (var item in _listBox.SelectedItems)
            {
                var name = GetDisplayText(item);
                if (!string.IsNullOrWhiteSpace(name))
                    seletedName.Add(name);
                else
                    seletedName.Add(item.ToString());
            }

            foreach (var item in _listBoxSearch.SelectedItems)
            {
                if (_listBox.SelectedItems.Contains(item))
                    continue;
                var name = GetDisplayText(item);
                if (!string.IsNullOrWhiteSpace(name))
                    seletedName.Add(name);
                else
                    seletedName.Add(item.ToString());
            }

            Text = string.Join(Delimiter, seletedName.ToArray());
        }

        private void OnListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                foreach (var item in e.RemovedItems)
                {
                    if (_checkBox.IsChecked == true)
                    {
                        _checkBox.Unchecked -= OnCheckBox_Unchecked;
                        if (_listBox.Items.Count == 1)
                            _checkBox.IsChecked = false;
                        else
                            _checkBox.IsChecked = null;
                        _checkBox.Unchecked += OnCheckBox_Unchecked;
                    }

                    if (_listBoxSearch.SelectedItems.Contains(item))
                        _listBoxSearch.SelectedItems.Remove(item);
                    if (selectedSearchList.Contains(item))
                        selectedSearchList.Remove(item);
                }

                SelectionChecked(_listBox);
            }


            if (e.AddedItems.Count > 0)
                SelectionChecked(_listBox);
            Combination();
            var selectedItems = _listBox.SelectedItems;
            if (SelectedItems == null)
                SelectedItems = selectedItems;
            else
            {
                foreach (var item in selectedItems)
                {
                    if (!SelectedItems.Contains(item))
                        SelectedItems.Add(item);
                }
            }
        }

        private void OnListBoxSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_listBoxSearch.IsVisible)
                return;
            if (e.RemovedItems.Count > 0)
            {
                foreach (var item in e.RemovedItems)
                {
                    if (selectedSearchList.Contains(item))
                        selectedSearchList.Remove(item);
                    if (_listBoxSearch.Items.Contains(item))
                    {
                        if (_listBox.SelectedItems.Contains(item))
                            _listBox.SelectedItems.Remove(item);
                    }

                    if (selectedList.Contains(item))
                        selectedList.Remove(item);
                }

                Combination();
                SelectionChecked(_listBoxSearch);
            }

            if (e.AddedItems.Count > 0)
            {
                foreach (var item in e.AddedItems)
                    if (!_listBox.SelectedItems.Contains(item))
                        _listBox.SelectedItems.Add(item);
                Combination();
                SelectionChecked(_listBoxSearch);
            }
        }

        private void SelectionChecked(ListBox listbox)
        {
            if (listbox.SelectedItems.Count > 0
                &&
                listbox.Items.Count == listbox.SelectedItems.Count)
            {
                _checkBox.Checked -= OnCheckBox_Checked;
                _checkBox.IsChecked = true;
                _checkBox.Checked += OnCheckBox_Checked;
            }
            else
            {
                _checkBox.Checked -= OnCheckBox_Checked;
                if (listbox.SelectedItems.Count > 0
                    &&
                    listbox.Items.Count == listbox.SelectedItems.Count)
                {
                    if (_checkBox.IsChecked != true)
                        _checkBox.IsChecked = true;
                }
                else
                {
                    if (listbox.SelectedItems.Count == 0)
                        _checkBox.IsChecked = false;
                    else
                        _checkBox.IsChecked = null;
                }

                _checkBox.Checked += OnCheckBox_Checked;
            }
        }

        private string GetDisplayText(object dataItem, string path = null)
        {
            if (dataItem == null) return string.Empty;
            return GetPropertyValue(dataItem);
        }

        private void OnTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(theLastText)) theLastText = _textBox.Text;
            SearchText(_textBox.Text);
        }

        private void SearchText(string _text)
        {
            var text = _text;
            if (string.IsNullOrWhiteSpace(text))
            {
                if (_listBoxSearch.Visibility != Visibility.Collapsed)
                    _listBoxSearch.Visibility = Visibility.Collapsed;
                if (_listBox.Visibility != Visibility.Visible)
                    _listBox.Visibility = Visibility.Visible;
            }
            else
            {
                if (_listBoxSearch.Visibility != Visibility.Visible)
                    _listBoxSearch.Visibility = Visibility.Visible;
                if (_listBox.Visibility != Visibility.Collapsed)
                    _listBox.Visibility = Visibility.Collapsed;
                var listSearch = new List<object>();
                foreach (var item in _listBox.Items)
                {
                    var str = GetPropertyValue(item);
                    if (string.IsNullOrWhiteSpace(str))
                        str = item.ToString();
                    if (!string.IsNullOrWhiteSpace(str))
                        if (str.ToUpperInvariant().Contains(text.ToUpperInvariant()))
                            listSearch.Add(item);
                }

                foreach (var item in selectedList)
                    if (!listSearch.Contains(item))
                        listSearch.Add(item);

                var lastItem = ItemsSourceSearch;
                ItemsSourceSearch = listSearch;
                SelectionChecked(_listBoxSearch);
                selectedItems.Clear();
                foreach (var item in _listBoxSearch.Items)
                    if (_listBox.SelectedItems.Contains(item))
                        if (!_listBoxSearch.SelectedItems.Contains(item))
                            _listBoxSearch.SelectedItems.Add(item);
            }
        }

        private string GetPropertyValue(object item)
        {
            var result = string.Empty;
            var nameParts = DisplayMemberPath.Split('.');
            if (nameParts.Length == 1)
            {
                var property = item.GetType().GetProperty(DisplayMemberPath);
                if (property != null)
                    return (property.GetValue(item, null) ?? string.Empty).ToString();
            }

            return result.ToUpperInvariant();
        }

        private static void OnIsDropDownOpenChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var multiSelectionSearchComboBox = o as MultiSelectionSearchComboBox;
            if (multiSelectionSearchComboBox != null)
                multiSelectionSearchComboBox.OnIsOpenChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnIsOpenChanged(bool oldValue, bool newValue)
        {
            if (!newValue)
                RaiseRoutedEvent(ClosedEvent);
        }

        private void RaiseRoutedEvent(RoutedEvent routedEvent)
        {
            var args = new RoutedEventArgs(routedEvent, this);
            RaiseEvent(args);
        }

        private static void OnMaxDropDownHeightChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var comboBox = o as MultiSelectionSearchComboBox;
            if (comboBox != null)
                comboBox.OnMaxDropDownHeightChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual void OnMaxDropDownHeightChanged(double oldValue, double newValue)
        {
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mltiSelectionSearchComboBox = d as MultiSelectionSearchComboBox;

            if (e.NewValue != null)
            {
                var collection = e.NewValue as IList;
                if (collection.Count <= 0) return;
                mltiSelectionSearchComboBox._listBox.SelectionChanged -=
                    mltiSelectionSearchComboBox.OnListBox_SelectionChanged;
                foreach (var item in collection)
                {
                    var name = mltiSelectionSearchComboBox.GetPropertyValue(item);
                    object model = null;
                    if (!string.IsNullOrWhiteSpace(name))
                        model = mltiSelectionSearchComboBox._listBox.ItemsSource.OfType<object>().FirstOrDefault(h =>
                            mltiSelectionSearchComboBox.GetPropertyValue(h) == name);
                    else
                        model = mltiSelectionSearchComboBox._listBox.ItemsSource.OfType<object>()
                            .FirstOrDefault(h => h == item);
                    if (model != null && !mltiSelectionSearchComboBox._listBox.SelectedItems.Contains(item))
                        mltiSelectionSearchComboBox._listBox.SelectedItems.Add(model);
                }

                mltiSelectionSearchComboBox._listBox.SelectionChanged +=
                    mltiSelectionSearchComboBox.OnListBox_SelectionChanged;
                mltiSelectionSearchComboBox.Combination();
            }
        }
    }
}