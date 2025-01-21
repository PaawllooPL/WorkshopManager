using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WorkshopManager.DAL.EF;
using WorkshopManager.Model.DataModels;
using WorkshopManager.ViewModels.VM;
using WorkshopManager.Web.Controllers;

namespace WorkshopManager.Controllers
{
    [Authorize(Roles = "Owner")]
    public class MechanicController : BaseController
    {
        private readonly ApplicationDbContext _dbContext;

        public MechanicController(ILogger logger, ApplicationDbContext context) : base(logger)
        {
            _dbContext = context;
        }

        // Akcja do wyświetlenia listy mechaników
        public IActionResult MechanicsList()
        {
            List<MechanicVM> model = _dbContext.Mechanics
                .Select(ro => new MechanicVM
                {
                    Id = ro.Id,
                    FirstName = ro.FirstName,
                    LastName = ro.LastName,
                    BankAccountNumber = ro.BankAccountNumber
                })
                .ToList();

            return View(model);
        }

      
        public IActionResult AddMechanic()
        {
            var mechanicVM = new MechanicVM();
            return View(mechanicVM);
        }

        [HttpPost]
        public async Task<IActionResult> AddMechanic(MechanicVM mechanicVM)
        {
            if (ModelState.IsValid)
            {
                var mechanic = new Mechanic
                {
                    FirstName = mechanicVM.FirstName,
                    LastName = mechanicVM.LastName,
                    BankAccountNumber = mechanicVM.BankAccountNumber,
                    EmploymentDate = DateTime.Now,
                };

                _dbContext.Mechanics.Add(mechanic);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(MechanicsList)); 
            }

            return View(mechanicVM); 
        }
        [HttpGet]
        public IActionResult MechanicsStatistics()
        {
            var mechanics = _dbContext.Mechanics.ToList();
            var mechanicsStatistics = mechanics.Select(mechanic =>
            {
                var mechanicsCompletedTasks = _dbContext.RepairTasks
                                                        .Include(rt => rt.RepairOrder)
                                                        .Where(rt => rt.RepairOrder.MechanicId == mechanic.Id &&
                                                                     rt.RepairOrder.EndDate != null &&
                                                                     rt.IsCompleted)
                                                        .Select(rt => new
                                                        {
                                                            RepairTask = rt,
                                                            EndDate = rt.RepairOrder.EndDate,
                                                        })
                                                        .ToList();
                return new MechanicStatisticsVM
                {
                    Id = mechanic.Id,
                    FirstName = mechanic.FirstName,
                    LastName = mechanic.LastName,
                    EmploymentDate = mechanic.EmploymentDate,
                    AllTimeEarnings = mechanicsCompletedTasks.Sum(rt => rt.RepairTask.Cost),
                    Last30DaysEarnings = mechanicsCompletedTasks.Where(rt => rt.EndDate > DateTime.Now.AddDays(-30)).Sum(rt => rt.RepairTask.Cost),
                    Last7DaysEarnings = mechanicsCompletedTasks.Where(rt => rt.EndDate > DateTime.Now.AddDays(-7)).Sum(rt => rt.RepairTask.Cost),
                };
            });
            return View(mechanicsStatistics);
        }
    }
}
