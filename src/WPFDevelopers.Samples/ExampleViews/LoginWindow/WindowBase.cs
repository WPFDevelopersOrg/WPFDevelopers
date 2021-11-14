using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    public class WindowBase : Window
    {
        /// <summary>
        /// 初始化窗口
        /// </summary>
        protected void InitWindow()
        {
            AddWindowControl();
        }

        /// <summary>
        /// 添加窗口控制
        /// </summary>
        protected void AddWindowControl()
        {
            // 关闭按钮
            ((Button)GetTemplateChild("CloseButton")).Click += delegate
            {
                Close();
            };
        }
       
    }
}
