using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WorkshopManager.DAL.EF;
using WorkshopManager.Model.DataModels;
using System.Threading.Tasks;
using System.Linq;
using WorkshopManager.ViewModels.VM;

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
    }
}





