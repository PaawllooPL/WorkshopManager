using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.DAL.EF;
using WorkshopManager.Model.DataModels;
using WorkshopManager.ViewModels.VM;

namespace WorkshopManager.Web.Controllers
{
	[Authorize(Roles = "Owner")]
	public class OwnerRepairController : BaseController
	{
        private readonly ApplicationDbContext _dbContext;
        public OwnerRepairController(ILogger logger, ApplicationDbContext context) : base(logger)
        {
            _dbContext = context;
        }

        public IActionResult Index()
		{
			return View();
		}

        public IActionResult PendingRequests()
        {
            List<PendingEstimateVM> model = _dbContext.RepairOrders
                .Where(ro => ro.Status == RepairOrderStatusValue.PendingEstimate || ro.Status == RepairOrderStatusValue.ClientApproval)
                .Select(ro => new PendingEstimateVM
                {
                    SubmissionDate = ro.SubmissionDate,
                    Status = ro.Status
                })
                .ToList();

            return View(model);
        }

    }
}
