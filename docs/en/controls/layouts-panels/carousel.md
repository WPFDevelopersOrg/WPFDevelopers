# wd:Carousel

> Category: [Layout & Panels](../index.md#layouts-panels) · Available in the WPFDevelopers NuGet package.

Carousel (auto-play / dot indicators / arrows / click navigation)

## Example Source

[CarouselExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CarouselExample.xaml)

## Detailed Usage

`Carousel` is a carousel control provided by WPFDevelopers, supporting auto-play, dot indicators, arrow navigation, click navigation, and more.

### Basic Usage (XAML Declaration)

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

### Data Binding Usage (MVVM)

```xml
<wd:Carousel
    ItemsSource="{Binding ImagePaths}"
    DisplayMemberPath="URL"
    AutoPlay="True"
    ItemClickCommand="{Binding CarouselClickCommand}" />
```

```csharp
// Data model
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
        Toast.Push($"Clicked image - {model.Title}", ToastImage.Success, true);
    }
});
```

### Key Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ItemsSource` | IEnumerable | null | Data source |
| `SelectedIndex` | int | 0 | Current selected item index |
| `SelectedItem` | object | null | Current selected item |
| `AutoPlay` | bool | false | Enable auto-play |
| `AutoPlayInterval` | TimeSpan | 3 seconds | Auto-play interval |
| `AnimationDuration` | double | 0.5 | Transition animation duration (seconds) |
| `ShowDots` | bool | true | Show dot indicators |
| `ShowArrows` | bool | true | Show left/right arrows |
| `ItemTemplate` | DataTemplate | null | Data template |
| `DisplayMemberPath` | string | null | Display member path (shows property value directly) |

### Events

| Event | Description |
|-------|-------------|
| `ItemClick` | Fired when a slide is clicked |
| `SelectedItemChanged` | Fired when selected item changes |

### Methods

| Method | Description |
|--------|-------------|
| `GoToNext()` | Navigate to next slide |
| `GoToPrevious()` | Navigate to previous slide |

---

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

