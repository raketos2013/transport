using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager_Web.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Globalization;

namespace FileManager_Web.Controllers
{
	[Authorize(Roles = "o.br.ДИТ")]
	public class UserLogsController : Controller
    {
        private readonly ILogger<UserLogsController> _logger;
        private readonly UserLogging _userLogging;
        private readonly AppDbContext _appDbContext;

        public UserLogsController(ILogger<UserLogsController> logger, UserLogging userLogging, AppDbContext context)
        {
            _appDbContext = context;
            _logger = logger;
            _userLogging = userLogging;
        }

		// GET: UserLogsController
		
		public ActionResult Index(string dateFrom, string dateTo)
        {
            DateTime date = dateFrom == null ? DateTime.Today : DateTime.Parse(dateFrom);
            DateTime date2 = dateTo == null ? DateTime.Today : DateTime.Parse(dateTo);

            List<UserLogEntity> userLogs = _appDbContext.UserLog.Where(x => x.DateTimeLog.Date >= date &&
                                                                             x.DateTimeLog.Date <= date2 ).OrderByDescending(x => x.DateTimeLog).ToList();
            ViewBag.FilterDateFrom = date.ToString("yyyy-MM-dd");
            ViewBag.FilterDateTo = date2.ToString("yyyy-MM-dd");
            return View(userLogs);
        }

        // GET: UserLogsController/Details/5
        public ActionResult Details(string dateTime, string username)
        {
            DateTime myDate = DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss.ffffff",
                                       CultureInfo.InvariantCulture);

            UserLogEntity userLogs = _appDbContext.UserLog.First(x => x.DateTimeLog == myDate && x.UserName == username);

            return View(userLogs);
        }

        
    }
}
