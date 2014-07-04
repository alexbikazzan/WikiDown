using System;

namespace WikiDown
{
    public class ArticleIdNotEnsuredException : Exception
    {
        public ArticleIdNotEnsuredException(string originalSlug, string ensuredSlug)
        {
            this.OriginalSlug = originalSlug;
            this.EnsuredSlug = ensuredSlug;
        }

        public string OriginalSlug { get; private set; }

        public string EnsuredSlug { get; private set; }
    }
}