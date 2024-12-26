using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = BorderHeaderTemplateName, Type = typeof(Border))]
    [TemplatePart(Name = BorderMarkTemplateName, Type = typeof(Border))]
    public class Drawer : HeaderedContentControl
    {
        private const string BorderHeaderTemplateName = "PART_Header";
        private const string BorderMarkTemplateName = "PART_Mark";

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(Position), typeof(Drawer),
                new PropertyMetadata(Position.Left));

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(Drawer),
                new PropertyMetadata(false, OnIsOpenChanged));

        private Storyboard _enterStoryboard;
        private Storyboard _exitStoryboard;

        private Border _headerBorder;
        private double _headerHeight;
        private double _headerWidth;
        private Border _markBorder;

        static Drawer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Drawer), new FrameworkPropertyMetadata(typeof(Drawer)));
        }

        public Position Position
        {
            get => (Position) GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public bool IsOpen
        {
            get => (bool) GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as Drawer;
            if (ctrl != null)
            {
                if (ctrl.IsOpen)
                {
                    ctrl._headerBorder.Visibility = Visibility.Visible;
                    ctrl._enterStoryboard.Begin();
                }
                else
                {
                    ctrl._exitStoryboard.Begin();
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _headerBorder = GetTemplateChild(BorderHeaderTemplateName) as Border;
            if (_headerBorder != null)
                _headerBorder.Loaded += HeaderBorder_Loaded;
            _markBorder = GetTemplateChild(BorderMarkTemplateName) as Border;
            if (_markBorder != null)
                _markBorder.MouseDown += MarkBorder_MouseDown;
        }

        private void MarkBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsOpen = false;
        }

        private void HeaderBorder_Loaded(object sender, RoutedEventArgs e)
        {
            TranslateTransform translateTransform;
            DoubleAnimation animation, exitAnimation;
            switch (Position)
            {
                case Position.Left:
                case Position.Right:
                    _headerWidth = _headerBorder.ActualWidth;
                    if (Position == Position.Left)
                        translateTransform = new TranslateTransform(-_headerWidth, 0);
                    else
                        translateTransform = new TranslateTransform(_headerWidth, 0);
                    _headerBorder.RenderTransform = new TransformGroup
                    {
                        Children = new TransformCollection {translateTransform}
                    };
                    animation = new DoubleAnimation
                    {
                        From = Position == Position.Left ? -_headerWidth : _headerWidth,
                        To = 0,
                        Duration = TimeSpan.FromMilliseconds(300)
                    };

                    Storyboard.SetTarget(animation, _headerBorder);
                    Storyboard.SetTargetProperty(animation, new PropertyPath("RenderTransform.Children[0].X"));
                    _enterStoryboard = new Storyboard();
                    _enterStoryboard.Children.Add(animation);

                    exitAnimation = new DoubleAnimation
                    {
                        From = 0,
                        To = Position == Position.Left ? -_headerWidth : _headerWidth,
                        Duration = TimeSpan.FromMilliseconds(300)
                    };

                    Storyboard.SetTarget(exitAnimation, _headerBorder);
                    Storyboard.SetTargetProperty(exitAnimation, new PropertyPath("RenderTransform.Children[0].X"));
                    _exitStoryboard = new Storyboard();
                    _exitStoryboard.Completed += delegate
                    {
                        if (!IsOpen)
                            _headerBorder.Visibility = Visibility.Collapsed;
                    };
                    _exitStoryboard.Children.Add(exitAnimation);
                    break;
                case Position.Top:
                case Position.Bottom:
                    _headerHeight = _headerBorder.ActualHeight;
                    if (Position == Position.Top)
                        translateTransform = new TranslateTransform(0, -_headerHeight);
                    else
                        translateTransform = new TranslateTransform(0, _headerHeight);
                    _headerBorder.RenderTransform = new TransformGroup
                    {
                        Children = new TransformCollection {translateTransform}
                    };
                    animation = new DoubleAnimation
                    {
                        From = Position == Position.Top ? -_headerHeight : _headerHeight,
                        To = 0,
                        Duration = TimeSpan.FromMilliseconds(300)
                    };

                    Storyboard.SetTarget(animation, _headerBorder);
                    Storyboard.SetTargetProperty(animation, new PropertyPath("RenderTransform.Children[0].Y"));
                    _enterStoryboard = new Storyboard();
                    _enterStoryboard.Children.Add(animation);

                    exitAnimation = new DoubleAnimation
                    {
                        From = 0,
                        To = Position == Position.Top ? -_headerHeight : _headerHeight,
                        Duration = TimeSpan.FromMilliseconds(300)
                    };

                    Storyboard.SetTarget(exitAnimation, _headerBorder);
                    Storyboard.SetTargetProperty(exitAnimation, new PropertyPath("RenderTransform.Children[0].Y"));
                    _exitStoryboard = new Storyboard();
                    _exitStoryboard.Completed += delegate
                    {
                        if (!IsOpen)
                            _headerBorder.Visibility = Visibility.Collapsed;
                    };
                    _exitStoryboard.Children.Add(exitAnimation);
                    break;
            }

            _headerBorder.Visibility = Visibility.Collapsed;
            _headerBorder.Loaded -= HeaderBorder_Loaded;
        }
    }
}