using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WorkshopManager.DAL.EF;
using WorkshopManager.Model.DataModels;
using System.Threading.Tasks;
using System.Linq;
using WorkshopManager.ViewModels.VM;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace WorkshopManager.Web.Controllers
{
    public class RepairTaskController : BaseController
    {
        private readonly ApplicationDbContext _dbContext;

        public RepairTaskController(ILogger<RepairTaskController> logger, ApplicationDbContext context) : base(logger)
        {
            _dbContext = context;
        }

        // Akcja do zaakceptowania zadania naprawczego

        [HttpPost]
        public async Task<IActionResult> AcceptTask(int id)
        {
            var task = _dbContext.RepairTasks.FirstOrDefault(rt => rt.Id == id);
            if (task == null)
            {
                return NotFound(); // Zadanie nie znalezione
            }

            task.AcceptedByCustomer = true;
            await _dbContext.SaveChangesAsync();

            // Przekierowanie z parametrem `id` na stronę szczegółów
            return RedirectToAction("InProgressRepairDetails", "ClientRepair", new { id = task.RepairOrderId });
        }

        [HttpPost]
        public async Task<IActionResult> RejectTask(int id)
        {
            var task = _dbContext.RepairTasks.FirstOrDefault(rt => rt.Id == id);
            if (task == null)
            {
                return NotFound(); // Zadanie nie znalezione
            }

            task.AcceptedByCustomer = false;
            await _dbContext.SaveChangesAsync();

            // Przekierowanie z parametrem `id` na stronę szczegółów
            return RedirectToAction("InProgressRepairDetails", "ClientRepair", new { id = task.RepairOrderId });
        }

        public IActionResult Add(int id)
        {
            return View(id);
        }

        [HttpPost]
        public IActionResult Add(AddRepairTaskSubmitFormVM vm)
        {
			var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (ownerId == null)
			{
				return Unauthorized();
			}

			var order = _dbContext.RepairOrders
                .Include(r => r.Tasks)
                .FirstOrDefault(o => o.Id == vm.RepairId);
			if (order == null)
			{
				return NotFound();
			}
            var repairTask = new RepairTask
            {
                Description = vm.Description,
                Cost = vm.Cost,
                AcceptedByCustomer = null
            };
            order.Tasks.Add(repairTask);
            _dbContext.SaveChanges();

			return RedirectToAction("InProgressRepairDetails", "OwnerRepair", new { id = vm.RepairId });
        }
    }
}





