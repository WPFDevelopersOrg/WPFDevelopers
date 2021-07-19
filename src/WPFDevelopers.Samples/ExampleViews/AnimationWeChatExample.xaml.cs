using System.Windows.Controls;
using System.Windows.Input;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// AnimationWeChatExample.xaml 的交互逻辑
    /// </summary>
    public partial class AnimationWeChatExample : UserControl
    {
        public AnimationWeChatExample()
        {
            InitializeComponent();
        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.AudioPlay.IsLit)
                this.AudioPlay.IsLit = false;
            else
                this.AudioPlay.IsLit = true;
        }
    }
}
