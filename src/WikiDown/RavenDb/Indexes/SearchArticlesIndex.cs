using System.Linq;

using Raven.Client.Indexes;

namespace WikiDown.RavenDb.Indexes
{
    public class SearchArticlesIndex : AbstractMultiMapIndexCreationTask<SearchArticlesIndex.Result>
    {
        public SearchArticlesIndex()
        {
            this.AddMap<Article>(
                articles => from article in articles
                            where !article.IsHidden
                            select new Result { Content = new object[] { article.Title, article.ActiveRevisionId } });

            this.Index(x => x.Content, Raven.Abstractions.Indexing.FieldIndexing.Analyzed);
        }

        public class Result
        {
            public object[] Content { get; set; }
        }
    }

    //public class SearchBlogPosts : AbstractMultiMapIndexCreationTask<SearchBlogPosts.Result>
    //{
    //    public SearchBlogPosts()
    //    {
    //        this.AddMap<BlogPost>(
    //            blogPosts =>
    //            from post in blogPosts
    //            where !post.IsDeleted
    //            select new Result { Content = new object[] { post.Title, post.Content, } });

    //        this.AddMap<BlogPost>(
    //            blogPosts =>
    //            from post in blogPosts
    //            where !post.IsDeleted
    //            from tag in post.Tags
    //            select new Result { Content = new object[] { tag } });

    //        this.Index(x => x.Content, Raven.Abstractions.Indexing.FieldIndexing.Analyzed);
    //    }

    //    public class Result
    //    {
    //        public object[] Content { get; set; }
    //    }
    //}
}