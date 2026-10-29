using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Samples.Controls
{
    public class Theme : ListBox
    {
        private static readonly DependencyProperty CurrentPrimaryColorProperty =
            DependencyProperty.Register("CurrentPrimaryColor", typeof(Color), typeof(Theme),
                new PropertyMetadata(default(Color), OnCurrentPrimaryColorChanged));

        private bool _synchronizingSelection;
       
        private readonly List<Brush> themes = new List<Brush>
        {
            new SolidColorBrush(Color.FromRgb(64, 158, 255)),  // #409EFF
            new SolidColorBrush(Color.FromRgb(255, 3, 62)),    // #FF033E
            new SolidColorBrush(Color.FromRgb(162, 27, 252)),  // #A21BFC
            new SolidColorBrush(Color.FromRgb(254, 148, 38)),  // #FE9426
            new SolidColorBrush(Color.FromRgb(0, 176, 80)),    // #00B050
            new SolidColorBrush(Color.FromRgb(255, 0, 127))    // #FF007F
        };

        public ObservableCollection<ThemeItem> ThemeItems { get; set; }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Theme), new PropertyMetadata(Orientation.Horizontal));


        static Theme()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Theme),
                new FrameworkPropertyMetadata(typeof(Theme)));
        }

        public Theme()
        {
            SelectionMode = SelectionMode.Single;
            ThemeItems = new ObservableCollection<ThemeItem>();
            foreach (var item in themes)
            {
                ThemeItems.Add(new ThemeItem
                {
                    Background = item,
                });
            }
            ItemsSource = ThemeItems;
            SetResourceReference(CurrentPrimaryColorProperty, "WD.PrimaryColor");
            Loaded += (sender, e) => SynchronizeSelection();
        }

        private static void OnCurrentPrimaryColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Theme)d).SynchronizeSelection();
        }

        private void SynchronizeSelection()
        {
            var color = (Color)GetValue(CurrentPrimaryColorProperty);
            ThemeItem selectedItem = null;
            foreach (var item in ThemeItems)
            {
                var brush = item.Background as SolidColorBrush;
                if (brush != null && brush.Color == color)
                {
                    selectedItem = item;
                    break;
                }
            }

            _synchronizingSelection = true;
            try
            {
                SelectedItem = selectedItem;
            }
            finally
            {
                _synchronizingSelection = false;
            }
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ThemeItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ThemeItem();
        }
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            if (_synchronizingSelection) return;
            if (SelectedItem == null) return;
            var item = SelectedItem as ThemeItem;
            if (item == null) return;
            if (item.Background is SolidColorBrush solidColorBrush)
                ThemeManager.Instance.SetColor(solidColorBrush.Color);
        }
    }
}