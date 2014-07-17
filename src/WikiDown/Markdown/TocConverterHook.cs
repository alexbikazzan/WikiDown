using System;
using System.Linq;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

namespace WikiDown.Markdown
{
    public class TocConverterHook : RegexReplaceConverterHook
    {
        private const string DisplayRegexPattern = "@{TOC}";

        private const string DisplayRegexReplace = "<pre><code>[Table of contents]</code></pre>";

        private const string TocRegexReplace = @"(<[^>]+>)?" + DisplayRegexPattern + @"(</[^>]+>)?";

        public TocConverterHook()
            : base(DisplayRegexPattern, DisplayRegexReplace, ConverterHookType.PostConversion)
        {
        }

        public override string Apply(string html)
        {
            if (string.IsNullOrWhiteSpace(html) || !html.Contains(DisplayRegexPattern))
            {
                return html;
            }

            var document = new HtmlDocument();
            document.LoadHtml(html);

            string tocHtml = CreateToc(document);

            return Regex.Replace(html, TocRegexReplace, tocHtml, RegexOptions.Multiline);
        }

        private static HtmlNode AppendNode(
            HtmlDocument document,
            string name,
            string className = null,
            HtmlNode parentNode = null)
        {
            var node = document.CreateElement(name);

            if (!string.IsNullOrWhiteSpace(className))
            {
                node.SetAttributeValue("class", className);
            }

            if (parentNode != null)
            {
                parentNode.AppendChild(node);
            }

            return node;
        }

        private static string CreateToc(HtmlDocument document)
        {
            var headerTags =
                (document.DocumentNode.SelectNodes(
                    "//*[self::h1 or self::h2 or self::h3 or self::h4 or self::h5 or self::h6]")
                 ?? Enumerable.Empty<HtmlNode>()).ToList();

            if (!headerTags.Any())
            {
                return string.Empty;
            }

            var div = AppendNode(document, "div", "toc panel panel-default pull-left");

            var head = AppendNode(document, "div", "panel-heading", div);
            head.InnerHtml = "Contents";

            var body = AppendNode(document, "div", "panel-body", div);
            var list = AppendNode(document, "ol", parentNode: body);

            int lastLevel = GetHeaderLevel(headerTags.First());

            foreach (var header in headerTags)
            {
                int level = GetHeaderLevel(header);
                if (level > lastLevel)
                {
                    var lastItem = list.LastChild;
                    list = AppendNode(document, "ol", parentNode: lastItem);
                }
                else if (level < lastLevel)
                {
                    var parentList = FindParentList(FindParentList(list));
                    list = parentList ?? list;
                }

                var item = AppendNode(document, "li", parentNode: list);

                var link = AppendNode(document, "a", parentNode: item);
                link.InnerHtml = header.InnerText;
                link.SetAttributeValue("href", "#" + header.Id);

                lastLevel = level;
            }

            var clearfix = AppendNode(document, "div", "clearfix");

            return div.OuterHtml + clearfix.OuterHtml;
        }

        private static int GetHeaderLevel(HtmlNode node)
        {
            var name = node.Name.ToLowerInvariant();
            switch (name)
            {
                case "h1":
                    return 1;
                case "h2":
                    return 2;
                case "h3":
                    return 3;
                case "h4":
                    return 4;
                case "h5":
                    return 5;
                case "h6":
                    return 6;
            }
            return -1;
        }

        private static HtmlNode FindParentList(HtmlNode node)
        {
            var parent = (node != null) ? node.ParentNode : null;

            while (parent != null && !parent.Name.Equals("ol", StringComparison.InvariantCultureIgnoreCase)
                   && parent.ParentNode != null && parent.GetAttributeValue("class", null) != "panel-body")
            {
                parent = parent.ParentNode;
            }

            return parent;
        }
    }
}