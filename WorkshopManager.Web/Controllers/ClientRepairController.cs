using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using WorkshopManager.Model.DataModels;

namespace WorkshopManager.Web.Controllers
{
	[Authorize(Roles = "Client")]
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
