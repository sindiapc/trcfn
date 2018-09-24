using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRAC.Intranet.Services.Model
{
    public class YTDRevenueChart
    {
        public List<BrandRevenue> ActualRevenue { get; set; }
        public List<BrandRevenue> PlannedRevenue { get; set; }
        public BrandRevenue CurrentYearTotalRevenue { get; set; }
        public BrandRevenue PrevYearTotalRevenue { get; set; }
    }
}
