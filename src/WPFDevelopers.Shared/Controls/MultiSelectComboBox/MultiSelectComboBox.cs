using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = PART_Popup, Type = typeof(Popup))]
    [TemplatePart(Name = PART_SimpleWrapPanel, Type = typeof(Panel))]
    [TemplatePart(Name = CheckBoxTemplateName, Type = typeof(CheckBox))]
    [TemplatePart(Name = TextBoxTemplateName, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_DropDownPanel, Type = typeof(Panel))]
    [TemplatePart(Name = ListViewTemplateNameSearch, Type = typeof(DataGrid))]
    [TemplatePart(Name = DropDownScrollViewer, Type = typeof(ScrollViewer))]

    public class MultiSelectComboBox : ListView
    {
        private const string PART_Popup = "PART_Popup";
        private const string PART_SimpleWrapPanel = "PART_SimpleWrapPanel";
        private const string CheckBoxTemplateName = "PART_SelectAll";
        private const string TextBoxTemplateName = "PART_TextBox";
        private const string PART_DropDownPanel = "PART_DropDown";
        private const string ListViewTemplateNameSearch = "PART_SearchSelector";
        private const string DropDownScrollViewer = "DropDownScrollViewer";

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
        private ListView _listViewSearch;
        private TextBox _textBox;
        private Panel _panelDropDown;
        private ScrollViewer _scrollViewer;
        private CheckBox _checkBox;
        private string _theLastText;
        private bool _isUpdating;

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

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (element is MultiSelectComboBoxItem comboBoxItem)
            {
                if (View == null)
                    comboBoxItem.Content = this.GetDisplayAndSelectedValue(item);
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            if (!IsLoaded) return;
            if (SelectedItems != null)
            {
                foreach (var item in e.AddedItems)
                {
                    if (!SelectedItems.Contains(item))
                    {
                        TrySelectItem(item);
                    }
                }
                foreach (var item in e.RemovedItems)
                {
                    TryUnselectItem(item);
                }
                if (!_isUpdating && SelectedItemsExt != null)
                {
                    _isUpdating = true;
                    try
                    {
                        if (SelectedItemsExt is IList list)
                        {
                            if (list.Count > 0)
                                list.Clear();
                            foreach (var itm in SelectedItems.Cast<object>())
                                list.Add(itm);
                        }
                    }
                    finally
                    {
                        _isUpdating = false;
                    }
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
            _listViewSearch = GetTemplateChild(ListViewTemplateNameSearch) as ListView;

            _checkBox = GetTemplateChild(CheckBoxTemplateName) as CheckBox;
            if (_checkBox != null)
            {
                _checkBox.Checked -= OnCheckBox_Checked;
                _checkBox.Unchecked -= OnCheckBox_Unchecked;
                _checkBox.Checked += OnCheckBox_Checked;
                _checkBox.Unchecked += OnCheckBox_Unchecked;
            }
            if (_listViewSearch != null)
            {
                _listViewSearch.IsVisibleChanged -= OnDataGridSearch_IsVisibleChanged;
                _listViewSearch.IsVisibleChanged += OnDataGridSearch_IsVisibleChanged;
                _listViewSearch.SelectionChanged -= OnDataGridSearch_SelectionChanged;
                _listViewSearch.SelectionChanged += OnDataGridSearch_SelectionChanged;
            }

            if (ShowType == ShowType.Tag)
            {
                AddHandler(Controls.Tag.CloseEvent, new RoutedEventHandler(Tags_Close));
                _panel = GetTemplateChild(PART_SimpleWrapPanel) as Panel;
            }
            SyncListViewViews();
            Loaded += OnMultiSelectComboBox_Loaded;
        }

        private void OnMultiSelectComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateText();
        }

        public MultiSelectComboBox()
        {
            IsVisibleChanged += OnMultiSelectComboBox_IsVisibleChanged;
        }

        private void OnMultiSelectComboBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible && IsLoaded && IsVisible)
            {
                InvalidateMeasure();
                InvalidateArrange();
                UpdateLayout();
                UpdateText();
            }
        }

        private void SyncListViewViews()
        {
            if (_listViewSearch != null && View != null)
            {
                _listViewSearch.View = CloneGridView(View as GridView);
            }
        }

        private static GridView CloneGridView(GridView originalGridView)
        {
            if (originalGridView == null) return null;
            var newGridView = new GridView();
            foreach (GridViewColumn column in originalGridView.Columns)
            {
                var newColumn = new GridViewColumn
                {
                    Header = column.Header,
                    Width = column.Width,
                    DisplayMemberBinding = column.DisplayMemberBinding,
                    CellTemplate = column.CellTemplate,
                    CellTemplateSelector = column.CellTemplateSelector
                };
                newGridView.Columns.Add(newColumn);
            }
            return newGridView;
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

        private void OnDataGridSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_listViewSearch.IsVisible)
                return;
            if (e.RemovedItems.Count > 0)
            {
                foreach (var item in e.RemovedItems)
                {
                    if (selectedSearchList.Contains(item))
                        selectedSearchList.Remove(item);
                    if (_listViewSearch.Items.Contains(item))
                    {
                        if (SelectedItems.Contains(item))
                            TryUnselectItem(item);
                    }

                    if (selectedList.Contains(item))
                        selectedList.Remove(item);
                }

                UpdateText();
                SelectionChecked(_listViewSearch);
            }

            if (e.AddedItems.Count > 0)
            {
                foreach (var item in e.AddedItems)
                {
                    if (!SelectedItems.Contains(item))
                        TrySelectItem(item);
                }

                UpdateText();
                SelectionChecked(_listViewSearch);
            }
        }

        private void OnDataGridSearch_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                UpdateIsChecked(_listViewSearch);
        }

        private void UpdateIsChecked(ListView listView)
        {
            _checkBox.Checked -= OnCheckBox_Checked;
            if (listView.Items.Count > 0 && listView.Items.Count == listView.SelectedItems.Count)
            {
                if (_checkBox.IsChecked != true)
                    _checkBox.IsChecked = true;
            }
            else
            {
                if (listView.SelectedItems.Count == 0)
                    _checkBox.IsChecked = false;
                else
                    _checkBox.IsChecked = null;
            }

            _checkBox.Checked += OnCheckBox_Checked;
        }

        private void OnCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_listViewSearch.Visibility == Visibility.Visible)
                _listViewSearch.UnselectAll();
            else
                UnselectAll();
        }

        private void OnCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (_listViewSearch.Visibility == Visibility.Visible)
                _listViewSearch.SelectAll();
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
                if (_listViewSearch.Visibility != Visibility.Collapsed)
                    _listViewSearch.Visibility = Visibility.Collapsed;
                if (_scrollViewer.Visibility != Visibility.Visible)
                {
                    _scrollViewer.Visibility = Visibility.Visible;
                    UpdateIsChecked(this);
                }

            }
            else
            {
                if (_listViewSearch.Visibility != Visibility.Visible)
                    _listViewSearch.Visibility = Visibility.Visible;
                if (_scrollViewer.Visibility != Visibility.Collapsed)
                    _scrollViewer.Visibility = Visibility.Collapsed;
                var listSearch = new List<object>();
                foreach (var item in Items)
                {
                    var str = this.GetDisplayAndSelectedValue(item).ToString();
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
                SelectionChecked(_listViewSearch);
                selectedItems.Clear();
                foreach (var item in _listViewSearch.Items)
                {
                    //if (SelectedItems.Contains(item))
                    //    if (!_listViewSearch.SelectedItems.Contains(item))
                    //        _listViewSearch.SelectedItems.Add(item);
                    if (SelectedItems.Contains(item))
                    {
                        if (!_listViewSearch.SelectedItems.Contains(item))
                        {
                            if (SelectionMode == SelectionMode.Single)
                            {
                                _listViewSearch.SelectedItem = item;
                            }
                            else
                            {
                                if (!_listViewSearch.SelectedItems.Contains(item))
                                    _listViewSearch.SelectedItems.Add(item);
                            }
                        }
                    }
                }
            }
        }

        private void SelectionChecked(ListView listView)
        {
            if (_checkBox == null) return;
            if (listView.SelectedItems.Count > 0
                &&
                listView.Items.Count == listView.SelectedItems.Count)
            {
                _checkBox.Checked -= OnCheckBox_Checked;
                _checkBox.IsChecked = true;
                _checkBox.Checked += OnCheckBox_Checked;
            }
            else
            {
                _checkBox.Checked -= OnCheckBox_Checked;
                if (listView.SelectedItems.Count > 0
                    &&
                    listView.Items.Count == listView.SelectedItems.Count)
                {
                    if (_checkBox.IsChecked != true)
                        _checkBox.IsChecked = true;
                }
                else
                {
                    if (listView.SelectedItems.Count == 0)
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
            var newValue = string.Join(Delimiter, SelectedItems.Cast<object>().Select(x => this.GetDisplayAndSelectedValue(x).ToString()));
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
            bool isUsingGridView = View != null;
            if (isUsingGridView)
            {
                if (multiSelectComboBoxItem != null)
                {
                    if ((!string.IsNullOrEmpty(SelectedValuePath) || !string.IsNullOrEmpty(DisplayMemberPath)) && item != null)
                    {
                        var propertyInfo = item.GetType().GetProperty(SelectedValuePath);
                        propertyInfo = propertyInfo == null ? item.GetType().GetProperty(DisplayMemberPath) : propertyInfo;
                        if (propertyInfo != null)
                        {
                            tag.Content = propertyInfo.GetValue(item, null);
                        }
                        else
                        {
                            tag.Content = multiSelectComboBoxItem.Content;
                        }
                    }
                    else
                    {
                        var contentPresenter = multiSelectComboBoxItem.Template?.FindName("PART_ContentPresenter", multiSelectComboBoxItem) as ContentPresenter;
                        if (contentPresenter?.Content != null)
                        {
                            tag.Content = contentPresenter.Content;
                        }
                        else
                        {
                            tag.Content = multiSelectComboBoxItem.Content;
                        }
                    }
                }
            }
            else
            {
                if (ItemsSource != null && (!string.IsNullOrEmpty(SelectedValuePath) || !string.IsNullOrEmpty(DisplayMemberPath)))
                {
                    var bindingPath = !string.IsNullOrEmpty(SelectedValuePath) ? SelectedValuePath : DisplayMemberPath;
                    var property = item.GetType().GetProperty(bindingPath);
                    if (property != null && property.GetValue(item, null) != null)
                    {
                        var binding = new Binding(bindingPath) { Source = item };
                        tag.SetBinding(ContentControl.ContentProperty, binding);
                    }
                    else
                        tag.Content = item;
                }
                else
                {
                    if (item != null)
                        tag.Content = item;
                }
            }

            if (multiSelectComboBoxItem != null)
                tag.Tag = multiSelectComboBoxItem;
            else
                tag.Tag = item;
            ElementHelper.SetCornerRadius(tag, new CornerRadius(3));
            _panel.Children.Add(tag);
        }

        private void Tags_Close(object sender, RoutedEventArgs e)
        {
            var tag = (Tag)e.OriginalSource;
            if (tag.Tag is MultiSelectComboBoxItem multiSelectComboBoxItem)
            {
                multiSelectComboBoxItem.SetCurrentValue(IsSelectedProperty, false);
            }
            else
            {
                var item = tag.Tag;
                if (item != null)
                {
                    if (SelectedItems.Contains(item))
                    {
                        TryUnselectItem(item);
                    }
                    else
                    {
                        var match = Items.Cast<object>().FirstOrDefault(h => this.GetDisplayAndSelectedValue(h).ToString() == this.GetDisplayAndSelectedValue(item).ToString());
                        if (match != null && SelectedItems.Contains(match))
                            TryUnselectItem(match);
                    }
                }
            }
        }

        private static void OnSelectedItemsExtChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as MultiSelectComboBox;
            if (ctrl == null) return;
            if (e.OldValue is INotifyCollectionChanged oldCollection)
                oldCollection.CollectionChanged -= ctrl.OnSelectedItemsExtCollectionChanged;
            if (e.NewValue != null && ctrl.ItemsSource != null)
            {
                if (e.NewValue is INotifyCollectionChanged newCollection)
                {
                    ctrl.SelectedItemsExt = (IList)newCollection;
                    newCollection.CollectionChanged += ctrl.OnSelectedItemsExtCollectionChanged;
                }
                var collection = e.NewValue as IList;
                if (collection.Count <= 0) return;
                ctrl.SelectedItems.Clear();
                ctrl._isUpdating = true;
                foreach (var item in collection)
                {
                    var name = ctrl.GetDisplayAndSelectedValue(item).ToString();
                    object model = null;
                    if (!string.IsNullOrWhiteSpace(name))
                        model = ctrl.ItemsSource.OfType<object>().FirstOrDefault(h =>
                            ctrl.GetDisplayAndSelectedValue(h).ToString() == name);
                    else
                        model = ctrl.ItemsSource.OfType<object>()
                            .FirstOrDefault(h => h == item);
                    if (model != null && !ctrl.SelectedItems.Contains(item))
                        ctrl.TrySelectItem(model);

                }
                ctrl._isUpdating = false;
                if (ctrl.IsLoaded)
                    ctrl.UpdateText();
            }
        }

        private void OnSelectedItemsExtCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isUpdating) return;
            if (e.Action == NotifyCollectionChangedAction.Reset)
                SelectedItems.Clear();
            _isUpdating = true;
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (SelectedItems.Contains(item))
                        TryUnselectItem(item);
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (!SelectedItems.Contains(item))
                        TrySelectItem(item);
                }
            }
            _isUpdating = false;
        }

        private void TrySelectItem(object item)
        {
            if (SelectionMode == SelectionMode.Single)
            {
                SelectedItem = item;
            }
            else
            {
                if (!SelectedItems.Contains(item))
                    SelectedItems.Add(item);
            }
        }

        private void TryUnselectItem(object item)
        {
            if (SelectionMode == SelectionMode.Single)
            {
                if (Equals(SelectedItem, item))
                    SelectedItem = null;
            }
            else
            {
                if (SelectedItems.Contains(item))
                    SelectedItems.Remove(item);
            }
        }

    }
    public enum ShowType
    {
        Text,
        Tag
    }
}