using Microsoft.AspNetCore.Mvc;

namespace WorkshopManager.Web.Controllers
{
	public class ClientRepairController : BaseController
	{
		public ClientRepairController(ILogger logger) : base(logger)
		{

		}

		public IActionResult Index()
		{
			return View();
		}
	}
}
