using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WPFDevelopers.Samples.Helpers;

namespace WPFDevelopers.Samples.ExampleViews.DrawerMenu
{
    /// <summary>
    /// Win10MenuExample.xaml 的交互逻辑
    /// </summary>
    public partial class DrawerMenuExample : UserControl
    {
        private List<Uri> _uriList = new List<Uri>()
        {
            new Uri("ExampleViews/DrawerMenu/HomePage.xaml",UriKind.Relative),
            new Uri("ExampleViews/DrawerMenu/EdgePage.xaml",UriKind.Relative),
        };
        public DrawerMenuExample()
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
            WPFDevelopers.Controls.MessageBox.Show("点击了云盘","提示");
        });
        public ICommand MailCommand => new RelayCommand(obj =>
        {
            WPFDevelopers.Controls.MessageBox.Show("点击了邮件","提示");
        });
        public ICommand VideoCommand => new RelayCommand(obj =>
        {
            WPFDevelopers.Controls.MessageBox.Show("点击了视频","提示");
        });
    }
}
