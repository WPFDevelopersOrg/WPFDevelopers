using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Microsoft.Expression.Controls
{
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(PathListBoxItem))]
    public sealed class PathListBox : ListBox
    {
        private ItemsPanelTemplate ItemsPanelListener
        {
            get
            {
                return (ItemsPanelTemplate)base.GetValue(PathListBox.ItemsPanelListenerProperty);
            }
            set
            {
                base.SetValue(PathListBox.ItemsPanelListenerProperty, value);
            }
        }

        public LayoutPathCollection LayoutPaths
        {
            get
            {
                return (LayoutPathCollection)base.GetValue(PathListBox.LayoutPathsProperty);
            }
        }

        public double StartItemIndex
        {
            get
            {
                return (double)base.GetValue(PathListBox.StartItemIndexProperty);
            }
            set
            {
                base.SetValue(PathListBox.StartItemIndexProperty, value);
            }
        }

        public bool WrapItems
        {
            get
            {
                return (bool)base.GetValue(PathListBox.WrapItemsProperty);
            }
            set
            {
                base.SetValue(PathListBox.WrapItemsProperty, value);
            }
        }

        public PathListBox()
        {
            base.DefaultStyleKey = typeof(PathListBox);
            LayoutPathCollection value = new LayoutPathCollection();
            base.SetValue(PathListBox.LayoutPathsProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.itemsPanel = null;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new PathListBoxItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is PathListBoxItem;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.ItemsPanelListener == null)
            {
                base.SetBinding(PathListBox.ItemsPanelListenerProperty, new Binding("ItemsPanel")
                {
                    Source = this,
                    Mode = BindingMode.OneWay
                });
            }
            if (this.itemsPanel == null)
            {
                this.itemsPanel = (this.GetItemsHost() as PathPanel);
                if (this.itemsPanel != null)
                {
                    this.SetOneWayBinding(this.itemsPanel, PathPanel.LayoutPathsProperty, "LayoutPaths");
                    this.SetOneWayBinding(this.itemsPanel, PathPanel.StartItemIndexProperty, "StartItemIndex");
                    this.SetOneWayBinding(this.itemsPanel, PathPanel.WrapItemsProperty, "WrapItems");
                }
            }
            return base.ArrangeOverride(finalSize);
        }

        private static void ItemsPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PathListBox pathListBox = d as PathListBox;
            if (pathListBox == null)
            {
                return;
            }
            if (e.Property == PathListBox.ItemsPanelListenerProperty && e.NewValue != e.OldValue)
            {
                pathListBox.itemsPanel = null;
            }
            pathListBox.InvalidateArrange();
        }

        private void SetOneWayBinding(DependencyObject target, DependencyProperty targetProperty, string sourceProperty)
        {
            BindingOperations.SetBinding(target, targetProperty, new Binding(sourceProperty)
            {
                Source = this,
                Mode = BindingMode.OneWay
            });
        }

        private PathPanel itemsPanel;

        private static readonly DependencyProperty ItemsPanelListenerProperty = DependencyProperty.Register("ItemsPanelListener", typeof(ItemsPanelTemplate), typeof(PathListBox), new PropertyMetadata(new PropertyChangedCallback(PathListBox.ItemsPanelChanged)));

        public static readonly DependencyProperty LayoutPathsProperty = DependencyProperty.Register("LayoutPaths", typeof(LayoutPathCollection), typeof(PathListBox), new PropertyMetadata(null));

        public static readonly DependencyProperty StartItemIndexProperty = DependencyProperty.Register("StartItemIndex", typeof(double), typeof(PathListBox), new PropertyMetadata(0.0));

        public static readonly DependencyProperty WrapItemsProperty = DependencyProperty.Register("WrapItems", typeof(bool), typeof(PathListBox), new PropertyMetadata(false));
    }
}
