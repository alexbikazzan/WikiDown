using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using WikiDown.Website.Security;
using WikiDown.Website.ViewModels;

namespace WikiDown.Website.Controllers
{
    public class AccountsController : WikiDownControllerBase
    {
        [HttpGet]
        [Route("login", Name = RouteNames.Login)]
        public ActionResult Login(string returnUrl)
        {
            var model = new AuthLoginViewModel();
            return this.View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [Route("login")]
        public async Task<ActionResult> Login(AuthLoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await this.UserManager.FindAsync(model.Username, model.Password);
                if (user != null)
                {
                    this.SignInAsync(user, true /*isPersistent*/);
                    return this.RedirectToLocal(returnUrl);
                }

                this.ModelState.AddModelError("model", "Invalid username or password.");
            }

            return this.View(model);
        }

        [AllowAnonymous]
        [Authorize]
        [Route("logout", Name = RouteNames.Logout)]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();

            string redirectUrl = this.Url.Login();
            return this.Redirect(redirectUrl);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }

            string redirectUrl = this.Url.Empty();
            return this.Redirect(redirectUrl);
        }

        private async void SignInAsync(WikiDownUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut();

            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            var authenticationProperties = new AuthenticationProperties { IsPersistent = isPersistent };

            AuthenticationManager.SignIn(authenticationProperties, identity);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return this.HttpContext.GetOwinContext().Authentication;
            }
        }
    }
}