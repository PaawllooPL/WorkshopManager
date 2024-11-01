using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WorkshopManager.Web.Controllers
{
	[Authorize(Roles = "Owner")]
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
