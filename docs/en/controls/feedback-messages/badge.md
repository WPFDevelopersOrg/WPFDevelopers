# wd:Badge (attached property)

> Category: [Notifications & Messages](../index.md#feedback-messages) · Available in the WPFDevelopers NuGet package.

Badge notification

## Example Source

[BadgeExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/BadgeExample.xaml)

## XAML Usage

```xml
<Button
                    wd:Badge.IsShow="{Binding ElementName=MyBadgeToggleButton, Path=IsChecked}"
                    wd:Badge.Text="new"
                    Content="Default"
                    Style="{DynamicResource WD.PrimaryButton}" />
```

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

