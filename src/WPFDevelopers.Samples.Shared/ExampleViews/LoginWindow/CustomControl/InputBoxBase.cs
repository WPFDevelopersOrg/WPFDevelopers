using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WPFDevelopers.Samples.ExampleViews.LoginWindow.CustomControl
{
    public abstract class InputBoxBase : UserControl
    {
        #region 抽象方法

        /// <summary>应用文本</summary>
        protected abstract void ApplyText();

        /// <summary>应用占位文本</summary>
        protected abstract void ApplyPlaceHolder();

        /// <summary>应用图标</summary>
        protected abstract void ApplyIcon(BitmapImage icon);

        #endregion

        #region 依赖项属性

        #region 文本

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(InputBoxBase), new PropertyMetadata("", OnTextChanged));
        private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            InputBoxBase control = sender as InputBoxBase;
            control.ApplyText();
        }

        #endregion

        #region 占位文本

        public string PlaceHolder
        {
            get => (string)GetValue(PlaceHolderProperty);
            set => SetValue(PlaceHolderProperty, value);
        }
        public static readonly DependencyProperty PlaceHolderProperty =
            DependencyProperty.Register("PlaceHolder", typeof(string), typeof(InputBoxBase), new PropertyMetadata("", OnPlaceHolderChanged));
        private static void OnPlaceHolderChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            InputBoxBase control = sender as InputBoxBase;
            control.ApplyPlaceHolder();
        }

        #endregion

        #region 图标

        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(InputBoxBase), new PropertyMetadata("", OnIconChanged));
        private static void OnIconChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            InputBoxBase control = sender as InputBoxBase;
            if (control.Icon == "") control.ApplyIcon(null);
            else
            {
                try
                {
                    control.ApplyIcon(new BitmapImage(new Uri("pack://application:,,,/WPFDevelopers.Samples;component/Resources/Assets/" + control.Icon)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    control.ApplyIcon(null);
                }
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// 清除按钮_单击
        /// </summary>
        protected void Clear_Click(object sender, RoutedEventArgs e)
        {
            // 清除文本
            Text = "";
        }
    }
}