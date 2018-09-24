using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRAC.Intranet.Services.Model
{
    public class Airtime
    {
        public string Brand { get; set; }
        public bool IsBrand { get; set; }
        public string TotalRevenue { get; set; }
        public string RevenueRunRate { get; set; }
        public string AirtimeRevenue { get; set; }
        public string MTDRevenue { get; set; }
        public string AirtimeRevenueMinutes { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
