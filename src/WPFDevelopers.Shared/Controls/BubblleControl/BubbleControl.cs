using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = BorderTemplateName, Type = typeof(Border))]
    [TemplatePart(Name = EllipseTemplateName, Type = typeof(Ellipse))]
    [TemplatePart(Name = RotateTransformTemplateName, Type = typeof(RotateTransform))]
    public class BubblleControl : Control
    {
        private const string BorderTemplateName = "PART_Border";
        private const string EllipseTemplateName = "PART_Ellipse";
        private const string RotateTransformTemplateName = "PART_EllipseRotateTransform";
        private const string ListBoxTemplateName = "PART_ListBox";

        private static readonly Type _typeofSelf = typeof(BubblleControl);

        private ObservableCollection<BubblleItem> _items = new ObservableCollection<BubblleItem>();


        private Border _border;
        private Ellipse _ellipse;
        private RotateTransform _rotateTransform;
        private Brush[] brushs;
        private ItemsControl _listBox;
        private static RoutedCommand _clieckCommand;

        class BubblleItem
        {
            public string Text { get; set; }
            public Brush Bg { get; set; }
        }

        static BubblleControl()
        {
            InitializeCommands();
            DefaultStyleKeyProperty.OverrideMetadata(_typeofSelf, new FrameworkPropertyMetadata(_typeofSelf));
        }

        #region Event

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), _typeofSelf);
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        #endregion

        #region Command

        private static RoutedCommand _clickCommand = null;

        private static void InitializeCommands()
        {
            _clickCommand = new RoutedCommand("Click", _typeofSelf);

            CommandManager.RegisterClassCommandBinding(_typeofSelf, new CommandBinding(_clickCommand, OnClickCommand, OnCanClickCommand));
        }

        public static RoutedCommand ClickCommand
        {
            get { return _clickCommand; }
        }

        private static void OnClickCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var ctrl = sender as BubblleControl;

            ctrl.SetValue(SelectedTextPropertyKey, e.Parameter?.ToString());
            ctrl.RaiseEvent(new RoutedEventArgs(ClickEvent));
        }

        private static void OnCanClickCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion

        #region readonly Properties

        private static readonly DependencyPropertyKey SelectedTextPropertyKey =
           DependencyProperty.RegisterReadOnly("SelectedText", typeof(string), _typeofSelf, new PropertyMetadata(null));
        public static readonly DependencyProperty SelectedTextProperty = SelectedTextPropertyKey.DependencyProperty;
        public string SelectedText
        {
            get { return (string)GetValue(SelectedTextProperty); }
        }
        public new static readonly DependencyProperty BorderBackgroundProperty =
            DependencyProperty.Register("BorderBackground", typeof(Brush), typeof(BubblleControl),
                new PropertyMetadata(null));

        public new static readonly DependencyProperty EarthBackgroundProperty =
            DependencyProperty.Register("EarthBackground", typeof(Brush), typeof(BubblleControl),
                new PropertyMetadata(Brushes.DarkOrchid));
        public Brush BorderBackground
        {
            get => (Brush)this.GetValue(BorderBackgroundProperty);
            set => this.SetValue(BorderBackgroundProperty, (object)value);
        }
        public Brush EarthBackground
        {
            get => (Brush)this.GetValue(EarthBackgroundProperty);
            set => this.SetValue(EarthBackgroundProperty, (object)value);
        }
        #endregion

        #region Property

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<string>), typeof(BubblleControl), new PropertyMetadata(null, OnItemsSourcePropertyChanged));
        public IEnumerable<string> ItemsSource
        {
            get { return (IEnumerable<string>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void OnItemsSourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = obj as BubblleControl;
            var newValue = e.NewValue as IEnumerable<string>;

            if (newValue == null)
            {
                ctrl._items.Clear();
                return;
            }

            foreach (var item in newValue)
            {
                ctrl._items.Add(new BubblleItem { Text = item, Bg = ControlsHelper.RandomBrush() });
            }
        }

        #endregion

        #region Override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _border = GetTemplateChild(BorderTemplateName) as Border;
            _ellipse = GetTemplateChild(EllipseTemplateName) as Ellipse;
            _rotateTransform = GetTemplateChild(RotateTransformTemplateName) as RotateTransform;
            Loaded += delegate
            {
                var point = _border.TranslatePoint(new Point(_border.ActualWidth / 2, _border.ActualHeight / 2),
                    _ellipse);
                _rotateTransform.CenterX = point.X - _ellipse.ActualWidth / 2;
                _rotateTransform.CenterY = point.Y - _ellipse.ActualHeight / 2;
            };
            _listBox = GetTemplateChild(ListBoxTemplateName) as ItemsControl;
            _listBox.ItemsSource = _items;
        }

        #endregion
    }
}