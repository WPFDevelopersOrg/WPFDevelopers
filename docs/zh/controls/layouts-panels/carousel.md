# wd:Carousel

> 分类：[布局与面板](../index.md#layouts-panels) · 可通过 WPFDevelopers NuGet 包使用。

轮播图（自动播放/圆点指示器/箭头/点击跳转）

## 示例源码

[CarouselExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CarouselExample.xaml)

## 详细用法

`Carousel` 是 WPFDevelopers 提供的轮播图控件，支持自动播放、圆点指示器、箭头导航、点击跳转等功能。

### 基础用法（XAML 声明）

```xml
<wd:Carousel
    Width="400" Height="200"
    AutoPlay="True"
    AutoPlayInterval="0:0:4"
    ShowArrows="True"
    ItemClick="Carousel_ItemClick">
    <Border Background="#722ed1">
        <TextBlock Text="Slide 1" Foreground="White" FontSize="24" />
    </Border>
    <Border Background="#eb2f96">
        <TextBlock Text="Slide 2" Foreground="White" FontSize="24" />
    </Border>
    <Border Background="#1890ff">
        <TextBlock Text="Slide 3" Foreground="White" FontSize="24" />
    </Border>
</wd:Carousel>
```

### 数据绑定用法（MVVM）

```xml
<wd:Carousel
    ItemsSource="{Binding ImagePaths}"
    DisplayMemberPath="URL"
    AutoPlay="True"
    ItemClickCommand="{Binding CarouselClickCommand}" />
```

```csharp
// 数据模型
public class CarouselSlideModel
{
    public string Title { get; set; }
    public string URL { get; set; }
}

// ViewModel
public ObservableCollection<CarouselSlideModel> ImagePaths { get; set; }

public ICommand CarouselClickCommand => new RelayCommand(param =>
{
    if (param is CarouselSlideModel model)
    {
        Toast.Push($"点击了图片 - {model.Title}", ToastImage.Success, true);
    }
});
```

### 主要属性

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `ItemsSource` | IEnumerable | null | 数据源 |
| `SelectedIndex` | int | 0 | 当前选中项索引 |
| `SelectedItem` | object | null | 当前选中项 |
| `AutoPlay` | bool | false | 是否自动播放 |
| `AutoPlayInterval` | TimeSpan | 3秒 | 自动播放间隔 |
| `AnimationDuration` | double | 0.5 | 切换动画时长（秒） |
| `ShowDots` | bool | true | 是否显示圆点指示器 |
| `ShowArrows` | bool | true | 是否显示左右箭头 |
| `ItemTemplate` | DataTemplate | null | 数据模板 |
| `DisplayMemberPath` | string | null | 显示成员路径（直接显示属性值） |

### 事件

| 事件 | 说明 |
|------|------|
| `ItemClick` | 点击幻灯片时触发 |
| `SelectedItemChanged` | 选中项变化时触发 |

### 方法

| 方法 | 说明 |
|------|------|
| `GoToNext()` | 切换到下一张 |
| `GoToPrevious()` | 切换到上一张 |

---

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
