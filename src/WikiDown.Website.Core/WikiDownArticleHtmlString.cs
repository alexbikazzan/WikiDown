using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using HtmlAgilityPack;
using WikiDown.Markdown;

namespace WikiDown.Website
{
    public class WikiDownArticleHtmlString : IHtmlString
    {
        public const string NoArticleCssClass = "no-article";

        private static readonly Regex WikiLinkPrefixRemoveRegex = new Regex(@"^[\/]?wiki\/", RegexOptions.Multiline);

        private readonly string html;

        private readonly Repository repository;

        public WikiDownArticleHtmlString(ArticleResult articleResult, Repository repository)
        {
            if (articleResult == null)
            {
                throw new ArgumentNullException("articleResult");
            }
            if (articleResult.ArticleRevision == null)
            {
                throw new ArgumentOutOfRangeException(
                    "articleResult",
                    "ArticleResult cannot have a null ArticleRevision.");
            }
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            this.html = MarkdownService.MakeHtml(articleResult.ArticleRevision.MarkdownContent);
            this.repository = repository;
        }

        public WikiDownArticleHtmlString(ArticleRevision articleRevision, Repository repository)
        {
            if (articleRevision == null)
            {
                throw new ArgumentNullException("articleRevision");
            }
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            this.html = MarkdownService.MakeHtml(articleRevision.MarkdownContent);
            this.repository = repository;
        }

        public string ToHtmlString()
        {
            if (string.IsNullOrWhiteSpace(this.html))
            {
                return this.html;
            }

            var document = new HtmlDocument();
            document.LoadHtml(html);

            this.FlagWikiLinks(document);

            return document.DocumentNode.OuterHtml;
        }

        private void FlagWikiLinks(HtmlDocument document)
        {
            var linkNodes = document.DocumentNode.SelectNodes("//a") ?? Enumerable.Empty<HtmlNode>();

            var wikiLinks = from link in linkNodes
                            let href = link.GetAttributeValue("href", null)
                            where
                                href != null
                                && (href.StartsWith("wiki/", StringComparison.InvariantCultureIgnoreCase)
                                    || href.StartsWith("/wiki/", StringComparison.InvariantCultureIgnoreCase))
                            select link;

            foreach (var wikiLink in wikiLinks)
            {
                this.SetLinkCssClass(wikiLink);
            }
        }

        private void SetLinkCssClass(HtmlNode wikiLink)
        {
            var href = wikiLink.GetAttributeValue("href", null) ?? string.Empty;

            string linkArticle = WikiLinkPrefixRemoveRegex.Replace(href, string.Empty).TrimEnd('/');

            var article = this.repository.GetArticleExists(linkArticle);
            if (article == null)
            {
                wikiLink.SetAttributeValue("class", NoArticleCssClass);
            }
        }
    }
}