using System.Linq;
using System.Web.Http;

namespace WikiDown.Website.Controllers.Api
{
    [RoutePrefix("api/wiki")]
    public class WikiController : WikiDownApiControllerBase
    {
        //[HttpGet]
        //[Route("search")]
        //public dynamic SearchArticleTitles([FromUri] string q)
        //{
        //    return this.CurrentRepository.SearchArticleTitles(q);
        //}

        [HttpGet]
        [Route("autocomplete")]
        public dynamic SearchAutocomplete([FromUri] string q)
        {
            // TODO: Take 10
            var results = this.CurrentRepository.SearchArticleTitles(q);

            var items = from result in results
                        let title = ArticleSlugUtility.Decode(result.Slug)
                        let redirectTitle =
                            !string.IsNullOrWhiteSpace(result.RedirectToSlug)
                                ? ArticleSlugUtility.Decode(result.RedirectToSlug)
                                : null
                        let redirectFrom = !string.IsNullOrWhiteSpace(redirectTitle) ? title : null
                        let value = !string.IsNullOrWhiteSpace(redirectTitle) ? redirectTitle : title
                        let slug =
                            !string.IsNullOrWhiteSpace(result.RedirectToSlug) ? result.RedirectToSlug : result.Slug
                        let url = this.Url.Route(RouteNames.WikiArticle, new { slug })
                        select new { value, data = url, redirect = redirectFrom };

            var suggestions = items.ToList();

            return new { query = q, suggestions };
        }
    }
}