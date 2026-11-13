using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using WPFDevelopers;

namespace WPFDevelopers.Samples
{
    public sealed class SampleLanguageManager
    {
        private static readonly Lazy<SampleLanguageManager> LazyInstance =
            new Lazy<SampleLanguageManager>(() => new SampleLanguageManager());
        private readonly ResourceManager _resourceManager;
        private readonly Dictionary<string, string> _resourceKeys =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly List<TranslationPattern> _translationPatterns = new List<TranslationPattern>();
        private readonly ConditionalWeakTable<DependencyObject, Dictionary<DependencyProperty, LocalizedValue>> _localizedValues =
            new ConditionalWeakTable<DependencyObject, Dictionary<DependencyProperty, LocalizedValue>>();
        private readonly List<WeakReference> _exampleViews = new List<WeakReference>();
        private bool _initialized;

        private SampleLanguageManager()
        {
            _resourceManager = new ResourceManager(
                "WPFDevelopers.Samples.Languages.SamplesLanguage",
                typeof(SampleLanguageManager).Assembly);
            LoadTranslationPatterns();
        }

        public static SampleLanguageManager Instance => LazyInstance.Value;

        public string TranslateText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            var key = Regex.Replace(text, @"\s+", " ").Trim();
            return GetTranslation(key, LanguageManager.Instance.CurrentCulture) ?? text;
        }

        public void SetLocalizedText(DependencyObject element, DependencyProperty property, string text)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (property == null)
                throw new ArgumentNullException(nameof(property));
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            var values = _localizedValues.GetOrCreateValue(element);
            if (!values.TryGetValue(property, out var value))
            {
                value = new LocalizedValue(text);
                values.Add(property, value);
            }

            value.OriginalValue = text;
            ApplyTranslation(element, property, value);
        }

        public void RegisterExampleView(FrameworkElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (_exampleViews.Exists(reference => ReferenceEquals(reference.Target, element)))
            {
                LocalizeTree(element);
                return;
            }

            _exampleViews.Add(new WeakReference(element));
            LocalizeTree(element);
        }

        public void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;
            LanguageManager.Instance.PropertyChanged += LanguageManager_PropertyChanged;
            EventManager.RegisterClassHandler(
                typeof(FrameworkElement),
                FrameworkElement.LoadedEvent,
                new RoutedEventHandler(FrameworkElement_Loaded),
                true);
        }

        private void FrameworkElement_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is FrameworkElement element))
            {
                return;
            }

            if (IsExampleView(element.GetType()))
            {
                RegisterExampleView(element);
                return;
            }

            if (!ReferenceEquals(sender, e.OriginalSource))
            {
                return;
            }

            if (!IsWithinExampleView(element))
            {
                return;
            }

            if (element is TextBlock)
            {
                LocalizeTree(element);
            }
            else
            {
                LocalizeElement(element);
            }
        }

        private static bool IsWithinExampleView(DependencyObject element)
        {
            var current = GetParent(element);
            while (current != null)
            {
                if (IsExampleView(current.GetType()))
                {
                    return true;
                }

                current = GetParent(current);
            }

            return false;
        }

        private static DependencyObject GetParent(DependencyObject element)
        {
            if (element is ContextMenu contextMenu && contextMenu.PlacementTarget != null)
            {
                return contextMenu.PlacementTarget;
            }

            if (element is ToolTip toolTip && toolTip.PlacementTarget != null)
            {
                return toolTip.PlacementTarget;
            }

            var parent = LogicalTreeHelper.GetParent(element);
            if (parent != null)
            {
                return parent;
            }

            return element is Visual || element is Visual3D
                ? VisualTreeHelper.GetParent(element)
                : null;
        }

        private void LanguageManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(LanguageManager.CurrentCulture))
            {
                return;
            }

            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null)
            {
                return;
            }

            if (dispatcher.CheckAccess())
            {
                LocalizeExampleViews();
            }
            else
            {
                dispatcher.BeginInvoke(new Action(LocalizeExampleViews), DispatcherPriority.DataBind);
            }
        }

        private void LocalizeExampleViews()
        {
            for (var index = _exampleViews.Count - 1; index >= 0; index--)
            {
                var reference = _exampleViews[index];
                var element = reference.Target as DependencyObject;
                if (element == null)
                {
                    _exampleViews.RemoveAt(index);
                    continue;
                }

                if (element is FrameworkElement frameworkElement && !frameworkElement.IsLoaded)
                {
                    continue;
                }

                LocalizeTree(element);
            }
        }

        public void LocalizeTree(DependencyObject root)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            var visited = new HashSet<DependencyObject>();
            var pending = new Stack<DependencyObject>();
            pending.Push(root);

            while (pending.Count > 0)
            {
                var element = pending.Pop();
                if (!visited.Add(element))
                {
                    continue;
                }

                LocalizeElement(element);

                if (element is FrameworkElement frameworkElement && frameworkElement.ContextMenu != null)
                {
                    pending.Push(frameworkElement.ContextMenu);
                }

                if (element is DataGrid dataGrid)
                {
                    foreach (var column in dataGrid.Columns)
                    {
                        LocalizeElement(column);
                    }
                }

                if (element is ListView listView && listView.View is GridView gridView)
                {
                    foreach (var column in gridView.Columns)
                    {
                        LocalizeElement(column);
                    }
                }

                // Plain TextBlock.Text owns its generated Run; translating both overwrites the cached source.
                if (element is TextBlock plainTextBlock &&
                    (plainTextBlock.ReadLocalValue(TextBlock.TextProperty) is string ||
                     BindingOperations.IsDataBound(plainTextBlock, TextBlock.TextProperty)))
                {
                    continue;
                }

                foreach (var child in LogicalTreeHelper.GetChildren(element))
                {
                    if (child is DependencyObject dependencyObject)
                    {
                        pending.Push(dependencyObject);
                    }
                }

                if (element is Visual || element is Visual3D)
                {
                    var childCount = VisualTreeHelper.GetChildrenCount(element);
                    for (var index = 0; index < childCount; index++)
                    {
                        pending.Push(VisualTreeHelper.GetChild(element, index));
                    }
                }

                if (element is TextBlock textBlock)
                {
                    foreach (Inline inline in textBlock.Inlines)
                    {
                        pending.Push(inline);
                    }
                }

                if (element is Span span)
                {
                    foreach (Inline inline in span.Inlines)
                    {
                        pending.Push(inline);
                    }
                }
            }
        }

        private void LocalizeElement(DependencyObject element)
        {
            var ancestor = element;
            while (ancestor != null)
            {
                if (ancestor is ComboBoxItem languageItem &&
                (string.Equals(languageItem.Tag as string, "zh-cn", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(languageItem.Tag as string, "en-US", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(languageItem.Tag as string, "ko-KR", StringComparison.OrdinalIgnoreCase)))
                {
                    return;
                }

                ancestor = GetParent(ancestor);
            }

            var localValues = element.GetLocalValueEnumerator();
            while (localValues.MoveNext())
            {
                var entry = localValues.Current;
                if ((entry.Value is string || BindingOperations.IsDataBound(element, entry.Property)) &&
                    IsTextProperty(entry.Property.Name) &&
                    !(entry.Property.Name == "Text" &&
                      (element is TextBoxBase || element is ComboBox)))
                {
                    LocalizeProperty(element, entry.Property);
                }
            }
        }

        private static bool IsTextProperty(string propertyName)
        {
            switch (propertyName)
            {
                case "Content":
                case "Description":
                case "Format":
                case "Header":
                case "HelpText":
                case "Hint":
                case "Label":
                case "Message":
                case "Name":
                case "Placeholder":
                case "Prompt":
                case "Text":
                case "Title":
                case "ToolTip":
                case "Watermark":
                    return true;
                default:
                    return false;
            }
        }

        private void LocalizeProperty(DependencyObject element, DependencyProperty property)
        {
            var localValue = element.ReadLocalValue(property);
            var isDataBound = BindingOperations.IsDataBound(element, property);
            var source = localValue as string;
            if (source == null && isDataBound)
            {
                source = element.GetValue(property) as string;
            }

            if (source == null)
            {
                return;
            }

            var localizedValues = _localizedValues.GetOrCreateValue(element);
            if (!localizedValues.TryGetValue(property, out var localizedValue))
            {
                localizedValue = new LocalizedValue(source);
                localizedValues.Add(property, localizedValue);
            }
            else if (!string.Equals(source, localizedValue.AppliedValue, StringComparison.Ordinal))
            {
                localizedValue.OriginalValue = source;
            }

            ApplyTranslation(element, property, localizedValue);
        }

        private void ApplyTranslation(
            DependencyObject element,
            DependencyProperty property,
            LocalizedValue localizedValue)
        {
            if (localizedValue.PropertyDescriptor == null)
            {
                AttachValueChanged(element, property, localizedValue);
            }

            var key = Regex.Replace(localizedValue.OriginalValue, @"\s+", " ").Trim();
            var translation = GetTranslation(key, LanguageManager.Instance.CurrentCulture);
            localizedValue.AppliedValue = translation ?? localizedValue.OriginalValue;
            localizedValue.IsApplyingTranslation = true;
            try
            {
                if (BindingOperations.IsDataBound(element, property))
                {
                    element.SetCurrentValue(property, localizedValue.AppliedValue);
                }
                else
                {
                    element.SetValue(property, localizedValue.AppliedValue);
                }
            }
            finally
            {
                localizedValue.IsApplyingTranslation = false;
            }
        }

        private void AttachValueChanged(
            DependencyObject element,
            DependencyProperty property,
            LocalizedValue localizedValue)
        {
            var descriptor = DependencyPropertyDescriptor.FromProperty(property, element.GetType());
            if (descriptor == null)
            {
                return;
            }

            EventHandler handler = (sender, args) =>
            {
                if (!localizedValue.IsApplyingTranslation &&
                    !string.Equals(
                        element.GetValue(property) as string,
                        localizedValue.AppliedValue,
                        StringComparison.Ordinal))
                {
                    LocalizeProperty(element, property);
                }
            };

            descriptor.AddValueChanged(element, handler);
            localizedValue.PropertyDescriptor = descriptor;
            localizedValue.ValueChangedHandler = handler;
            if (element is FrameworkElement frameworkElement)
            {
                RoutedEventHandler unloadedHandler = null;
                unloadedHandler = (sender, args) =>
                {
                    frameworkElement.Unloaded -= unloadedHandler;
                    DetachValueChanged(element, localizedValue);
                };
                frameworkElement.Unloaded += unloadedHandler;
            }
        }

        private static void DetachValueChanged(
            DependencyObject element,
            LocalizedValue localizedValue)
        {
            if (localizedValue.PropertyDescriptor != null && localizedValue.ValueChangedHandler != null)
            {
                localizedValue.PropertyDescriptor.RemoveValueChanged(element, localizedValue.ValueChangedHandler);
                localizedValue.PropertyDescriptor = null;
                localizedValue.ValueChangedHandler = null;
            }
        }

        private void LoadTranslationPatterns()
        {
            var resources = _resourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, false);
            if (resources == null)
            {
                throw new MissingManifestResourceException(
                    "The neutral sample-language resource was not embedded.");
            }

            foreach (DictionaryEntry entry in resources)
            {
                if (!(entry.Key is string key))
                {
                    continue;
                }

                _resourceKeys.Add(key, key);

                var placeholders = Regex.Matches(key, @"\{[^{}]+\}");
                if (placeholders.Count == 0)
                {
                    continue;
                }

                var pattern = new StringBuilder("^");
                var offset = 0;
                foreach (Match placeholder in placeholders)
                {
                    pattern.Append(Regex.Escape(key.Substring(offset, placeholder.Index - offset)));
                    pattern.Append("(.*?)");
                    offset = placeholder.Index + placeholder.Length;
                }

                pattern.Append(Regex.Escape(key.Substring(offset)));
                pattern.Append("$");
                _translationPatterns.Add(new TranslationPattern(
                    key,
                    new Regex(pattern.ToString(), RegexOptions.CultureInvariant)));
            }

            _translationPatterns.Sort((left, right) => right.Key.Length.CompareTo(left.Key.Length));
        }

        private string GetTranslation(string key, CultureInfo culture)
        {
            var resourceKey = _resourceKeys.TryGetValue(key, out var canonicalKey)
                ? canonicalKey
                : key;
            var translation = _resourceManager.GetString(resourceKey, culture);
            if (string.Equals(key, "new", StringComparison.Ordinal) &&
                culture.TwoLetterISOLanguageName == "en" &&
                string.Equals(translation, "NEW", StringComparison.Ordinal))
            {
                translation = "New";
            }

            if (translation != null)
            {
                return translation;
            }

            foreach (var pattern in _translationPatterns)
            {
                var match = pattern.Regex.Match(key);
                if (!match.Success)
                {
                    continue;
                }

                translation = _resourceManager.GetString(pattern.Key, culture);
                if (translation == null)
                {
                    continue;
                }

                var placeholders = Regex.Matches(translation, @"\{[^{}]+\}");
                if (placeholders.Count != match.Groups.Count - 1)
                {
                    continue;
                }

                for (var index = placeholders.Count - 1; index >= 0; index--)
                {
                    var placeholder = placeholders[index];
                    translation = translation.Remove(placeholder.Index, placeholder.Length)
                        .Insert(placeholder.Index, match.Groups[index + 1].Value);
                }

                return translation;
            }

            return null;
        }

        private static bool IsExampleView(Type type)
        {
            var typeNamespace = type.Namespace;
            return IsInExampleNamespace(typeNamespace, "WPFDevelopers.Samples.ExampleViews") ||
                IsInExampleNamespace(typeNamespace, "WPFDevelopers.Sample.ExampleViews");
        }

        private static bool IsInExampleNamespace(string typeNamespace, string exampleNamespace)
        {
            return typeNamespace == exampleNamespace ||
                (typeNamespace != null && typeNamespace.StartsWith(
                    exampleNamespace + ".",
                    StringComparison.Ordinal));
        }

        private sealed class LocalizedValue
        {
            public LocalizedValue(string originalValue)
            {
                OriginalValue = originalValue;
                AppliedValue = originalValue;
            }

            public string OriginalValue { get; set; }
            public string AppliedValue { get; set; }
            public bool IsApplyingTranslation { get; set; }
            public DependencyPropertyDescriptor PropertyDescriptor { get; set; }
            public EventHandler ValueChangedHandler { get; set; }
        }

        private sealed class TranslationPattern
        {
            public TranslationPattern(string key, Regex regex)
            {
                Key = key;
                Regex = regex;
            }

            public string Key { get; }
            public Regex Regex { get; }
        }
    }
}
