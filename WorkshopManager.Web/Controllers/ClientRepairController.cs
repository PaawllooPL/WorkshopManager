using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.DAL.EF;
using WorkshopManager.Model.DataModels;
using WorkshopManager.ViewModels.VM;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace WorkshopManager.Web.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientRepairController : BaseController
    {
        private readonly ApplicationDbContext _dbContext;

        public ClientRepairController(ILogger logger, ApplicationDbContext dbContext) : base(logger)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddOrder()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddOrder(AddRepairOrder viewModel)
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
                RegistrationNumber = viewModel.RegistrationNumber
            };

            _dbContext.RepairOrders.Add(order);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
