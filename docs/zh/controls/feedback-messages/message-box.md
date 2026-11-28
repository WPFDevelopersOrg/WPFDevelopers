# MessageBox

> 分类：[提示与消息](../index.md#feedback-messages) · 可通过 WPFDevelopers NuGet 包使用。

消息对话框

## 示例源码

[MessageBoxExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/MessageBoxExample.xaml)

## 详细用法

`MessageBox` 是 WPFDevelopers 提供的消息对话框静态类，用于替代 `System.Windows.MessageBox`，提供更美观、更可定制的消息弹窗。

> **注意**：`MessageBox` 是一个 **静态类**，不是 XAML 控件，只能从 C# 代码中调用。

### 引入命名空间

为避免与 `System.Windows.MessageBox` 冲突，建议使用别名引入：

```csharp
using MessageBox = WPFDevelopers.Controls.MessageBox;
```

### 方法签名

`MessageBox.Show()` 提供 5 个重载方法：

```csharp
// 1. 仅消息文本（默认 OK 按钮，无图标）
MessageBoxResult Show(string messageBoxText, Window owner = null, bool isDefault = true)

// 2. 消息文本 + 标题
MessageBoxResult Show(string messageBoxText, string caption, Window owner = null, bool isDefault = true)

// 3. 消息文本 + 标题 + 按钮组合
MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, Window owner = null, bool isDefault = true)

// 4. 消息文本 + 标题 + 图标
MessageBoxResult Show(string messageBoxText, string caption, MessageBoxImage icon, Window owner = null, bool isDefault = true)

// 5. 消息文本 + 标题 + 按钮组合 + 图标（完整参数）
MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, Window owner = null, bool isDefault = true)
```

### 参数说明

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `messageBoxText` | string | — | 消息内容 |
| `caption` | string | — | 对话框标题 |
| `button` | MessageBoxButton | OK | 按钮组合：`OK` / `OKCancel` / `YesNo` / `YesNoCancel` |
| `icon` | MessageBoxImage | 无 | 图标类型：`Information` / `Warning` / `Error` / `Question` |
| `owner` | Window | null | 父窗口。传入后对话框居中显示并带遮罩层 |
| `isDefault` | bool | true | 是否将第一个按钮设为默认按钮（Enter 键触发） |

### 图标与配色映射

| MessageBoxImage | 图标 | 配色 |
|-----------------|------|------|
| `Information` | 信息图标 | 成功色（绿色） |
| `Warning` | 警告图标 | 警告色（橙色） |
| `Error` | 错误图标 | 危险色（红色） |
| `Question` | 问号图标 | 主题色（蓝色） |

### 使用示例

### 1. 信息提示对话框

```csharp
// 文件删除成功提示
MessageBox.Show("文件删除成功。", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
```

### 2. 警告对话框

```csharp
// 使用默认 OK 按钮
MessageBox.Show("执行此操作可能导致文件无法打开！", "警告", MessageBoxImage.Warning);
```

### 3. 错误对话框

```csharp
MessageBox.Show("当前文件不存在。", "错误", MessageBoxImage.Error);
```

### 4. 确认对话框（带返回值处理）

```csharp
var result = MessageBox.Show("当前文件不存在，是否继续？", "询问", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

switch (result)
{
    case MessageBoxResult.Yes:
        // 用户点击"是"
        break;
    case MessageBoxResult.No:
        // 用户点击"否"
        break;
    case MessageBoxResult.Cancel:
        // 用户点击"取消"或关闭
        break;
}
```

### 5. 指定父窗口（带遮罩层）

```csharp
// 传入 owner 参数后，对话框会显示在父窗口中央并覆盖遮罩层
MessageBox.Show("操作成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information, owner: this);
```

### 交互行为

- **关闭方式**：点击右上角关闭按钮、按 `Escape` 键、点击按钮
- **父窗口**：传入 `owner` 时对话框居中于父窗口并显示遮罩；不传入时自动检测当前窗口或屏幕居中
- **按钮文本**：自动跟随系统语言本地化（通过 `LanguageManager`）

---

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
