using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class NavScrollPanel : Control
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(NavScrollPanel),
                new PropertyMetadata(null, OnItemsSourceChanged));

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(NavScrollPanel),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(NavScrollPanel),
                new PropertyMetadata(-1, OnSelectedIndexChanged));

        private StackPanel _contentPanel;

        private ListBox _navListBox;
        private WDScrollViewer _scrollViewer;
        private bool _suppressAutoSelectDuringAnimation;

        static NavScrollPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavScrollPanel),
                new FrameworkPropertyMetadata(typeof(NavScrollPanel)));
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable) GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate) GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public int SelectedIndex
        {
            get => (int) GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _navListBox = GetTemplateChild("PART_ListBox") as ListBox;
            _scrollViewer = GetTemplateChild("PART_ScrollViewer") as WDScrollViewer;
            _contentPanel = GetTemplateChild("PART_ContentPanel") as StackPanel;

            if (_navListBox != null)
            {
                _navListBox.DisplayMemberPath = "Title";
                _navListBox.SelectionChanged -= NavListBox_SelectionChanged;
                _navListBox.SelectionChanged += NavListBox_SelectionChanged;
            }

            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
                _scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            }

            RenderContent();
        }

        private void NavListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _suppressAutoSelectDuringAnimation = true;
            SelectedIndex = _navListBox.SelectedIndex;
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_suppressAutoSelectDuringAnimation) return;
            var currentOffset = _scrollViewer.VerticalOffset;
            var viewportHeight = _scrollViewer.ViewportHeight;
            for (var i = 0; i < _contentPanel.Children.Count; i++)
            {
                var element = _contentPanel.Children[i] as FrameworkElement;
                if (element == null) continue;
                var relativePoint = element.TransformToAncestor(_scrollViewer).Transform(new Point(0, 0));
                if (relativePoint.Y >= 0 && relativePoint.Y < viewportHeight)
                {
                    if (_navListBox.SelectedIndex != i)
                    {
                        _navListBox.SelectionChanged -= NavListBox_SelectionChanged;
                        _navListBox.SelectedIndex = i;
                        _navListBox.SelectionChanged += NavListBox_SelectionChanged;
                    }

                    break;
                }
            }
        }


        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NavScrollPanel ctrl) ctrl.RenderContent();
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NavScrollPanel ctrl)
            {
                var index = (int) e.NewValue;

                if (ctrl._contentPanel != null &&
                    index >= 0 && index < ctrl._contentPanel.Children.Count)
                {
                    var target = ctrl._contentPanel.Children[index] as FrameworkElement;
                    if (target != null)
                    {
                        var virtualPoint = target.TranslatePoint(new Point(0, 0), ctrl._contentPanel);
                        ctrl._scrollViewer.AnimateScroll(virtualPoint.Y,
                            () => { ctrl._suppressAutoSelectDuringAnimation = false; });
                    }
                }
            }
        }

        private void RenderContent()
        {
            if (_contentPanel == null || ItemsSource == null || ItemTemplate == null)
                return;
            _contentPanel.Children.Clear();
            foreach (var item in ItemsSource)
            {
                var content = new ContentControl
                {
                    Content = item,
                    ContentTemplate = ItemTemplate
                };
                _contentPanel.Children.Add(content);
            }

            var panel = new SmallPanel {Height = 100};
            _contentPanel.Children.Add(panel);
        }
    }
}