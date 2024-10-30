using Microsoft.AspNetCore.Mvc;

namespace WorkshopManager.Web.Controllers
{
	public class RepairTaskController : BaseController
	{
		public RepairTaskController(ILogger logger) : base(logger)
		{

		}

		public IActionResult Index()
		{
			return View();
		}
	}
}
