using System.Net;
using System.Web.Http;

namespace WikiDown.Website.ApiModels
{
    public class ArticlesMetaApiModel
    {
        public ArticlesMetaApiModel()
        {
        }

        public ArticlesMetaApiModel(Article article)
        {
            this.IsDeleted = article.IsDeleted;
        }

        public bool IsDeleted { get; set; }

        public void Save(ArticleId articleId, Repository repository)
        {
            var article = repository.GetArticle(articleId);
            if (article == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            article.IsDeleted = this.IsDeleted;

            repository.SaveArticle(article);
        }
    }
}