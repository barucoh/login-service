using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginService.Models
{
    public class User
    {
        public string _id { get; set; }
        public string password { get; set; }

    }

    public class Token
    {
        public string _id { get; set; }
        public int _ttl { get; set; }
        public DateTime createdTime { get; set; }
        public Token()
        {

        }
    }
}
