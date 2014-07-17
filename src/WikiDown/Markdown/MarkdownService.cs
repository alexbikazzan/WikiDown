using System;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

namespace WikiDown.Markdown
{
    public static class MarkdownService
    {
        private static readonly MarkdownSharp.Markdown MarkdownConverter = new MarkdownSharp.Markdown();

        private static readonly Regex ServerSideRegexRemove = new Regex(
            @"^((?:<[^>]+>)?@{[^}]+}(</[^>]+>)?)",
            RegexOptions.Multiline);

        private static readonly Regex FlattenNewlinesRegex = new Regex(@"\r\n\t|[\s\s]+", RegexOptions.Multiline);

        private static readonly Regex HtmlNewlineRegex = new Regex(@"[\s]{2,}", RegexOptions.Multiline);

        private static readonly Regex HtmlParagraphsRegex = new Regex(@"[\s]{2,}", RegexOptions.Multiline);

        private static readonly Regex TrimLinebreaksRegex = new Regex(@"[\s]{3,}", RegexOptions.Multiline);

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

            string html = MarkdownConverter.Transform(convertedMarkdown);

            string convertedHtml = converterHooksConfig.ApplyPostConversions(html);
            return convertedHtml;
        }

        public static string MakeText(string markdown)
        {
            return MakeText(markdown, ConverterHooksConfig.Default);
        }

        public static string MakeText(string markdown, ConverterHooksConfig converterHooksConfig)
        {
            var trimmedMarkdown = TrimServerSideMarkdown(markdown);

            var html = MakeHtml(trimmedMarkdown, converterHooksConfig);

            var stripped = GetDocumentText(html);
            return stripped;
        }

        public static string MakeTextHtmlLinebreaks(string markdown)
        {
            return MakeTextHtmlLinebreaks(markdown, ConverterHooksConfig.Default);
        }

        public static string MakeTextHtmlLinebreaks(string markdown, ConverterHooksConfig converterHooksConfig)
        {
            var text = MakeText(markdown, converterHooksConfig);

            string htmlLinebreaksText = ReplaceWithHtmlLinebreaks(text);
            return htmlLinebreaksText;
        }

        private static string ReplaceWithHtmlLinebreaks(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            string htmlLinebreaksText = HtmlParagraphsRegex.Replace(text, "</p><p>");
            htmlLinebreaksText = HtmlNewlineRegex.Replace(htmlLinebreaksText, "<br/>");

            return string.Format("<p>{0}</p>", htmlLinebreaksText);
        }

        public static string MakeTextFlat(string markdown)
        {
            return MakeTextFlat(markdown, ConverterHooksConfig.Default);
        }

        public static string MakeTextFlat(string markdown, ConverterHooksConfig converterHooksConfig)
        {
            var text = MakeText(markdown, converterHooksConfig);

            string flattened = FlattenNewlinesRegex.Replace(text, " ");
            return flattened;
        }

        private static string TrimServerSideMarkdown(string markdown)
        {
            return !string.IsNullOrWhiteSpace(markdown)
                       ? ServerSideRegexRemove.Replace(markdown, string.Empty)
                       : markdown;
        }

        private static string GetDocumentText(string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);

            string documentText = document.DocumentNode.InnerText ?? string.Empty;

            string trimmed = TrimLinebreaksRegex.Replace(documentText, "\n\n");
            return trimmed.Trim();
        }
    }
}