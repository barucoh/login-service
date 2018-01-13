using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginService.Models
{
    public class Token
    {
        public string ID { get; set; }
        public int TTL { get; set; }
        public DateTime CreatedTime { get; set; }
        public Token()
        {

        }
    }
}
