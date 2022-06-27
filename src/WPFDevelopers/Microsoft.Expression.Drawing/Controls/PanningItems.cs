using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace Microsoft.Expression.Drawing.Controls
{
    public class PanningItems : Selector
    {
        static PanningItems()
        {
            SelectedItemProperty.OverrideMetadata(typeof(PanningItems), new FrameworkPropertyMetadata(new PropertyChangedCallback(selectedItemChanged)));
            FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(Grid));
            frameworkElementFactory.SetValue(ClipToBoundsProperty, true);
            FrameworkElementFactory frameworkElementFactory2 = new FrameworkElementFactory(typeof(ContentPresenter));
            Binding binding = new Binding();
            binding.RelativeSource = RelativeSource.TemplatedParent;
            binding.Path = new PropertyPath(PreviousItemProperty);
            frameworkElementFactory2.SetBinding(ContentPresenter.ContentProperty, binding);
            Binding binding2 = new Binding();
            binding2.RelativeSource = RelativeSource.TemplatedParent;
            binding2.Path = new PropertyPath(ItemTemplateProperty);
            frameworkElementFactory2.SetBinding(ContentPresenter.ContentTemplateProperty, binding2);
            frameworkElementFactory2.Name = "Previous";
            frameworkElementFactory.AppendChild(frameworkElementFactory2);
            FrameworkElementFactory frameworkElementFactory3 = new FrameworkElementFactory(typeof(ContentPresenter));
            Binding binding3 = new Binding();
            binding3.RelativeSource = RelativeSource.TemplatedParent;
            binding3.Path = new PropertyPath(SelectedItemProperty);
            frameworkElementFactory3.SetBinding(ContentPresenter.ContentProperty, binding3);
            Binding binding4 = new Binding();
            binding4.RelativeSource = RelativeSource.TemplatedParent;
            binding4.Path = new PropertyPath(ItemTemplateProperty);
            frameworkElementFactory3.SetBinding(ContentPresenter.ContentTemplateProperty, binding4);
            frameworkElementFactory3.Name = "Current";
            frameworkElementFactory.AppendChild(frameworkElementFactory3);
            FrameworkElementFactory frameworkElementFactory4 = new FrameworkElementFactory(typeof(ContentPresenter));
            Binding binding5 = new Binding();
            binding5.RelativeSource = RelativeSource.TemplatedParent;
            binding5.Path = new PropertyPath(NextItemProperty);
            frameworkElementFactory4.SetBinding(ContentPresenter.ContentProperty, binding5);
            Binding binding6 = new Binding();
            binding6.RelativeSource = RelativeSource.TemplatedParent;
            binding6.Path = new PropertyPath(ItemTemplateProperty);
            frameworkElementFactory4.SetBinding(ContentPresenter.ContentTemplateProperty, binding6);
            frameworkElementFactory4.Name = "Next";
            frameworkElementFactory.AppendChild(frameworkElementFactory4);
            ControlTemplate controlTemplate = new ControlTemplate(typeof(PanningItems));
            controlTemplate.VisualTree = frameworkElementFactory;
            Style style = new Style(typeof(PanningItems));
            Setter item = new Setter(TemplateProperty, controlTemplate);
            style.Setters.Add(item);
            style.Seal();
            StyleProperty.OverrideMetadata(typeof(PanningItems), new FrameworkPropertyMetadata(style));
        }

        public PanningItems()
        {
            SelectedIndex = 0;
        }

        public System.Windows.Controls.Orientation ScrollDirection
        {
            get
            {
                return (System.Windows.Controls.Orientation)GetValue(ScrollDirectionProperty);
            }
            set
            {
                SetValue(ScrollDirectionProperty, value);
            }
        }

        public double FlickTolerance
        {
            get
            {
                return (double)GetValue(FlickToleranceProperty);
            }
            set
            {
                SetValue(FlickToleranceProperty, value);
            }
        }

        public object PreviousItem
        {
            get
            {
                return GetValue(PreviousItemProperty);
            }
            set
            {
                SetValue(PreviousItemProperty, value);
            }
        }

        public object NextItem
        {
            get
            {
                return GetValue(NextItemProperty);
            }
            set
            {
                SetValue(NextItemProperty, value);
            }
        }

        public bool LoopContents
        {
            get
            {
                return (bool)GetValue(LoopContentsProperty);
            }
            set
            {
                SetValue(LoopContentsProperty, value);
            }
        }

        public double SliderValue
        {
            get
            {
                return (double)GetValue(SliderValueProperty);
            }
            set
            {
                SetValue(SliderValueProperty, value);
            }
        }



        

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            previousTransform = new TranslateTransform();
            previous = (ContentPresenter)Template.FindName("Previous", this);
            if (previous != null)
            {
                previous.Opacity = 0.0;
                previous.RenderTransform = previousTransform;
            }
            currentTransform = new TranslateTransform();
            current = (ContentPresenter)Template.FindName("Current", this);
            if (current != null)
            {
                current.RenderTransform = currentTransform;
            }
            nextTransform = new TranslateTransform();
            next = (ContentPresenter)Template.FindName("Next", this);
            if (next != null)
            {
                next.Opacity = 0.0;
                next.RenderTransform = nextTransform;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            CaptureMouse();
            OnGestureDown(e.GetPosition(this));
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            OnGestureUp();
            ReleaseMouseCapture();
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            OnGestureMove(e.GetPosition(this));
            base.OnMouseMove(e);
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            OnGestureUp();
            base.OnLostMouseCapture(e);
        }

        protected override void OnTouchDown(TouchEventArgs e)
        {
            CaptureTouch(e.TouchDevice);
            OnGestureDown(e.GetTouchPoint(this).Position);
            base.OnTouchDown(e);
        }

        protected override void OnTouchUp(TouchEventArgs e)
        {
            OnGestureUp();
            ReleaseTouchCapture(e.TouchDevice);
            base.OnTouchUp(e);
        }

        protected override void OnTouchMove(TouchEventArgs e)
        {
            OnGestureMove(e.GetTouchPoint(this).Position);
            base.OnTouchMove(e);
        }

        protected override void OnLostTouchCapture(TouchEventArgs e)
        {
            OnGestureUp();
            base.OnLostTouchCapture(e);
        }

        private static void flickToleranceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private static object coerceFlickTolerance(DependencyObject sender, object value)
        {
            double val = (double)value;
            return Math.Max(Math.Min(1.0, val), 0.0);
        }

        private static void sliderValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PanningItems panningItems = (PanningItems)sender;
            if (panningItems.previous != null && panningItems.current != null && panningItems.next != null)
            {
                panningItems.previous.Opacity = 1.0;
                panningItems.next.Opacity = 1.0;
                if (panningItems.ScrollDirection == System.Windows.Controls.Orientation.Horizontal)
                {
                    panningItems.previousTransform.X = panningItems.current.ActualWidth * (panningItems.SliderValue - 1.0);
                    panningItems.currentTransform.X = panningItems.current.ActualWidth * panningItems.SliderValue;
                    panningItems.nextTransform.X = panningItems.current.ActualWidth * (panningItems.SliderValue + 1.0);
                    panningItems.previousTransform.Y = 0.0;
                    panningItems.currentTransform.Y = 0.0;
                    panningItems.nextTransform.Y = 0.0;
                    return;
                }
                panningItems.previousTransform.X = 0.0;
                panningItems.currentTransform.X = 0.0;
                panningItems.nextTransform.X = 0.0;
                panningItems.previousTransform.Y = panningItems.current.ActualHeight * (panningItems.SliderValue - 1.0);
                panningItems.currentTransform.Y = panningItems.current.ActualHeight * panningItems.SliderValue;
                panningItems.nextTransform.Y = panningItems.current.ActualHeight * (panningItems.SliderValue + 1.0);
            }
        }

        private static void selectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PanningItems panningItems = (PanningItems)sender;
            int selectedIndex = panningItems.SelectedIndex;
            if (selectedIndex == -1 || panningItems.Items.Count == 0)
            {
                panningItems.PreviousItem = null;
                panningItems.NextItem = null;
                return;
            }
            if (selectedIndex == 0)
            {
                if (panningItems.LoopContents)
                {
                    panningItems.PreviousItem = panningItems.Items[panningItems.Items.Count - 1];
                }
                else
                {
                    panningItems.PreviousItem = null;
                }
            }
            else
            {
                panningItems.PreviousItem = panningItems.Items[selectedIndex - 1];
            }
            if (selectedIndex != panningItems.Items.Count - 1)
            {
                panningItems.NextItem = panningItems.Items[selectedIndex + 1];
                return;
            }
            if (panningItems.LoopContents)
            {
                panningItems.NextItem = panningItems.Items[0];
                return;
            }
            panningItems.NextItem = null;
        }

        private void OnGestureDown(Point point)
        {
            touchDown = point;
            isDragging = true;
        }

        private void OnGestureUp()
        {
            if (isDragging)
            {
                AnimateSliderValueTo(0.0);
            }
            isDragging = false;
        }

        private void OnGestureMove(Point point)
        {
            if (isDragging)
            {
                Vector vector = point - touchDown;
                if (ScrollDirection == System.Windows.Controls.Orientation.Horizontal)
                {
                    SliderValue = vector.X / current.ActualWidth;
                }
                else
                {
                    SliderValue = vector.Y / current.ActualHeight;
                }
                if (Math.Abs(SliderValue) >= FlickTolerance)
                {
                    isDragging = false;
                    int num = SelectedIndex;
                    if (num != -1)
                    {
                        if (SliderValue > 0.0)
                        {
                            num--;
                            SliderValue -= 1.0;
                        }
                        else
                        {
                            num++;
                            SliderValue += 1.0;
                        }
                        num += Items.Count;
                        num %= Items.Count;
                        SelectedIndex = num;
                    }
                    AnimateSliderValueTo(0.0);
                }
            }
        }

        private void AnimateSliderValueTo(double target)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation(target, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimation.FillBehavior = System.Windows.Media.Animation.FillBehavior.Stop;
            doubleAnimation.Completed += delegate (object o, EventArgs e)
            {
                SliderValue = 0.0;
            };
            BeginAnimation(SliderValueProperty, doubleAnimation);
        }

        public static readonly DependencyProperty ScrollDirectionProperty = DependencyProperty.Register("ScrollDirection", typeof(Orientation), typeof(PanningItems), new PropertyMetadata(System.Windows.Controls.Orientation.Horizontal));

        public static readonly DependencyProperty FlickToleranceProperty = DependencyProperty.Register("FlickTolerance", typeof(double), typeof(PanningItems), new PropertyMetadata(0.25, new PropertyChangedCallback(flickToleranceChanged), new CoerceValueCallback(coerceFlickTolerance)));

        public static readonly DependencyProperty PreviousItemProperty = DependencyProperty.Register("PreviousItem", typeof(object), typeof(PanningItems), new PropertyMetadata(null));

        public static readonly DependencyProperty NextItemProperty = DependencyProperty.Register("NextItem", typeof(object), typeof(PanningItems), new PropertyMetadata(null));

        public static readonly DependencyProperty LoopContentsProperty = DependencyProperty.Register("LoopContents", typeof(bool), typeof(PanningItems), new PropertyMetadata(true));

        public static readonly DependencyProperty SliderValueProperty = DependencyProperty.Register("SliderValue", typeof(double), typeof(PanningItems), new PropertyMetadata(0.0, new PropertyChangedCallback(sliderValueChanged)));

        private Point touchDown;

        private bool isDragging;

        private TranslateTransform previousTransform;

        private TranslateTransform currentTransform;

        private TranslateTransform nextTransform;

        private ContentPresenter previous;

        private ContentPresenter current;

        private ContentPresenter next;
    }
}
