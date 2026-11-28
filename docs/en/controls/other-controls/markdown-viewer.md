# wd:MarkdownViewer

> Category: [Other Controls](../index.md#other-controls) · Available in the WPFDevelopers NuGet package.

Renders Markdown as rich text in WPF, useful for guides, README content, and logs.

## Example Source

[MarkdownViewerExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/MarkdownViewerExample.xaml)

## Detailed Usage

`wd:MarkdownViewer` renders Markdown content as rich text in WPF, which is ideal for README files, help documentation, changelogs, and in-app knowledge content. It supports common Markdown elements such as headings, lists, blockquotes, code blocks, tables, links, and images.

The most commonly used property is `Markdown`, which accepts the Markdown text directly:

```xml
<Grid>
    <wd:MarkdownViewer
        Margin="12"
        Padding="12"
        FontSize="14"
        Markdown="## Hello Markdown&#10;&#10;This is **WPFDevelopers** MarkdownViewer." />
</Grid>
```

You can also bind the content dynamically:

```xml
<wd:MarkdownViewer
    Margin="12"
    Padding="12"
    Markdown="{Binding MarkdownText}" />
```

```csharp
public string MarkdownText { get; } = "# WPFDevelopers\n\n- Supports headings\n- Supports lists\n- Supports code blocks";
```

Common usage notes:

- `Markdown`: the Markdown text to render
- `Padding`: spacing inside the content area
- `FontSize`, `FontFamily`, `Foreground`: text style customization

The complete sample and interactive demo are available here:

- `src/WPFDevelopers.Samples.Shared/ExampleViews/MarkdownViewerExample.xaml`
- `src/WPFDevelopers.Samples.Shared/ExampleViews/MarkdownViewerExample.xaml.cs`

If you need to display a README, help document, release notes, or internal documentation in your app, `MarkdownViewer` is the most direct option.
---

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

