using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkshopManager.Model.DataModels;

namespace WorkshopManager.ViewModels.VM
{
    public class PendingEstimateVM
    {
        public DateTime SubmissionDate { get; set; }
        public RepairOrderStatusValue Status { get; set; }
    }
}
