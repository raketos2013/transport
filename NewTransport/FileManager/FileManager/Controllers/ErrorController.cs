using Microsoft.AspNetCore.Mvc;

namespace FileManager.Controllers;

[Route("Error/{statusCode}")]
public class ErrorController : Controller
{
    // GET: ErrorController
    public ActionResult Error(int? statusCode)
    {
        if (statusCode.HasValue)
        {
            if (statusCode == 404)
            {
                return View("NotFound");
            }
        }
        return View();
    }
}
