using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRAC.Intranet.Services.Model
{
    public class RetailActivation
    {
        public string Brand { get; set; }
        public bool IsBrand { get; set; }
        public string RetailerName { get; set; }
        public string ActivationCount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
