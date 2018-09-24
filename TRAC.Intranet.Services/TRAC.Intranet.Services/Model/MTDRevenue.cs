using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRAC.Intranet.Services.Model
{
    public class MTDRevenue
    {
        public string Revenue { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public bool IsCurrentYear { get; set; }
        public bool IsActual { get; set; }
        public bool IsCurrentMonth { get; set; }
        public string PlannedPercentage { get; set; }
        public string PrevYearPercentage { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
