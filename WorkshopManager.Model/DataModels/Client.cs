using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WorkshopManager.Model.DataModels
{
    public class Client : User
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
		IList<RepairOrder> repairOrders { get; set; } = null!;
	}
}
