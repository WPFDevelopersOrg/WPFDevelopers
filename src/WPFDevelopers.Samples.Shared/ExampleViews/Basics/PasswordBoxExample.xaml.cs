using System.Windows.Controls;
namespace WPFDevelopers.Samples.ExampleViews.Basics
{
    public partial class PasswordBoxExample : UserControl
    {
        public PasswordBoxExample()
        {
            InitializeComponent();
            myPasswordBox.Password = "WPFDevelopers";
        }
    }
}
