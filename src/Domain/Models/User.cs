using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models;

public class User : IdentityUser<Guid>
{
	     public bool IsActive { get; set; } = true;
	     public string? FirstName {  get; set; }
	     public string? LastName { get; set; }

          

}

