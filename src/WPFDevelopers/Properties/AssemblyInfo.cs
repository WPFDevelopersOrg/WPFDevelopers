using System.Windows;
using System.Windows.Markup;

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //主题特定资源词典所处位置
                                     //(未在页面中找到资源时使用，
                                     //或应用程序资源字典中找到时使用)
    ResourceDictionaryLocation.SourceAssembly //常规资源词典所处位置
                                              //(未在页面中找到资源时使用，
                                              //、应用程序或任何主题专用资源字典中找到时使用)
)]
[assembly: XmlnsPrefix("https://github.com/WPFDevelopersOrg/WPFDevelopers", "WPFDevelopers")]
[assembly: XmlnsDefinition("https://github.com/WPFDevelopersOrg/WPFDevelopers", "WPFDevelopers")]
[assembly: XmlnsDefinition("https://github.com/WPFDevelopersOrg/WPFDevelopers", "WPFDevelopers.Controls")]
[assembly: XmlnsDefinition("https://github.com/WPFDevelopersOrg/WPFDevelopers", "WPFDevelopers.Assists")]

[assembly: XmlnsPrefix("http://www.microsoft.net/controls", "Microsoft.Expression.Controls")]
[assembly: XmlnsDefinition("http://www.microsoft.net/controls", "Microsoft.Expression.Controls")]

[assembly: XmlnsPrefix("http://www.microsoft.net/drawing", "Microsoft.Expression.Drawing")]
[assembly: XmlnsDefinition("http://www.microsoft.net/drawing", "Microsoft.Expression.Controls")]
[assembly: XmlnsDefinition("http://www.microsoft.net/drawing", "Microsoft.Expression.Media")]
[assembly: XmlnsDefinition("http://www.microsoft.net/drawing", "Microsoft.Expression.Shapes")]

