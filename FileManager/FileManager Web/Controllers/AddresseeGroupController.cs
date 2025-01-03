using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager_Web.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileManager_Web.Controllers
{
	[Authorize(Roles = "o.br.ДИТ")]
	public class AddresseeGroupController : Controller
	{
		private readonly ILogger<AddresseeGroupController> _logger;
		private readonly UserLogging _userLogging;
		private readonly AppDbContext _appDbContext;

        public AddresseeGroupController(ILogger<AddresseeGroupController> logger, UserLogging userLogging, AppDbContext context)
        {
			_logger = logger;
			_userLogging = userLogging;
			_appDbContext = context;
		}

        public IActionResult Addressees()
		{
			List<AddresseeGroupEntity> groups = _appDbContext.AddresseeGroup.ToList();
			return View(groups);
		}

		[HttpPost]
		public IActionResult AddresseeList(int groupId)
		{
			List<AddresseeEntity> addressees = _appDbContext.Addressee.Where(x => x.MailGroupId == groupId).ToList();	
			
			return PartialView(addressees);
		}

		public IActionResult CreateGroup(string number, string name)
		{
			AddresseeGroupEntity group = new AddresseeGroupEntity();
			group.Id = int.Parse(number);
			group.Name = name;
			_appDbContext.AddresseeGroup.Add(group);
			_appDbContext.SaveChanges();

			//_userLogging.Logging(HttpContext.User.Identity.Name, $"Создание группы рассылки: {group.Id}", JsonSerializer.Serialize(group));

			return RedirectToAction("Addressees");
		}
	}
}
