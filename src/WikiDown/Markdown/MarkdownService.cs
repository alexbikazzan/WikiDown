using System;

namespace WikiDown.Markdown
{
    public static class MarkdownService
    {
        public static string MakeHtml(string markdown)
        {
            return MakeHtml(markdown, ConverterHooksConfig.Default);
        }

        public static string MakeHtml(string markdown, ConverterHooksConfig converterHooksConfig)
        {
            if (markdown == null)
            {
                throw new ArgumentNullException("markdown");
            }
            if (converterHooksConfig == null)
            {
                throw new ArgumentNullException("converterHooksConfig");
            }

            string convertedMarkdown = converterHooksConfig.ApplyPreConversions(markdown);

            var markDown = new MarkdownSharp.Markdown();
            string html = markDown.Transform(convertedMarkdown);

            string convertedHtml = converterHooksConfig.ApplyPostConversions(html);
            return convertedHtml;
        }
    }
}