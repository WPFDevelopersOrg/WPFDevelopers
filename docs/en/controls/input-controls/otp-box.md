# wd:OtpBox

> Category: [Input Controls](../index.md#input-controls) · Available in the WPFDevelopers NuGet package.

OTP verification code input (auto-jump / backspace / paste)

## Example Source

[OtpBoxExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/OtpBoxExample.xaml)

## Detailed Usage

`OtpBox` is a control designed for OTP (One-Time Password) input scenarios, supporting auto-focus jump, backspace retreat, paste fill, and more.

### Basic Usage

```xml
<wd:OtpBox Length="6" Completed="OtpBoxCompleted" />
```

### MVVM Binding

```xml
<wd:OtpBox
    Length="6"
    Value="{Binding OtpCode, Mode=TwoWay}"
    CompletedCommand="{Binding VerifyCommand}" />
```

```csharp
// Event approach
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

// Command approach
public ICommand CompletedCommand => new RelayCommand(param =>
{
    var pwd = param.ToString();
    // Validation logic...
});
```

### Key Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Length` | int | 4 | Number of OTP digits |
| `Value` | string | "" | Current value (supports two-way binding) |
| `State` | ControlState | None | Validation state (None/Success/Error) |
| `CompletedCommand` | ICommand | null | Command executed on completion |

### Events

| Event | Description |
|-------|-------------|
| `Completed` | Fired when all digits are filled |

### Interaction Features

| Action | Behavior |
|--------|----------|
| Enter a digit | Auto-jump to next input box |
| Backspace (empty box) | Jump to previous box and delete last character |
| Ctrl+V Paste | Auto-fill digit by digit, filter non-numeric characters |
| Arrow keys ← → | Switch between input boxes |
| Enter / Tab | Jump to next input box |
| Error state | Auto-clear and refocus after 1.5 seconds |
| Success state | Auto-reset to default state after 1.5 seconds |

---

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

