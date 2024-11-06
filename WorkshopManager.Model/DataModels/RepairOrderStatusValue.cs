using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkshopManager.Model.DataModels
{
	public enum RepairOrderStatusValue
	{
        PendingEstimate = 0,
        ClientApproval = 1,
        Accepted = 2,
        InProgress = 3,
        Completed = 4,
    }
}
