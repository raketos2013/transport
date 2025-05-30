using FileManager.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using System.Text.Json;


namespace FileManager_Web.Controllers;



public class AccountController(ILogger<AccountController> logger, IUserLogService userLogService) : Controller
{

    private readonly ILogger _logger = logger;

    public IActionResult Login()
    {

        return View();
    }


    public IActionResult Logout()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        userLogService.AddLog(HttpContext.User.Identity.Name, "Выход из системы", JsonSerializer.Serialize(""));
        return RedirectToAction("Login");
    }

    [NonAction]
    public void LogoutX()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        userLogService.AddLog(HttpContext.User.Identity.Name, "Выход из системы", JsonSerializer.Serialize(""));
    }


    [HttpPost]
    public IActionResult Login(string? username, string? password)
    {

        UserPrincipal userPrincipal = null;
        bool isAuthenticated;
        ClaimsPrincipal principal = null;
        PrincipalSearchResult<Principal> groups = null;
        ClaimsIdentity identity;
        try
        {
            if (!string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
            {
                ViewBag.MessageAuthenticate = "password is null";
                return RedirectToAction("Login");
            }

            ContextType authenticationType = ContextType.Domain;
            PrincipalContext principalContext = new(authenticationType);

            isAuthenticated = principalContext.ValidateCredentials(username, password, ContextOptions.Negotiate);
            if (isAuthenticated)
            {
                userPrincipal = UserPrincipal.FindByIdentity(principalContext, username);
                groups = userPrincipal.GetAuthorizationGroups();
                //userLogging.Logging(username, "Пользователь авторизован", JsonSerializer.Serialize(""));
            }
            if (!isAuthenticated || userPrincipal == null)
            {
                ViewBag.MessageAuthenticate = "Имя пользователя или пароль не верны";
                userLogService.AddLog(username, ViewBag.MessageAuthenticate, JsonSerializer.Serialize(""));
                return View();
            }
            else if (userPrincipal.IsAccountLockedOut())
            {
                ViewBag.MessageAuthenticate = "Пользователь блокирован";
                userLogService.AddLog(username, ViewBag.MessageAuthenticate, JsonSerializer.Serialize(""));
                return View();
            }
            else if (userPrincipal.Enabled.HasValue && userPrincipal.Enabled.Value == false)
            {
                ViewBag.MessageAuthenticate = "Пользователь блокирован";
                userLogService.AddLog(username, ViewBag.MessageAuthenticate, JsonSerializer.Serialize(""));
                return View();
            }

            //Create the identity for the user  

            List<Claim> claims = [];
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
            userLogService.AddLog(username, "Вход в систему", JsonSerializer.Serialize(""));
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
