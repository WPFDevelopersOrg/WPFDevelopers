# wd:Pagination

> 分类：[图表与数据可视化](../index.md#charts-data-visualization) · 可通过 WPFDevelopers NuGet 包使用。

分页控件

## 示例源码

[PaginationExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/PaginationExample.xaml)

## XAML 用法

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

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
