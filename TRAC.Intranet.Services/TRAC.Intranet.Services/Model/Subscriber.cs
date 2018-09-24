using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRAC.Intranet.Services.Model
{
    public class Subscriber
    {
        public string Brand { get; set; }
        public Int64 ActiveSubscribers { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsBrand { get; set; }
        public string NetGains { get; set; }
        public string Goal { get; set; }
        
    }
}
