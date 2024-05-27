using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using System;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// DrapViewExample.xaml 的交互逻辑
    /// </summary>
    public partial class DrapViewExample : UserControl
    {
        Point start;
        TransformThumb element;
        Rectangle shape;
        ThumbType selectThumb;
        Brush currentBrush = new SolidColorBrush(Colors.Red);
        public DrapViewExample()
        {
            InitializeComponent();
        }
        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //鼠标左键按下
            Debug.WriteLine("鼠标左键按下");
            // 更新选中项
            UpdateDraw();
            // 判定是否包含快捷键

            // 判定左键按下
            if (element != null && element.PrintState == PrintState.Edit)
            {
                // 设置选项选中
                element.IsSeleted = true;
                element = null;
                e.Handled = true;
                return;
            }

            start = e.GetPosition(MainCanvas);
            // 创建几何实例
            shape = new Rectangle();
            shape.Stroke = currentBrush;
            shape.Fill = currentBrush;
            shape.StrokeThickness = 1;
            shape.Width = 0;
            shape.Height = 0;
            shape.MouseLeftButtonDown += (s, e1) => {
                e.Handled = false;
            };
            element = new TransformThumb();
            element.MouseLeftButtonDown += (s, e2) => {
                TransformThumb transform = s as TransformThumb;
                if (transform != null)
                {
                    Debug.WriteLine($"节点：{transform.Id} 节点按下");
                    transform.PrintState = PrintState.Edit;
                    element = transform;
                    e.Handled = false;
                }
            };
            element.MouseLeftButtonUp += (s, e3) => {
                e.Handled = false;
            };
            
            element.Content = shape;
            element.Width = shape.Width;
            element.Height = shape.Height;
            element.ThumbType = selectThumb;
            element.ContentType = shape.GetType();
            element.PrintState = PrintState.Printing;
            
        }

        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 鼠标左键键起
            Debug.WriteLine("鼠标左键键起");

            // 更新选中项
            UpdateDraw();
            // 键下和键起在同一个位置表示页面无选择项
            Point end = Mouse.GetPosition(MainCanvas);
            if (end == start)
            {
                // 移除节点
                MainCanvas.Children.Remove(element);
                // 重置绘制节点
                element = null;
                return;
            }
            if (element != null && element.PrintState != PrintState.Edit)
            {
                element.IsSeleted = true;
                element.PrintState = PrintState.Edit;
            }
            // 重置绘制节点
            element = null;
        }

        private void UpdateDraw()
        {
            // 设置其他项默认隐藏
            foreach (var item in MainCanvas.Children)
            {
                var child = item as TransformThumb;
                if (child != null && child.IsSeleted && element != null)
                {
                    // 重置非选中项
                    if (child != element)
                    {
                        child.IsSeleted = false;
                        child.PrintState = PrintState.Normal;
                    }
                }
            }
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (element == null)
            {
                return;
            }
            if (!NodeExist(element))
            {
                // 设置几何位置
                Canvas.SetLeft(element, start.X);
                Canvas.SetTop(element, start.Y);
                // 更新其他选中项
                UpdateDraw();
                // 添加到面板中
                MainCanvas.Children.Add(element);
                lst_layers.Items.Add(element.ToString());
            }
            // 鼠标移动(鼠标未按下移动事件不生效)
            if (e.LeftButton == MouseButtonState.Pressed && element.PrintState == PrintState.Printing)
            {
                Debug.WriteLine("鼠标移动绘制几何");
                Point current = e.GetPosition(MainCanvas);
                // 判定几何实例
                // 判定鼠标移动点是否小于起始点
                if (current.X - start.X < 0)
                {
                    // 设置起始点为移动点
                    Canvas.SetLeft(element, current.X);
                }
                if (current.Y - start.Y < 0)
                {
                    // 设置起始点为移动点
                    Canvas.SetTop(element, current.Y);
                }
                shape.SetValue(WidthProperty, Math.Abs(current.X - start.X));
                shape.SetValue(HeightProperty, Math.Abs(current.Y - start.Y));
                element.Width = Math.Abs(current.X - start.X);
                element.Height = Math.Abs(current.Y - start.Y);
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton != null)
            {
                ThumbType result;
                if (Enum.TryParse(radioButton.Content.ToString(), out result))
                {
                    selectThumb = result;
                }
            }
        }

        private void btn_remove_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < MainCanvas.Children.Count; i++)
            {
                UIElement item = MainCanvas.Children[i];
                if (item is TransformThumb)
                {
                    var tranform = item as TransformThumb;
                    if (tranform != null && tranform.IsSeleted)
                    {
                        MainCanvas.Children.Remove(tranform);
                        lst_layers.Items.Remove(tranform.ToString());
                    }
                }
            }
        }

        private bool NodeExist(TransformThumb node)
        {
            bool result = false;
            int count = MainCanvas.Children.Count;
            for (int i = 0; i < count; i++)
            {
                UIElement item = MainCanvas.Children[i];
                if (item is TransformThumb)
                {
                    var tranform = item as TransformThumb;
                    if (tranform == node)
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        private void btn_removeAll_Click(object sender, RoutedEventArgs e)
        {
            MainCanvas.Children.Clear();
            lst_layers.Items.Clear();
        }

        private void StackPanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is RadioButton)
            {
                var radioButton = (RadioButton)e.Source;
                currentBrush = radioButton.Background;
            }
        }
    }
}
