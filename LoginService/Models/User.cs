using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginService.Models
{
    public class User
    {
        public string _id { get; set; }
        public string _rev { get; set; }
        public string password { get; set; }
    }
}
