using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFDevelopers.Controls
{
    /// <summary>
    /// TextBox 控件最好替换成可以设置最值且只能输入数字的输入框
    /// </summary>
    [TemplatePart(Name = CountPerPageTextBoxTemplateName, Type = typeof(TextBox))]
    [TemplatePart(Name = JustPageTextBoxTemplateName, Type = typeof(TextBox))]
    [TemplatePart(Name = ListBoxTemplateName, Type = typeof(ListBox))]
    public class Pagination : Control
    {
        private static readonly Type _typeofSelf = typeof(Pagination);

        private const string CountPerPageTextBoxTemplateName = "PART_CountPerPageTextBox";
        private const string JustPageTextBoxTemplateName = "PART_JumpPageTextBox";
        private const string ListBoxTemplateName = "PART_ListBox";

        private const string Ellipsis = "···";

        private TextBox _countPerPageTextBox;
        private TextBox _jumpPageTextBox;
        private ListBox _listBox;

        private static RoutedCommand _prevCommand = null;
        private static RoutedCommand _nextCommand = null;

        static Pagination()
        {
            InitializeCommands();

            DefaultStyleKeyProperty.OverrideMetadata(_typeofSelf, new FrameworkPropertyMetadata(_typeofSelf));
        }

        #region Command

        private static void InitializeCommands()
        {
            _prevCommand = new RoutedCommand("Prev", _typeofSelf);
            _nextCommand = new RoutedCommand("Next", _typeofSelf);

            CommandManager.RegisterClassCommandBinding(_typeofSelf, new CommandBinding(_prevCommand, OnPrevCommand, OnCanPrevCommand));
            CommandManager.RegisterClassCommandBinding(_typeofSelf, new CommandBinding(_nextCommand, OnNextCommand, OnCanNextCommand));
        }

        public static RoutedCommand PrevCommand
        {
            get { return _prevCommand; }
        }

        public static RoutedCommand NextCommand
        {
            get { return _nextCommand; }
        }

        private static void OnPrevCommand(object sender, RoutedEventArgs e)
        {
            var ctrl = sender as Pagination;
            ctrl.Current--;
        }

        private static void OnCanPrevCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            var ctrl = sender as Pagination;
            e.CanExecute = ctrl.Current > 1;
        }

        private static void OnNextCommand(object sender, RoutedEventArgs e)
        {
            var ctrl = sender as Pagination;
            ctrl.Current++;
        }

        private static void OnCanNextCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            var ctrl = sender as Pagination;
            e.CanExecute = ctrl.Current < ctrl.PageCount;
        }

        #endregion

        #region Properties

        private static readonly DependencyPropertyKey PagesPropertyKey =
           DependencyProperty.RegisterReadOnly("Pages", typeof(IEnumerable<string>), _typeofSelf, new PropertyMetadata(null));
        public static readonly DependencyProperty PagesProperty = PagesPropertyKey.DependencyProperty;
        public IEnumerable<string> Pages
        {
            get { return (IEnumerable<string>)GetValue(PagesProperty); }
        }

        private static readonly DependencyPropertyKey PageCountPropertyKey =
           DependencyProperty.RegisterReadOnly("PageCount", typeof(int), _typeofSelf, new PropertyMetadata(1, OnPageCountPropertyChanged));
        public static readonly DependencyProperty PageCountProperty = PageCountPropertyKey.DependencyProperty;
        public int PageCount
        {
            get { return (int)GetValue(PageCountProperty); }
        }

        private static void OnPageCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as Pagination;
            var pageCount = (int)e.NewValue;

            /*
            if (ctrl._jumpPageTextBox != null)
                ctrl._jumpPageTextBox.Maximum = pageCount;
            */
        }

        public static readonly DependencyProperty IsLiteProperty = DependencyProperty.Register("IsLite", typeof(bool), _typeofSelf, new PropertyMetadata(false));
        public bool IsLite
        {
            get { return (bool)GetValue(IsLiteProperty); }
            set { SetValue(IsLiteProperty, value); }
        }

        public static readonly DependencyProperty CountProperty = DependencyProperty.Register("Count", typeof(int), _typeofSelf, new PropertyMetadata(0, OnCountPropertyChanged, CoerceCount));
        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        private static object CoerceCount(DependencyObject d, object value)
        {
            var count = (int)value;
            return Math.Max(count, 0);
        }

        private static void OnCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as Pagination;
            var count = (int)e.NewValue;

            ctrl.SetValue(PageCountPropertyKey, (int)Math.Ceiling(count * 1.0 / ctrl.CountPerPage));
            ctrl.UpdatePages();
        }

        public static readonly DependencyProperty CountPerPageProperty = DependencyProperty.Register("CountPerPage", typeof(int), _typeofSelf, new PropertyMetadata(50, OnCountPerPagePropertyChanged, CoerceCountPerPage));
        public int CountPerPage
        {
            get { return (int)GetValue(CountPerPageProperty); }
            set { SetValue(CountPerPageProperty, value); }
        }

        private static object CoerceCountPerPage(DependencyObject d, object value)
        {
            var countPerPage = (int)value;
            return Math.Max(countPerPage, 1);
        }

        private static void OnCountPerPagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as Pagination;
            var countPerPage = (int)e.NewValue;

            if (ctrl._countPerPageTextBox != null)
                ctrl._countPerPageTextBox.Text = countPerPage.ToString();

            ctrl.SetValue(PageCountPropertyKey, (int)Math.Ceiling(ctrl.Count * 1.0 / countPerPage));

            if (ctrl.Current != 1)
                ctrl.Current = 1;
            else
                ctrl.UpdatePages();
        }

        public static readonly DependencyProperty CurrentProperty = DependencyProperty.Register("Current", typeof(int), _typeofSelf, new PropertyMetadata(1, OnCurrentPropertyChanged, CoerceCurrent));
        public int Current
        {
            get { return (int)GetValue(CurrentProperty); }
            set { SetValue(CurrentProperty, value); }
        }

        private static object CoerceCurrent(DependencyObject d, object value)
        {
            var current = (int)value;
            var ctrl = d as Pagination;

            return Math.Max(current, 1);
        }

        private static void OnCurrentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as Pagination;
            var current = (int)e.NewValue;

            if (ctrl._listBox != null)
                ctrl._listBox.SelectedItem = current.ToString();

            if (ctrl._jumpPageTextBox != null)
                ctrl._jumpPageTextBox.Text = current.ToString();

            ctrl.UpdatePages();
        }

        #endregion

        #region Override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            UnsubscribeEvents();

            _countPerPageTextBox = GetTemplateChild(CountPerPageTextBoxTemplateName) as TextBox;
            _jumpPageTextBox = GetTemplateChild(JustPageTextBoxTemplateName) as TextBox;
            _listBox = GetTemplateChild(ListBoxTemplateName) as ListBox;

            Init();

            SubscribeEvents();
        }

        #endregion

        #region Event

        /// <summary>
        /// 分页
        /// </summary>
        private void OnCountPerPageTextBoxChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(_countPerPageTextBox.Text, out int _ountPerPage))
                CountPerPage = _ountPerPage;
        }

        /// <summary>
        /// 跳转页
        /// </summary>
        private void OnJumpPageTextBoxChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(_jumpPageTextBox.Text, out int _current))
                Current = _current;
        }

        /// <summary>
        /// 选择页
        /// </summary>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_listBox.SelectedItem == null)
                return;

            Current = int.Parse(_listBox.SelectedItem.ToString());
        }

        #endregion

        #region Private

        private void Init()
        {
            SetValue(PageCountPropertyKey, (int)Math.Ceiling(Count * 1.0 / CountPerPage));

            _jumpPageTextBox.Text = Current.ToString();
            //_jumpPageTextBox.Maximum = PageCount;

            _countPerPageTextBox.Text = CountPerPage.ToString();

            if (_listBox != null)
                _listBox.SelectedItem = Current.ToString();
        }

        private void UnsubscribeEvents()
        {
            if (_countPerPageTextBox != null)
                _countPerPageTextBox.TextChanged -= OnCountPerPageTextBoxChanged;

            if (_jumpPageTextBox != null)
                _jumpPageTextBox.TextChanged -= OnJumpPageTextBoxChanged;

            if (_listBox != null)
                _listBox.SelectionChanged -= OnSelectionChanged;
        }

        private void SubscribeEvents()
        {
            if (_countPerPageTextBox != null)
                _countPerPageTextBox.TextChanged += OnCountPerPageTextBoxChanged;

            if (_jumpPageTextBox != null)
                _jumpPageTextBox.TextChanged += OnJumpPageTextBoxChanged;

            if (_listBox != null)
                _listBox.SelectionChanged += OnSelectionChanged;
        }

        private void UpdatePages()
        {
            SetValue(PagesPropertyKey, GetPagers(Count, Current));

            if (_listBox != null && _listBox.SelectedItem == null)
                _listBox.SelectedItem = Current.ToString();
        }

        private IEnumerable<string> GetPagers(int count, int current)
        {
            if (count == 0)
                return null;

            if (PageCount <= 7)
                return Enumerable.Range(1, PageCount).Select(p => p.ToString()).ToArray();

            if (current <= 4)
                return new string[] { "1", "2", "3", "4", "5", Ellipsis, PageCount.ToString() };

            if (current >= PageCount - 3)
                return new string[] { "1", Ellipsis, (PageCount - 4).ToString(), (PageCount - 3).ToString(), (PageCount - 2).ToString(), (PageCount - 1).ToString(), PageCount.ToString() };

            return new string[] { "1", Ellipsis, (current - 1).ToString(), current.ToString(), (current + 1).ToString(), Ellipsis, PageCount.ToString() };
        }

        #endregion
    }
}

