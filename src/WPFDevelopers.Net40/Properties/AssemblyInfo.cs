using System.Windows;
using System.Windows.Markup;

[assembly:ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page,
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page,
                                              // app, or any theme specific resource dictionaries)
)]
[assembly: XmlnsPrefix("https://github.com/WPFDevelopersOrg/WPFDevelopers", "WPFDevelopers")]
[assembly: XmlnsDefinition("https://github.com/WPFDevelopersOrg/WPFDevelopers", "WPFDevelopers")]
[assembly: XmlnsDefinition("https://github.com/WPFDevelopersOrg/WPFDevelopers", "WPFDevelopers.Converts")]
[assembly: XmlnsDefinition("https://github.com/WPFDevelopersOrg/WPFDevelopers", "WPFDevelopers.Helpers")]
[assembly: XmlnsDefinition("https://github.com/WPFDevelopersOrg/WPFDevelopers", "WPFDevelopers.Net40")]
[assembly: XmlnsDefinition("https://github.com/WPFDevelopersOrg/WPFDevelopers", "WPFDevelopers.Controls")]
[assembly: XmlnsDefinition("https://github.com/WPFDevelopersOrg/WPFDevelopers", "Microsoft.Windows.Shell")]
[assembly: XmlnsDefinition("https://github.com/WPFDevelopersOrg/WPFDevelopers", "WPFDevelopers.Assists")]

[assembly: XmlnsPrefix("http://www.microsoft.net/controls", "Microsoft.Expression.Controls")]
[assembly: XmlnsDefinition("http://www.microsoft.net/controls", "Microsoft.Expression.Controls")]

[assembly: XmlnsPrefix("http://www.microsoft.net/drawing", "Microsoft.Expression.Drawing")]
[assembly: XmlnsDefinition("http://www.microsoft.net/drawing", "Microsoft.Expression.Drawing.Controls")]
[assembly: XmlnsDefinition("http://www.microsoft.net/drawing", "Microsoft.Expression.Drawing.Media")]
[assembly: XmlnsDefinition("http://www.microsoft.net/drawing", "Microsoft.Expression.Drawing.Shapes")]