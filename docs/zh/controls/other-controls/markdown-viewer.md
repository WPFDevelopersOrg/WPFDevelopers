# wd:MarkdownViewer

> 分类：[其他控件](../index.md#other-controls) · 可通过 WPFDevelopers NuGet 包使用。

将 Markdown 文本渲染为 WPF 富文本，适合展示说明文档、README、日志等内容。

## 示例源码

[MarkdownViewerExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/MarkdownViewerExample.xaml)

## 详细用法

`wd:MarkdownViewer` 用于将 Markdown 文本渲染为 WPF 中的富文本展示，适合展示说明文档、README、日志、知识库内容等。它支持标题、列表、引用、代码块、表格、链接、图片等常见 Markdown 语法。

最常用的属性是 `Markdown`，直接设置字符串即可：

```xml
<Grid>
    <wd:MarkdownViewer
        Margin="12"
        Padding="12"
        FontSize="14"
        Markdown="## Hello Markdown&#10;&#10;This is **WPFDevelopers** MarkdownViewer." />
</Grid>
```

也可以通过绑定内容：

```xml
<wd:MarkdownViewer
    Margin="12"
    Padding="12"
    Markdown="{Binding MarkdownText}" />
```

```csharp
public string MarkdownText { get; } = "# WPFDevelopers\n\n- 支持标题\n- 支持列表\n- 支持代码块";
```

常见用法说明：

- `Markdown`：要渲染的 Markdown 文本
- `Padding`：内容区的内边距
- `FontSize` / `FontFamily` / `Foreground`：文本样式控制

完整示例和交互式演示可参考：

- `src/WPFDevelopers.Samples.Shared/ExampleViews/MarkdownViewerExample.xaml`
- `src/WPFDevelopers.Samples.Shared/ExampleViews/MarkdownViewerExample.xaml.cs`

如果你需要展示 README、帮助文档、更新日志或内部说明，`MarkdownViewer` 是最直接的选择。
---

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
