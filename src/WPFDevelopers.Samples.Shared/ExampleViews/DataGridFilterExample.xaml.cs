using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Controls;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// DataGridFilterExample.xaml 的交互逻辑
    /// </summary>
    public partial class DataGridFilterExample : UserControl
    {
        public FilterEngine<Person> Engines
        {
            get { return (FilterEngine<Person>)GetValue(EnginesProperty); }
            set { SetValue(EnginesProperty, value); }
        }

        public static readonly DependencyProperty EnginesProperty =
            DependencyProperty.Register("Engines", typeof(FilterEngine<Person>), typeof(DataGridFilterExample), new PropertyMetadata(null));
        public DataGridFilterExample()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += DataGridFilterExample_Loaded;
        }

        private void DataGridFilterExample_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //Binding
            Engines = new FilterEngine<Person>();
            var list = new List<Person>();
            for (int i = 0; i < 10000; i++)
            {
                list.Add(new Person
                {
                    Name = "Name" + i,
                    City = i % 3 == 0 ? "Beijing" : "Shanghai",
                    Age = 20 + (i % 10)
                });
            }
            Engines.Source = list;
            Engines.Refresh();
            Engines.OnFilterApplied += OnFilterAppliedHandler;
            //C#
            var Persons = new FilterEngine<Person>();
            Persons.Source = list;
            Persons.Refresh();
            DataGridHelper.SetFilterEngine(myDataGridCodeCSharp, Persons);
            myDataGridCodeCSharp.ItemsSource = Persons.TypedView;
            //default
            myDataGridDefault.ItemsSource = list;
        }
        private void OnFilterAppliedHandler(object sender, FilterAppliedEventArgs args)
        {
           
        }
    }
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string City { get; set; }
    }
}
