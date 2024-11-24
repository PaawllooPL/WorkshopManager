using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.DAL.EF;
using WorkshopManager.Model.DataModels;
using WorkshopManager.ViewModels.VM;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using WorkshopManager.Services;

namespace WorkshopManager.Web.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientRepairController : BaseController
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<Client> _userManager;
        private readonly StatusDescriptionService _statusDescriptionService;

        public ClientRepairController(ILogger logger, ApplicationDbContext dbContext, UserManager<Client> userManager, StatusDescriptionService statusDescriptionService) : base(logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _statusDescriptionService = statusDescriptionService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RepairRequest()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RepairRequest(AddRepairRequestVM viewModel)
        {
            var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (clientId == null)
            {
                return Unauthorized();
            }

            var registrationNumberPattern = @"^[A-Z]{2,3}\s[A-Z0-9]{1,5}[A-Z]{0,3}$|^[A-Z]{1,2}\s[A-Z0-9]{1,5}[A-Z]{0,3}$|^[A-Z0-9]{1,2}\s[A-Z]{4,}$|^\d{1,6}$";

            var regex = new Regex(registrationNumberPattern);

            if (!regex.IsMatch(viewModel.RegistrationNumber))
            {
                ModelState.AddModelError("RegistrationNumber", "Numer rejestracyjny jest nieprawidłowy.");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var order = new RepairOrder
            {
                ClientId = int.Parse(clientId),
                SubmissionDate = DateTime.Now,
                EntryIssueDescription = viewModel.EntryIssueDescription,
                RegistrationNumber = viewModel.RegistrationNumber,
                Status = RepairOrderStatusValue.PendingEstimate,
            };

            _dbContext.RepairOrders.Add(order);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> UpdateContactInfo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user1 = await _userManager.FindByIdAsync(userId);
            if (user1 == null)
            {
                return NotFound();
            }


            var model = new UpdateContactInfoVM
            {
                PhoneNumber = user1.PhoneNumber ?? "",
                FirstName = user1.FirstName ?? "",
                LastName = user1.LastName ?? "",
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateContactInfo(UpdateContactInfoVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = !model.FirstName.IsNullOrEmpty() ? model.FirstName : null;
            user.LastName = !model.LastName.IsNullOrEmpty() ? model.LastName : null;
            user.PhoneNumber = !model.PhoneNumber.IsNullOrEmpty() ? model.PhoneNumber : null;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Udało się zaktualizować profil.";
            return RedirectToAction("UpdateContactInfo");
        }

        [Authorize(Roles = "Client")]
        public IActionResult PendingEstimates()
        {
            var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (clientId == null)
            {
                return Unauthorized();
            }
            var orders = _dbContext.RepairOrders
                .Where(o => o.ClientId == int.Parse(clientId) &&
                            (o.Status == RepairOrderStatusValue.PendingEstimate || o.Status == RepairOrderStatusValue.ClientApproval))
                .ToList();

            var pendingEstimatesVM = orders
                .Where(order => order.Status == RepairOrderStatusValue.PendingEstimate)
                .Select(order => new PendingEstimateVM
                {
                    Id = order.Id,
                    RegistrationNumber = order.RegistrationNumber,
                    EntryIssueDescription = order.EntryIssueDescription,
                    SubmissionDate = order.SubmissionDate,
                    StatusDescription = _statusDescriptionService.GetStatusDescription(order.Status),
                    EntryEstimatedCost = order.EntryEstimatedCost
                }).ToList();

            var clientApprovalsVM = orders
                .Where(order => order.Status == RepairOrderStatusValue.ClientApproval)
                .Select(order => new PendingEstimateVM
                {
                    Id = order.Id,
                    RegistrationNumber = order.RegistrationNumber,
                    EntryIssueDescription = order.EntryIssueDescription,
                    SubmissionDate = order.SubmissionDate,
                    StatusDescription = _statusDescriptionService.GetStatusDescription(order.Status),
                    EntryEstimatedCost = order.EntryEstimatedCost
                }).ToList();

            var viewModel = new PendingEstimatesVM
            {
                PendingEstimates = pendingEstimatesVM,
                ClientApprovals = clientApprovalsVM
            };

            return View(viewModel);
        }

        public string GetStatusDescription(RepairOrderStatusValue status)
        {
            return _statusDescriptionService.GetStatusDescription(status);
        }

        [HttpPost]
        public IActionResult RespondToEstimate(int id, string response)
        {
            var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (clientId == null)
            {
                return Unauthorized();
            }

            var order = _dbContext.RepairOrders.FirstOrDefault(o => o.Id == id && o.ClientId == int.Parse(clientId));
            if (order == null)
            {
                return NotFound();
            }

            if (response == "accept")
            {
                order.Status = RepairOrderStatusValue.Accepted;
                TempData["SuccessMessage"] = "Wycena została zaakceptowana.";
            }
            else if (response == "reject")
            {
                _dbContext.RepairOrders.Remove(order);
                TempData["SuccessMessage"] = "Wycena została odrzucona, a zlecenie usunięte.";
            }
            else
            {
                TempData["ErrorMessage"] = "Nieznana akcja.";
                return RedirectToAction("PendingEstimates");
            }

            _dbContext.SaveChanges();
            return RedirectToAction("PendingEstimates");
        }

        [Authorize(Roles = "Client")]
        public IActionResult ActiveRepairs()
        {
            var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (clientId == null)
            {
                return Unauthorized();
            }

            var orders = _dbContext.RepairOrders
                .Where(o => o.ClientId == int.Parse(clientId) &&
                            (o.Status == RepairOrderStatusValue.Accepted || o.Status == RepairOrderStatusValue.InProgress))
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

        [Authorize(Roles = "Client")]
        public IActionResult ActiveRepairDetails(int id)
        {
            var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (clientId == null)
            {
                return Unauthorized();
            }

            var order = _dbContext.RepairOrders
                .Where(o => o.ClientId == int.Parse(clientId) && o.Id == id)
                .FirstOrDefault();

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

    }
}
