using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkshopManager.Model.DataModels;

namespace WorkshopManager.Services
{
    public class StatusDescriptionService
    {
        public string GetStatusDescription(RepairOrderStatusValue status)
        {
            return status switch
            {
                RepairOrderStatusValue.PendingEstimate => "Oczekiwanie na wycenę przez warsztat",
                RepairOrderStatusValue.ClientApproval => "Wyceny oczekują na akceptację przez klienta",
                RepairOrderStatusValue.Accepted => "Zaakceptowane przez klienta",
                RepairOrderStatusValue.InProgress => "W trakcie realizacji",
                RepairOrderStatusValue.Completed => "Zakończone",
                _ => "Nieznany status"
            };
        }
    }

}
