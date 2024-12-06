using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkshopManager.ViewModels.VM
{
    public class ActiveRepairDetailVM
    {
        public ActiveRepairDetailVM() { }
        public ActiveRepairDetailVM(int id, string registrationNumber, string entryIssueDescription, DateTime submissionDate, string statusDescription, decimal? entryEstimatedCost)
        {
            Id = id;
            RegistrationNumber = registrationNumber;
            EntryIssueDescription = entryIssueDescription;
            SubmissionDate = submissionDate;
            StatusDescription = statusDescription;
            EntryEstimatedCost = entryEstimatedCost;
        }

        public int Id { get; set; }
        public string RegistrationNumber { get; set; } = null!;
        public string EntryIssueDescription { get; set; } = null!;
        public DateTime SubmissionDate { get; set; }
        public string StatusDescription { get; set; } = null!;
        public decimal? EntryEstimatedCost { get; set; }
    }

}
