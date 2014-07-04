using System.Linq;

using Raven.Client.Indexes;

namespace WikiDown.RavenDb.Indexes
{
    //public class ArticleSlugsIndex : AbstractMultiMapIndexCreationTask<ArticleSlugsIndex.Result>
    //{
    //    public ArticleSlugsIndex()
    //    {
    //        this.AddMap<Article>(
    //            articles => from article in articles
    //                        let articleSlug = ArticleSlugUtility.Encode(article.Title)
    //                        where !article.IsDeleted
    //                        select
    //                            new SearchArticlesIndex.Result
    //                                {
    //                                    Content =
    //                                        new object[]
    //                                            { article.Title, article.ActiveRevisionId }
    //                                });

    //        this.Index(x => x.Content, Raven.Abstractions.Indexing.FieldIndexing.Analyzed);
    //    }

    //    public class Result
    //    {
    //        public object[] Content { get; set; }
    //    }
    //}
}