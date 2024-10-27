using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkshopManager.Model.DataModels
{
    public class RepairOrder
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public int? MechanicId { get; set; }
        public string? RepairStatusId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal EntryEstimatedCost { get; set; }

        public string EntryIssueDescription { get; set; } = null!;
        public string RegistrationNumber { get; set; } = null!;
    }
}
