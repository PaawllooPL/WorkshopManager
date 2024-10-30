using Microsoft.AspNetCore.Mvc;

namespace WorkshopManager.Web.Controllers
{
	public class OwnerRepairController : BaseController
	{
		public OwnerRepairController(ILogger logger) : base(logger)
		{

		}

		public IActionResult Index()
		{
			return View();
		}
	}
}
