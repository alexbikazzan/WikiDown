using System.Collections.Generic;
using System.Linq;

using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using Raven.Client.Linq.Indexing;

namespace WikiDown.RavenDb.Indexes
{
    public class SearchArticlesIndex : AbstractMultiMapIndexCreationTask<SearchArticlesIndex.Result>
    {
        public const int HighlightFragmentLength = 128;

        public SearchArticlesIndex()
        {
            this.AddMap<Article>(
                articles => from article in articles
                            let activeRevision =
                                (article.ActiveRevisionId != null)
                                    ? this.LoadDocument<ArticleRevision>(article.ActiveRevisionId)
                                    : null
                            let markdownContent = (activeRevision != null) ? activeRevision.TextContent : null
                            select
                                new Result
                                    {
                                        //Id = article.Id,
                                        CanReadAccess = (int)article.ArticleAccess.CanRead,
                                        RedirectToSlug = (string)null,
                                        Slug = article.Slug,
                                        Tags = article.Tags,
                                        TextContent = markdownContent
                                    });

            this.AddMap<ArticleRedirect>(
                redirects => from redirect in redirects
                             select
                                 new Result
                                     {
                                         //Id = (string)null,
                                         CanReadAccess = -1,
                                         RedirectToSlug = redirect.RedirectToArticleSlug,
                                         Slug = redirect.RedirectFromArticleSlug,
                                         Tags = new string[0],
                                         TextContent = (string)null,
                                     });

            this.Reduce = results => from result in results
                                     group result by result.Slug
                                     into g
                                     select
                                         new Result
                                             {
                                                 //Id = g.Select(x => x.Id).First(x => !string.IsNullOrWhiteSpace(x)),
                                                 CanReadAccess = g.Max(x => x.CanReadAccess),
                                                 RedirectToSlug =
                                                     g.Select(x => x.RedirectToSlug)
                                                     .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)),
                                                 Slug = g.Key,
                                                 Tags = g.SelectMany(x => x.Tags),
                                                 TextContent =
                                                     g.Select(x => x.TextContent)
                                                     .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x))
                                             };

            this.Index(x => x.RedirectToSlug, FieldIndexing.Analyzed);
            this.Index(x => x.Tags, FieldIndexing.Analyzed);
            this.Index(x => x.TextContent, FieldIndexing.Analyzed);
            this.Index(x => x.Slug, FieldIndexing.Analyzed);

            this.Store(x => x.TextContent, FieldStorage.Yes);

            this.TermVector(x => x.TextContent, FieldTermVector.WithPositionsAndOffsets);
        }

        public class Result
        {
            //public string Id { get; set; }

            public int CanReadAccess { get; set; }

            public string RedirectToSlug { get; set; }

            public string TextContent { get; set; }

            public string Slug { get; set; }

            public IEnumerable<string> Tags { get; set; }
        }
    }
}