using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;
using SmartPresence.Web.Infrastructure;
using SmartPresence.Services.Users;
using SmartPresence.Services.Users.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace SmartPresence.Web.Areas
{
    [Authorize]
    [Alerts]
    [ModelStateToTempData]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class AuthenticatedBaseController : Controller
    {
        private IUserService _userService => HttpContext.RequestServices.GetService<IUserService>();

        public AuthenticatedBaseController() { }

        protected IdentitaViewModel Identita
        {
            get
            {
                return (IdentitaViewModel)ViewData[IdentitaViewModel.VIEWDATA_IDENTITACORRENTE_KEY];
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                if (context.HttpContext != null && context.HttpContext.User != null && context.HttpContext.User.Identity.IsAuthenticated)
                {
                    var email = context.HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Email).First().Value;
                    var surnameName = _userService.GetSurnameName(new UserIdentificationRequest(email));

                    //Console.WriteLine($"useraervice is present? {_userService is not null}");
                    //Console.WriteLine($"email: {email}");
                    //Console.WriteLine($"name {surnameName}");

                    ViewData[IdentitaViewModel.VIEWDATA_IDENTITACORRENTE_KEY] = new IdentitaViewModel
                    {
                        EmailUtenteCorrente = email,
                        SurnameName = surnameName
                    };

                }
                else
                {
                    HttpContext.SignOutAsync();
                    this.SignOut();

                    context.Result = new RedirectResult(context.HttpContext.Request.GetEncodedUrl());
                    Alerts.AddError(this, "L'utente non possiede i diritti per visualizzare la risorsa richiesta");
                }

                base.OnActionExecuting(context);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
