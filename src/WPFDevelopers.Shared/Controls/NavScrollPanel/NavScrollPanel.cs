using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = ListBoxTemplateName, Type = typeof(ListBox))]
    [TemplatePart(Name = ScrollViewerTemplateName, Type = typeof(WDScrollViewer))]
    [TemplatePart(Name = StackPanelTemplateName, Type = typeof(StackPanel))]
    public class NavScrollPanel : ItemsControl
    {
        private const string ListBoxTemplateName = "PART_ListBox";
        private const string ScrollViewerTemplateName = "PART_ScrollViewer";
        private const string StackPanelTemplateName = "PART_ContentPanel";

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

        public int SelectedIndex
        {
            get => (int) GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _navListBox = GetTemplateChild(ListBoxTemplateName) as ListBox;
            _scrollViewer = GetTemplateChild(ScrollViewerTemplateName) as WDScrollViewer;
            _contentPanel = GetTemplateChild(StackPanelTemplateName) as StackPanel;

            if (_navListBox != null)
            {
                _navListBox.SelectionChanged -= NavListBox_SelectionChanged;
                _navListBox.SelectionChanged += NavListBox_SelectionChanged;
            }

            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
                _scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            }

            RefreshContent();
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            RefreshContent();
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

        private void RefreshContent()
        {
            if (_navListBox == null || _contentPanel == null) return;
            _navListBox.Items.Clear();
            _contentPanel.Children.Clear();
            foreach (var item in Items)
            {
                if (item is NavScrollPanelItem panelItem)
                {
                    _navListBox.Items.Add(panelItem.Header);
                    
                    var content = new NavScrollPanelItem
                    {
                        Header = panelItem.Header,
                        Content = panelItem.Content,
                    };
                    _contentPanel.Children.Add(content);
                }
            }
            if (_contentPanel.Children.Count > 1)
                _contentPanel.Children.Add(new Border { Background = Brushes.Transparent, Height = 200 });
        }
    }
}