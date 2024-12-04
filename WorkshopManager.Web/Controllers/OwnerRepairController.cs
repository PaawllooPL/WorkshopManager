using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Diagnostics;
using System.Security.Claims;
using WorkshopManager.DAL.EF;
using WorkshopManager.Model.DataModels;
using WorkshopManager.Services;
using WorkshopManager.ViewModels.VM;
using static NuGet.Packaging.PackagingConstants;

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

        public IActionResult ActiveRepairs()
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ownerId == null)
            {
                return Unauthorized();
            }

            var orders = _dbContext.RepairOrders
                .Where(o => (o.Status == RepairOrderStatusValue.Accepted || o.Status == RepairOrderStatusValue.InProgress))
                .ToList();

            var acceptedOrders = orders
                .Where(order => order.Status == RepairOrderStatusValue.Accepted)
                .Select(order => new ActiveRepairVM
                {
                    Id = order.Id,
                    RegistrationNumber = order.RegistrationNumber,
                    EntryIssueDescription = order.EntryIssueDescription,
                    SubmissionDate = order.SubmissionDate,
                    StatusDescription = _statusDescriptionService.GetStatusDescription(order.Status),
                    EntryEstimatedCost = order.EntryEstimatedCost
                })
                .ToList();

            var inProgressOrders = orders
                .Where(order => order.Status == RepairOrderStatusValue.InProgress)
                .Select(order => new ActiveRepairVM
                {
                    Id = order.Id,
                    RegistrationNumber = order.RegistrationNumber,
                    EntryIssueDescription = order.EntryIssueDescription,
                    SubmissionDate = order.SubmissionDate,
                    StatusDescription = _statusDescriptionService.GetStatusDescription(order.Status),
                    EntryEstimatedCost = order.EntryEstimatedCost
                })
                .ToList();

            var viewModel = new ActiveRepairsVM
            {
                AcceptedOrders = acceptedOrders,
                InProgressOrders = inProgressOrders
            };

            return View(viewModel);
        }

        public IActionResult ActiveRepairDetails(int id)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ownerId == null)
            {
                return Unauthorized();
            }

            var order = _dbContext.RepairOrders
               .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }
            
            var viewModel = new ActiveRepairDetailVM
            {
                Id = order.Id,
                RegistrationNumber = order.RegistrationNumber,
                EntryIssueDescription = order.EntryIssueDescription,
                SubmissionDate = order.SubmissionDate,
                StatusDescription = _statusDescriptionService.GetStatusDescription(order.Status),
                EntryEstimatedCost = order.EntryEstimatedCost
            };

            return View("ActiveRepairDetails", viewModel);
        }
        public IActionResult StartRepair(int id)
        {
            var repair = _dbContext.RepairOrders.FirstOrDefault(o => o.Id == id);
            if (repair == null || repair.Status != RepairOrderStatusValue.Accepted)
            {
                NotFound();
            }
            var mechanics = _dbContext.Mechanics.Select(m => new MechanicVM(m.Id, m.FirstName, m.LastName)).ToList();
            return View(new StartRepairFormVM(id, mechanics));
        }
        [HttpPost]
        public IActionResult StartRepair(StartRepairSubmitFormVM formData)
        {
            var repair = _dbContext.RepairOrders.FirstOrDefault(r => r.Id == formData.RepairId);
            if (repair == null)
                return NotFound();

            var mechanic = _dbContext.Mechanics.FirstOrDefault(m => m.Id == formData.MechanicId);
            if (mechanic == null)
                return NotFound();
            
            repair.Mechanic = mechanic;
            repair.Status = RepairOrderStatusValue.InProgress;

            _dbContext.SaveChanges();
            
            return RedirectToAction("ActiveRepairs");
        }
    }
}
