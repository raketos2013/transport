using FileManager.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace FileManager.Controllers;

public class AccountController(ILogger<AccountController> logger,
                                IUserLogService userLogService,
                                IAuthService authService) : Controller
{
    private readonly ILogger _logger = logger;

    public IActionResult Login()
    {

        return View();
    }


    public async Task<IActionResult> Logout()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await userLogService.AddLog(HttpContext.User.Identity.Name, "Выход из системы", JsonSerializer.Serialize(""));
        return RedirectToAction(nameof(Login));
    }

    [NonAction]
    public async Task LogoutX()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await userLogService.AddLog(HttpContext.User.Identity.Name, "Выход из системы", JsonSerializer.Serialize(""));
    }

    [HttpPost]
    public async Task<IActionResult> Login(string? username, string? password)
    {
        ClaimsIdentity? identity;
        ClaimsPrincipal? principal;
        bool isFromPermissionRule = false;
        try
        {
            if (!string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
            {
                ViewBag.MessageAuthenticate = "password is null";
                return RedirectToAction("Login");
            }
            bool isAuthenticated;
            string displayName;
            (isAuthenticated, displayName) = await authService.AuthenticateUser(username, password);

            if (isAuthenticated)
            {
                List<Claim> claims =
                [
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "o.br.ДИТ"),
                    new Claim(ClaimsIdentity.DefaultNameClaimType, username),
                    new Claim(ClaimTypes.GivenName, displayName),
                ];


                identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                principal = new ClaimsPrincipal(identity);
                if (principal != null && HttpContext != null)
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = true });
                }
            }
            else
            {
                ViewBag.MessageAuthenticate = "Неправильный логин и (или) пароль";
                return View();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{message}", ex.Message);
        }


        //UserPrincipal userPrincipal = null;
        //bool isAuthenticated;
        //ClaimsPrincipal principal = null;
        //PrincipalSearchResult<Principal> groups = null;
        //ClaimsIdentity identity;
        //try
        //{
        //    if (!string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
        //    {
        //        ViewBag.MessageAuthenticate = "password is null";
        //        return RedirectToAction("Login");
        //    }

        //    ContextType authenticationType = ContextType.Domain;
        //    PrincipalContext principalContext = new(authenticationType);

        //    isAuthenticated = principalContext.ValidateCredentials(username, password, ContextOptions.Negotiate);
        //    if (isAuthenticated)
        //    {
        //        userPrincipal = UserPrincipal.FindByIdentity(principalContext, username);
        //        groups = userPrincipal.GetAuthorizationGroups();
        //        //userLogging.Logging(username, "Пользователь авторизован", JsonSerializer.Serialize(""));
        //    }
        //    if (!isAuthenticated || userPrincipal == null)
        //    {
        //        ViewBag.MessageAuthenticate = "Имя пользователя или пароль не верны";
        //        userLogService.AddLog(username, ViewBag.MessageAuthenticate, JsonSerializer.Serialize(""));
        //        return View();
        //    }
        //    else if (userPrincipal.IsAccountLockedOut())
        //    {
        //        ViewBag.MessageAuthenticate = "Пользователь блокирован";
        //        userLogService.AddLog(username, ViewBag.MessageAuthenticate, JsonSerializer.Serialize(""));
        //        return View();
        //    }
        //    else if (userPrincipal.Enabled.HasValue && userPrincipal.Enabled.Value == false)
        //    {
        //        ViewBag.MessageAuthenticate = "Пользователь блокирован";
        //        userLogService.AddLog(username, ViewBag.MessageAuthenticate, JsonSerializer.Serialize(""));
        //        return View();
        //    }

        //    //Create the identity for the user  

        //    List<Claim> claims = [];
        //    foreach (var gr in groups)
        //    {
        //        if (gr is GroupPrincipal)
        //        {
        //            if (gr.Name == "o.br.ДИТ")
        //            {
        //                claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, gr.Name));
        //            }

        //        }
        //    }
        //    claims.Add(new Claim(ClaimsIdentity.DefaultNameClaimType, userPrincipal.Name));
        //    claims.Add(new Claim(ClaimTypes.GivenName, userPrincipal.DisplayName));
        //    /*if (userPrincipal.EmailAddress != null)
        //    {
        //        claims.Add(new Claim(ClaimTypes.Email, userPrincipal.EmailAddress));
        //    }*/


        //    identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        //    principal = new ClaimsPrincipal(identity);
        //    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        //    userLogService.AddLog(username, "Вход в систему", JsonSerializer.Serialize(""));
        //return RedirectToAction("Tasks", "Task");

        //}
        //catch (Exception ex)
        //{
        //    userPrincipal = null;
        //    identity = null;
        //    principal = null;
        //    _logger.LogError(ex, ex.Message);
        //}

        return RedirectToAction("Tasks", "Task");
    }
}
