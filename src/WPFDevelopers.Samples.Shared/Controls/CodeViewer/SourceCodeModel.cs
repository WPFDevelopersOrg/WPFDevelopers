namespace WPFDevelopers.Samples.Controls
{
    public class SourceCodeModel
    {
        public CodeType CodeType { get; set; }
        public string Haader { get; set; }

        public string CodeSource { get; set; }

    }
    public enum CodeType
    {
        Xaml,
        CSharp,
    }
}
