# wd:ColorPicker

> 分类：[输入控件](../index.md#input-controls) · 可通过 WPFDevelopers NuGet 包使用。

颜色选择器

## 示例源码

[ColorPickerExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ColorPickerExample.xaml)

## 详细用法

`ColorPicker` 保留内嵌面板用法，底部下拉框可直接选择 RGB、HSL 或 HEX，支持通过 `ColorType` 设置初始格式。

```xml
<wd:ColorPicker SelectedColor="{Binding AccentColor}" ColorType="HEX" />
```

同一个 `ColorPicker` 设置 `DisplayMode="Popup"` 后，提供点击色块打开面板的用法；默认 `DisplayMode="Inline"` 保留现有内嵌展示：

```xml
<wd:ColorPicker DisplayMode="Popup" SelectedColor="{Binding AccentColor}" />
<wd:ColorPicker DisplayMode="Popup" SelectedColor="{Binding AccentColor}"
                     ShowColorText="True"
                     SwatchSize="24"
                     ColorType="HEX" />
```

- `SelectedColor` 为 `Color`，默认双向绑定，修改颜色时立即同步，无需确认。
- `ShowColorText` 默认为 `False`；启用后在色块右侧显示 HEX，不透明颜色为 `#RRGGBB`，带透明度颜色为 `#AARRGGBB`。
- `SwatchSize` 为色块边长，默认为 18，可设置为 24、32 等正数；触发器总尺寸还包含 `Padding` 和边框。
- `IsDropDownOpen` 支持双向绑定；点击外部或按 Esc 关闭，F4 或 Alt+下箭头切换打开状态，也可通过聚焦触发器后按空格打开。
- 鼠标点击触发器时，在松开后展开面板，以保证外部点击关闭所需的鼠标捕获；切换格式后仍可点击外部关闭。
- `ShowColorText`、`SwatchSize` 和 `IsDropDownOpen` 用于 Popup 模式，Inline 模式不会打开弹窗。运行时切换 `DisplayMode` 会关闭弹窗并保留颜色和格式。
- `SelectedColorChanged` 为颜色改变时触发的冒泡事件。两种模式共用取色逻辑及主题资源，控件的 `Width`、`Height` 和 `Padding` 在 Popup 模式下设置触发器尺寸，不影响内部面板。

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
