using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkshopManager.ViewModels.VM;

public class UpdateContactInfoVM
{
    [Required]
    public string? FirstName { get; set; }
    
    [Required]
    public string? LastName { get; set; }

    [Required]
    [Phone]
    public string? PhoneNumber { get; set; }
}
