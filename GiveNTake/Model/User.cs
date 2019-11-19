using System;
using System.Collections.Generic;

namespace GiveNTake.Model
{
    public class User
    {
        public string UserId { get; set; }
        public IList<Product> Products { get; set; }
    }
}
