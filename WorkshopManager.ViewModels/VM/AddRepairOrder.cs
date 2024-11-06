using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkshopManager.ViewModels.VM
{
    public class AddRepairOrder
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int MechanicId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string EntryIssueDescription { get; set; } = null!;
        public decimal? EntryEstimatedCost { get; set; }
        public string RegistrationNumber { get; set; } = null!;
    }
}

