using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkshopManager.ViewModels.VM
{
    public class MechanicStatisticsVM
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime EmploymentDate { get; set; }
        public decimal AllTimeEarnings { get; set; } = 0m;
        public decimal Last30DaysEarnings { get; set; } = 0m;
        public decimal Last7DaysEarnings { get; set; } = 0m;
    }
}
