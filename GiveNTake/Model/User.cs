using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace GiveNTake.Model
{
    public class User : IdentityUser
    {
        //public string UserId { get; set; }
        public IList<Product> Products { get; set; }
    }
}
