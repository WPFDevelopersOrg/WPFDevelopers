# wd:WDBorder

> Category: [Basic Controls & Styles](../index.md#basic-controls) · Available in the WPFDevelopers NuGet package.

`WDBorder` derives from WPF `Border` and adds a read-only `ContentClip` geometry for clipping child content to the rounded interior. It does not clip its content automatically; bind `ContentClip` to the `Clip` property of each child that needs clipping.

## Example Source

- [Usage in the TreeView style](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Net40/Styles/Styles.TreeView.xaml)
- [WDBorder implementation](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Shared/Controls/BaseControls/WDBorder.cs)

## Detailed Usage

Declare the WPFDevelopers namespace and bind `ContentClip` to the content element:

```xml
<wd:WDBorder
    xmlns:wd="https://github.com/WPFDevelopersOrg/WPFDevelopers"
    Width="220"
    Height="140"
    Background="#E6F4FF"
    BorderBrush="#1677FF"
    BorderThickness="1"
    CornerRadius="16">
    <Grid Clip="{Binding Path=ContentClip, RelativeSource={RelativeSource AncestorType=wd:WDBorder}}">
        <TextBlock
            HorizontalAlignment="Center"
            VerticalAlignment="Center"
            Text="Content inside rounded corners" />
    </Grid>
</wd:WDBorder>
```

### Key Properties

| Property | Type | Description |
|------|------|------|
| `ContentClip` | `Geometry` | Read-only clipping geometry calculated from the control size, `CornerRadius`, and `BorderThickness` |
| `CornerRadius` | `CornerRadius` | Inherited from `Border`; controls the corner radius |
| `BorderThickness` | `Thickness` | Inherited from `Border`; controls the border thickness and the inset of the clipping region |

`ContentClip` is primarily useful in custom control templates. Bind it to an inner `Grid`, `Image`, or scrollable content element to keep children within the rounded boundary.

---

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)
