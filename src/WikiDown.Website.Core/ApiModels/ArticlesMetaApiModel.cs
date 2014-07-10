using System.Net;
using System.Security.Principal;
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
            this.IsHidden = article.IsHidden;
        }

        public bool IsHidden { get; set; }

        public void Save(ArticleId articleId, Repository repository)
        {
            var article = repository.GetArticle(articleId);
            if (article == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            article.IsHidden = this.IsHidden;

            repository.SaveArticle(article);
        }
    }
}