using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkshopManager.Model.DataModels;

namespace WorkshopManager.ViewModels.VM
{
    public class ActiveRepairVM
    {
        public int Id { get; set; }

        public string RegistrationNumber { get; set; } = null!;

        public string EntryIssueDescription { get; set; } = null!;

        public DateTime SubmissionDate { get; set; }

        public RepairOrderStatusValue Status { get; set; }

        public string StatusDescription { get; set; } = null!;

        public decimal? EntryEstimatedCost { get; set; }
    }
}
