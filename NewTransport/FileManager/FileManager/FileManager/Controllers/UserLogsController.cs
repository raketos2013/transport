using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Services;
using FileManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FileManager.Controllers;

[Authorize(Roles = "o.br.ДИТ")]
public class UserLogsController(IUserLogService userLogService) : Controller
{

    // GET: UserLogsController
    public async Task<ActionResult> Index(string dateFrom = "", string dateTo = "")
    {
        DateTime date = dateFrom == "" ? DateTime.Today : DateTime.Parse(dateFrom);
        DateTime date2 = dateTo == "" ? DateTime.Today : DateTime.Parse(dateTo);

        //ViewBag.FilterDateFrom = date.ToString("yyyy-MM-dd");
        //ViewBag.FilterDateTo = date2.ToString("yyyy-MM-dd");

        var userLogsAsync = await userLogService.GetAllLogs();
        var userLogs = userLogsAsync.Where(x => x.DateTimeLog.Date >= date &&
                                                x.DateTimeLog.Date <= date2)
                                                            .OrderByDescending(x => x.DateTimeLog)
                                                            .ToList();
        UserLogViewModel model = new()
        {
            DateFrom = date,
            DateTo = date2,
            Logs = userLogs
        };
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> Index(UserLogViewModel model)
    {
        var timeFrom = model.TimeFrom.TimeOfDay;
        var timeTo = model.TimeTo.TimeOfDay;
        var userLogsAsync = await userLogService.GetAllLogs();
        var userLogs = userLogsAsync.Where(x => x.DateTimeLog.Date >= model.DateFrom &&
                                                x.DateTimeLog.Date <= model.DateTo)
                                                            .OrderByDescending(x => x.DateTimeLog)
                                                            .ToList();
        if (timeFrom != DateTime.Today.TimeOfDay)
        {
            userLogs = userLogs.Where(x => x.DateTimeLog.TimeOfDay >= timeFrom).ToList();
        }
        if (timeTo != DateTime.Today.TimeOfDay)
        {
            userLogs = userLogs.Where(x => x.DateTimeLog.TimeOfDay <= timeTo).ToList();
        }
        if (!string.IsNullOrEmpty(model.User))
        {
            switch (model.UserOption)
            {
                case Core.Enums.FilterOptions.Equal:
                    userLogs = userLogs.Where(x => x.UserName == model.User).ToList();
                    break;
                case Core.Enums.FilterOptions.NotEqual:
                    userLogs = userLogs.Where(x => x.UserName != model.User).ToList();
                    break;
                case Core.Enums.FilterOptions.More:
                    userLogs = userLogs.Where(x => string.Compare(model.User, x.UserName) > 0).ToList();
                    break;
                case Core.Enums.FilterOptions.Less:
                    userLogs = userLogs.Where(x => string.Compare(model.User, x.UserName) < 0).ToList();
                    break;
                case Core.Enums.FilterOptions.MoreEqual:
                    userLogs = userLogs.Where(x => string.Compare(model.User, x.UserName) >= 0).ToList();
                    break;
                case Core.Enums.FilterOptions.LessEqual:
                    userLogs = userLogs.Where(x => string.Compare(model.User, x.UserName) <= 0).ToList();
                    break;
                default:
                    break;
            }
        }
        if (!string.IsNullOrEmpty(model.Action))
        {
            switch (model.ActionOption)
            {
                case Core.Enums.FilterOptions.Equal:
                    userLogs = userLogs.Where(x => x.Action == model.Action).ToList();
                    break;
                case Core.Enums.FilterOptions.NotEqual:
                    userLogs = userLogs.Where(x => x.Action != model.Action).ToList();
                    break;
                case Core.Enums.FilterOptions.More:
                    userLogs = userLogs.Where(x => string.Compare(model.Action, x.Action) > 0).ToList();
                    break;
                case Core.Enums.FilterOptions.Less:
                    userLogs = userLogs.Where(x => string.Compare(model.Action, x.Action) < 0).ToList();
                    break;
                case Core.Enums.FilterOptions.MoreEqual:
                    userLogs = userLogs.Where(x => string.Compare(model.Action, x.Action) >= 0).ToList();
                    break;
                case Core.Enums.FilterOptions.LessEqual:
                    userLogs = userLogs.Where(x => string.Compare(model.Action, x.Action) <= 0).ToList();
                    break;
                default:
                    break;
            }
        }
        UserLogViewModel viewModel = new()
        {
            DateFrom = model.DateFrom,
            DateTo = model.DateTo,
            TimeFrom = model.TimeFrom,
            TimeTo = model.TimeTo,
            User = model.User,
            Action = model.Action,
            Logs = userLogs
        };
        return View(viewModel);
    }

    public async Task<IActionResult> LogsList(string dateFrom = "", string dateTo = "")
    {
        DateTime date = dateFrom == "" ? DateTime.Today : DateTime.Parse(dateFrom);
        DateTime date2 = dateTo == "" ? DateTime.Today : DateTime.Parse(dateTo);

        var userLogsAsync = await userLogService.GetAllLogs();
        var userLogs = userLogsAsync.Where(x => x.DateTimeLog.Date >= date &&
                                                x.DateTimeLog.Date <= date2)
                                                            .OrderByDescending(x => x.DateTimeLog)
                                                            .ToList();
        ViewBag.FilterDateFrom = date.ToString("yyyy-MM-dd");
        ViewBag.FilterDateTo = date2.ToString("yyyy-MM-dd");
        return PartialView("_LogsList", userLogs);
    }

    // GET: UserLogsController/Details/5
    public async Task<ActionResult> Details(string dateTime, string username)
    {
        DateTime myDate = DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss.ffffff",
                                   CultureInfo.InvariantCulture);

        var userLogsAsync = await userLogService.GetAllLogs();
        var userLogs = userLogsAsync.First(x => x.DateTimeLog == myDate &&
                                                            x.UserName == username);

        return PartialView("_LogDetails", userLogs);
    }


}
