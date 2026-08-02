using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = DocumentHostTemplateName, Type = typeof(RichTextBox))]
    public class MarkdownViewer : Control
    {
        private const string DocumentHostTemplateName = "PART_DocumentHost";

        public static readonly DependencyProperty MarkdownProperty =
            DependencyProperty.Register("Markdown", typeof(string), typeof(MarkdownViewer),
                new PropertyMetadata(string.Empty, OnMarkdownChanged));

        private RichTextBox _documentHost;
        private readonly DispatcherTimer _themeProbeTimer;
        private Color _lastPrimaryColor;
        private ThemeType _lastThemeType;

        static MarkdownViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MarkdownViewer),
                new FrameworkPropertyMetadata(typeof(MarkdownViewer)));
        }

        public MarkdownViewer()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            SizeChanged += OnSizeChanged;

            _themeProbeTimer = new DispatcherTimer(DispatcherPriority.Background);
            _themeProbeTimer.Interval = TimeSpan.FromMilliseconds(120);
            _themeProbeTimer.Tick += ThemeProbeTimer_Tick;
        }

        public string Markdown
        {
            get { return (string)GetValue(MarkdownProperty); }
            set { SetValue(MarkdownProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _documentHost = GetTemplateChild(DocumentHostTemplateName) as RichTextBox;
            EnsureDocumentHostConfigured();
            RenderMarkdown();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == FontSizeProperty ||
                e.Property == FontFamilyProperty ||
                e.Property == ForegroundProperty ||
                e.Property == PaddingProperty)
            {
                RenderMarkdown();
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            WPFDevelopers.Resources.ThemeChanged -= Resources_ThemeChanged;
            WPFDevelopers.Resources.ThemeChanged += Resources_ThemeChanged;

            SyncThemeSnapshot();
            if (!_themeProbeTimer.IsEnabled)
                _themeProbeTimer.Start();

            RenderMarkdown();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            WPFDevelopers.Resources.ThemeChanged -= Resources_ThemeChanged;

            if (_themeProbeTimer.IsEnabled)
                _themeProbeTimer.Stop();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Math.Abs(e.NewSize.Width - e.PreviousSize.Width) > 1)
                RenderMarkdown();
        }

        private static void OnMarkdownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MarkdownViewer;
            if (control != null)
                control.RenderMarkdown();
        }

        private void Resources_ThemeChanged(ThemeType currentTheme)
        {
            SyncThemeSnapshot();
            RenderMarkdown();
        }

        private void ThemeProbeTimer_Tick(object sender, EventArgs e)
        {
            var resources = ThemeManager.Instance.Resources;
            if (resources == null)
                return;

            var currentTheme = resources.Theme;
            var currentColor = ThemeManager.Instance.PrimaryColor;

            if (currentTheme != _lastThemeType || currentColor != _lastPrimaryColor)
            {
                _lastThemeType = currentTheme;
                _lastPrimaryColor = currentColor;
                RenderMarkdown();
            }
        }

        private void SyncThemeSnapshot()
        {
            var resources = ThemeManager.Instance.Resources;
            if (resources == null)
                return;

            _lastThemeType = resources.Theme;
            _lastPrimaryColor = ThemeManager.Instance.PrimaryColor;
        }

        private void EnsureDocumentHostConfigured()
        {
            if (_documentHost == null)
                return;

            _documentHost.IsReadOnly = true;
            _documentHost.IsDocumentEnabled = true;
            _documentHost.BorderThickness = new Thickness(0);
            _documentHost.Background = Brushes.Transparent;
            _documentHost.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            _documentHost.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            _documentHost.Focusable = false;
        }

        private void RenderMarkdown()
        {
            if (_documentHost == null)
                return;

            EnsureDocumentHostConfigured();

            var document = new FlowDocument();
            document.PagePadding = new Thickness(0);
            document.TextAlignment = TextAlignment.Left;
            document.FontFamily = FontFamily;
            document.FontSize = FontSize > 0 ? FontSize : 13;
            document.Foreground = ResolveBrush("WD.PrimaryTextBrush", Foreground);

            var blocks = ParseMarkdown(Markdown ?? string.Empty);
            foreach (var block in blocks)
                document.Blocks.Add(CreateBlock(block));

            if (document.Blocks.Count == 0)
                document.Blocks.Add(new Paragraph());

            _documentHost.Document = document;
        }

        private Block CreateBlock(MarkdownBlock block)
        {
            if (block is HeaderBlock)
                return CreateHeader((HeaderBlock)block);
            if (block is ParagraphBlock)
                return CreateParagraph((ParagraphBlock)block);
            if (block is HorizontalRuleBlock)
                return CreateHorizontalRule();
            if (block is UnorderedListBlock)
                return CreateUnorderedList((UnorderedListBlock)block);
            if (block is OrderedListBlock)
                return CreateOrderedList((OrderedListBlock)block);
            if (block is QuoteBlock)
                return CreateQuote((QuoteBlock)block);
            if (block is CodeBlock)
                return CreateCodeBlock((CodeBlock)block);
            if (block is ImageBlock)
                return CreateImageBlock((ImageBlock)block);
            if (block is TableBlock)
                return CreateTableBlock((TableBlock)block);

            return new Paragraph();
        }

        private Paragraph CreateHeader(HeaderBlock block)
        {
            var paragraph = new Paragraph();
            paragraph.Margin = new Thickness(0, 8, 0, 4);
            paragraph.FontWeight = FontWeights.SemiBold;

            if (block.Level == 1) paragraph.FontSize = documentFontScale(2.0);
            else if (block.Level == 2) paragraph.FontSize = documentFontScale(1.65);
            else if (block.Level == 3) paragraph.FontSize = documentFontScale(1.4);
            else if (block.Level == 4) paragraph.FontSize = documentFontScale(1.2);
            else paragraph.FontSize = documentFontScale(1.05);

            AddInlineTokens(paragraph.Inlines, ParseInlines(block.Text));
            return paragraph;
        }

        private Paragraph CreateParagraph(ParagraphBlock block)
        {
            var paragraph = new Paragraph();
            paragraph.Margin = new Thickness(0, 2, 0, 6);
            paragraph.LineHeight = documentFontScale(1.6);
            AddInlineTokens(paragraph.Inlines, ParseInlines(block.Text));
            return paragraph;
        }

        private BlockUIContainer CreateHorizontalRule()
        {
            var line = new Border();
            line.Height = 1;
            line.Margin = new Thickness(0, 8, 0, 8);
            line.Background = ResolveBrush("WD.BaseBrush", Brushes.LightGray);
            return new BlockUIContainer(line);
        }

        private List CreateUnorderedList(UnorderedListBlock block)
        {
            var list = new List();
            list.MarkerStyle = TextMarkerStyle.Disc;
            list.Margin = new Thickness(18, 2, 0, 6);
            foreach (var text in block.Items)
            {
                var paragraph = new Paragraph();
                paragraph.Margin = new Thickness(0, 0, 0, 2);
                AddInlineTokens(paragraph.Inlines, ParseInlines(text));
                list.ListItems.Add(new ListItem(paragraph));
            }
            return list;
        }

        private List CreateOrderedList(OrderedListBlock block)
        {
            var list = new List();
            list.MarkerStyle = TextMarkerStyle.Decimal;
            list.Margin = new Thickness(18, 2, 0, 6);
            foreach (var text in block.Items)
            {
                var paragraph = new Paragraph();
                paragraph.Margin = new Thickness(0, 0, 0, 2);
                AddInlineTokens(paragraph.Inlines, ParseInlines(text));
                list.ListItems.Add(new ListItem(paragraph));
            }
            return list;
        }

        private BlockUIContainer CreateQuote(QuoteBlock block)
        {
            var panel = new DockPanel();

            var quoteLine = new Border();
            quoteLine.Width = 4;
            quoteLine.Background = ResolveBrush("WD.PrimaryBrush", Brushes.SteelBlue);
            quoteLine.Margin = new Thickness(0, 0, 10, 0);
            DockPanel.SetDock(quoteLine, Dock.Left);

            var body = new TextBlock();
            body.TextWrapping = TextWrapping.Wrap;
            body.Foreground = ResolveBrush("WD.RegularTextBrush", Foreground);
            body.Margin = new Thickness(0);
            body.FontSize = documentFontScale(1);
            AddInlineTokens(body.Inlines, ParseInlines(block.Text));

            var container = new Border();
            container.Background = ResolveQuoteBackground();
            container.BorderBrush = ResolveBrush("WD.BaseBrush", Brushes.LightGray);
            container.BorderThickness = new Thickness(1);
            container.Padding = new Thickness(10, 8, 10, 8);
            container.Margin = new Thickness(0, 4, 0, 8);
            container.CornerRadius = ElementHelper.GetCornerRadius(this);

            panel.Children.Add(quoteLine);
            panel.Children.Add(body);
            container.Child = panel;

            return new BlockUIContainer(container);
        }

        private BlockUIContainer CreateCodeBlock(CodeBlock block)
        {
            var card = new Border();
            card.BorderBrush = ResolveBrush("WD.BaseBrush", Brushes.LightGray);
            card.BorderThickness = new Thickness(1);
            card.Background = ResolveCodeBodyBackground();
            card.CornerRadius = ElementHelper.GetCornerRadius(this);
            card.Margin = new Thickness(0, 6, 0, 10);

            var root = new Grid();
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var header = new Border();
            header.Background = ResolveCodeHeaderBackground();
            header.Padding = new Thickness(10, 4, 10, 4);
            header.CornerRadius = new CornerRadius(card.CornerRadius.TopLeft, card.CornerRadius.TopRight, 0, 0);

            var headerDock = new DockPanel();
            var lang = string.IsNullOrWhiteSpace(block.Language) ? "text" : block.Language.Trim().ToLowerInvariant();
            var codeText = block.Code ?? string.Empty;

            var copyButton = new Button();
            copyButton.Content = "Copy";
            copyButton.Padding = new Thickness(8, 2, 8, 2);
            copyButton.Margin = new Thickness(0, 0, 8, 0);
            copyButton.FontSize = documentFontScale(0.75);
            copyButton.ToolTip = "Copy code";
            copyButton.Focusable = false;
            copyButton.IsTabStop = false;

            var copyButtonStyle = TryFindResource("WD.DefaultButton") as Style;
            if (copyButtonStyle != null)
                copyButton.Style = copyButtonStyle;
            copyButton.Click += (sender, args) =>
            {
                try
                {
                    Clipboard.SetText(codeText);
                    copyButton.Content = "Copied";
                    copyButton.IsEnabled = false;

                    var resetTimer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromMilliseconds(1200)
                    };
                    resetTimer.Tick += (s, e) =>
                    {
                        resetTimer.Stop();
                        copyButton.Content = "Copy";
                        copyButton.IsEnabled = true;
                    };
                    resetTimer.Start();
                }
                catch
                {
                    copyButton.Content = "Copy failed";
                    copyButton.IsEnabled = false;

                    var resetTimer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromMilliseconds(1200)
                    };
                    resetTimer.Tick += (s, e) =>
                    {
                        resetTimer.Stop();
                        copyButton.Content = "Copy";
                        copyButton.IsEnabled = true;
                    };
                    resetTimer.Start();
                }
            };

            var langBadge = new Border();
            langBadge.CornerRadius = ElementHelper.GetCornerRadius(this);
            langBadge.BorderBrush = ResolveBrush("WD.PrimaryBrush", Brushes.RoyalBlue);
            langBadge.BorderThickness = new Thickness(1);
            langBadge.Padding = new Thickness(8, 0, 8, 0);
            langBadge.VerticalAlignment = VerticalAlignment.Center;

            var langText = new TextBlock();
            langText.Text = GetCodeLanguageLabel(lang);
            langText.Foreground = ResolveBrush("WD.PrimaryBrush", Brushes.RoyalBlue);
            langText.FontSize = documentFontScale(0.75);
            langText.FontWeight = FontWeights.SemiBold;
            langBadge.Child = langText;

            var headerActions = new StackPanel();
            headerActions.Orientation = Orientation.Horizontal;
            headerActions.HorizontalAlignment = HorizontalAlignment.Right;
            headerActions.Children.Add(copyButton);
            headerActions.Children.Add(langBadge);
            headerDock.Children.Add(headerActions);
            header.Child = headerDock;
            Grid.SetRow(header, 0);

            var lines = (block.Code ?? string.Empty).Replace("\r", string.Empty).Split('\n');
            if (lines.Length > 0 && lines[lines.Length - 1] == string.Empty)
                lines = lines.Take(lines.Length - 1).ToArray();
            if (lines.Length == 0)
                lines = new[] { string.Empty };

            var contentGrid = new Grid();
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var numberBorder = new Border();
            numberBorder.Background = ResolveCodeGutterBackground();
            numberBorder.Padding = new Thickness(10, 8, 10, 8);
            numberBorder.CornerRadius = new CornerRadius(0, 0, 0, card.CornerRadius.BottomLeft);

            var lineNumbers = new TextBlock();
            lineNumbers.FontFamily = new FontFamily("Consolas");
            lineNumbers.Foreground = ResolveCodeLineNumberBrush();
            lineNumbers.FontSize = documentFontScale(0.85);
            lineNumbers.Text = string.Join("\n", Enumerable.Range(1, lines.Length).Select(i => i.ToString(CultureInfo.InvariantCulture)));
            numberBorder.Child = lineNumbers;
            Grid.SetColumn(numberBorder, 0);

            var codeBorder = new Border();
            codeBorder.Background = ResolveCodeBodyBackground();
            codeBorder.Padding = new Thickness(10, 8, 10, 8);
            codeBorder.CornerRadius = new CornerRadius(0, 0, card.CornerRadius.BottomRight, 0);
            codeBorder.Child = BuildHighlightedCodeBody(lines, lang);
            Grid.SetColumn(codeBorder, 1);

            contentGrid.Children.Add(numberBorder);
            contentGrid.Children.Add(codeBorder);

            var contentScroll = new ScrollViewer();
            contentScroll.Background = Brushes.Transparent;
            contentScroll.BorderThickness = new Thickness(0);
            contentScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            contentScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            contentScroll.Focusable = false;
            contentScroll.IsTabStop = false;
            contentScroll.PreviewMouseWheel += ForwardMouseWheelToParent;
            contentScroll.Content = contentGrid;
            Grid.SetRow(contentScroll, 1);

            root.Children.Add(header);
            root.Children.Add(contentScroll);
            card.Child = root;

            return new BlockUIContainer(card);
        }

        private Block CreateImageBlock(ImageBlock block)
        {
            try
            {
                var uri = BuildImageUri(block.Url);
                if (uri == null)
                    return CreateParagraph(new ParagraphBlock { Text = string.IsNullOrWhiteSpace(block.AltText) ? block.Url : block.AltText });

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = uri;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                var image = new Image();
                image.Source = bitmap;
                image.Stretch = Stretch.Uniform;
                image.MaxWidth = Math.Max(120, ActualWidth - 80);
                image.Margin = new Thickness(0, 4, 0, 2);

                var stack = new StackPanel();
                stack.Children.Add(image);
                if (!string.IsNullOrWhiteSpace(block.AltText))
                {
                    var caption = new TextBlock();
                    caption.Text = block.AltText;
                    caption.Margin = new Thickness(0, 4, 0, 0);
                    caption.Foreground = ResolveBrush("WD.RegularTextBrush", Foreground);
                    caption.FontSize = documentFontScale(0.85);
                    stack.Children.Add(caption);
                }

                var container = new Border();
                container.BorderBrush = ResolveBrush("WD.BaseBrush", Brushes.LightGray);
                container.BorderThickness = new Thickness(1);
                container.CornerRadius = ElementHelper.GetCornerRadius(this);
                container.Padding = new Thickness(6);
                container.Margin = new Thickness(0, 4, 0, 8);
                container.Child = stack;

                return new BlockUIContainer(container);
            }
            catch
            {
                return CreateParagraph(new ParagraphBlock { Text = string.IsNullOrWhiteSpace(block.AltText) ? block.Url : block.AltText });
            }
        }

        private BlockUIContainer CreateTableBlock(TableBlock block)
        {
            var grid = new Grid();
            grid.SnapsToDevicePixels = true;
            grid.ShowGridLines = false;

            var rowCount = 1 + block.Rows.Count;
            var colCount = block.Headers.Count;
            if (colCount == 0)
                return new BlockUIContainer(new Border());

            for (var i = 0; i < colCount; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            for (var i = 0; i < rowCount; i++)
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            for (var col = 0; col < colCount; col++)
            {
                var header = CreateTableCell(block.Headers[col], true, TableAlignment.Left, 0, col, rowCount, colCount);
                Grid.SetRow(header, 0);
                Grid.SetColumn(header, col);
                grid.Children.Add(header);
            }

            for (var row = 0; row < block.Rows.Count; row++)
            {
                var rowCells = block.Rows[row];
                for (var col = 0; col < colCount; col++)
                {
                    var text = col < rowCells.Count ? rowCells[col] : string.Empty;
                    var align = col < block.Alignments.Count ? block.Alignments[col] : TableAlignment.Left;
                    var cell = CreateTableCell(text, false, align, row + 1, col, rowCount, colCount);
                    Grid.SetRow(cell, row + 1);
                    Grid.SetColumn(cell, col);
                    grid.Children.Add(cell);
                }
            }

            var border = new Border();
            border.Margin = new Thickness(0, 6, 0, 10);
            border.BorderBrush = ResolveBrush("WD.BaseBrush", Brushes.LightGray);
            border.BorderThickness = new Thickness(1);
            border.CornerRadius = ElementHelper.GetCornerRadius(this);

            var scroll = new ScrollViewer();
            scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            scroll.Focusable = false;
            scroll.IsTabStop = false;
            scroll.PreviewMouseWheel += ForwardMouseWheelToParent;
            scroll.Content = grid;

            border.Child = scroll;
            return new BlockUIContainer(border);
        }

        private Border CreateTableCell(string text, bool isHeader, TableAlignment align, int rowIndex, int colIndex, int rowCount, int colCount)
        {
            var border = new Border();
            border.BorderBrush = ResolveBrush("WD.BaseBrush", Brushes.LightGray);
            border.BorderThickness = new Thickness(0.5);
            border.Padding = new Thickness(8, 6, 8, 6);
            border.Background = isHeader ? ResolveCodeHeaderBackground() : ResolveBrush("WD.BackgroundBrush", Brushes.White);

            var corner = ElementHelper.GetCornerRadius(this);
            border.CornerRadius = new CornerRadius(
                rowIndex == 0 && colIndex == 0 ? corner.TopLeft : 0,
                rowIndex == 0 && colIndex == colCount - 1 ? corner.TopRight : 0,
                rowIndex == rowCount - 1 && colIndex == colCount - 1 ? corner.BottomRight : 0,
                rowIndex == rowCount - 1 && colIndex == 0 ? corner.BottomLeft : 0);

            var tb = new TextBlock();
            tb.TextWrapping = TextWrapping.Wrap;
            tb.FontSize = documentFontScale(0.9);
            tb.Foreground = ResolveBrush("WD.PrimaryTextBrush", Foreground);
            tb.FontWeight = isHeader ? FontWeights.SemiBold : FontWeights.Normal;
            if (align == TableAlignment.Center) tb.TextAlignment = TextAlignment.Center;
            else if (align == TableAlignment.Right) tb.TextAlignment = TextAlignment.Right;
            else tb.TextAlignment = TextAlignment.Left;
            AddInlineTokens(tb.Inlines, ParseInlines(text));
            border.Child = tb;
            return border;
        }

        private void ForwardMouseWheelToParent(object sender, MouseWheelEventArgs e)
        {
            if (e.Handled)
                return;

            var target = FindVerticalScrollAncestor(sender as DependencyObject);
            if (target == null)
                return;

            e.Handled = true;
            var forwardedArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = UIElement.MouseWheelEvent,
                Source = sender
            };
            target.RaiseEvent(forwardedArgs);
        }

        private static ScrollViewer FindVerticalScrollAncestor(DependencyObject start)
        {
            var current = GetParentObject(start);
            while (current != null)
            {
                var scrollViewer = current as ScrollViewer;
                if (scrollViewer != null && scrollViewer.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled)
                    return scrollViewer;

                current = GetParentObject(current);
            }

            return null;
        }

        private static DependencyObject GetParentObject(DependencyObject obj)
        {
            if (obj == null)
                return null;

            DependencyObject parent = null;
            try
            {
                parent = VisualTreeHelper.GetParent(obj);
            }
            catch (InvalidOperationException)
            {
            }

            if (parent != null)
                return parent;

            return LogicalTreeHelper.GetParent(obj);
        }

        private Uri BuildImageUri(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            Uri absolute;
            if (Uri.TryCreate(url, UriKind.Absolute, out absolute))
                return absolute;

            if (File.Exists(url))
                return new Uri(Path.GetFullPath(url), UriKind.Absolute);

            var appBase = AppDomain.CurrentDomain.BaseDirectory;
            var localPath = Path.Combine(appBase, url.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(localPath))
                return new Uri(localPath, UriKind.Absolute);

            var samplesLocalPath = Path.Combine(appBase, "Resources", url.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(samplesLocalPath))
                return new Uri(samplesLocalPath, UriKind.Absolute);

            if (Uri.TryCreate("pack://application:,,,/" + url.TrimStart('/'), UriKind.Absolute, out absolute))
                return absolute;

            if (Uri.TryCreate("pack://application:,,,/WPFDevelopers.Samples;component/" + url.TrimStart('/'), UriKind.Absolute, out absolute))
                return absolute;

            return null;
        }

        private static string ExtractUrlFromLinkTarget(string rawTarget)
        {
            if (string.IsNullOrWhiteSpace(rawTarget))
                return string.Empty;

            var target = rawTarget.Trim();
            var quoteIndex = target.IndexOf('"');
            if (quoteIndex > 0)
                return target.Substring(0, quoteIndex).Trim();

            quoteIndex = target.IndexOf('\'');
            if (quoteIndex > 0)
                return target.Substring(0, quoteIndex).Trim();

            return target;
        }

        private void AddInlineTokens(InlineCollection inlines, List<InlineToken> tokens)
        {
            foreach (var token in tokens)
            {
                if (token.Kind == InlineTokenKind.Text)
                {
                    inlines.Add(new Run(token.Text));
                    continue;
                }

                if (token.Kind == InlineTokenKind.Bold)
                {
                    inlines.Add(new Bold(new Run(token.Text)));
                    continue;
                }

                if (token.Kind == InlineTokenKind.Italic)
                {
                    inlines.Add(new Italic(new Run(token.Text)));
                    continue;
                }

                if (token.Kind == InlineTokenKind.BoldItalic)
                {
                    inlines.Add(new Bold(new Italic(new Run(token.Text))));
                    continue;
                }

                if (token.Kind == InlineTokenKind.StrikeThrough)
                {
                    var run = new Run(token.Text);
                    run.TextDecorations = TextDecorations.Strikethrough;
                    run.Foreground = ResolveMutedTextBrush();
                    inlines.Add(run);
                    continue;
                }

                if (token.Kind == InlineTokenKind.Code)
                {
                    var run = new Run(token.Text);
                    run.FontFamily = new FontFamily("Consolas");
                    run.Background = ResolveCodeInlineBackground();
                    run.Foreground = ResolveBrush("WD.PrimaryBrush", Brushes.RoyalBlue);
                    inlines.Add(run);
                    continue;
                }

                if (token.Kind == InlineTokenKind.Link)
                {
                    var hyperlink = new Hyperlink(new Run(token.Text));
                    hyperlink.Foreground = ResolveBrush("WD.PrimaryBrush", Brushes.RoyalBlue);
                    hyperlink.TextDecorations = TextDecorations.Underline;

                    var linkTarget = ExtractUrlFromLinkTarget(token.Url);
                    Uri linkUri;
                    if (Uri.TryCreate(linkTarget, UriKind.Absolute, out linkUri))
                        hyperlink.NavigateUri = linkUri;

                    hyperlink.Click += Hyperlink_Click;
                    inlines.Add(hyperlink);
                    continue;
                }

                if (token.Kind == InlineTokenKind.Image)
                {
                    var imageUri = BuildImageUri(token.Url);
                    if (imageUri != null)
                    {
                        try
                        {
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = imageUri;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();

                            var image = new Image();
                            image.Source = bitmap;
                            image.Stretch = Stretch.Uniform;
                            image.MaxHeight = 120;
                            image.MaxWidth = 240;

                            inlines.Add(new InlineUIContainer(image));
                            continue;
                        }
                        catch
                        {
                        }
                    }

                    inlines.Add(new Run(string.IsNullOrWhiteSpace(token.Text) ? "[image]" : token.Text));
                }
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var hyperlink = sender as Hyperlink;
            if (hyperlink == null)
                return;

            var uri = hyperlink.NavigateUri;
            if (uri == null)
                return;

            try
            {
                Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true });
            }
            catch
            {
                try { Process.Start(uri.AbsoluteUri); }
                catch { }
            }
        }

        private Brush ResolveBrush(string key, Brush fallback)
        {
            Brush brush = null;
            if (ThemeManager.Instance.Resources != null)
                brush = ThemeManager.Instance.Resources.TryFindResource<Brush>(key);

            if (brush != null)
                return brush;

            return fallback ?? Brushes.Black;
        }

        private bool IsDarkTheme()
        {
            if (ThemeManager.Instance.Resources == null)
                return false;

            return ThemeManager.Instance.Resources.Theme == ThemeType.Dark;
        }

        private Brush ResolveQuoteBackground()
        {
            var primary = ThemeManager.Instance.PrimaryColor;
            return new SolidColorBrush(Color.FromArgb(30, primary.R, primary.G, primary.B));
        }

        private Brush ResolveCodeHeaderBackground()
        {
            return IsDarkTheme()
                ? new SolidColorBrush(Color.FromRgb(34, 36, 40))
                : new SolidColorBrush(Color.FromRgb(246, 248, 252));
        }

        private Brush ResolveCodeBodyBackground()
        {
            return IsDarkTheme()
                ? new SolidColorBrush(Color.FromRgb(42, 42, 42))
                : new SolidColorBrush(Color.FromRgb(249, 250, 252));
        }

        private Brush ResolveCodeGutterBackground()
        {
            return IsDarkTheme()
                ? new SolidColorBrush(Color.FromRgb(36, 38, 42))
                : new SolidColorBrush(Color.FromRgb(250, 251, 254));
        }

        private Brush ResolveCodeLineNumberBrush()
        {
            return IsDarkTheme()
                ? new SolidColorBrush(Color.FromRgb(128, 136, 150))
                : new SolidColorBrush(Color.FromRgb(150, 156, 166));
        }

        private Brush ResolveCodeTextBrush()
        {
            return IsDarkTheme()
                ? new SolidColorBrush(Color.FromRgb(232, 236, 242))
                : new SolidColorBrush(Color.FromRgb(36, 40, 48));
        }

        private Brush ResolveMutedTextBrush()
        {
            return IsDarkTheme()
                ? new SolidColorBrush(Color.FromRgb(170, 176, 186))
                : new SolidColorBrush(Color.FromRgb(118, 124, 136));
        }

        private Brush ResolveCodeInlineBackground()
        {
            return ResolveQuoteBackground();
        }

        private FrameworkElement BuildHighlightedCodeBody(string[] lines, string lang)
        {
            var normalizedLang = NormalizeCodeLanguage(lang);
            if (normalizedLang == "text")
                normalizedLang = DetectCodeLanguage(lines);

            var panel = new StackPanel();
            for (var i = 0; i < lines.Length; i++)
            {
                var lineText = new TextBlock();
                lineText.TextWrapping = TextWrapping.NoWrap;
                lineText.FontFamily = new FontFamily("Consolas");
                lineText.FontSize = documentFontScale(0.95);
                lineText.Foreground = ResolveCodeTextBrush();
                AppendHighlightedCodeRuns(lineText.Inlines, lines[i], normalizedLang);
                panel.Children.Add(lineText);
            }

            return panel;
        }

        private void AppendHighlightedCodeRuns(InlineCollection inlines, string line, string lang)
        {
            var normalizedLang = NormalizeCodeLanguage(lang);
            var tokens = TokenizeCodeLine(line, normalizedLang);
            for (var i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                var run = new Run(token);
                run.Foreground = ClassifyCodeToken(token, normalizedLang, tokens, i);
                inlines.Add(run);
            }
        }

        private static List<string> TokenizeCodeLine(string line, string lang)
        {
            if (IsMarkupLanguage(lang))
            {
                var result = new List<string>();
                var matches = Regex.Matches(line ?? string.Empty, "(\"[^\"]*\"|'[^']*'|</|/>|[<>=/]|[A-Za-z_][A-Za-z0-9_:\\.-]*|\\s+|.)");
                foreach (Match match in matches)
                    result.Add(match.Value);
                return result;
            }

            return Regex.Split(line, "(\\s+|[;.,(){}\\[\\]=+\\-*/<>!&|:%?])")
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();
        }

        private Brush ClassifyCodeToken(string token, string lang, List<string> tokens, int index)
        {
            var langLower = NormalizeCodeLanguage(lang);

            if (IsMarkupLanguage(langLower))
                return ClassifyMarkupToken(token, tokens, index);

            var keywords = new HashSet<string>(GetLanguageKeywords(langLower), StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(token))
                return ResolveCodeTextBrush();

            if (Regex.IsMatch(token, "^//.*") || Regex.IsMatch(token, "^#.*") || token.StartsWith("/*", StringComparison.Ordinal) || token.StartsWith("*", StringComparison.Ordinal) || token.EndsWith("*/", StringComparison.Ordinal))
            {
                return IsDarkTheme()
                    ? new SolidColorBrush(Color.FromRgb(122, 132, 146))
                    : new SolidColorBrush(Color.FromRgb(121, 132, 145));
            }

            if (keywords.Contains(token) || keywords.Contains(token.TrimEnd('(')))
            {
                return IsDarkTheme()
                    ? new SolidColorBrush(Color.FromRgb(127, 194, 255))
                    : new SolidColorBrush(Color.FromRgb(0, 92, 197));
            }

            if ((token.StartsWith("\"", StringComparison.Ordinal) && token.EndsWith("\"", StringComparison.Ordinal)) ||
                (token.StartsWith("'", StringComparison.Ordinal) && token.EndsWith("'", StringComparison.Ordinal)))
            {
                return IsDarkTheme()
                    ? new SolidColorBrush(Color.FromRgb(165, 214, 125))
                    : new SolidColorBrush(Color.FromRgb(46, 125, 50));
            }

            if (Regex.IsMatch(token, "^\\d+(\\.\\d+)?$"))
            {
                return IsDarkTheme()
                    ? new SolidColorBrush(Color.FromRgb(245, 171, 116))
                    : new SolidColorBrush(Color.FromRgb(198, 91, 23));
            }

            if (token == "{" || token == "}" || token == "(" || token == ")" || token == "[" || token == "]")
            {
                return IsDarkTheme()
                    ? new SolidColorBrush(Color.FromRgb(255, 202, 124))
                    : new SolidColorBrush(Color.FromRgb(193, 124, 0));
            }

            return ResolveCodeTextBrush();
        }

        private Brush ClassifyMarkupToken(string token, List<string> tokens, int index)
        {
            if (string.IsNullOrWhiteSpace(token))
                return ResolveCodeTextBrush();

            if ((token.StartsWith("\"", StringComparison.Ordinal) && token.EndsWith("\"", StringComparison.Ordinal)) ||
                (token.StartsWith("'", StringComparison.Ordinal) && token.EndsWith("'", StringComparison.Ordinal)))
            {
                return IsDarkTheme()
                    ? new SolidColorBrush(Color.FromRgb(165, 214, 125))
                    : new SolidColorBrush(Color.FromRgb(46, 125, 50));
            }

            if (token == "<" || token == ">" || token == "</" || token == "/>" || token == "=" || token == "/")
            {
                return IsDarkTheme()
                    ? new SolidColorBrush(Color.FromRgb(255, 202, 124))
                    : new SolidColorBrush(Color.FromRgb(193, 124, 0));
            }

            var prev = GetPreviousSignificantToken(tokens, index);
            var next = GetNextSignificantToken(tokens, index);
            if (prev == "<" || prev == "</")
            {
                return IsDarkTheme()
                    ? new SolidColorBrush(Color.FromRgb(127, 194, 255))
                    : new SolidColorBrush(Color.FromRgb(0, 92, 197));
            }

            if (next == "=")
            {
                return IsDarkTheme()
                    ? new SolidColorBrush(Color.FromRgb(245, 171, 116))
                    : new SolidColorBrush(Color.FromRgb(198, 91, 23));
            }

            if (token.IndexOf(':') >= 0)
            {
                return IsDarkTheme()
                    ? new SolidColorBrush(Color.FromRgb(166, 214, 255))
                    : new SolidColorBrush(Color.FromRgb(21, 101, 192));
            }

            return ResolveCodeTextBrush();
        }

        private static string GetPreviousSignificantToken(List<string> tokens, int index)
        {
            for (var i = index - 1; i >= 0; i--)
            {
                if (!string.IsNullOrWhiteSpace(tokens[i]))
                    return tokens[i];
            }

            return string.Empty;
        }

        private static string GetNextSignificantToken(List<string> tokens, int index)
        {
            for (var i = index + 1; i < tokens.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(tokens[i]))
                    return tokens[i];
            }

            return string.Empty;
        }

        private static bool IsMarkupLanguage(string lang)
        {
            return lang == "xaml" || lang == "xml" || lang == "html";
        }

        private static string[] GetLanguageKeywords(string langLower)
        {
            if (langLower == "csharp")
            {
                return new[]
                {
                    "var", "int", "string", "double", "float", "bool", "void", "class", "public", "private", "protected", "internal", "static", "return",
                    "if", "else", "for", "foreach", "while", "do", "switch", "case", "break", "continue", "new", "null", "true", "false", "this", "base",
                    "override", "virtual", "abstract", "sealed", "using", "namespace", "try", "catch", "finally", "throw", "async", "await", "typeof", "is", "as",
                    "in", "from", "where", "select", "let", "group", "by", "into", "orderby", "join", "on", "equals", "Console", "List", "Dictionary", "Task",
                    "IEnumerable", "IAsyncEnumerable"
                };
            }

            if (langLower == "javascript" || langLower == "typescript")
            {
                return new[]
                {
                    "const", "let", "var", "function", "return", "if", "else", "for", "while", "class", "extends", "import", "export", "default", "from",
                    "new", "this", "null", "undefined", "true", "false", "async", "await", "try", "catch", "throw", "typeof", "instanceof", "console", "Promise",
                    "Map", "Set", "Array", "type", "interface", "enum", "implements", "readonly", "any", "unknown", "never", "keyof", "infer"
                };
            }

            if (langLower == "python")
            {
                return new[]
                {
                    "def", "class", "import", "from", "as", "return", "if", "elif", "else", "for", "while", "try", "except", "finally", "raise", "with", "yield",
                    "lambda", "pass", "break", "continue", "and", "or", "not", "is", "in", "None", "True", "False", "self", "print", "range", "len", "str", "int",
                    "float", "list", "dict", "set", "tuple"
                };
            }

            if (langLower == "xml" || langLower == "html")
            {
                return new[] { "xml", "html", "head", "body", "div", "span", "p", "a", "img", "script", "style", "link", "meta", "title" };
            }

            if (langLower == "css")
            {
                return new[] { "important" };
            }

            if (langLower == "sql")
            {
                return new[]
                {
                    "SELECT", "FROM", "WHERE", "INSERT", "UPDATE", "DELETE", "CREATE", "DROP", "ALTER", "TABLE", "INTO", "VALUES", "SET", "JOIN", "LEFT", "RIGHT",
                    "INNER", "OUTER", "ON", "GROUP", "BY", "ORDER", "HAVING", "LIMIT", "AS", "AND", "OR", "NOT", "NULL", "IS", "IN", "EXISTS", "BETWEEN", "LIKE",
                    "DISTINCT", "COUNT", "SUM", "AVG", "MAX", "MIN"
                };
            }

            if (langLower == "bash")
            {
                return new[]
                {
                    "if", "then", "else", "fi", "for", "while", "do", "done", "case", "esac", "function", "return", "exit", "echo", "export", "source", "cd", "pwd",
                    "ls", "grep", "sed", "awk", "cat", "mkdir", "rm", "cp", "mv", "chmod", "chown", "sudo", "apt", "yum", "brew"
                };
            }

            return new string[0];
        }

        private static string NormalizeCodeLanguage(string lang)
        {
            var value = (lang ?? string.Empty).Trim().ToLowerInvariant();
            if (value == "c#" || value == "cs") return "csharp";
            if (value == "js") return "javascript";
            if (value == "ts") return "typescript";
            if (value == "py") return "python";
            if (value == "xaml") return "xaml";
            if (value == "sh" || value == "shell" || value == "zsh") return "bash";
            return string.IsNullOrWhiteSpace(value) ? "text" : value;
        }

        private static string GetCodeLanguageLabel(string lang)
        {
            switch (NormalizeCodeLanguage(lang))
            {
                case "csharp":
                    return "C#";
                case "javascript":
                    return "JS";
                case "typescript":
                    return "TS";
                case "python":
                    return "Py";
                case "xaml":
                    return "XAML";
                case "xml":
                    return "XML";
                case "html":
                    return "HTML";
                case "bash":
                    return "Bash";
                case "sql":
                    return "SQL";
                default:
                    return "Text";
            }
        }

        private static string DetectCodeLanguage(string[] lines)
        {
            var text = string.Join("\n", lines ?? new string[0]);

            if (Regex.IsMatch(text, "\\b(namespace|using|public|private|protected|internal|class|static|Console)\\b"))
                return "csharp";
            if (Regex.IsMatch(text, "<\\s*[A-Za-z_][A-Za-z0-9_:\\.-]*") || Regex.IsMatch(text, "\\bxmlns(:[A-Za-z_][A-Za-z0-9_]*)?=", RegexOptions.IgnoreCase))
                return "xaml";
            if (Regex.IsMatch(text, "\\b(def|import|from|lambda|yield|None|True|False)\\b"))
                return "python";
            if (Regex.IsMatch(text, "\\b(const|let|function|=>|console\\.log|import|export)\\b"))
                return "javascript";
            if (Regex.IsMatch(text, "\\b(type|interface|implements|readonly|keyof|infer)\\b"))
                return "typescript";

            return "text";
        }

        private double documentFontScale(double factor)
        {
            var baseSize = FontSize > 0 ? FontSize : 13;
            return Math.Round(baseSize * factor, 2, MidpointRounding.AwayFromZero);
        }

        private static List<MarkdownBlock> ParseMarkdown(string text)
        {
            var result = new List<MarkdownBlock>();
            if (string.IsNullOrWhiteSpace(text))
                return result;

            var lines = text.Replace("\r", string.Empty).Split('\n');
            var paragraphBuilder = new StringBuilder();
            var inCodeBlock = false;
            var codeBuilder = new StringBuilder();
            var codeLang = string.Empty;
            var listBuffer = new List<string>();
            var orderedListBuffer = new List<string>();

            Action flushParagraph = delegate
            {
                if (paragraphBuilder.Length == 0)
                    return;

                var content = paragraphBuilder.ToString().Trim();
                if (content.Length > 0)
                    result.Add(new ParagraphBlock { Text = content });

                paragraphBuilder.Clear();
            };

            Action flushLists = delegate
            {
                if (listBuffer.Count > 0)
                {
                    var list = new UnorderedListBlock();
                    list.Items.AddRange(listBuffer);
                    result.Add(list);
                    listBuffer.Clear();
                }

                if (orderedListBuffer.Count > 0)
                {
                    var list = new OrderedListBlock();
                    list.Items.AddRange(orderedListBuffer);
                    result.Add(list);
                    orderedListBuffer.Clear();
                }
            };

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var trimmed = line.Trim();

                if (inCodeBlock)
                {
                    if (trimmed.StartsWith("```", StringComparison.Ordinal))
                    {
                        result.Add(new CodeBlock { Code = codeBuilder.ToString().TrimEnd('\n'), Language = codeLang });
                        codeBuilder.Clear();
                        codeLang = string.Empty;
                        inCodeBlock = false;
                    }
                    else
                    {
                        codeBuilder.AppendLine(line);
                    }
                    continue;
                }

                if (trimmed.StartsWith("```", StringComparison.Ordinal))
                {
                    flushParagraph();
                    flushLists();
                    inCodeBlock = true;
                    codeLang = trimmed.Length > 3 ? trimmed.Substring(3).Trim() : string.Empty;
                    continue;
                }

                if (trimmed.StartsWith("|") && IsTableSeparator(lines, i + 1))
                {
                    flushParagraph();
                    flushLists();
                    result.Add(ParseTable(lines, ref i));
                    continue;
                }

                if (string.IsNullOrWhiteSpace(trimmed))
                {
                    flushParagraph();
                    flushLists();
                    continue;
                }

                if (Regex.IsMatch(trimmed, "^(-{3,}|\\*{3,}|_{3,})$"))
                {
                    flushParagraph();
                    flushLists();
                    result.Add(new HorizontalRuleBlock());
                    continue;
                }

                var headerMatch = Regex.Match(line, "^(#{1,6})\\s+(.*)");
                if (headerMatch.Success)
                {
                    flushParagraph();
                    flushLists();
                    result.Add(new HeaderBlock
                    {
                        Level = headerMatch.Groups[1].Value.Length,
                        Text = headerMatch.Groups[2].Value.Trim()
                    });
                    continue;
                }

                if (trimmed.StartsWith(">", StringComparison.Ordinal))
                {
                    flushParagraph();
                    flushLists();

                    var quoteBuilder = new StringBuilder();
                    while (i < lines.Length)
                    {
                        var currentLine = lines[i];
                        var currentTrimmed = currentLine.TrimStart();
                        if (!currentTrimmed.StartsWith(">", StringComparison.Ordinal))
                            break;

                        var content = currentTrimmed.Substring(1);
                        if (content.StartsWith(" ", StringComparison.Ordinal))
                            content = content.Substring(1);

                        if (quoteBuilder.Length > 0)
                            quoteBuilder.AppendLine();
                        quoteBuilder.Append(content.TrimEnd());
                        i++;
                    }

                    i--;
                    result.Add(new QuoteBlock { Text = quoteBuilder.ToString() });
                    continue;
                }

                if (trimmed.StartsWith("- ", StringComparison.Ordinal) ||
                    trimmed.StartsWith("* ", StringComparison.Ordinal) ||
                    trimmed.StartsWith("+ ", StringComparison.Ordinal))
                {
                    flushParagraph();
                    if (orderedListBuffer.Count > 0)
                    {
                        var ordered = new OrderedListBlock();
                        ordered.Items.AddRange(orderedListBuffer);
                        result.Add(ordered);
                        orderedListBuffer.Clear();
                    }
                    listBuffer.Add(trimmed.Substring(2).Trim());
                    continue;
                }

                var orderedMatch = Regex.Match(trimmed, "^(\\d+)\\.\\s+(.*)");
                if (orderedMatch.Success)
                {
                    flushParagraph();
                    if (listBuffer.Count > 0)
                    {
                        var unordered = new UnorderedListBlock();
                        unordered.Items.AddRange(listBuffer);
                        result.Add(unordered);
                        listBuffer.Clear();
                    }
                    orderedListBuffer.Add(orderedMatch.Groups[2].Value.Trim());
                    continue;
                }

                var imageMatch = Regex.Match(trimmed, "^!\\[([^\\]]*)\\]\\(([^)]+)\\)$");
                if (imageMatch.Success)
                {
                    flushParagraph();
                    flushLists();
                    result.Add(new ImageBlock
                    {
                        AltText = imageMatch.Groups[1].Value,
                        Url = imageMatch.Groups[2].Value
                    });
                    continue;
                }

                flushLists();
                if (paragraphBuilder.Length > 0)
                    paragraphBuilder.AppendLine();
                paragraphBuilder.Append(line);
            }

            if (inCodeBlock)
                result.Add(new CodeBlock { Code = codeBuilder.ToString().TrimEnd('\n'), Language = codeLang });

            flushParagraph();
            flushLists();
            return result;
        }

        private static bool IsTableSeparator(string[] lines, int idx)
        {
            if (idx < 0 || idx >= lines.Length)
                return false;

            var line = lines[idx].Trim();
            if (!line.StartsWith("|", StringComparison.Ordinal))
                return false;

            var cells = line.Split('|');
            if (cells.Length < 3)
                return false;

            for (var i = 1; i < cells.Length - 1; i++)
            {
                var cell = cells[i].Trim().Trim(':');
                if (string.IsNullOrEmpty(cell) || cell.Any(c => c != '-'))
                    return false;
            }

            return true;
        }

        private static TableBlock ParseTable(string[] lines, ref int idx)
        {
            var table = new TableBlock();
            table.Headers = SplitTableCells(lines[idx].Trim());
            table.Alignments = ParseTableAlignments(lines[idx + 1].Trim());

            idx += 2;
            while (idx < lines.Length)
            {
                var line = lines[idx].Trim();
                if (string.IsNullOrEmpty(line) || !line.StartsWith("|", StringComparison.Ordinal))
                    break;

                if (IsTableSeparator(lines, idx))
                    break;

                table.Rows.Add(SplitTableCells(line));
                idx++;
            }

            idx--;
            return table;
        }

        private static List<TableAlignment> ParseTableAlignments(string line)
        {
            var result = new List<TableAlignment>();
            var cells = line.Split('|');
            for (var i = 1; i < cells.Length - 1; i++)
            {
                var cell = cells[i].Trim();
                var left = cell.StartsWith(":", StringComparison.Ordinal);
                var right = cell.EndsWith(":", StringComparison.Ordinal);
                if (left && right) result.Add(TableAlignment.Center);
                else if (right) result.Add(TableAlignment.Right);
                else result.Add(TableAlignment.Left);
            }
            return result;
        }

        private static List<string> SplitTableCells(string line)
        {
            var cells = line.Split('|').Select(c => c.Trim()).ToList();
            if (cells.Count > 0 && cells[0] == string.Empty) cells.RemoveAt(0);
            if (cells.Count > 0 && cells[cells.Count - 1] == string.Empty) cells.RemoveAt(cells.Count - 1);
            return cells;
        }

        private static List<InlineToken> ParseInlines(string text)
        {
            var result = new List<InlineToken>();
            if (string.IsNullOrEmpty(text))
                return result;

            const string pattern =
                "(`[^`]+`)|(!\\[([^\\]]*)\\]\\(([^)]+)\\))|(\\[([^\\]]+)\\]\\(([^)]+)\\))|(\\*\\*\\*(.+?)\\*\\*\\*)|(___(.+?)___)|(\\*\\*(.+?)\\*\\*)|(__(.+?)__)|(\\*(.+?)\\*)|(_(.+?)_)|(~~(.+?)~~)";

            var matches = Regex.Matches(text, pattern);
            var start = 0;

            foreach (Match match in matches)
            {
                if (match.Index > start)
                {
                    var plain = text.Substring(start, match.Index - start);
                    if (!string.IsNullOrEmpty(plain))
                        result.Add(new InlineToken(InlineTokenKind.Text, plain, null));
                }

                if (match.Groups[1].Success)
                    result.Add(new InlineToken(InlineTokenKind.Code, match.Groups[1].Value.Trim('`'), null));
                else if (match.Groups[3].Success)
                    result.Add(new InlineToken(InlineTokenKind.Image, match.Groups[3].Value, match.Groups[4].Value));
                else if (match.Groups[6].Success)
                    result.Add(new InlineToken(InlineTokenKind.Link, match.Groups[6].Value, match.Groups[7].Value));
                else if (match.Groups[9].Success)
                    result.Add(new InlineToken(InlineTokenKind.BoldItalic, match.Groups[9].Value, null));
                else if (match.Groups[11].Success)
                    result.Add(new InlineToken(InlineTokenKind.BoldItalic, match.Groups[11].Value, null));
                else if (match.Groups[13].Success)
                    result.Add(new InlineToken(InlineTokenKind.Bold, match.Groups[13].Value, null));
                else if (match.Groups[15].Success)
                    result.Add(new InlineToken(InlineTokenKind.Bold, match.Groups[15].Value, null));
                else if (match.Groups[17].Success)
                    result.Add(new InlineToken(InlineTokenKind.Italic, match.Groups[17].Value, null));
                else if (match.Groups[19].Success)
                    result.Add(new InlineToken(InlineTokenKind.Italic, match.Groups[19].Value, null));
                else if (match.Groups[21].Success)
                    result.Add(new InlineToken(InlineTokenKind.StrikeThrough, match.Groups[21].Value, null));

                start = match.Index + match.Length;
            }

            if (start < text.Length)
            {
                var plain = text.Substring(start);
                if (!string.IsNullOrEmpty(plain))
                    result.Add(new InlineToken(InlineTokenKind.Text, plain, null));
            }

            return result;
        }

        private abstract class MarkdownBlock { }

        private sealed class HeaderBlock : MarkdownBlock
        {
            public int Level;
            public string Text;
        }

        private sealed class ParagraphBlock : MarkdownBlock
        {
            public string Text;
        }

        private sealed class HorizontalRuleBlock : MarkdownBlock { }

        private sealed class UnorderedListBlock : MarkdownBlock
        {
            public readonly List<string> Items = new List<string>();
        }

        private sealed class OrderedListBlock : MarkdownBlock
        {
            public readonly List<string> Items = new List<string>();
        }

        private sealed class QuoteBlock : MarkdownBlock
        {
            public string Text;
        }

        private sealed class CodeBlock : MarkdownBlock
        {
            public string Code;
            public string Language;
        }

        private sealed class ImageBlock : MarkdownBlock
        {
            public string AltText;
            public string Url;
        }

        private sealed class TableBlock : MarkdownBlock
        {
            public List<string> Headers = new List<string>();
            public List<List<string>> Rows = new List<List<string>>();
            public List<TableAlignment> Alignments = new List<TableAlignment>();
        }

        private enum TableAlignment
        {
            Left,
            Center,
            Right
        }

        private enum InlineTokenKind
        {
            Text,
            Bold,
            Italic,
            BoldItalic,
            Code,
            StrikeThrough,
            Link,
            Image
        }

        private sealed class InlineToken
        {
            public InlineToken(InlineTokenKind kind, string text, string url)
            {
                Kind = kind;
                Text = text;
                Url = url;
            }

            public readonly InlineTokenKind Kind;
            public readonly string Text;
            public readonly string Url;
        }
    }
}
