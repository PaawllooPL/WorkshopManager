using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
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
                    Id = ro.Id,
                    SubmissionDate = ro.SubmissionDate,
                    Status = ro.Status
                })
                .ToList();

            return View(model);
        }

        public IActionResult RespondToRequest(int id)
        {
            var repairOrder = _dbContext.RepairOrders
                .FirstOrDefault(ro => ro.Id == id);

            if (repairOrder == null)
            {
                return NotFound("Zlecenie o podanym ID nie zostało znalezione.");
            }

            var viewModel = new PendingEstimateVM
            {
                Id = repairOrder.Id,
                RegistrationNumber = repairOrder.RegistrationNumber,
                EntryIssueDescription = repairOrder.EntryIssueDescription,
                SubmissionDate = repairOrder.SubmissionDate,
                Status = repairOrder.Status,
                EntryEstimatedCost = repairOrder.EntryEstimatedCost
            };

            return View(viewModel);
        }


        [HttpPost]
        public IActionResult RespondToRequest(int id, PendingEstimateVM viewModel)
        {
            var repairOrder = _dbContext.RepairOrders.FirstOrDefault(ro => ro.Id == id);

            if (repairOrder == null)
            {
                return NotFound("Zlecenie o podanym ID nie zostało znalezione.");
            }

            repairOrder.EntryEstimatedCost = viewModel.EntryEstimatedCost.Value;    // Zaktualizowanie EstimatedCost
            repairOrder.Status = RepairOrderStatusValue.ClientApproval; // Zmiana statusu na ClientApproval

            _dbContext.SaveChanges();

            return RedirectToAction("PendingRequests");
        }


        [HttpGet]
        public IActionResult RejectRequest(int id)
        {
            var repairOrder = _dbContext.RepairOrders.FirstOrDefault(ro => ro.Id == id);

            if (repairOrder == null)
            {
                return NotFound("Zlecenie o podanym ID nie zostało znalezione.");
            }

            _dbContext.RepairOrders.Remove(repairOrder);

            _dbContext.SaveChanges();
            
            return RedirectToAction("PendingRequests");
        }
    }
}
