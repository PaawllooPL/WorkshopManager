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
        [Display(Name = "Submission Date")]
        public DateTime SubmissionDate { get; set; }
       
        [Display(Name = "Status")]
        public RepairOrderStatusValue Status { get; set; }
    }
}
