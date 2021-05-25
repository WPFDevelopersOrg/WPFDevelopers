using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CutImage
{
    public partial class DragDropView : DockPanel
    {
        public event Action UpdateImageEvent;
        #region 依赖属性
        public static readonly DependencyProperty ParentMaxWidthProperty = DependencyProperty.Register("ParentMaxWidth", typeof(double), typeof(DragDropView), new PropertyMetadata(null));
        public double ParentMaxWidth
        {
            get
            {
                return (double)this.GetValue(ParentMaxWidthProperty);
            }
            set
            {
                this.SetValue(ParentMaxWidthProperty, value);
            }
        }

        public static readonly DependencyProperty ParentMaxHeightProperty = DependencyProperty.Register("ParentMaxHeight", typeof(double), typeof(DragDropView), new PropertyMetadata(null));
        public double ParentMaxHeight
        {
            get
            {
                return (double)this.GetValue(ParentMaxHeightProperty);
            }
            set
            {
                this.SetValue(ParentMaxHeightProperty, value);
            }
        }
        #endregion
        public DragDropView()
        {
            InitializeComponent();
            RegisterEventListener();
        }
        protected virtual void RegisterEventListener()
        {
            this.rect.DragDelta += OnDragDeltaHandler;
            this.rectRightBottom.DragDelta += OnRightBottomDragDeltaHandler;
        }

        public System.Drawing.Rectangle GetCutRectangle()
        {
            return new System.Drawing.Rectangle((int)Canvas.GetLeft(this), (int)Canvas.GetTop(this), (int)this.ActualWidth, (int)this.ActualHeight);
        }

        //public void Init(int left, int top, int width, int height)
        //{
        //    Canvas.SetLeft(this, left);
        //    Canvas.SetTop(this, top);

        //    this.Width = width;
        //    this.Height = height;
        //}

        #region 中间拖动
        private void OnDragDeltaHandler(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            double left = Canvas.GetLeft(this) + e.HorizontalChange;
            double top = Canvas.GetTop(this) + e.VerticalChange;

            if (left < 0) left = 0;
            if (top < 0) top = 0;

            if (left + this.Width > this.ParentMaxWidth) left = this.ParentMaxWidth - this.Width;
            if (top + this.Height > this.ParentMaxHeight) top = this.ParentMaxHeight - this.Height;

            Canvas.SetLeft(this, left);
            Canvas.SetTop(this, top);
            if (UpdateImageEvent != null)
            {
                UpdateImageEvent();
            }
        }
        #endregion

        #region 右下点拖动
        private void OnRightBottomDragDeltaHandler(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Width + e.HorizontalChange > 0) this.Width += e.HorizontalChange;
            double left = Canvas.GetLeft(this);
            if (left + this.Width > this.ParentMaxWidth)
            {
                this.Width = this.ParentMaxWidth - left;
            }

            if (this.Height + e.VerticalChange > 0) this.Height += e.VerticalChange;
            double top = Canvas.GetTop(this);
            if (top + this.Height > this.ParentMaxHeight)
            {
                this.Height = this.ParentMaxHeight - top;
            }
            if (UpdateImageEvent != null)
            {
                UpdateImageEvent();
            }
        }
        #endregion
    }
}
