using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WPFDevelopers;

namespace WPFDevelopers.Samples.ExampleViews
{
    public partial class MarkdownViewerExample : UserControl
    {
        private const double MinZoom = 0.5;
        private const double MaxZoom = 2.0;
        private const double ZoomStep = 0.15;
        private const double BasePreviewFontSize = 13.0;

        private string _currentFilePath = string.Empty;
        private double _fontScale = 1.0;

        private const string InitialMarkdown =
@"# WPFDevelopers MarkdownViewer

`MarkdownViewer` supports common Markdown syntax and follows the current WD theme.

## Inline Styles
- **Bold text**
- *Italic text*
- ***Bold italic text***
- ~~Strikethrough~~
- `Inline code`
- [WPFDevelopers GitHub](https://github.com/WPFDevelopersOrg/WPFDevelopers)

## Quote
> Theme brushes come from WD dynamic resources, so light/dark and primary color switch automatically.

## Code Block
```csharp
public static string Hello(string name)
{
    return ""Hello, "" + name + ""!"";
}
```

```python
def fibonacci(n):
    a, b = 0, 1
    for _ in range(n):
        yield a
        a, b = b, a + b

print(list(fibonacci(10)))
```

```javascript
const greeting = ""Hello, World!"";
console.log(greeting);

function sum(a, b) {
    return a + b;
}
```

```typescript
type User = {
    id: number;
    name: string;
};

const user: User = { id: 1, name: ""WD"" };
console.log(user.name);
```

```xaml
<Application x:Class=""YourApp.App""
             xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
             xmlns:wd=""https://github.com/WPFDevelopersOrg/WPFDevelopers""
             StartupUri=""MainWindow.xaml"">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <!-- 1. Theme must be imported first -->
                <ResourceDictionary Source=""pack://application:,,,/WPFDevelopers;component/Themes/Theme.xaml"" />
                <!-- 2. wd:Resources must come AFTER Theme.xaml -->
                <wd:Resources Radius=""4"" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

---

## Ordered List
1. Edit markdown on the left.
2. Preview updates on the right.
3. Change WD theme and colors to verify follow-up.

## Table
| Shortcut | Action |
| :-- | :-- |
| Ctrl+O | Open file |
| Ctrl+S | Save file |
| Ctrl++ | Zoom in |
| Ctrl+- | Zoom out |

## Images
![Sample Image](Resources/Images/Craouse/Slide0.png)
";

        public MarkdownViewerExample()
        {
            InitializeComponent();
            EditorTextBox.Text = InitialMarkdown;
            PreviewViewer.Markdown = InitialMarkdown;
            ApplyZoom();
        }

        private void EditorTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshPreview();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Markdown Files (*.md)|*.md|All Files (*.*)|*.*"
            };

            if (dlg.ShowDialog() == true && File.Exists(dlg.FileName))
            {
                EditorTextBox.Text = File.ReadAllText(dlg.FileName);
                _currentFilePath = dlg.FileName;
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                Filter = "Markdown Files (*.md)|*.md|All Files (*.*)|*.*",
                FileName = string.IsNullOrWhiteSpace(_currentFilePath) ? "untitled.md" : _currentFilePath
            };

            if (dlg.ShowDialog() == true)
            {
                File.WriteAllText(dlg.FileName, EditorTextBox.Text ?? string.Empty);
                _currentFilePath = dlg.FileName;
            }
        }

        private void LoadSample_Click(object sender, RoutedEventArgs e)
        {
            EditorTextBox.Text = InitialMarkdown;
            RefreshPreview();
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            _fontScale = Math.Min(_fontScale + ZoomStep, MaxZoom);
            ApplyZoom();
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            _fontScale = Math.Max(_fontScale - ZoomStep, MinZoom);
            ApplyZoom();
        }

        private void ApplyZoom()
        {
            PreviewViewer.FontSize = BasePreviewFontSize * _fontScale;
            ZoomTextBlock.Text = ((int)Math.Round(_fontScale * 100.0)).ToString() + "%";
            ForceRefreshPreviewVisual();
        }

        private void RefreshPreview()
        {
            if (PreviewViewer == null)
                return;

            var markdown = EditorTextBox == null ? string.Empty : (EditorTextBox.Text ?? string.Empty);
            if (string.Equals(PreviewViewer.Markdown, markdown, StringComparison.Ordinal))
                PreviewViewer.Markdown = string.Empty;

            PreviewViewer.Markdown = markdown;
            ForceRefreshPreviewVisual();
        }

        private void ForceRefreshPreviewVisual()
        {
            if (PreviewViewer == null)
                return;

            PreviewViewer.InvalidateMeasure();
            PreviewViewer.InvalidateArrange();
            PreviewViewer.InvalidateVisual();
            PreviewViewer.UpdateLayout();
        }
    }
}
