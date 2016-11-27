using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebApp.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        //[Required]
        //[StringLength(50)]
        //public string FirstName { get; set; }

        //[Required]
        //[StringLength(50)]
        //public string LastName { get; set; }
    }

    public class ApplicationRole : IdentityRole
    {
        //[Required]
        //[StringLength(50)]
        //public string ProperName { get; set; }
    }
}
