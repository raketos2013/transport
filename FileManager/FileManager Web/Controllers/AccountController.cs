using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.DirectoryServices.ActiveDirectory;
using System.Net;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using System.Runtime.InteropServices;
using FileManager_Web.Logging;

namespace FileManager_Web.Controllers
{


    public class AccountController : Controller
    {

        private readonly ILogger _logger;
        private readonly UserLogging _userLogging;

        public AccountController(ILogger<AccountController> logger, UserLogging userLogging)
        {
            _logger = logger;
            _userLogging = userLogging; 
        }

        public IActionResult Login()
        {
            
            return View();
        }


        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _userLogging.Logging(HttpContext.User.Identity.Name, "Выход из системы", "");
            return RedirectToAction("Login");
        }

        [NonAction]
        public void LogoutX()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _userLogging.Logging(HttpContext.User.Identity.Name, "Выход из системы", "");
        }


        [HttpPost]
        public IActionResult Login(string? username, string? password)
        {

            UserPrincipal userPrincipal = null;
            bool isAuthenticated = false;
            ClaimsIdentity identity = null;
            ClaimsPrincipal principal = null;
            PrincipalSearchResult<Principal> groups = null;
            try
            {
                if (!string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
                {
                    ViewBag.MessageAuthenticate = "password is null";
                    return RedirectToAction("Login");
                }

                ContextType authenticationType = ContextType.Domain;
                PrincipalContext principalContext = new PrincipalContext(authenticationType);

                isAuthenticated = principalContext.ValidateCredentials(username, password, ContextOptions.Negotiate);
                if (isAuthenticated)
                {
                    userPrincipal = UserPrincipal.FindByIdentity(principalContext, username);
                    groups = userPrincipal.GetAuthorizationGroups();
                    _userLogging.Logging(username, "Пользователь авторизован", "");
                }
                if (!isAuthenticated || userPrincipal == null)
                {
                    ViewBag.MessageAuthenticate = "Имя пользователя или пароль не верны";
                    _userLogging.Logging(username, ViewBag.MessageAuthenticate, "");
                    return View();
                }
                else if (userPrincipal.IsAccountLockedOut())
                {
                    ViewBag.MessageAuthenticate = "Пользователь блокирован";
                    _userLogging.Logging(username, ViewBag.MessageAuthenticate, "");
                    return View();
                }
                else if (userPrincipal.Enabled.HasValue && userPrincipal.Enabled.Value == false)
                {
                    ViewBag.MessageAuthenticate = "Пользователь блокирован";
                    _userLogging.Logging(username, ViewBag.MessageAuthenticate, "");
                    return View();
                }

                //Create the identity for the user  

                List<Claim> claims = new List<Claim>();
                foreach (var gr in groups)
                {
                    if (gr is GroupPrincipal)
                    {
                        if (gr.Name == "o.br.ДИТ")
                        {
                            claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, gr.Name));
                        }
                        
                    }
                }
                claims.Add(new Claim(ClaimsIdentity.DefaultNameClaimType, userPrincipal.Name));
                claims.Add(new Claim(ClaimTypes.GivenName, userPrincipal.DisplayName));
                /*if (userPrincipal.EmailAddress != null)
                {
                    claims.Add(new Claim(ClaimTypes.Email, userPrincipal.EmailAddress));
                }*/
                

                identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                principal = new ClaimsPrincipal(identity);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                _userLogging.Logging(username, "Вход в систему", "");
                return RedirectToAction("Tasks", "Task");

            }
            catch (Exception ex)
            {
                userPrincipal = null;
                identity = null;
                principal = null;
                _logger.LogError(ex, ex.Message);
            }

            return View();
        }
    }



}
