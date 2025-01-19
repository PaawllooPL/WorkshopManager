using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkshopManager.Model.DataModels;

namespace WorkshopManager.ViewModels.VM
{
    public class CompletedRepairVM
    {
        public int Id { get; set; }

        public string RegistrationNumber { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal EntryEstimatedCost { get; set; }
        public decimal FinalCost { get; set; }
    }
}
