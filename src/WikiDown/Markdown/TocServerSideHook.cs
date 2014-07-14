namespace WikiDown.Markdown
{
    public class TocServerSideHook : ConverterHook
    {
        private const string DisplayRegexPattern = "{TOC}";

        private const string DisplayRegexReplace = "<pre><code>[Table of Content]</code></pre>";

        public TocServerSideHook()
            : base(DisplayRegexPattern, DisplayRegexReplace, ConverterHookType.ServerSide)
        {
        }

        public override string Apply(string input)
        {
            // TODO: User HtmlAgility-pack
            return input;
        }
    }
}