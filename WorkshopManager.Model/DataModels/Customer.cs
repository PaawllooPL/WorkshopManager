using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkshopManager.Model.DataModels
{
    public class Customer : User
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }
}
