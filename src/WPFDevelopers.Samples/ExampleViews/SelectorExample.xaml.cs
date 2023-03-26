using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.ObjectModel;
using WPFDevelopers.Sample.Models;
using System;
using System.Linq;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// SelectorExample.xaml 的交互逻辑
    /// </summary>
    public partial class SelectorExample : UserControl
    {
        public ObservableCollection<UserModel> ListArrays
        {
            get { return (ObservableCollection<UserModel>)GetValue(ListArraysProperty); }
            set { SetValue(ListArraysProperty, value); }
        }

        public static readonly DependencyProperty ListArraysProperty =
            DependencyProperty.Register("ListArrays", typeof(ObservableCollection<UserModel>), typeof(SelectorExample), new PropertyMetadata(null));


        public UserModel SelectItem
        {
            get { return (UserModel)GetValue(SelectItemProperty); }
            set { SetValue(SelectItemProperty, value); }
        }

        public static readonly DependencyProperty SelectItemProperty =
            DependencyProperty.Register("SelectItem", typeof(UserModel), typeof(SelectorExample), new PropertyMetadata(null));


        public SelectorExample()
        {
            InitializeComponent();
            Loaded += SelectorExample_Loaded;
        }

        private void SelectorExample_Loaded(object sender, RoutedEventArgs e)
        {
            ListArrays = new ObservableCollection<UserModel>();
            for (int i = 0; i < 200; i++)
            {
                ListArrays.Add(new UserModel { Name = i.ToString(), Date = DateTime.Now.AddDays(i) });
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var index = 150;
            if (string.IsNullOrWhiteSpace(MyTextBox.Text) || !int.TryParse(MyTextBox.Text, out index))
                index = 150;
            SelectItem = ListArrays.FirstOrDefault(i => i.Name == index.ToString());
        }
    }
}
