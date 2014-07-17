using System.Linq;

using HtmlAgilityPack;

namespace WikiDown.Markdown
{
    public class HeadingsConverterHook : ServerSideConverterHook
    {
        public HeadingsConverterHook()
            : base(ConverterHookType.PostConversion)
        {
        }

        public override string Apply(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return html;
            }

            var document = new HtmlDocument();
            document.LoadHtml(html);

            ModifyHeaders(document);

            return document.DocumentNode.OuterHtml;
        }

        private static void ModifyHeaders(HtmlDocument document)
        {
            var headerTags =
                (document.DocumentNode.SelectNodes(
                    "//*[self::h1 or self::h2 or self::h3 or self::h4 or self::h5 or self::h6]")
                 ?? Enumerable.Empty<HtmlNode>()).ToList();

            if (!headerTags.Any())
            {
                return;
            }

            foreach (var header in headerTags)
            {
                string title = header.InnerText;
                string encodedTitle = ArticleSlugUtility.Encode(title);

                header.SetAttributeValue("id", encodedTitle);
            }
        }
    }
}