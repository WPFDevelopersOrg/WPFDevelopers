# Dialog

> 分类：[提示与消息](../index.md#feedback-messages) · 可通过 WPFDevelopers NuGet 包使用。

用于显示自定义内容的通用弹窗，支持输入校验和 MVVM 用法。

## 示例源码

[DialogExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Dialog/DialogExample.xaml)

## 详细用法

`Dialog` 用于显示自定义表单或确认内容，支持 `UserControl` 和 `Page`，自带遮罩、标题及确定/取消按钮。

### 打开弹窗与输入校验

```csharp
var editor = new UserEditor(); // 自定义 UserControl
var dialog = new WPFDevelopers.Controls.Dialog
{
    Title = "编辑用户",
    Content = editor,
    Owner = this,
    DialogWidth = 520
};
dialog.Closing += (sender, e) =>
{
    if (e.Reason == WPFDevelopers.Controls.DialogCloseReason.Confirm)
        e.Cancel = !editor.Validate(); // 业务校验失败时保持打开
};
if (await dialog.ShowAsync() == WPFDevelopers.Controls.DialogResult.Confirmed)
{
    Save(editor.DataContext);
}
```

`Validate()` 和 `Save()` 为业务方法，需自行实现。点击确定时，在 `Closing` 中校验输入，设置 `e.Cancel = true` 即可保持弹窗打开，并在内容中显示错误提示。WPF 绑定校验错误也会自动阻止确认。

### 常用配置

| 属性/方法 | 默认值 | 说明 |
|---|---|---|
| `Content` | null | 必填，使用未放入其他容器的 `UserControl` 或 `Page` |
| `Title` | 空字符串 | 标题 |
| `CornerRadius` | `10` | 圆角，设为 `new CornerRadius(0)` 使用直角 |
| `Owner` | null | 父窗口，未指定时自动查找当前窗口 |
| `DialogWidth` / `DialogHeight` | `440` / `double.NaN` | 宽高，`double.NaN` 表示自适应 |
| `ShowFooter` | true | 是否显示底部按钮 |
| `ConfirmText` / `CancelText` | null | 按钮文本，默认跟随语言设置 |
| `IsConfirmEnabled` | true | 可绑定确认按钮启用状态 |
| `IsOpen` | false | 双向绑定打开状态，false 请求取消 |
| `ClosedCommand` | null | 关闭后执行，参数为 `DialogResult` |
| `ShowAsync(owner = null)` | — | 打开弹窗，等待关闭结果：`Confirmed` 或 `Canceled` |
| `Close(reason)` | `Cancel` | 请求关闭，仍会触发 `Closing` 校验 |

- 在 UI 线程调用；同一父窗口只支持一个弹窗，打开期间不要更换 `Content`。
- 取消不会回滚已更新的 ViewModel；需要撤销编辑时，请使用数据副本。
- 自定义窗口模板需包含 `AdornerDecorator`。需要可拖动的独立窗口时，使用自定义 `Window` 配合 `MaskShowDialog()`。

上例使用 `await`（.NET 4.5+）。.NET 4.0 使用同一个 API，通过 UI 线程调度器处理结果：

```csharp
dialog.ShowAsync().ContinueWith(task =>
{
    if (task.Result == WPFDevelopers.Controls.DialogResult.Confirmed)
        Save(editor.DataContext);
}, TaskScheduler.FromCurrentSynchronizationContext());
```

### MVVM

将 Dialog 声明在 `Resources` 中，不要直接放入布局。显式指定 ViewModel（下例的 `DialogViewModel` 为已声明的资源）：

```xml
<wd:Dialog x:Key="EditorDialog"
           DataContext="{StaticResource DialogViewModel}"
           Title="编辑用户"
           IsOpen="{Binding IsDialogOpen, Mode=TwoWay}"
           IsConfirmEnabled="{Binding CanConfirm}"
           ClosedCommand="{Binding DialogClosedCommand}">
    <UserControl>
        <TextBox Text="{Binding Name, UpdateSourceTrigger=PropertyChanged}" />
    </UserControl>
</wd:Dialog>
```

打开按钮引用该 Dialog 的 DataContext：

```xml
<Button Content="打开"
        DataContext="{Binding DataContext, Source={StaticResource EditorDialog}}"
        Command="{Binding OpenDialogCommand}" />
```

`OpenDialogCommand` 将 `IsDialogOpen` 设为 true；`CanConfirm` 控制确定按钮是否可用。关闭后，`DialogClosedCommand` 接收 `DialogResult.Confirmed` 或 `Canceled`。

完整示例：[C# / MVVM](https://github.com/WPFDevelopersOrg/WPFDevelopers/tree/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Dialog)。

---

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
