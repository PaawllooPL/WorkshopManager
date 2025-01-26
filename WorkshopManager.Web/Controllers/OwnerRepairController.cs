using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
        private readonly EmailNotificationService _emailNotificationService;

        public OwnerRepairController(
            ILogger logger, 
            ApplicationDbContext context, 
            StatusDescriptionService statusDescriptionService, 
            EmailNotificationService emailNotificationService) : base(logger)
        {
            _dbContext = context;
            _statusDescriptionService = statusDescriptionService;
            _emailNotificationService = emailNotificationService;
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
            _emailNotificationService.SendRepairCostEstimatedNotification(viewModel.Id);

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

        public IActionResult AcceptedRepairDetails(int id)
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

            var viewModel = new AcceptedRepairDetailsVM
            {
                Id = order.Id,
                RegistrationNumber = order.RegistrationNumber,
                EntryIssueDescription = order.EntryIssueDescription,
                SubmissionDate = order.SubmissionDate,
                StatusDescription = _statusDescriptionService.GetStatusDescription(order.Status),
                EntryEstimatedCost = order.EntryEstimatedCost
            };

            return View("AcceptedRepairDetails", viewModel);
        }

        public IActionResult InProgressRepairDetails(int id)
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

            var repairTasks = _dbContext.RepairTasks
                .Where(rt => rt.RepairOrderId == id)
                .Select(rt => new RepairTaskVM
                {
                    Id = rt.Id,
                    Cost = rt.Cost,
                    Description = rt.Description ?? string.Empty,
                    AcceptedByCustomer = rt.AcceptedByCustomer,
                    IsCompleted = rt.IsCompleted,
                })
                .ToList();

            var viewModel = new InProgressRepairDetailsVM
            {
                Id = order.Id,
                RegistrationNumber = order.RegistrationNumber,
                EntryIssueDescription = order.EntryIssueDescription,
                SubmissionDate = order.SubmissionDate,
                StatusDescription = _statusDescriptionService.GetStatusDescription(order.Status),
                EntryEstimatedCost = order.EntryEstimatedCost,
                RepairTasks = repairTasks
            };

            return View("InProgressRepairDetails", viewModel);
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
            repair.StartDate = DateTime.Now;

            _dbContext.SaveChanges();
            _emailNotificationService.SendStartRepairNotification(formData.RepairId);

            return RedirectToAction("ActiveRepairs");
        }
        [HttpPost]
        public IActionResult CompleteRepair(int id)
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
            if (order.Status != RepairOrderStatusValue.InProgress)
            {
                return BadRequest();
            }
            order.Status = RepairOrderStatusValue.Completed;
            order.EndDate = DateTime.Now;
            
            _dbContext.Update(order);
            _dbContext.SaveChanges();
            _emailNotificationService.SendRepairCompletedNotification(id);

            return RedirectToAction("CompletedRepairs", "OwnerRepair", new { id });
        }

        public IActionResult CompletedRepairs()
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ownerId == null)
            {
                return Unauthorized();
            }

            var orders = _dbContext.RepairOrders
               .Include(ro => ro.Tasks)
               .Where(ro => ro.Status == RepairOrderStatusValue.Completed).ToList();

            var viewModel = orders.Select(o => new CompletedRepairVM
            {
                Id = o.Id,
                RegistrationNumber = o.RegistrationNumber,
                StartDate = (DateTime)o.StartDate!,
                EndDate = (DateTime)o.EndDate!,
                EntryEstimatedCost = (decimal)o.EntryEstimatedCost!,
                FinalCost = o.Tasks.Aggregate(0m, (finalCost, rt) => rt.IsCompleted ? (finalCost + rt.Cost) : finalCost),
            }).ToList();

            return View("CompletedRepairs", viewModel);
        }

        public IActionResult GenerateRepairSummary(int id)
        {
            var repair = _dbContext.RepairOrders
                                    .Include(ro => ro.Client)
                                    .Include(ro => ro.Mechanic)
                                    .Include(ro => ro.Tasks)
                                    .FirstOrDefault(ro => ro.Id == id);
            if(repair == null)
            {
                return NotFound();
            }
                                                    
            using (var memoryStream = new MemoryStream())
            {
                // Tworzenie dokumentu PDF
                using (var writer = new PdfWriter(memoryStream))
                using (var pdf = new PdfDocument(writer))
                {
                    var document = new Document(pdf);

                    var header = new Paragraph("Podsumowanie naprawy")
                        .SetFontSize(24)
                        .SetTextAlignment(TextAlignment.CENTER);
                    document.Add(header);

                    var repairHeader = new Paragraph("Informacje o naprawie")
                        .SetFontSize(16)
                        .SetTextAlignment(TextAlignment.CENTER);
                    document.Add(repairHeader);

                    //var clientFirstName = new Paragraph()
                    var repairInfoTable = new Table(9);
                    repairInfoTable.AddHeaderCell("Id");
                    repairInfoTable.AddHeaderCell("Imie");
                    repairInfoTable.AddHeaderCell("Nazwisko");
                    repairInfoTable.AddHeaderCell("Numer telefonu");
                    repairInfoTable.AddHeaderCell("Numer rejestracyjny");
                    repairInfoTable.AddHeaderCell("Rozpoczecie naprawy");
                    repairInfoTable.AddHeaderCell("Zakonczenie naprawy");
                    repairInfoTable.AddHeaderCell("Wstepna wycena");
                    repairInfoTable.AddHeaderCell("Finalny koszt");

                    repairInfoTable.AddCell(repair.Id.ToString());
                    repairInfoTable.AddCell(!String.IsNullOrWhiteSpace(repair.Client.FirstName) ? repair.Client.FirstName : "Nie podano");
                    repairInfoTable.AddCell(!String.IsNullOrWhiteSpace(repair.Client.LastName) ? repair.Client.LastName : "Nie podano");
                    repairInfoTable.AddCell(!String.IsNullOrWhiteSpace(repair.Client.PhoneNumber) ? repair.Client.PhoneNumber : "Nie podano");
                    repairInfoTable.AddCell(repair.RegistrationNumber);
                    repairInfoTable.AddCell(repair.StartDate.ToString());
                    repairInfoTable.AddCell(repair.EndDate.ToString());
                    repairInfoTable.AddCell(repair.EntryEstimatedCost.ToString());
                    repairInfoTable.AddCell(repair.Tasks.Aggregate(0m, (finalCost, rt) => rt.IsCompleted ? (finalCost + rt.Cost) : finalCost).ToString());
                    // Dodanie tabeli do dokumentu
                    document.Add(repairInfoTable);

                    var repairTasksHeader = new Paragraph("Lista czynnosci naprawczych")
                        .SetFontSize(16)
                        .SetTextAlignment(TextAlignment.CENTER);
                    document.Add(repairTasksHeader);

                    var repairTasksTable = new Table(4);
                    repairTasksTable.AddHeaderCell("Cena");
                    repairTasksTable.AddHeaderCell("Opis");
                    repairTasksTable.AddHeaderCell("Zaakceptowane przez klienta?");
                    repairTasksTable.AddHeaderCell("Wykonane?");

                    foreach(var task in repair.Tasks)
                    {
                        repairTasksTable.AddCell(task.Cost.ToString());
                        repairTasksTable.AddCell(task.Description);
                        repairTasksTable.AddCell(task.AcceptedByCustomer == true ? "tak" : "nie");
                        repairTasksTable.AddCell(task.IsCompleted == true ? "tak" : "nie");
                    }
                    document.Add(repairTasksTable);

                    document.Close();
                }

                // Zwrócenie PDF do pobrania bez przenoszenia do nowego okna
                var fileBytes = memoryStream.ToArray();
                var fileName = $"raport_naprawy_{repair.RegistrationNumber}_{repair.EndDate}.pdf";
                return File(fileBytes, "application/pdf", fileName);
            }
        }
    }
}