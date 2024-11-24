using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkshopManager.ViewModels.VM
{
    public class ActiveRepairsVM
    {
        public List<ActiveRepairVM> AcceptedOrders { get; set; } = null!;
        public List<ActiveRepairVM> InProgressOrders { get; set; } = null!;
    }
}
