using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFDevelopers.Samples.Helpers;

namespace WPFDevelopers.Samples.ExampleViews.Win10Menu
{
    /// <summary>
    /// Win10MenuExample.xaml 的交互逻辑
    /// </summary>
    public partial class Win10MenuExample : UserControl
    {
        private List<Uri> _uriList = new List<Uri>()
        {
            new Uri("ExampleViews/Win10Menu/HomePage.xaml",UriKind.Relative),
            new Uri("ExampleViews/Win10Menu/EdgePage.xaml",UriKind.Relative),
        };
        public Win10MenuExample()
        {
            InitializeComponent();
            myFrame.Navigate(_uriList[0]);
        }

        public ICommand HomeCommand => new RelayCommand(obj =>
        {
            myFrame.Navigate(_uriList[0]);
        });
        public ICommand EdgeCommand => new RelayCommand(obj =>
        {
            myFrame.Navigate(_uriList[1]);
        });
        public ICommand CloudCommand => new RelayCommand(obj =>
        {
            MessageBox.Show("点击了云盘");
        });
        public ICommand MailCommand => new RelayCommand(obj =>
        {
            MessageBox.Show("点击了邮件");
        });
        public ICommand VideoCommand => new RelayCommand(obj =>
        {
            MessageBox.Show("点击了视频");
        });
    }
}
