using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkshopManager.Model.DataModels;

namespace WorkshopManager.ViewModels.VM
{
    public class PendingEstimateVM
    {
        public int Id { get; set; }  // Id z RepairOrder

        [Display(Name = "Registration number")]
        public string RegistrationNumber { get; set; } = null!;

        [Display(Name = "Issue description")]
        public string EntryIssueDescription { get; set; } = null!;

        [Display(Name = "Submission Date")]
        public DateTime SubmissionDate { get; set; }

        [Display(Name = "Status")]
        public RepairOrderStatusValue Status { get; set; }

        [Display(Name = "Status description")]
        public string StatusDescription { get; set; } = null!;

        [Display(Name = "Estimated cost")]
        public decimal? EntryEstimatedCost { get; set; }
    }
}
