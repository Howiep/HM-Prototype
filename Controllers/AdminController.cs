using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using HeedeMoestrup.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using static HeedeMoestrup.Models.ModelContext;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using HeedeMoestrup.Models.ViewModels;

namespace HeedeMoestrup.Controllers
{
    public class AdminController : Controller
    {

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult LogIn(string returnUrl)
        {
            var model = new LogInModel
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> LogIn(LogInModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var usermanager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(new ModelContext()));

            var user = usermanager.Find(model.Name, model.Password);

            if (user != null)
            {
                var identity = await usermanager.CreateIdentityAsync(
                    user, DefaultAuthenticationTypes.ApplicationCookie);

                var ctx = Request.GetOwinContext();
                var authManager = ctx.Authentication;

                authManager.SignIn(identity);

                return Redirect(GetRedirectUrl(model.ReturnUrl));
            }

            // user authN failed
            ModelState.AddModelError("", "Invalid Name or Password");

            return View();
        }


        public ActionResult LogOut()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("index", "Admin");
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = new IdentityUser
            {
                UserName = model.Name
            };
            var usermanager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(new ModelContext()));

            var result = await usermanager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("index", "Admin");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }

            return View();
        }

        private string GetRedirectUrl(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return Url.Action("Index", "Admin");
            }

            return returnUrl;
        }

        protected override void Dispose(bool disposing)
        {
            var userStore = new UserStore<IdentityUser>();
            var userManager = new UserManager<IdentityUser>(userStore);

            if (disposing && userManager != null)
            {
                userManager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}