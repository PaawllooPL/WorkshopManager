using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WorkshopManager.DAL.EF;
using WorkshopManager.Model.DataModels;
using WorkshopManager.Services;
using WorkshopManager.ViewModels.VM;

namespace WorkshopManager.Web.Controllers
{
    [Authorize(Roles = "Owner")]
    public class OwnerRepairController : BaseController
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly StatusDescriptionService _statusDescriptionService;
        public OwnerRepairController(ILogger logger, ApplicationDbContext context, StatusDescriptionService statusDescriptionService) : base(logger)
        {
            _dbContext = context;
            _statusDescriptionService = statusDescriptionService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PendingRequests()
        {
            // Zlecenia bez wyceny 
            var pendingEstimateList = _dbContext.RepairOrders
             .Where(ro => ro.Status == RepairOrderStatusValue.PendingEstimate)
             .Select(ro => new PendingEstimateVM
             {
                 Id = ro.Id,
                 RegistrationNumber = ro.RegistrationNumber,
                 EntryIssueDescription = ro.EntryIssueDescription,
                 SubmissionDate = ro.SubmissionDate,
                 StatusDescription = _statusDescriptionService.GetStatusDescription(ro.Status),
                 EntryEstimatedCost = ro.EntryEstimatedCost
             })
                .ToList();

            // Zlecenia wycenione przez wlasiciela, oczekujące na akceptację klienta
            var clientApprovalList = _dbContext.RepairOrders
             .Where(ro => ro.Status == RepairOrderStatusValue.ClientApproval)
             .Select(ro => new ClientApprovalVM
             {
                 Id = ro.Id,
                 RegistrationNumber = ro.RegistrationNumber,
                 EntryIssueDescription = ro.EntryIssueDescription,
                 SubmissionDate = ro.SubmissionDate,
                 StatusDescription = _statusDescriptionService.GetStatusDescription(ro.Status),
                 EntryEstimatedCost = ro.EntryEstimatedCost
             })
                .ToList();

            var viewModel = new PendingRequestsVM
            {
                PendingEstimates = pendingEstimateList,
                ClientApprovals = clientApprovalList
            };

            return View(viewModel);
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
                StatusDescription = _statusDescriptionService.GetStatusDescription(repairOrder.Status),
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
        [HttpPost]
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
