using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkshopManager.Model.DataModels;

namespace WorkshopManager.ViewModels.VM
{
    public class InProgressRepairDetailsVM
    {
        public InProgressRepairDetailsVM() { }
        public InProgressRepairDetailsVM(int id, string registrationNumber, string entryIssueDescription, DateTime submissionDate, string statusDescription, decimal? entryEstimatedCost, List<RepairTaskVM> repairTasks)
        {
            Id = id;
            RegistrationNumber = registrationNumber;
            EntryIssueDescription = entryIssueDescription;
            SubmissionDate = submissionDate;
            StatusDescription = statusDescription;
            EntryEstimatedCost = entryEstimatedCost;
            RepairTasks = repairTasks ?? new List<RepairTaskVM>();
        }

        public int Id { get; set; }
        public string RegistrationNumber { get; set; } = null!;
        public string EntryIssueDescription { get; set; } = null!;
        public DateTime SubmissionDate { get; set; }
        public string StatusDescription { get; set; } = null!;
        public decimal? EntryEstimatedCost { get; set; }
        public IList<RepairTaskVM> RepairTasks { get; set; } = new List<RepairTaskVM>();
    }

}
