using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkshopManager.ViewModels.VM
{
    public class RepairTaskVM
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public decimal Cost { get; set; }
        public bool? AcceptedByCustomer { get; set; }
    }
}
