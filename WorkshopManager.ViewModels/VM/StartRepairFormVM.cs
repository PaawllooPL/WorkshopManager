using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkshopManager.ViewModels.VM
{
    public class StartRepairFormVM
    {
        public int Id { get; set; } 
        public IEnumerable<MechanicVM> Mechanics { get; set; }

        public StartRepairFormVM(int id, IEnumerable<MechanicVM> mechanics)
        {
            this.Id = id;
            this.Mechanics = mechanics;
        }
    }
}
