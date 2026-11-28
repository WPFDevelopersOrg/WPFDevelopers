# wd:OtpBox

> 分类：[输入控件](../index.md#input-controls) · 可通过 WPFDevelopers NuGet 包使用。

OTP 验证码输入框（自动跳转/退格/粘贴）

## 示例源码

[OtpBoxExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/OtpBoxExample.xaml)

## 详细用法

`OtpBox` 是专为 OTP（一次性密码）输入场景设计的控件，支持自动聚焦跳转、退格回退、粘贴填充等功能。

### 基础用法

```xml
<wd:OtpBox Length="6" Completed="OtpBoxCompleted" />
```

### MVVM 绑定

```xml
<wd:OtpBox
    Length="6"
    Value="{Binding OtpCode, Mode=TwoWay}"
    CompletedCommand="{Binding VerifyCommand}" />
```

```csharp
// Event 方式
private void OtpBoxCompleted(object sender, RoutedEventArgs e)
{
    var otpBox = e.OriginalSource as OtpBox;
    var pwd = otpBox?.Value ?? string.Empty;

    if (pwd != _otpPassword)
    {
        myOtpBox.State = ControlState.Error;
        return;
    }

    myOtpBox.State = ControlState.Success;
}

// Command 方式
public ICommand CompletedCommand => new RelayCommand(param =>
{
    var pwd = param.ToString();
    // 验证逻辑...
});
```

### 主要属性

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `Length` | int | 4 | 验证码位数 |
| `Value` | string | "" | 当前输入值（支持双向绑定） |
| `State` | ControlState | None | 验证状态（None/Success/Error） |
| `CompletedCommand` | ICommand | null | 完成时执行的命令 |

### 事件

| 事件 | 说明 |
|------|------|
| `Completed` | 所有位数填写完成后触发 |

### 交互特性

| 操作 | 行为 |
|------|------|
| 输入数字 | 自动跳到下一个输入框 |
| 退格（空框） | 跳到上一个框并删除末尾字符 |
| Ctrl+V 粘贴 | 自动逐位填充，非数字字符过滤 |
| 方向键 ← → | 在输入框之间切换 |
| Enter / Tab | 跳到下一个输入框 |
| Error 状态 | 1.5 秒后自动清空并重新聚焦 |
| Success 状态 | 1.5 秒后自动恢复默认状态 |

---

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
