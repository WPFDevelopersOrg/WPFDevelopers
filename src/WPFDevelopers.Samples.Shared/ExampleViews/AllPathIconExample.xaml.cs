using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFDevelopers.Controls;
using ListBox = System.Windows.Controls.ListBox;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// AllPathIconExample.xaml 的交互逻辑
    /// </summary>
    public partial class AllPathIconExample : UserControl
    {
        string PathIconXaml = " <wd:PathIcon Kind=\"{0}\" />";
        public List<PackIconKind> AllPathIcon
        {
            get { return (List<PackIconKind>)GetValue(AllPathIconProperty); }
            set { SetValue(AllPathIconProperty, value); }
        }

        public static readonly DependencyProperty AllPathIconProperty =
            DependencyProperty.Register("AllPathIcon", typeof(List<PackIconKind>), typeof(AllPathIconExample), new PropertyMetadata(null));


        public string PathIconText
        {
            get { return (string)GetValue(PathIconTextProperty); }
            set { SetValue(PathIconTextProperty, value); }
        }

        public static readonly DependencyProperty PathIconTextProperty =
            DependencyProperty.Register("PathIconText", typeof(string), typeof(AllPathIconExample), new PropertyMetadata(null));

        public Geometry Data
        {
            get { return (Geometry)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(Geometry), typeof(AllPathIconExample), new PropertyMetadata(null));



        public AllPathIconExample()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += OnAllPathIconExample_Loaded;
        }

        private void OnAllPathIconExample_Loaded(object sender, RoutedEventArgs e)
        {
            var iconList = new List<PackIconKind>();
            foreach (PackIconKind iconKind in Enum.GetValues(typeof(PackIconKind)))
            {
                iconList.Add(iconKind);
            }
            AllPathIcon = new List<PackIconKind>(iconList);
        }
        private void OnPathIconSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && sender is ListBox listBox)
            {
                var kindValue = e.AddedItems[0].ToString();
                PathIconText = string.Format(PathIconXaml, kindValue);

                var selectedItem = e.AddedItems[0];
                var listBoxItem = listBox.ItemContainerGenerator.ContainerFromItem(selectedItem) as ListBoxItem;

                if (listBoxItem != null)
                {
                    var pathIcon = WPFDevelopers.Helpers.ControlsHelper.FindVisualChild<PathIcon>(listBoxItem);
                    if (pathIcon != null)
                        Data = pathIcon.Data;
                }
            }
        }
        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            SetClipboard(PathIconText);
        }
        private void BtnCopyGeometry_Click(object sender, RoutedEventArgs e)
        {
            SetClipboard(Data.ToString());   
        }
        void SetClipboard(string text)
        {
            Clipboard.SetText(text);
            Message.Push("已复制剪切板");
        }
    }
}
