using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkshopManager.ViewModels.VM
{
    public class PendingRequestsVM
    {
        public List<PendingEstimateVM> PendingEstimates { get; set; } = null!;
        public List<ClientApprovalVM> ClientApprovals { get; set; } = null!;
    }
}
