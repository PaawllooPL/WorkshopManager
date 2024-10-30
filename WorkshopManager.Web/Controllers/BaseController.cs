using Microsoft.AspNetCore.Mvc;

namespace WorkshopManager.Web.Controllers
{
	public class BaseController : Controller
	{
		protected readonly ILogger Logger;
		public BaseController(ILogger logger)
		{
			Logger = logger;
		}
	}
}
