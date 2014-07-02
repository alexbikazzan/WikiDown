using System;
using System.Web.Http;

namespace WikiDown.Website.Controllers.Api
{
    public abstract class ApiControllerBase : ApiController
    {
        private readonly Lazy<Repository> currentRepository;

        protected ApiControllerBase(Repository repository = null)
        {
            this.currentRepository =
                new Lazy<Repository>(() => repository ?? new Repository(MvcApplication.DocumentStore));
        }

        public Repository CurrentRepository
        {
            get
            {
                return this.currentRepository.Value;
            }
        }
    }
}