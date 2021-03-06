﻿using System.Linq;

using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace WikiDown.RavenDb.Indexes
{
    public class ArticleRedirectsIndex : AbstractIndexCreationTask<ArticleRedirect>
    {
        public ArticleRedirectsIndex()
        {
            this.Map =
                redirects =>
                from redirect in redirects
                select new { redirect.RedirectFromArticleSlug, redirect.RedirectToArticleSlug };

            this.Index(x => x.RedirectFromArticleSlug, FieldIndexing.Analyzed);
            this.Index(x => x.RedirectToArticleSlug, FieldIndexing.Analyzed);
        }
    }
}