using System.Collections.Generic;
using System;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using System.Reflection;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// StepExample.xaml 的交互逻辑
    /// </summary>
    public partial class BreadCrumbBarExample : UserControl
    {
        private int index = 0;
        private List<BreadcrumbItem> Breadcrumbs = new List<BreadcrumbItem>();

        public ObservableCollection<BreadcrumbItem> BreadcrumbItems
        {
            get { return (ObservableCollection<BreadcrumbItem>)GetValue(BreadcrumbItemsProperty); }
            set { SetValue(BreadcrumbItemsProperty, value); }
        }

        public static readonly DependencyProperty BreadcrumbItemsProperty =
            DependencyProperty.Register("BreadcrumbItems", typeof(ObservableCollection<BreadcrumbItem>), typeof(BreadCrumbBarExample), new PropertyMetadata(null));

        public BreadcrumbItem BreadcrumbItem
        {
            get { return (BreadcrumbItem)GetValue(BreadcrumbItemProperty); }
            set { SetValue(BreadcrumbItemProperty, value); }
        }

        public static readonly DependencyProperty BreadcrumbItemProperty =
            DependencyProperty.Register("BreadcrumbItem", typeof(BreadcrumbItem), typeof(BreadCrumbBarExample), new PropertyMetadata(null));

        public BreadCrumbBarExample()
        {
            InitializeComponent();
            Loaded += BreadCrumbBarExample_Loaded;

        }

        private void BreadCrumbBarExample_Loaded(object sender, RoutedEventArgs e)
        {
            var breadcrumbItems = new List<BreadcrumbItem>()
            {
                new BreadcrumbItem() { Text = "主页" , Uri=new Uri("pack://application:,,,/WPFDevelopers.Samples;component/ExampleViews/DrawerMenu/HomePage.xaml",UriKind.Absolute ) },
                new BreadcrumbItem() { Text = "Edge", Uri=new Uri("pack://application:,,,/WPFDevelopers.Samples;component/ExampleViews/DrawerMenu/EdgePage.xaml" ,UriKind.Absolute ) },
                new BreadcrumbItem() { Text = "邮件", Uri=new Uri("pack://application:,,,/WPFDevelopers.Samples;component/ExampleViews/DrawerMenu/EmailPage.xaml" ,UriKind.Absolute ) },

            };
            Breadcrumbs = breadcrumbItems;
            myFrame.Navigate(Breadcrumbs[index].Uri);
            BreadcrumbItems = new ObservableCollection<BreadcrumbItem>() { Breadcrumbs[index] };
        }
        private void BreadCrumbBar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            myFrame.Navigate(BreadcrumbItem.Uri);
            index = BreadcrumbItems.IndexOf(BreadcrumbItem);
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            index += 1;
            if (index >= Breadcrumbs.Count) return;
            var model = Breadcrumbs[index];
            if (BreadcrumbItems.Contains(model))
            {
                BreadcrumbItem = model;
                return;
            }
            BreadcrumbItems.Add(model);
            BreadcrumbItem = model;
        }

    }
    public class BreadcrumbItem
    {
        public string Text { get; set; }
        public Uri Uri { get; set; }
    }
}
