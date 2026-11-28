# wd:Pagination

> Category: [Charts & Data Visualization](../index.md#charts-data-visualization) · Available in the WPFDevelopers NuGet package.

Pagination control

## Example Source

[PaginationExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/PaginationExample.xaml)

## XAML Usage

```xml
<wd:Pagination
                        Grid.Row="2"
                        Grid.Column="0"
                        Count="{Binding NormalPaginationViewModel.Count, RelativeSource={RelativeSource AncestorType=UserControl}, Mode=TwoWay}"
                        CountPerPage="{Binding NormalPaginationViewModel.CountPerPage, Mode=TwoWay, RelativeSource={RelativeSource AncestorType=UserControl}}"
                        Current="{Binding NormalPaginationViewModel.Current, Mode=TwoWay, RelativeSource={RelativeSource AncestorType=UserControl}}"
                        IsLite="False"
                        JumpPageClick="PaginationJumpPageClick"
                        NextClick="PaginationNextClick"
                        PageClick="PaginationPageClick"
                        PrevClick="PaginationPrevClick" />
```

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

