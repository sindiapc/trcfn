using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRAC.Intranet.Services.Model
{
    public class HomeCharts
    {
        public List<MTDRevenue> MTDRevenueData { get; set; }
        public List<YTDRevenue> YTDRevenueData { get; set; }
        public List<Subscriber> ActiveSubscribersTotalData { get; set; }
        public List<Subscriber> ActiveSubscribersByBrandData { get; set; }
        public List<NPS> NPSByBrandData { get; set; }
        public List<JiraIssue> JiraPriorityProjectsData { get; set; }
    }
}
