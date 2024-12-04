using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkshopManager.Model.DataModels;

namespace WorkshopManager.ViewModels.VM
{
    public class MechanicVM
    {
        public MechanicVM() { }
        public MechanicVM(int id, string firstName, string lastName, string bankAccountNumber)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            BankAccountNumber = bankAccountNumber;
        }
        public MechanicVM(int id, string firstName, string lastName)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            BankAccountNumber = null!;
        }

        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
		public string BankAccountNumber { get; set; } = null!;
	}
}
