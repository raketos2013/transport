using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Globalization;

namespace FileManager.Controllers;

[Authorize(Roles = "o.br.ДИТ")]
public class UserLogsController(IUserLogService userLogService) : Controller
{

    // GET: UserLogsController
    public ActionResult Index(string dateFrom = "", string dateTo = "")
    {
        DateTime date = dateFrom == "" ? DateTime.Today : DateTime.Parse(dateFrom);
        DateTime date2 = dateTo == "" ? DateTime.Today : DateTime.Parse(dateTo);

        ViewBag.FilterDateFrom = date.ToString("yyyy-MM-dd");
        ViewBag.FilterDateTo = date2.ToString("yyyy-MM-dd");
        return View();
    }

    public IActionResult LogsList(string dateFrom = "", string dateTo = "")
    {
        DateTime date = dateFrom == "" ? DateTime.Today : DateTime.Parse(dateFrom);
        DateTime date2 = dateTo == "" ? DateTime.Today : DateTime.Parse(dateTo);

        List<UserLogEntity> userLogs = userLogService.GetAllLogs().Where(x => x.DateTimeLog.Date >= date &&
                                                                                x.DateTimeLog.Date <= date2)
                                                            .OrderByDescending(x => x.DateTimeLog)
                                                            .ToList();
        ViewBag.FilterDateFrom = date.ToString("yyyy-MM-dd");
        ViewBag.FilterDateTo = date2.ToString("yyyy-MM-dd");
        return PartialView("_LogsList", userLogs);
    }

    // GET: UserLogsController/Details/5
    public ActionResult Details(string dateTime, string username)
    {
        DateTime myDate = DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss.ffffff",
                                   CultureInfo.InvariantCulture);

        UserLogEntity userLogs = userLogService.GetAllLogs()
                                                .First(x => x.DateTimeLog == myDate &&
                                                            x.UserName == username);

        return PartialView("_LogDetails", userLogs);
    }


}
