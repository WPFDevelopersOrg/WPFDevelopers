param($installPath, $toolsPath, $package, $project)

$ErrorActionPreference = 'Stop'
$presentationNamespace = 'http://schemas.microsoft.com/winfx/2006/xaml/presentation'
$xamlNamespace = 'http://schemas.microsoft.com/winfx/2006/xaml'
$wdNamespace = 'https://github.com/WPFDevelopersOrg/WPFDevelopers'
$wdClrNamespace = 'clr-namespace:WPFDevelopers;assembly=WPFDevelopers'
$themeSource = 'pack://application:,,,/WPFDevelopers;component/Themes/Theme.xaml'
$relativeThemeSource = '/WPFDevelopers;component/Themes/Theme.xaml'

function Get-ChildElementByName($parent, [string]$name, [string]$namespace) {
    $matches = @($parent.ChildNodes | Where-Object {
        $_ -is [System.Xml.XmlElement] -and $_.LocalName -eq $name -and $_.NamespaceURI -eq $namespace
    })
    if ($matches.Count -gt 1) {
        throw "Multiple '$name' elements were found in '$($parent.Name)'. App.xaml was not changed."
    }
    if ($matches.Count -eq 1) { return $matches[0] }
}

function Add-FirstElement($parent, $element) {
    $depth = 0
    for ($node = $parent; $node -is [System.Xml.XmlElement]; $node = $node.ParentNode) { $depth++ }
    $indent = ' ' * (4 * $depth)
    $parentIndent = ' ' * (4 * ($depth - 1))
    $first = $null
    foreach ($child in $parent.ChildNodes) {
        if ($child -is [System.Xml.XmlElement]) { $first = $child; break }
    }
    if ($null -ne $first) {
        [void]$parent.InsertBefore($element, $first)
        [void]$parent.InsertBefore($xml.CreateWhitespace($newline + $indent), $first)
    }
    else {
        $last = $parent.LastChild
        if ($last -is [System.Xml.XmlWhitespace] -and $last.Value -match '[\r\n]') {
            [void]$parent.InsertBefore($xml.CreateWhitespace($newline + $indent), $last)
            [void]$parent.InsertBefore($element, $last)
        }
        else {
            [void]$parent.AppendChild($xml.CreateWhitespace($newline + $indent))
            [void]$parent.AppendChild($element)
            [void]$parent.AppendChild($xml.CreateWhitespace($newline + $parentIndent))
        }
    }
}

function Insert-AfterElement($parent, $element, $previous) {
    $depth = 0
    for ($node = $parent; $node -is [System.Xml.XmlElement]; $node = $node.ParentNode) { $depth++ }
    $whitespace = $xml.CreateWhitespace($newline + (' ' * (4 * $depth)))
    [void]$parent.InsertAfter($whitespace, $previous)
    [void]$parent.InsertAfter($element, $whitespace)
}

function Find-AppItems($items) {
    foreach ($item in $items) {
        if ($item.Name -ieq 'App.xaml') { $item }
        if ($null -ne $item.ProjectItems) { Find-AppItems $item.ProjectItems }
    }
}

if ($null -eq $project) {
    throw 'WPFDevelopers could not access the project. App.xaml was not changed.'
}

try {
    $properties = $project.Properties
    if ($null -eq $properties) {
        Write-Warning 'WPFDevelopers could not determine the project output type. App.xaml was not changed.'
        return
    }
    $outputProperty = $properties.Item('OutputType')
    $outputType = if ($null -ne $outputProperty) { [string]$outputProperty.Value } else { '' }
}
catch [System.Runtime.InteropServices.COMException] {
    Write-Warning "WPFDevelopers could not determine the project output type: $($_.Exception.Message). App.xaml was not changed."
    return
}
catch [System.Management.Automation.MethodInvocationException] {
    Write-Warning "WPFDevelopers could not determine the project output type: $($_.Exception.Message). App.xaml was not changed."
    return
}

# Visual Studio's prjOutputType enum uses 0 for WinExe, 1 for Exe, and 2 for Library.
if (@('2', 'Library', 'prjOutputTypeLibrary') -contains $outputType) {
    Write-Host 'WPFDevelopers skipped App.xaml configuration for a library project.'
    return
}
if (@('0', '1', 'WinExe', 'Exe', 'prjOutputTypeWinExe', 'prjOutputTypeExe') -notcontains $outputType) {
    Write-Warning "WPFDevelopers cannot identify project output type '$outputType' as an application. App.xaml was not changed."
    return
}

$appItems = @(Find-AppItems $project.ProjectItems)
if ($appItems.Count -eq 0) {
    Write-Warning 'WPFDevelopers did not find App.xaml in this project; application resources were not configured.'
    return
}
if ($appItems.Count -ne 1) {
    throw 'WPFDevelopers found multiple App.xaml project items. Configure the application resources manually.'
}

$item = $appItems[0]
$path = $item.FileNames(1)
if ([string]::IsNullOrWhiteSpace($path) -or -not [System.IO.File]::Exists($path)) {
    throw "WPFDevelopers could not find App.xaml at '$path'."
}
if ($null -ne $item.Document -and -not $item.Document.Saved) {
    throw "Save or close '$path' before installing WPFDevelopers; unsaved editor changes were not overwritten."
}

$originalBytes = [System.IO.File]::ReadAllBytes($path)
$reader = New-Object System.IO.StreamReader($path, $true)
try {
    $content = $reader.ReadToEnd()
    $encoding = $reader.CurrentEncoding
}
finally {
    $reader.Dispose()
}
$newline = if ($content.Contains("`r`n")) { "`r`n" } else { "`n" }

$xml = New-Object System.Xml.XmlDocument
$xml.PreserveWhitespace = $true
$xml.XmlResolver = $null
$settings = New-Object System.Xml.XmlReaderSettings
$settings.DtdProcessing = [System.Xml.DtdProcessing]::Prohibit
$settings.XmlResolver = $null
$xmlReader = [System.Xml.XmlReader]::Create($path, $settings)
try {
    $xml.Load($xmlReader)
}
finally {
    $xmlReader.Dispose()
}

$root = $xml.DocumentElement
if ($null -eq $root -or $root.LocalName -ne 'Application' -or $root.NamespaceURI -ne $presentationNamespace) {
    throw "'$path' is not a WPF Application XAML file. It was not changed."
}

$changed = $false
$resources = Get-ChildElementByName $root 'Application.Resources' $presentationNamespace
if ($null -eq $resources) {
    $resources = $xml.CreateElement('Application.Resources', $presentationNamespace)
    Add-FirstElement $root $resources
    $changed = $true
}

$elements = @($resources.ChildNodes | Where-Object { $_ -is [System.Xml.XmlElement] })
$dictionary = $null
foreach ($element in $elements) {
    if ($element.LocalName -eq 'ResourceDictionary' -and $element.NamespaceURI -eq $presentationNamespace -and
        -not $element.HasAttribute('Key', $xamlNamespace)) {
        if ($null -ne $dictionary -or $elements.Count -ne 1) {
            throw "'$path' has an unsupported Application.Resources structure. It was not changed."
        }
        $dictionary = $element
    }
}
if ($null -eq $dictionary) {
    $dictionary = $xml.CreateElement('ResourceDictionary', $presentationNamespace)
    while ($resources.HasChildNodes) {
        [void]$dictionary.AppendChild($resources.FirstChild)
    }
    foreach ($child in $dictionary.ChildNodes) {
        if ($child -is [System.Xml.XmlWhitespace] -and $child.Value -match '(\r?\n)([ \t]*)$') {
            $child.Value = $child.Value + '    '
        }
    }
    Add-FirstElement $resources $dictionary
    $changed = $true
}
elseif ($dictionary.HasAttribute('Source')) {
    if (@($dictionary.ChildNodes | Where-Object { $_ -is [System.Xml.XmlElement] }).Count -ne 0) {
        throw "'$path' uses a ResourceDictionary Source with child elements. Configure it manually."
    }
    $wrapper = $xml.CreateElement('ResourceDictionary', $presentationNamespace)
    [void]$resources.ReplaceChild($wrapper, $dictionary)
    $merged = $xml.CreateElement('ResourceDictionary.MergedDictionaries', $presentationNamespace)
    Add-FirstElement $wrapper $merged
    Add-FirstElement $merged $dictionary
    $dictionary = $wrapper
    $changed = $true
}

$merged = Get-ChildElementByName $dictionary 'ResourceDictionary.MergedDictionaries' $presentationNamespace
if ($null -eq $merged) {
    $merged = $xml.CreateElement('ResourceDictionary.MergedDictionaries', $presentationNamespace)
    Add-FirstElement $dictionary $merged
    $changed = $true
}

$theme = $null
$wdResources = $null
foreach ($element in $merged.ChildNodes) {
    if ($element -isnot [System.Xml.XmlElement]) { continue }
    if ($element.LocalName -eq 'ResourceDictionary' -and $element.NamespaceURI -eq $presentationNamespace -and
        ($element.GetAttribute('Source') -ieq $themeSource -or $element.GetAttribute('Source') -ieq $relativeThemeSource)) {
        if ($null -eq $theme) { $theme = $element }
        else { Write-Warning "Multiple WD theme dictionaries exist in '$path'; they were preserved." }
    }
    if ($element.LocalName -eq 'Resources' -and
        ($element.NamespaceURI -eq $wdNamespace -or $element.NamespaceURI -eq $wdClrNamespace)) {
        if ($null -eq $wdResources) { $wdResources = $element }
        else { Write-Warning "Multiple WD Resources elements exist in '$path'; they were preserved." }
    }
}

if ($null -eq $theme) {
    $theme = $xml.CreateElement('ResourceDictionary', $presentationNamespace)
    $theme.SetAttribute('Source', $themeSource)
    Add-FirstElement $merged $theme
    $changed = $true
}
if ($null -eq $wdResources) {
    $prefix = $root.GetPrefixOfNamespace($wdNamespace)
    if ([string]::IsNullOrEmpty($prefix)) {
        $prefix = 'wd'
        $suffix = 0
        while (-not [string]::IsNullOrEmpty($root.GetNamespaceOfPrefix($prefix))) {
            $suffix++
            $prefix = 'wd' + $suffix
        }
        $declaration = $xml.CreateAttribute('xmlns', $prefix, 'http://www.w3.org/2000/xmlns/')
        $declaration.Value = $wdNamespace
        [void]$root.Attributes.Append($declaration)
    }
    $wdResources = $xml.CreateElement($prefix, 'Resources', $wdNamespace)
    $wdResources.SetAttribute('Radius', '4')
    Insert-AfterElement $merged $wdResources $theme
    $changed = $true
}

$themeSeen = $false
foreach ($element in $merged.ChildNodes) {
    if ($element -eq $theme) { $themeSeen = $true }
    if ($element -eq $wdResources) {
        if (-not $themeSeen) {
            Write-Warning "WD Resources precedes Theme.xaml in '$path'; existing nodes were not moved."
        }
        break
    }
}

if (-not $changed) {
    Write-Host "WPFDevelopers: application resources already exist in '$path'."
    return
}

$bytes = $originalBytes
$hasUtf8Bom = $bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF
$declaration = $xml.FirstChild -as [System.Xml.XmlDeclaration]
if ($null -ne $declaration -and -not [string]::IsNullOrWhiteSpace($declaration.Encoding)) {
    $encoding = [System.Text.Encoding]::GetEncoding($declaration.Encoding)
}
if ($encoding.CodePage -eq 65001) {
    $encoding = New-Object System.Text.UTF8Encoding($hasUtf8Bom)
}
elseif ($encoding.CodePage -eq 1200 -or $encoding.CodePage -eq 1201) {
    $hasUtf16Bom = $bytes.Length -ge 2 -and
        (($bytes[0] -eq 0xFF -and $bytes[1] -eq 0xFE) -or ($bytes[0] -eq 0xFE -and $bytes[1] -eq 0xFF))
    $encoding = New-Object System.Text.UnicodeEncoding(($encoding.CodePage -eq 1201), $hasUtf16Bom)
}
$writerSettings = New-Object System.Xml.XmlWriterSettings
$writerSettings.Encoding = $encoding
$writerSettings.Indent = $true
$writerSettings.NewLineHandling = [System.Xml.NewLineHandling]::None
$temporaryPath = Join-Path ([System.IO.Path]::GetDirectoryName($path)) ('.' + [System.IO.Path]::GetFileName($path) + '.' + [Guid]::NewGuid().ToString('N') + '.tmp')
try {
    $writer = [System.Xml.XmlWriter]::Create($temporaryPath, $writerSettings)
    try { $xml.Save($writer) }
    finally { $writer.Dispose() }
    if ([Convert]::ToBase64String([System.IO.File]::ReadAllBytes($path)) -ne [Convert]::ToBase64String($originalBytes)) {
        throw "'$path' changed during installation. Its newer contents were not overwritten."
    }
    [System.IO.File]::Replace($temporaryPath, $path, [System.Management.Automation.Language.NullString]::Value)
}
finally {
    if ([System.IO.File]::Exists($temporaryPath)) { [System.IO.File]::Delete($temporaryPath) }
}

Write-Host "WPFDevelopers configured application resources in '$path'."
if ($null -ne $item.Document) {
    Write-Warning "Reload '$path' in Visual Studio if prompted to see the updated contents."
}
