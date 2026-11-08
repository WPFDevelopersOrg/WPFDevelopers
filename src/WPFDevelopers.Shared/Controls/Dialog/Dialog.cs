using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public class Dialog : ContentControl
    {
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(Dialog),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsOpenChanged));

        public static readonly DependencyProperty ClosedCommandProperty =
            DependencyProperty.Register("ClosedCommand", typeof(ICommand), typeof(Dialog), new PropertyMetadata(null));

        public static readonly DependencyProperty CornerRadiusProperty =
            Border.CornerRadiusProperty.AddOwner(typeof(Dialog),
                new FrameworkPropertyMetadata(new CornerRadius(10), (d, e) => ((Dialog)d).UpdateCornerClip()));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(Dialog), new PropertyMetadata("Dialog"));

        public static readonly DependencyProperty OwnerProperty =
            DependencyProperty.Register("Owner", typeof(System.Windows.Window), typeof(Dialog), new PropertyMetadata(null));

        public static readonly DependencyProperty DialogWidthProperty =
            DependencyProperty.Register("DialogWidth", typeof(double), typeof(Dialog),
                new PropertyMetadata(440d), IsValidDimension);

        public static readonly DependencyProperty DialogHeightProperty =
            DependencyProperty.Register("DialogHeight", typeof(double), typeof(Dialog),
                new PropertyMetadata(double.NaN), IsValidDimension);

        public static readonly DependencyProperty ShowFooterProperty =
            DependencyProperty.Register("ShowFooter", typeof(bool), typeof(Dialog), new PropertyMetadata(true));

        public static readonly DependencyProperty ConfirmTextProperty =
            DependencyProperty.Register("ConfirmText", typeof(string), typeof(Dialog), new PropertyMetadata(null));

        public static readonly DependencyProperty CancelTextProperty =
            DependencyProperty.Register("CancelText", typeof(string), typeof(Dialog), new PropertyMetadata(null));

        public static readonly DependencyProperty IsConfirmEnabledProperty =
            DependencyProperty.Register("IsConfirmEnabled", typeof(bool), typeof(Dialog), new PropertyMetadata(true));

        private static readonly DependencyPropertyKey DisplayContentPropertyKey =
            DependencyProperty.RegisterReadOnly("DisplayContent", typeof(object), typeof(Dialog), new PropertyMetadata(null));

        public static readonly DependencyProperty DisplayContentProperty = DisplayContentPropertyKey.DependencyProperty;

        public static readonly RoutedUICommand OpenCommand = new RoutedUICommand("Open", "Open", typeof(Dialog));
        public static readonly RoutedUICommand ConfirmCommand = new RoutedUICommand("Confirm", "Confirm", typeof(Dialog));
        public static readonly RoutedUICommand CancelCommand = new RoutedUICommand("Cancel", "Cancel", typeof(Dialog));
        public static readonly RoutedUICommand CloseCommand = new RoutedUICommand("Close", "Close", typeof(Dialog));

        public event EventHandler<DialogClosingEventArgs> Closing;
        public event EventHandler Closed;

        private static readonly Dictionary<Window, Dialog> OpenDialogs = new Dictionary<Window, Dialog>();
        private DialogAdorner _adorner;
        private AdornerLayer _layer;
        private Window _owner;
        private UIElement _background;
        private bool _backgroundEnabled;
        private IInputElement _previousFocus;
        private Frame _frame;
        private TaskCompletionSource<DialogResult> _completion;
        private bool _closing;

        static Dialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Dialog), new FrameworkPropertyMetadata(typeof(Dialog)));
            ContentProperty.OverrideMetadata(typeof(Dialog), new FrameworkPropertyMetadata(null, null, CoerceContent));
        }

        public Dialog()
        {
            CommandBindings.Add(new CommandBinding(OpenCommand,
                (sender, e) => SetCurrentValue(IsOpenProperty, true),
                (sender, e) => e.CanExecute = _adorner == null));
            CommandBindings.Add(new CommandBinding(ConfirmCommand,
                (sender, e) => Close(DialogCloseReason.Confirm),
                (sender, e) => e.CanExecute = _adorner != null && IsConfirmEnabled));
            CommandBindings.Add(new CommandBinding(CancelCommand,
                (sender, e) => Close(DialogCloseReason.Cancel),
                (sender, e) => e.CanExecute = _adorner != null));
            CommandBindings.Add(new CommandBinding(CloseCommand,
                (sender, e) => Close(DialogCloseReason.CloseButton),
                (sender, e) => e.CanExecute = _adorner != null));
            KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Cycle);
            KeyboardNavigation.SetControlTabNavigation(this, KeyboardNavigationMode.Cycle);
            FocusManager.SetIsFocusScope(this, true);
        }

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        public ICommand ClosedCommand
        {
            get => (ICommand)GetValue(ClosedCommandProperty);
            set => SetValue(ClosedCommandProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public Window Owner
        {
            get => (Window)GetValue(OwnerProperty);
            set => SetValue(OwnerProperty, value);
        }

        public double DialogWidth
        {
            get => (double)GetValue(DialogWidthProperty);
            set => SetValue(DialogWidthProperty, value);
        }

        public double DialogHeight
        {
            get => (double)GetValue(DialogHeightProperty);
            set => SetValue(DialogHeightProperty, value);
        }

        public bool ShowFooter
        {
            get => (bool)GetValue(ShowFooterProperty);
            set => SetValue(ShowFooterProperty, value);
        }

        public string ConfirmText
        {
            get => (string)GetValue(ConfirmTextProperty);
            set => SetValue(ConfirmTextProperty, value);
        }

        public string CancelText
        {
            get => (string)GetValue(CancelTextProperty);
            set => SetValue(CancelTextProperty, value);
        }

        public bool IsConfirmEnabled
        {
            get => (bool)GetValue(IsConfirmEnabledProperty);
            set => SetValue(IsConfirmEnabledProperty, value);
        }

        public object DisplayContent => GetValue(DisplayContentProperty);

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dialog = (Dialog)d;
            if ((bool)e.NewValue)
            {
                if (dialog._adorner == null)
                {
                    try { dialog.ShowAsync(); }
                    catch
                    {
                        dialog.SetCurrentValue(IsOpenProperty, false);
                        throw;
                    }
                }
            }
            else if (dialog._adorner != null)
            {
                try { dialog.Close(); }
                finally
                {
                    if (dialog._adorner != null)
                    {
                        dialog.SetCurrentValue(IsOpenProperty, true);
                        var adorner = dialog._adorner;
                        dialog.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() =>
                        {
                            if (dialog._adorner == adorner)
                            {
                                dialog.SetCurrentValue(IsOpenProperty, true);
                                BindingOperations.GetBindingExpression(dialog, IsOpenProperty)?.UpdateSource();
                            }
                        }));
                    }
                }
            }
        }

        private static bool IsValidDimension(object value)
        {
            var dimension = (double)value;
            return double.IsNaN(dimension) || (dimension > 0 && !double.IsInfinity(dimension));
        }

        private static object CoerceContent(DependencyObject d, object value)
        {
            var host = (Dialog)d;
            if (host._adorner != null && !ReferenceEquals(host.Content, value))
                throw new InvalidOperationException("Dialog.Content cannot be changed while the dialog is open.");
            if (value != null && !(value is UserControl) && !(value is Page))
                throw new ArgumentException("Dialog.Content must be a UserControl or Page.");
            if (value != null)
            {
                var element = (FrameworkElement)value;
                var parent = LogicalTreeHelper.GetParent(element);
                if ((parent != null && parent != host) || VisualTreeHelper.GetParent(element) != null)
                    throw new InvalidOperationException("Dialog.Content is already attached to another container.");
            }
            return value;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent is Page ? null : oldContent, newContent is Page ? null : newContent);
        }

        public Task<DialogResult> ShowAsync(Window owner = null)
        {
            VerifyAccess();
            if (_adorner != null)
                throw new InvalidOperationException("This Dialog is already open.");
            if (Content == null)
                throw new InvalidOperationException("Dialog.Content is required.");
            if (Parent != null || VisualTreeHelper.GetParent(this) != null)
                throw new InvalidOperationException("Dialog must not be attached to another container. Declare it in Resources or create it in code.");

            var content = (FrameworkElement)Content;
            var parent = LogicalTreeHelper.GetParent(content);
            if ((parent != null && parent != this) || VisualTreeHelper.GetParent(content) != null)
                throw new InvalidOperationException("Dialog.Content is already attached to another container.");

            owner = owner ?? Owner ?? ControlsHelper.GetDefaultWindow();
            if (owner == null || !owner.IsLoaded || !owner.IsVisible)
                throw new InvalidOperationException("Dialog requires a visible, loaded owner Window.");
            owner.ApplyTemplate();
            var decorator = FindDecorator(owner);
            if (decorator == null || decorator.Child == null)
                throw new InvalidOperationException("The owner Window requires an AdornerDecorator around its client content.");
            if (OpenDialogs.ContainsKey(owner))
                throw new InvalidOperationException("The owner Window already has an open Dialog.");

            _owner = owner;
            _layer = decorator.AdornerLayer;
            _background = decorator.Child;
            _backgroundEnabled = _background.IsEnabled;
            _previousFocus = Keyboard.FocusedElement;
            _completion = new TaskCompletionSource<DialogResult>();
            var task = _completion.Task;
            _adorner = new DialogAdorner(_background, this);
            OpenDialogs.Add(owner, this);
            try
            {
                if (content is Page)
                {
                    _frame = new Frame
                    {
                        NavigationUIVisibility = NavigationUIVisibility.Hidden,
                        JournalOwnership = JournalOwnership.OwnsJournal
                    };
                    _frame.Content = content;
                }
                SetValue(DisplayContentPropertyKey, (object)_frame ?? content);
                _background.IsEnabledChanged += OnBackgroundIsEnabledChanged;
                if (_background is FrameworkElement backgroundElement)
                    backgroundElement.Unloaded += OnBackgroundUnloaded;
                _background.SetCurrentValue(IsEnabledProperty, false);
                _owner.Closed += OnOwnerClosed;
                _owner.PreviewGotKeyboardFocus += OnOwnerPreviewGotKeyboardFocus;
                _owner.AddHandler(CommandManager.PreviewExecutedEvent, new ExecutedRoutedEventHandler(OnOwnerPreviewExecuted), true);
                _owner.AddHandler(Keyboard.KeyDownEvent, new KeyEventHandler(OnOwnerKeyDown), true);
                Loaded += OnDialogLoaded;
                _layer.Add(_adorner);
                SetCurrentValue(IsOpenProperty, true);
                CommandManager.InvalidateRequerySuggested();
                return task;
            }
            catch
            {
                Cleanup();
                throw;
            }
        }

        public void Close(DialogCloseReason reason = DialogCloseReason.Cancel)
        {
            VerifyAccess();
            if (!Enum.IsDefined(typeof(DialogCloseReason), reason))
                throw new ArgumentOutOfRangeException(nameof(reason));
            if (_adorner == null)
                throw new InvalidOperationException("This Dialog is not open.");
            if (reason == DialogCloseReason.Confirm && !IsConfirmEnabled)
                throw new InvalidOperationException("Confirmation is disabled.");
            if (_closing)
                throw new InvalidOperationException("A dialog close operation is already in progress.");
            _closing = true;
            bool canClose;
            try
            {
                canClose = CanClose(reason);
            }
            finally { _closing = false; }
            if (canClose && _adorner != null)
                Complete(reason == DialogCloseReason.Confirm ? DialogResult.Confirmed : DialogResult.Canceled);
        }

        internal bool CanClose(DialogCloseReason reason)
        {
            var valid = reason != DialogCloseReason.Confirm || UpdateSources(this);
            var args = new DialogClosingEventArgs(reason) { Cancel = !valid };
            Closing?.Invoke(this, args);
            return valid && !args.Cancel;
        }

        private static AdornerDecorator FindDecorator(DependencyObject element)
        {
            var decorator = element as AdornerDecorator;
            if (decorator != null)
                return decorator;
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                decorator = FindDecorator(VisualTreeHelper.GetChild(element, i));
                if (decorator != null)
                    return decorator;
            }
            return null;
        }

        private void OnDialogLoaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
            {
                if (_adorner != null)
                    FocusDialog();
            }));
        }

        private void FocusDialog()
        {
            MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            if (!IsKeyboardFocusWithin)
                Keyboard.Focus(this);
        }

        private void OnOwnerPreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.NewFocus is DependencyObject target && !IsDialogElement(target))
            {
                e.Handled = true;
                Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                {
                    if (_adorner != null && !IsKeyboardFocusWithin)
                        FocusDialog();
                }));
            }
        }

        private bool IsDialogElement(DependencyObject element)
        {
            var pending = new Stack<DependencyObject>();
            var visited = new HashSet<DependencyObject>();
            pending.Push(element);
            while (pending.Count > 0)
            {
                element = pending.Pop();
                if (element == null || !visited.Add(element))
                    continue;
                if (element == this)
                    return true;
                pending.Push(LogicalTreeHelper.GetParent(element));
                if (element is Visual || element is System.Windows.Media.Media3D.Visual3D)
                    pending.Push(VisualTreeHelper.GetParent(element));
                if (element is FrameworkElement frameworkElement)
                    pending.Push(frameworkElement.TemplatedParent);
                if (element is Popup popup)
                    pending.Push(popup.PlacementTarget);
                if (element is ContextMenu menu)
                    pending.Push(menu.PlacementTarget);
            }
            return false;
        }

        private void OnBackgroundIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                _background.SetCurrentValue(IsEnabledProperty, false);
        }

        private void OnBackgroundUnloaded(object sender, RoutedEventArgs e)
        {
            if (_adorner != null)
                Complete(DialogResult.Canceled);
        }

        private void OnOwnerPreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(e.OriginalSource is DependencyObject source) || !IsDialogElement(source))
                e.Handled = true;
        }

        private void OnOwnerKeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Handled && e.Key == Key.Escape)
            {
                Close(DialogCloseReason.Escape);
                e.Handled = true;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Handled || _owner == null)
                return;
            foreach (InputBinding binding in _owner.InputBindings)
            {
                if (binding.Gesture != null && binding.Gesture.Matches(_owner, e))
                {
                    e.Handled = true;
                    break;
                }
            }
        }

        private void OnOwnerClosed(object sender, EventArgs e)
        {
            Complete(DialogResult.Canceled);
        }

        private void Complete(DialogResult result)
        {
            var completion = _completion;
            Cleanup();
            try
            {
                Closed?.Invoke(this, EventArgs.Empty);
                var command = ClosedCommand;
                if (command != null && command.CanExecute(result))
                    command.Execute(result);
            }
            finally { completion.SetResult(result); }
        }

        private void Cleanup()
        {
            Loaded -= OnDialogLoaded;
            _owner.Closed -= OnOwnerClosed;
            _owner.PreviewGotKeyboardFocus -= OnOwnerPreviewGotKeyboardFocus;
            _owner.RemoveHandler(CommandManager.PreviewExecutedEvent, new ExecutedRoutedEventHandler(OnOwnerPreviewExecuted));
            _owner.RemoveHandler(Keyboard.KeyDownEvent, new KeyEventHandler(OnOwnerKeyDown));
            _background.IsEnabledChanged -= OnBackgroundIsEnabledChanged;
            if (_background is FrameworkElement backgroundElement)
                backgroundElement.Unloaded -= OnBackgroundUnloaded;
            _layer.Remove(_adorner);
            _adorner.Detach();
            SetValue(DisplayContentPropertyKey, null);
            var presenter = GetTemplateChild("PART_ContentPresenter") as ContentPresenter;
            if (presenter != null)
            {
                presenter.Content = null;
                presenter.ClearValue(ContentPresenter.ContentProperty);
            }
            if (_frame != null)
            {
                _frame.Content = null;
                var pagePresenter = VisualTreeHelper.GetParent((DependencyObject)Content) as ContentPresenter;
                if (pagePresenter != null)
                    pagePresenter.Content = null;
                _frame = null;
            }
            _background.SetCurrentValue(IsEnabledProperty, _backgroundEnabled);
            BindingOperations.GetBindingExpression(_background, IsEnabledProperty)?.UpdateTarget();
            OpenDialogs.Remove(_owner);
            _adorner = null;
            _completion = null;
            _layer = null;
            _background = null;
            var previousFocus = _previousFocus;
            _previousFocus = null;
            var owner = _owner;
            _owner = null;
            if (owner.IsVisible && previousFocus is UIElement previousElement && previousElement.IsVisible && previousElement.IsEnabled)
                Keyboard.Focus(previousFocus);
            SetCurrentValue(IsOpenProperty, false);
            CommandManager.InvalidateRequerySuggested();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == DialogWidthProperty || e.Property == DialogHeightProperty)
                _adorner?.InvalidateMeasure();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateCornerClip();
        }

        private void UpdateCornerClip()
        {
            var width = ActualWidth;
            var height = ActualHeight;
            if (width <= 0 || height <= 0)
                return;
            var radius = CornerRadius;
            var scale = 1.0;
            scale = Math.Min(scale, width / Math.Max(width, radius.TopLeft + radius.TopRight));
            scale = Math.Min(scale, width / Math.Max(width, radius.BottomLeft + radius.BottomRight));
            scale = Math.Min(scale, height / Math.Max(height, radius.TopLeft + radius.BottomLeft));
            scale = Math.Min(scale, height / Math.Max(height, radius.TopRight + radius.BottomRight));
            var topLeft = radius.TopLeft * scale;
            var topRight = radius.TopRight * scale;
            var bottomRight = radius.BottomRight * scale;
            var bottomLeft = radius.BottomLeft * scale;
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(new Point(topLeft, 0), true, true);
                context.LineTo(new Point(width - topRight, 0), true, false);
                AddCorner(context, new Point(width, topRight), topRight);
                context.LineTo(new Point(width, height - bottomRight), true, false);
                AddCorner(context, new Point(width - bottomRight, height), bottomRight);
                context.LineTo(new Point(bottomLeft, height), true, false);
                AddCorner(context, new Point(0, height - bottomLeft), bottomLeft);
                context.LineTo(new Point(0, topLeft), true, false);
                AddCorner(context, new Point(topLeft, 0), topLeft);
            }
            geometry.Freeze();
            Clip = geometry;
        }

        private static void AddCorner(StreamGeometryContext context, Point point, double radius)
        {
            if (radius == 0)
                context.LineTo(point, true, false);
            else
                context.ArcTo(point, new Size(radius, radius), 0, false, SweepDirection.Clockwise, true, false);
        }

        private static bool UpdateSources(DependencyObject element)
        {
            var valid = true;
            var properties = new HashSet<DependencyProperty>();
            var values = element.GetLocalValueEnumerator();
            while (values.MoveNext())
                properties.Add(values.Current.Property);
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(element))
            {
                var descriptor = DependencyPropertyDescriptor.FromProperty(property);
                if (descriptor != null)
                    properties.Add(descriptor.DependencyProperty);
            }
            foreach (var property in properties)
            {
                var expression = BindingOperations.GetBindingExpressionBase(element, property);
                var binding = expression as BindingExpression;
                binding?.UpdateSource();
                var multiBinding = expression as MultiBindingExpression;
                multiBinding?.UpdateSource();
                var priorityBinding = expression as PriorityBindingExpression;
                priorityBinding?.ActiveBindingExpression?.UpdateSource();
            }
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
                valid = UpdateSources(VisualTreeHelper.GetChild(element, i)) && valid;
            var frameworkElement = element as FrameworkElement;
            if (frameworkElement?.BindingGroup != null)
                valid = frameworkElement.BindingGroup.CommitEdit() && valid;
            return !Validation.GetHasError(element) && valid;
        }
    }
}
