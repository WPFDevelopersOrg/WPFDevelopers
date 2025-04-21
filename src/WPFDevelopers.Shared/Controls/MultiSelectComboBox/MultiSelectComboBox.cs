using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = PART_Popup, Type = typeof(Popup))]
    [TemplatePart(Name = PART_SimpleWrapPanel, Type = typeof(Panel))]
    [TemplatePart(Name = CheckBoxTemplateName, Type = typeof(CheckBox))]
    [TemplatePart(Name = TextBoxTemplateName, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_DropDownPanel, Type = typeof(Panel))]
    [TemplatePart(Name = ListBoxTemplateNameSearch, Type = typeof(ListBox))]
    [TemplatePart(Name = DropDownScrollViewer, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = PART_ItemsPresenter, Type = typeof(ItemsPresenter))]

    public class MultiSelectComboBox : ListBox
    {
        private const string PART_Popup = "PART_Popup";
        private const string PART_SimpleWrapPanel = "PART_SimpleWrapPanel";
        private const string CheckBoxTemplateName = "PART_SelectAll";
        private const string TextBoxTemplateName = "PART_TextBox";
        private const string PART_DropDownPanel = "PART_DropDown";
        private const string ListBoxTemplateNameSearch = "PART_SearchSelector";
        private const string DropDownScrollViewer = "DropDownScrollViewer";
        private const string PART_ItemsPresenter = "PART_ItemsPresenter";

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

        public static readonly DependencyProperty SearchWatermarkProperty =
            DependencyProperty.Register("SearchWatermark",
                typeof(string),
                typeof(MultiSelectComboBox),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ItemsSourceSearchProperty =
           DependencyProperty.Register("ItemsSourceSearch", typeof(IEnumerable), typeof(MultiSelectComboBox),
               new PropertyMetadata());

        public static readonly DependencyProperty IsSearchProperty =
            DependencyProperty.Register("IsSearch", typeof(bool), typeof(MultiSelectComboBox),
                new PropertyMetadata(true));

        public static readonly DependencyProperty SelectedItemsExtProperty =
            DependencyProperty.Register("SelectedItemsExt", typeof(IList), typeof(MultiSelectComboBox),
                new FrameworkPropertyMetadata(new ArrayList(),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                    OnSelectedItemsExtChanged));

        public static readonly DependencyProperty ShowTypeProperty =
            DependencyProperty.Register("ShowType", typeof(ShowType), typeof(MultiSelectComboBox),
                new PropertyMetadata(ShowType.Text));

        private HwndSource _hwndSource;
        private Window _window;
        private bool _ignoreTextValueChanged;
        private Popup _popup;
        private Panel _panel;
        private ListBox _listBoxSearch;
        private TextBox _textBox;
        private Panel _panelDropDown;
        private ScrollViewer _scrollViewer;
        private CheckBox _checkBox;
        private string _theLastText;
        private ItemsPresenter _itemsPresenter;

        private List<object> selectedItems;
        private List<object> selectedList;
        private List<object> selectedSearchList;

        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        [Bindable(true)]
        [Category("Layout")]
        [TypeConverter(typeof(LengthConverter))]
        public double MaxDropDownHeight
        {
            get => (double)GetValue(MaxDropDownHeightProperty);
            set => SetValue(MaxDropDownHeightProperty, value);
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

        public string Delimiter
        {
            get => (string)GetValue(DelimiterProperty);
            set => SetValue(DelimiterProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string SearchWatermark
        {
            get => (string)GetValue(SearchWatermarkProperty);
            set => SetValue(SearchWatermarkProperty, value);
        }

        public IEnumerable ItemsSourceSearch
        {
            get => (IEnumerable)GetValue(ItemsSourceSearchProperty);
            set => SetValue(ItemsSourceSearchProperty, value);
        }

        public bool IsSearch
        {
            get => (bool)GetValue(IsSearchProperty);
            set => SetValue(IsSearchProperty, value);
        }

        public IList SelectedItemsExt
        {
            get => (IList)GetValue(SelectedItemsExtProperty);
            set => SetValue(SelectedItemsExtProperty, value);
        }

        public ShowType ShowType
        {
            get => (ShowType)GetValue(ShowTypeProperty);
            set => SetValue(ShowTypeProperty, value);
        }

        [DllImport(Win32.User32)]
        private static extern IntPtr SetFocus(IntPtr hWnd);

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (MultiSelectComboBox)d;
            if (!(bool)e.NewValue)
                ctrl.Dispatcher.BeginInvoke(new Action(() => { Mouse.Capture(null); }),
                    DispatcherPriority.Send);
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as MultiSelectComboBox;
            if (ctrl != null)
                ctrl.OnTextChanged((string)e.OldValue, (string)e.NewValue);
        }

        public virtual void OnTextChanged(string oldValue, string newValue)
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
            base.OnSelectionChanged(e);
            if (SelectedItems != null)
            {
                foreach (var item in e.AddedItems)
                {
                    if (!SelectedItems.Contains(item))
                    {
                        SelectedItems.Add(item);
                    }
                }
                foreach (var item in e.RemovedItems)
                {
                    SelectedItems.Remove(item);
                }
                SelectionChecked(this);
            }
            UpdateText();
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            if (_textBox != null)
                ApplySearchLogic();
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            LanguageManager.Instance.PropertyChanged += Instance_PropertyChanged;
            selectedList = new List<object>();
            selectedSearchList = new List<object>();
            selectedItems = new List<object>();
            _textBox = GetTemplateChild(TextBoxTemplateName) as TextBox;
            if (_textBox != null)
                ApplySearchLogic();
            _window = Window.GetWindow(this);
            if (_window != null)
            {
                if (_window.IsInitialized)
                    WindowSourceInitialized(_window, EventArgs.Empty);
                else
                {
                    _window.SourceInitialized -= WindowSourceInitialized;
                    _window.SourceInitialized += WindowSourceInitialized;
                }
            }
            _panelDropDown = GetTemplateChild(PART_DropDownPanel) as Panel;

            _popup = GetTemplateChild(PART_Popup) as Popup;
            if (_popup != null && _window != null)
            {
                _popup.Closed += OnPopup_Closed;
                _popup.Closed -= OnPopup_Closed;
                _popup.Opened -= OnPopup_Opened;
                _popup.Opened += OnPopup_Opened;
                _popup.GotFocus -= OnPopup_GotFocus;
                _popup.GotFocus += OnPopup_GotFocus;
            }

            _scrollViewer = GetTemplateChild(DropDownScrollViewer) as ScrollViewer;
            _listBoxSearch = GetTemplateChild(ListBoxTemplateNameSearch) as ListBox;

            _checkBox = GetTemplateChild(CheckBoxTemplateName) as CheckBox;
            if (_checkBox != null)
            {
                _checkBox.Checked -= OnCheckBox_Checked;
                _checkBox.Unchecked -= OnCheckBox_Unchecked;
                _checkBox.Checked += OnCheckBox_Checked;
                _checkBox.Unchecked += OnCheckBox_Unchecked;
            }
            if (_listBoxSearch != null)
            {
                _listBoxSearch.IsVisibleChanged -= OnListBoxSearch_IsVisibleChanged;
                _listBoxSearch.IsVisibleChanged += OnListBoxSearch_IsVisibleChanged;
                _listBoxSearch.SelectionChanged -= OnListBoxSearch_SelectionChanged;
                _listBoxSearch.SelectionChanged += OnListBoxSearch_SelectionChanged;
            }
            _itemsPresenter = GetTemplateChild(PART_ItemsPresenter) as ItemsPresenter;
            if (ShowType == ShowType.Tag)
            {
                AddHandler(Controls.Tag.CloseEvent, new RoutedEventHandler(Tags_Close));
                _panel = GetTemplateChild(PART_SimpleWrapPanel) as Panel;
            }
        }

        private void ApplySearchLogic()
        {
            if (Items.Count > 0 && ItemsSource != null && IsSearch)
            {
                _textBox.Visibility = Visibility.Visible;
                _textBox.TextChanged -= OnTextbox_TextChanged;
                _textBox.TextChanged += OnTextbox_TextChanged;
            }
        }

        private void OnPopup_Closed(object sender, EventArgs e)
        {
            _window.PreviewMouseDown -= OnWindowPreviewMouseDown;
        }

        private void WindowSourceInitialized(object sender, EventArgs e)
        {
            var window = sender as Window;
            if (window != null)
            {
                _hwndSource = PresentationSource.FromVisual(window) as HwndSource;
                if (_hwndSource != null)
                {
                    _hwndSource.AddHook(WndProc);
                }
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_NCLBUTTONDOWN = 0x00A1;
            if (msg == WM_NCLBUTTONDOWN)
            {
                IsDropDownOpen = false;
            }
            return IntPtr.Zero;
        }

        private void OnPopup_Opened(object sender, EventArgs e)
        {
            _window.PreviewMouseDown += OnWindowPreviewMouseDown;
            UpdateTags();
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                if (_popup == null) return;

                double popupHeight = _panelDropDown.ActualHeight;
                var controlScreenPos = PointToScreen(new Point(0, RenderSize.Height));
                var window = Window.GetWindow(this);
                if (window == null) return;

                var windowScreenPos = window.PointToScreen(new Point(0, 0));
                double availableBottomSpace = (windowScreenPos.Y + window.ActualHeight) - controlScreenPos.Y;
                double availableTopSpace = controlScreenPos.Y - windowScreenPos.Y;
                if (availableBottomSpace < popupHeight && availableTopSpace > popupHeight)
                {
                    _popup.Placement = PlacementMode.Top;
                    _popup.VerticalOffset = -popupHeight - 2;
                }
                else
                {
                    _popup.Placement = PlacementMode.Bottom;
                    _popup.VerticalOffset = 0;
                }
            }));
        }

        private void OnWindowPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsMouseOver)
                IsDropDownOpen = false;
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
                        if (SelectedItems.Contains(item))
                            SelectedItems.Remove(item);
                    }

                    if (selectedList.Contains(item))
                        selectedList.Remove(item);
                }

                UpdateText();
                SelectionChecked(_listBoxSearch);
            }

            if (e.AddedItems.Count > 0)
            {
                foreach (var item in e.AddedItems)
                {
                    if (!SelectedItems.Contains(item))
                        SelectedItems.Add(item);
                }

                UpdateText();
                SelectionChecked(_listBoxSearch);
            }
        }

        private void OnListBoxSearch_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                UpdateIsChecked(_listBoxSearch);
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

        private void OnCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_listBoxSearch.Visibility == Visibility.Visible)
                _listBoxSearch.UnselectAll();
            else
                UnselectAll();
        }

        private void OnCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (_listBoxSearch.Visibility == Visibility.Visible)
                _listBoxSearch.SelectAll();
            else
                SelectAll();
        }

        private void OnTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_theLastText))
                _theLastText = _textBox.Text;
            SearchText(_textBox.Text);
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

        private void SearchText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                if (_listBoxSearch.Visibility != Visibility.Collapsed)
                    _listBoxSearch.Visibility = Visibility.Collapsed;
                if (_scrollViewer.Visibility != Visibility.Visible)
                {
                    _scrollViewer.Visibility = Visibility.Visible;
                    UpdateIsChecked(this);
                }

            }
            else
            {
                if (_listBoxSearch.Visibility != Visibility.Visible)
                    _listBoxSearch.Visibility = Visibility.Visible;
                if (_scrollViewer.Visibility != Visibility.Collapsed)
                    _scrollViewer.Visibility = Visibility.Collapsed;
                var listSearch = new List<object>();
                foreach (var item in Items)
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

                ItemsSourceSearch = listSearch;
                SelectionChecked(_listBoxSearch);
                selectedItems.Clear();
                foreach (var item in _listBoxSearch.Items)
                {
                    if (SelectedItems.Contains(item))
                        if (!_listBoxSearch.SelectedItems.Contains(item))
                            _listBoxSearch.SelectedItems.Add(item);
                }
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

        private string GetPropertyValue(object item)
        {
            if (string.IsNullOrEmpty(DisplayMemberPath))
            {
                if (item is ContentControl contentControl && contentControl.Content != null)
                {
                    return contentControl.Content.ToString() ?? string.Empty;
                }
                return item?.ToString() ?? string.Empty;
            }
            var result = string.Empty;
            var nameParts = DisplayMemberPath.Split('.');
            if (nameParts.Length == 1)
            {
                var property = item.GetType().GetProperty(DisplayMemberPath);
                if (property != null)
                {
                    return (property.GetValue(item, null) ?? string.Empty).ToString();
                }
            }
            return result.ToUpperInvariant();
        }

        private void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SelectAllContent = LanguageManager.Instance["SelectAll"];
        }

        private void OnMultiSelectComboBoxItem_Unselected(object sender, RoutedEventArgs e)
        {
            if (_ignoreTextValueChanged) return;
            _ignoreTextValueChanged = true;
            UnselectAll();
            _ignoreTextValueChanged = false;
            UpdateText();
        }

        private void OnMultiSelectComboBoxItem_Selected(object sender, RoutedEventArgs e)
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
                SetCurrentValue(TextProperty, newValue);
                if (ShowType == ShowType.Tag)
                    UpdateTags();
                _ignoreTextValueChanged = false;
            }
        }

        private void UpdateTags()
        {
            if (_panel == null) return;
            _panel.Children.Clear();
            foreach (var item in SelectedItems)
            {
                if (ItemContainerGenerator.ContainerFromItem(item) is MultiSelectComboBoxItem multiSelectComboBoxItem)
                    CreateTag(item, multiSelectComboBoxItem);
                else
                    CreateTag(item);
            }
        }

        void CreateTag(object item, MultiSelectComboBoxItem multiSelectComboBoxItem = null)
        {
            var tag = new Tag { Padding = new Thickness(2, 0, 0, 0) };
            if (ItemsSource != null)
            {
                var binding = new Binding(DisplayMemberPath) { Source = item };
                tag.SetBinding(ContentControl.ContentProperty, binding);
            }
            else
            {
                if (multiSelectComboBoxItem != null)
                    tag.Content = multiSelectComboBoxItem.Content;
            }
            if (multiSelectComboBoxItem != null)
                tag.Tag = multiSelectComboBoxItem;
            ElementHelper.SetCornerRadius(tag, new CornerRadius(3));
            _panel.Children.Add(tag);
        }

        private void Tags_Close(object sender, RoutedEventArgs e)
        {
            var tag = (Tag)e.OriginalSource;
            var multiSelectComboBoxItem = (MultiSelectComboBoxItem)tag.Tag;
            if (multiSelectComboBoxItem != null)
                multiSelectComboBoxItem.SetCurrentValue(IsSelectedProperty, false);
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

        private static void OnSelectedItemsExtChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var multiSelectComboBox = d as MultiSelectComboBox;
            if (multiSelectComboBox == null) return;
            if (e.NewValue != null)
            {
                var collection = e.NewValue as IList;
                if (collection.Count <= 0) return;
                multiSelectComboBox.SelectedItems.Clear();
                foreach (var item in collection)
                {
                    var name = multiSelectComboBox.GetPropertyValue(item);
                    object model = null;
                    if (!string.IsNullOrWhiteSpace(name))
                        model = multiSelectComboBox.ItemsSource.OfType<object>().FirstOrDefault(h =>
                            multiSelectComboBox.GetPropertyValue(h) == name);
                    else
                        model = multiSelectComboBox.ItemsSource.OfType<object>()
                            .FirstOrDefault(h => h == item);
                    if (model != null && !multiSelectComboBox.SelectedItems.Contains(item))
                        multiSelectComboBox.SelectedItems.Add(model);

                }
                multiSelectComboBox.UpdateText();
            }
        }
    }
    public enum ShowType
    {
        Text,
        Tag
    }
}