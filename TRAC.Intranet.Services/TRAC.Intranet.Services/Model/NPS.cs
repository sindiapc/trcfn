using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRAC.Intranet.Services.Model
{
    public class NPS
    {
        public string Brand { get; set; }
        public decimal Score { get; set; }
        public bool IsBrand { get; set; }
        public string SurveyDate { get; set; }
        public string SurveyMonth { get; set; }
        public string SurveyYear { get; set; }
    }
}

